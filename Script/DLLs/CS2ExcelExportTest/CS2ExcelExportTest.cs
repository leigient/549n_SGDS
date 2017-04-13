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

namespace CS2ExcelExportTest
{
    [ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual)]
    public class ExcelExporter
    {

        public string teststr
        {
            [return: MarshalAs(UnmanagedType.BStr)]
            get;

            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [return: MarshalAs(UnmanagedType.BStr)]
        public string SetStringBrackets(string paramstr, Int32 cnt)
        {
            return "[" + paramstr + "], cnt = " + Convert.ToString(cnt);
        }

        public string RunCMDBatch(string strBatchFile, string strWorkingDir = "", string strShowConsole = "true")
        {
            string strRet = "";

            ProcessStartInfo aPsi = new ProcessStartInfo();
            aPsi.FileName = "cmd.exe";
            aPsi.Arguments = ((strShowConsole.ToLower() == "true") ? "/K" : "/C") + " " + strBatchFile;

            if (strWorkingDir != "") { aPsi.WorkingDirectory = strWorkingDir; }

            aPsi.RedirectStandardOutput = true;
            aPsi.UseShellExecute = false;

            Process aProc = Process.Start(aPsi);
            //aProc.WaitForExit();
            strRet = aProc.StandardOutput.ReadToEnd();

            //ShowMessageBox(aPsi.WorkingDirectory);

            return strRet;
        }

        // int WINAPI MessageBox()
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        private static extern int MessageBox(IntPtr h, string strMsg, string strTitle, uint type);

        public void ShowMessageBox(string strMsg = "No Message", string strTitle = "No Title")
        {
            MessageBox((IntPtr)0, strMsg, strTitle, 0);
        }

        public string RunPSScript(string strPSFile = "", string strWorkingDir = "", string strShowConsole = "true")
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

            return strRet;
        }
    }

    static class UnmanagerdExports
    {
        [DllExport]
        [return: MarshalAs(UnmanagedType.IDispatch)]
        static Object CreateExcelExporter()
        {
            return new ExcelExporter();
        }
    }
}
