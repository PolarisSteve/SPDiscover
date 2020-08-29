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

namespace SPDiscover
{
    public static class Helpers
    {


        public static string CurrProcName { get; set; }
        public static string CurrProcText { get; set; }



        public static string AnchorHelper(string anchorstring, params object[] p )
        {
            string _anchorstring = anchorstring;

            if (Arguments.GetValByKey("launchwindow").ToLower() == "true")
                _anchorstring = anchorstring.Replace(" >", " target='blank' >");

            return string.Format(_anchorstring, p);
        }

        public static string ConnectionBuilder(Dictionary<string, string> parmcoll)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();


            foreach (var parameter in parmcoll.Keys)
            {
                string cmd = parameter.Trim().Replace("_", " ");

                if (builder.ContainsKey(cmd))
                {
                    builder[cmd] = parmcoll[parameter].Trim().Replace(";", "");
                }

            }


            if (Arguments.GetValByKey("ShowConnectionString").Length > 0)
                Console.WriteLine(builder.ConnectionString);


            return builder.ConnectionString; ;
        }



    }
}
