using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Win32;

namespace CodeSetup
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "CodeSetup";
            var currentFileName = Process.GetCurrentProcess().MainModule.FileName;
            if (currentFileName ==null)
            {
                Console.WriteLine("Process.GetCurrentProcess().MainModule.FileName is null !");
                Console.Read();
                Environment.Exit(0);
            }
            var workPlace = Path.GetDirectoryName(currentFileName);

            var vscodePortable = Path.Combine(workPlace, "vscode-portable.exe");

            if (!File.Exists(vscodePortable))
            {
                Console.WriteLine(vscodePortable+" is not exists !");
                Console.Read();
                Environment.Exit(0);
            }
label:
            Console.WriteLine("1. Add   Right mouse");
            Console.WriteLine("2.Remove Right mouse");
            var key=Console.ReadKey();
            Console.WriteLine();
            switch (key.KeyChar)
            {
                case '1':
                    AddVscode(vscodePortable);
                    Console.WriteLine("OK");
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    Environment.Exit(0);
                    break;
                case '2':
                    RemoveVscode(vscodePortable);
                    Console.WriteLine("OK");
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Please Press 1 or 2");
                    break;
            }
            goto label;

        }

        public static void AddVscode(string vscodePortable)
        {
            Add(vscodePortable, RegistryView.Registry32);
            Add(vscodePortable, RegistryView.Registry64);
            AddDirectory(vscodePortable, RegistryView.Registry32);
            AddDirectory(vscodePortable, RegistryView.Registry64);
        }

        public static void RemoveVscode(string vscodePortable)
        {
            Remove(RegistryView.Registry32);
            Remove(RegistryView.Registry64);
            RemoveDirectory(RegistryView.Registry32);
            RemoveDirectory(RegistryView.Registry64);
        }


        public static void Add(string vscodePortable , RegistryView registryView)
        {
            Remove(registryView);
            RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, registryView).OpenSubKey(@"SOFTWARE\Classes\*\shell",true).CreateSubKey("VSCode");
            var VSCode = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Classes\*\shell").OpenSubKey("VSCode",true);
            VSCode.SetValue("", "通过 Code 打开", RegistryValueKind.ExpandString);
            VSCode.SetValue("Icon", vscodePortable, RegistryValueKind.ExpandString);
            VSCode.CreateSubKey("command");
            var command = VSCode.OpenSubKey("command",true);
            command.SetValue("", $"\"{vscodePortable}\" \"%1\"");
        }

        public static void Remove(RegistryView registryView)
        {
            var root = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, registryView);
            var dynamic = root.OpenSubKey(@"SOFTWARE\Classes\*");
            var shell = dynamic.OpenSubKey("shell",true);
            var shellSubNames = shell.GetSubKeyNames();

            if (shellSubNames.Contains("VSCode"))
            {
                shell.DeleteSubKeyTree("VSCode");
            }
        }

        public static void AddDirectory(string vscodePortable, RegistryView registryView)
        {
            RemoveDirectory(registryView);
            RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, registryView).OpenSubKey(@"SOFTWARE\Classes\Directory\Background\shell", true).CreateSubKey("VSCode");
            var VSCode = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Classes\Directory\Background\shell").OpenSubKey("VSCode", true);
            VSCode.SetValue("", "通过 Code 打开", RegistryValueKind.ExpandString);
            VSCode.SetValue("Icon", vscodePortable, RegistryValueKind.ExpandString);
            VSCode.CreateSubKey("command");
            var command = VSCode.OpenSubKey("command", true);
            command.SetValue("", $"\"{vscodePortable}\" \"%V\"");
        }

        public static void RemoveDirectory(RegistryView registryView)
        {
            var root = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, registryView);
            var dynamic = root.OpenSubKey(@"SOFTWARE\Classes\Directory\Background");
            var shell = dynamic.OpenSubKey("shell", true);
            var shellSubNames = shell.GetSubKeyNames();

            if (shellSubNames.Contains("VSCode"))
            {
                shell.DeleteSubKeyTree("VSCode");
            }
        }



    }
}
