using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Diagnostics;
using System.Collections;

using nova.Utils;
using nova.Command;
using nova.Command.env;
using System.Runtime.ExceptionServices;
//using nova.Tests;

namespace novaf
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("(c) nova Initial Developers | Fri3nds .G");
            DesignFormat.Banner();

            Initnova();
        }

        public static void Initnova()
        {
            #region UnitTests
            //UnitTests.Test1();
            //UnitTests.Test2();
            //UnitTests.Test3();
            //UnitTests.Test4();
            //UnitTests.Test_RunCmd();

            //nova.Command.env.Vars.main_test();
            //UnitTests.Test_Exit();
            #endregion

            #region shall tests
            //abc:
            //List<string> parts= new List<string>();
            //parts = ["@bin","ls","--path", "$(BinPath)"];
            //for (int i = 0; i < parts.Count; i++)
            //{
            //    if (parts[i].StartsWith("$("))
            //    {
            //        parts[i] = parts[i].Replace("$(", "");
            //        parts[i] = parts[i].Replace(")", "");

            //        Console.WriteLine($":: {parts[i]}");
            //    }
            //    else
            //    {

            //    }
            //}
            //Console.ReadLine();
            //goto abc;
            #endregion

            //RunOnWindows.RunPythonFile(["C:\\Users\\Hamza\\vin_env\\ibin\\python\\usmnh\\tabl.py"]);
            //Environment.Exit(0);

            //{
            //    List<string> commands = UserInput.Prepare("@sudev do run d:G:\\fri3nds\\v-category-projects\\Developer-Grade-Virtual-OS\\Ju-hind-F\\Ju-Hind-F\\Ju-Hind-F\\Command\\env\\doker_lang\\test.dokr");

            //    IdentifyCommand.Identify(commands);
            //    List<string> parsed_commands = IdentifyCommand.ReturnThemPlease();

            //    PleaseCommandEnv.TheseCommands(parsed_commands);

            //    IdentifyCommand.CacheClean();

            //    Environment.Exit(0);
            //}

            //{
            //    nova_vm.Command.env.doker_lang.Interpreter.Interpret.Helper.ThisCode();


            //    Environment.Exit(0);

            //}

            #region Actual Init

        a:
            try
            {

                DesignFormat.TakeInput([$"\n{CommandEnv.CURRENT_USER_NAME}", "@", "novaf", ": ", $"{CommandEnv.CurrentDirDest}", $" ({CommandEnv.CURRENT_NODE_NAME})", " $ "]);

                List<string> commands = UserInput.Prepare(UserInput.Input());

                IdentifyCommand.Identify(commands);
                List<string> parsed_commands = IdentifyCommand.ReturnThemPlease();

                PleaseCommandEnv.TheseCommands(parsed_commands);

                IdentifyCommand.CacheClean();
                goto a;
            }
            catch (Exception exp) // makethis to tolarate CTRL+C and do not quit!!!!
            {
                errs.CacheClean();
                errs.New(exp.ToString());
                errs.ListThem();
                goto a;
            }

            #endregion
        }

    }
}