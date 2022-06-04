using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessStart
{
    class Program
    {
        static void Main(string[] args)
        {
            GetProcessAll();
            foreach (var item in args)
            {
                Console.WriteLine(item);
                string outStr = string.Empty;
                Task startProcessTask = new TaskFactory().StartNew(() => RunScript(item, null, out outStr));
                if (startProcessTask.IsCompleted) { startProcessTask.Dispose(); }
                Console.WriteLine(outStr);
            }
            bool res = false;
            while (!res)
            {
                res = ProcessIsRunning(args);
            }
        }

        public static string[] GetProcessName(string[] paths)
        {
            string[] nameArray=new string[paths.Length];
            for (int i = 0; i < paths.Length; i++)
            {
                nameArray[i] = Path.GetFileName(paths[i]);
            }
            return nameArray;
        }

        public static void GetProcessAll()
        {
            Process[] CIPR = Process.GetProcesses();
            foreach (var item in CIPR)
            {
                Console.WriteLine(item);
            }
        }

        public static bool ProcessIsRunning(String[] processPath)
        {
            GetProcessAll();
            bool[] result=new bool[processPath.Length];
            for (int i = 0; i < processPath.Length; i++)
            {
                if (File.Exists(processPath[i]))
                {
                    string name = Path.GetFileNameWithoutExtension(processPath[i]);
                    Process[] processes = Process.GetProcessesByName(name); // 获取指定名称的进程
                    if((processes != null && processes.Length > 0))
                    {
                        result[i] = true;
                        Console.WriteLine(name+" is running");
                    }
                    else
                    {
                        Console.WriteLine(name + " wait start");
                    }
                }
                else
                {
                    throw new Exception(processPath[i] + "file is not exists");
                }
            }
            foreach (var item in result)
            {
                if (!item)
                {
                    ProcessIsRunning(processPath);
                }
            }
            return true;
        }
    
        static void RunScript(string script, string arguments, out string errorMessage)
        {
            errorMessage = string.Empty;
            using (Process process = new Process())
            {
                process.OutputDataReceived += process_OutputDataReceived;
                ProcessStartInfo info = new ProcessStartInfo(script);
                info.Arguments = String.Join(" ", arguments);
                info.UseShellExecute = false;
                info.RedirectStandardError = true;
                info.RedirectStandardOutput = true;
                info.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo = info;
                process.EnableRaisingEvents = true;
                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();
                errorMessage = process.StandardError.ReadToEnd();
            }
        }

        static void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    // Write the output somewhere
                }
            }
        }
    }
}
