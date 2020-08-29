// Written by Steven Contos.
// Program is open source and available to all to use and modify as they see fit.
// Steve@Polaris.LLC
// Please retain this notice on the source code.
// Distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPDiscover
{
    public static class Arguments
    {
        static Dictionary<string, string> returnvals = new Dictionary<string, string>();

        /// <summary>
        /// If key is found will return it, otherwise returns empty string.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValByKey(string key )
        {
            if (returnvals.ContainsKey(key.ToLower()))
                return returnvals[key.ToLower()];
            else
                return string.Empty;
        
        }

        public static Dictionary<string,string> CheckParameters(string[] parms)
        {
                        
            //First check, ensure there are parameters.
            if (parms.Length == 0)
            {
                Console.WriteLine("You must enter at a parameter for the starting procedure including schema(dbo) and connection string parameters separated with a space");
                Console.WriteLine("The parameters must be represented by a command and a value separated by an equal sign");
                Console.WriteLine("The program can build a connection string using SqlConnectionStringBuilder, if a key has a space in it you must convert to an underscore (data source becomes data_source)  ");
                Console.WriteLine("Example: procedure=ProcedureA ConnectionStringValue(s)=FullyQualifiedDatabaseConnection separated with a space");
                Console.WriteLine("help=commands will list all valid commands");
                Console.WriteLine("Press any key to close");

                Console.Read();
            }
            else
            {
                //Second, check that each parameter is in correct format
                foreach (string parm in parms)
                {
                    if (parm.Split('=').Length < 2)
                    {
                        Console.WriteLine("The parameters must be represented by a command and a value");
                        Console.WriteLine("Example: procedure=ProcedureA, commands should not contain spaces data_source= rather than data source =");
                        Console.WriteLine("help=commands will list all valid commands");
                        Console.WriteLine("Press any key to close");

                        Console.Read();
                        return returnvals;
                    }
                }

                //Third, print out help if requested.
                foreach (string parm in parms)
                {
                    //although we say help=commands, any value will work.
                    if (parm.Split('=')[0].ToLower() == "help")
                    {
                        Console.WriteLine("The following are valid commands:");
                        Console.WriteLine("procedure, dbconnection, outputtype, launchwindow, and help");
                        Console.WriteLine("procedure=startprocedure - Required! Reads through the selected procedure and generates a file and scans for other procedures to iterate through");
                        Console.WriteLine("Any valid SqlConnectionStringBuilder acceptable key synonym, if the synonym contains a space, replace it with an underscore ");
                        Console.WriteLine("dbconnection=sqlconnectionstring - Use if you need to pass all parameters including user name and password.");
                        Console.WriteLine("OutputType - Htm or txt. Defaults to Htm");
                        Console.WriteLine("LaunchWindow - When type is Htm, will generate anchor tags to launch in a seperate window. Defaults to false");
                        Console.WriteLine("ShowConnectionString=true - optional. Use this to verify connection string built");
                        Console.WriteLine("Help - shows this help");

                        Console.WriteLine("Press any key to close");

                        Console.Read();
                        return returnvals;
                    }
                }

                


                //Fourth check for valid parms and store 
                foreach (string parm in parms)
                {
                    string cmd = parm.Split('=')[0].ToLower();
                    string pval = parm.Split('=')[1];
                    
                    if (!returnvals.ContainsKey(cmd)) //ensure only one entry
                    {
                        if (!pval.Contains(".") && cmd=="procedure")
                        {
                            Console.WriteLine("The starting object must contain a schema. Example dbo.procedure");
                            Console.Read();
                            returnvals.Clear();
                            return returnvals;
                        }

                        returnvals.Add(cmd, pval);
                    }
                    
                }
            }
            return returnvals; 
        }
    }
}
