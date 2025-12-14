using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Threading;

namespace GreenLumaLock
{
    internal class Program
    {
        private static readonly FileInfo lockFile = new FileInfo(Path.Combine(Environment.CurrentDirectory, "greenluma.lock"));
        private static readonly string GLInjectorPath = Path.Combine(Environment.CurrentDirectory, "DLLInjector.exe");
        // AFS App/File/Steam => EvCode
        // 000 Start => 32
        // 010 Invalid State => 15
        // 001 Restart Steam => 32
        // 011 Continue => 32
        // 1-- Internal Error => 63
        static void Main(string[] args)
        {

            if (args.Length > 0 && args[0] == "lock")
            {
                Console.WriteLine("Tracking steam...");
                Thread.Sleep(2000);
                Process[] plist = Process.GetProcessesByName("steam");
                if (plist.Length != 0)
                {
                    using (FileStream fs = new FileStream(
                        lockFile.FullName,
                        FileMode.Create,
                        FileAccess.ReadWrite,
                        FileShare.Read,
                        64,
                        FileOptions.DeleteOnClose
                    ))
                    {
                        Console.WriteLine("Active tracking...");
                        foreach (Process p in plist) p.WaitForExit();
                    }
                }
                Environment.Exit(0);
            }


            int exitCode = 63;
            if (!File.Exists(Program.GLInjectorPath))
            {
                Console.WriteLine("Unable to find GLInjectorPath..");
                if (args.Length == 0) Console.ReadLine();
                Environment.Exit(exitCode);
            }

            if (Process.GetProcessesByName("steam").Length > 0)
            {
                if (lockFile.Exists)
                {
                    Console.WriteLine("Steam running and lock file found, all working!");
                    exitCode = 32; //11
                }
                else //01
                {
                    Console.WriteLine("No lock file and steam running, killing steam...");
                    Process.Start("steam://exit");
                    foreach (Process p in Process.GetProcessesByName("steam")) p.WaitForExit();
                    Console.WriteLine("Starting steam with greenluma..");
                    Process.Start(Program.GLInjectorPath).WaitForExit();
                    ProcessStartInfo lockwatcher = new ProcessStartInfo
                    {
                        FileName = Assembly.GetExecutingAssembly().Location,
                        Arguments = "lock",
                        UseShellExecute = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    };
                    Process.Start(lockwatcher);
                    exitCode = 32;
                }
            }
            else
            {
                if (lockFile.Exists) //10
                {
                    Console.WriteLine("Invalid state found!");
                    exitCode = 15;
                }
                else
                {
                    //00
                    Console.WriteLine("Starting steam with greenluma..");
                    Process.Start(Program.GLInjectorPath).WaitForExit();
                    ProcessStartInfo lockwatcher = new ProcessStartInfo
                    {
                        FileName = Assembly.GetExecutingAssembly().Location,
                        Arguments = "lock",
                        UseShellExecute = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    };
                    Process.Start(lockwatcher);
                    exitCode = 32;
                }

            }

            if (args.Length == 0)
            {
                Console.WriteLine("Interactive mode, press a key to exit");
                Console.WriteLine("Remember close this program before closing steam otherwise there would be an error!");
                Console.ReadLine();
            }
            Environment.Exit(exitCode);
        }
    }
}
