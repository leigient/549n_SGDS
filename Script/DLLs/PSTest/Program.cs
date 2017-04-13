using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows;

using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PSTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string strRet = "";

            using (Runspace aRunspace = RunspaceFactory.CreateRunspace())
            {
                aRunspace.Open();

                using (Pipeline aPipeline = aRunspace.CreatePipeline())
                {
                    Command aCommand = new Command("Get-Process");
                    aCommand.Parameters.Add("Name", "note*");
                    aCommand.Parameters.Add("FileVersionInfo", null);

                    aPipeline.Commands.Add(aCommand);

                    foreach (PSObject aResult in aPipeline.Invoke())
                    {
                        //strRet += string.Format("{0,-24} | {1}\r\n", aResult.Members["ProcessName"].Value, aResult.Members["Id"].Value);
                        strRet += string.Format("{0} | {1} | {2}\r\n", aResult.Members["ProductVersion"].Value, aResult.Members["FileVersion"].Value, aResult.Members["FileName"].Value);

                        //foreach (PSMemberInfo Info in aResult.Members) { strRet += Info.Name + "|" + Info.Value; }
                    }
                }

            }

            Console.WriteLine(strRet);
        }
    }
}
