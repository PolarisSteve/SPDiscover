// Written by Steven Contos.
// Program is open source and available to all to use and modify as they see fit.
// Steve@Polaris.LLC
// Please retain this notice on the source code.
// Distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied



using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace SPDiscover
{

    public class ReadDBProcs
    {
        DependentDBObjects DDbo = new DependentDBObjects();
        ReadDbObjects RDbo = null;
        SqlCommand _cmd = null;
        string _outputtype;
        public StringBuilder DBText = null;


        public ReadDBProcs(SqlCommand cmd)
        {
            _cmd = cmd;
            _outputtype = Arguments.GetValByKey("outputtype") == string.Empty ? "htm" : Arguments.GetValByKey("outputtype");
            RDbo = new ReadDbObjects();
        }


        public void Read(string spName)
        {
            Helpers.CurrProcName = spName;
            Console.WriteLine(spName);



            DDbo._cmd = _cmd;
            RDbo.sqlCommand = _cmd;

            List<DependentData> currDependencies = DDbo.FindDependencies(spName.Trim());
            DependentData currDependency = currDependencies.First();

            DBText = RDbo.GetDBText(currDependency.dependentName, currDependency.dependentType, currDependencies.Skip(1).ToList());




            DBText.AppendLine("");

            if (currDependencies.Count > 1)
            {
                DBText.AppendLine("-------------- Dependent Objects --------------");
                DBText.AppendLine("");


                foreach (DependentData item in currDependencies.Skip(1))
                {

                    if (_outputtype.ToLower() == "htm")
                    {
                        //we can add a link, otherwise leave
                        DBText.AppendLine(string.Format("Object " + Helpers.AnchorHelper("<a href='{0}.htm' >{0}</a>, Type {1}", item.dependentName, item.dependentType)));
                    }
                    else
                    {
                        DBText.AppendLine(string.Format("Object {0}, Type {1}", item.dependentName, item.dependentType));
                    }
                }

            }

            if (_outputtype.ToLower() == "htm")
            {
                //Write to the template file
                Helpers.CurrProcText = DBText.ToString();


                PageHTM template = new PageHTM();
                string pageContent = template.TransformText();
                File.WriteAllText(string.Format("{0}.htm", spName), pageContent);


            }
            else
            {
                File.WriteAllText(string.Format("{0}.txt", spName), DBText.ToString());

            }



            //Lets see if we have more work to do

            foreach (var procedure in currDependencies.Skip(1))
            {
                //only visit once
                if (!procedure.isVisited)
                {
                    Read(procedure.dependentName);
                }

            }

        }

    }



}

