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
using System.Threading.Tasks;


//https://www.c-sharpcorner.com/UploadFile/67b45a/how-to-generate-a-create-table-script-for-an-existing-table/
//Highlighting provided by highlight.js https://highlightjs.org/

namespace SPDiscover
{


    class Program
    {
        static void Main(string[] args)
        {
            var parmcoll = Arguments.CheckParameters(args);
            if (parmcoll.Count == 0)
                return;

            //Build connection string using parameters passed in.
            string baseConfig = Helpers.ConnectionBuilder(parmcoll);

            try
            {
                using (var sqlcmd = new SqlCommand() { Connection = new SqlConnection(baseConfig) })
                {
                    var procReader = new ReadDBProcs(sqlcmd);
                    procReader.Read(parmcoll["procedure"]);
                }

                Console.WriteLine("Program Complete");
                Console.WriteLine("Press any key to close");
                Console.Read();

            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("Error occurred - {0}", e.Message));
                Console.WriteLine("Press any key to close");
                Console.Read();
            }

        }
    }
}
