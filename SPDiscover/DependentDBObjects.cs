// Written by Steven Contos.
// Program is open source and available to all to use and modify as they see fit.
// Steve@Polaris.LLC
// Please retain this notice on the source code.
// Distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied


using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPDiscover
{

    //This is where we determine what objects are dependencies for the current object.
    //They can be a table, a view, another stored procedure.
    //Each type might have different set of dependencies. Such as a table might have constraints or
    //triggers



    public class DependentData
    {
        
        public bool isVisited { get; set; }
        public string dependentName { get; set; }
        public string dependentType { get; set; }

    }

    public class DependentDBObjects
    {


        SqlDataReader _reader = null;


        public DependentDBObjects()
        {
            DependentsList = new List<DependentData>();
        }


        public SqlCommand _cmd { get; set; }

        public List<DependentData> DependentsList { get; set; }


        public List<DependentData> FindDependencies(string dependentName)
        {

            string schemaName = "dbo";
            string tableName = dependentName;

            if (dependentName.Contains("."))
            {
                schemaName = dependentName.Split('.')[0];
                tableName = dependentName.Split('.')[1];
            }

            List<DependentData> retVal = new List<DependentData>();

            //First we run this to determine type of object.

            string ctext = string.Format(@"
                                        select sch_do.name + '.' + do.name as Name, do.type as Type,1
                                            from sys.objects as o
                                            join sys.objects as do on do.object_id = o.object_id
                                            join sys.schemas as sch_o on sch_o.schema_id = o.schema_id
                                            join sys.schemas as sch_do on sch_do.schema_id = do.schema_id
                                            where
                                            o.name = '{0}' and sch_o.name = '{1}' and do.name = '{0}' and sch_o.name = '{1}'
                                        union
                                        select sch_do.name + '.' + do.name as Name, do.type as Type,2
                                            from sys.objects as o
                                            join sys.sql_expression_dependencies as d on d.referencing_id = o.object_id
                                            join sys.objects as do on do.name = d.referenced_entity_name
                                            join sys.schemas as sch_o on sch_o.schema_id = o.schema_id
                                            join sys.schemas as sch_do on sch_do.schema_id = do.schema_id
                                            where
                                            (o.name = '{0}' and sch_o.name = '{1}') and(do.name != '{0}' or sch_o.name != '{1}')
                                        union
                                          select sch_do.name + '.' + do.name as Name, do.type as Type,3
                                            from sys.objects as o
                                            join sys.triggers as d on d.parent_id = o.object_id
                                            join sys.objects as do on do.object_id = d.object_id
                                            join sys.schemas as sch_o on sch_o.schema_id = o.schema_id
                                            join sys.schemas as sch_do on sch_do.schema_id = do.schema_id
                                            where
                                            (o.name = '{0}' and sch_o.name = '{1}') and(do.name != '{0}' or sch_o.name != '{1}') order by 3", tableName, schemaName);


            _cmd.CommandText = ctext;
            _cmd.Connection.Open();


            string objectname = string.Empty;
            string objecttype = string.Empty;
            DependentData currentItem = null;
            bool firstRow = true;
            //first row will return the type of the selected object and set as visited.

            try
            {
                _reader = _cmd.ExecuteReader();
                if (_reader.HasRows)
                {
                    while (_reader.Read())
                    {
                        objectname = _reader.GetSqlString(0).Value;
                        objecttype = _reader.GetSqlString(1).Value;
                        
                        if (firstRow)
                        {
                            firstRow = false;

                            currentItem = DependentsList.FirstOrDefault(w => w.dependentName == dependentName.Trim() && w.dependentType == objecttype.Trim());
                            if (currentItem != null)
                            {
                                //update the current item, setting the isvisited to true.
                                //This can happen if it was loaded as part of another call
                                currentItem.isVisited = true;
                            }
                            else if (!DependentsList.Any(a => a.dependentName == objectname && a.dependentType == objecttype))
                            {
                                currentItem = new DependentData() { dependentName = objectname, dependentType = objecttype, isVisited = true };
                                DependentsList.Add(currentItem);
                            }
                                
                                
                            
                        }
                        
                        //return current set of dependencies so we can display on bottom.
                        retVal.Add(new DependentData() { dependentName = objectname, dependentType = objecttype, isVisited = DependentsList.Any(a => a.dependentName == objectname && a.dependentType == objecttype) });

                        if (!DependentsList.Any(a => a.dependentName == objectname && a.dependentType == objecttype))
                            DependentsList.Add(new DependentData() { dependentName = objectname, dependentType = objecttype, isVisited = false });

                    }

                }

            }
            catch (Exception ex)
            {
                var t = ex;

            }
            finally
            {
                if (_reader != null)
                    _reader.Close();
                _cmd.Connection.Close();
            }


            return retVal;

        }


    }
}
