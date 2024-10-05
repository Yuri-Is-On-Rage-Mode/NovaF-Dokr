using nova.Command;
using nova.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nova_vm.Command.env.doker_lang
{
    public class Interpreter
    {
        public static bool IsStarted = true;

        public class Interpret
        {
            public class Helper
            {
                public static void ThisCode(string Code)
                {
                    //List<string> list = new List<string>();
                    //list = Code.Split(";").ToList();

                    //foreach (string line in list)
                    //{
                    //    Console.Write($"\nEXECUTING: {line}");
                    //    Interpreter.Interpret.main.ThisCodeLineOnly(line);
                    //}
                AssemblyLoadEventArgs:
                    Console.Write("\n> ");

                    List<string> list = new List<string>();
                    list = Console.ReadLine().Split(";").ToList();

                    foreach (string line in list)
                    {
                        Interpreter.Interpret.main.ThisCodeLineOnly(line);
                    }

                    errs.ListThem();
                    errs.CacheClean();

                    goto AssemblyLoadEventArgs;
                }
            }

            public class main
            {
                public static void ThisCodeLineOnly(string CodeLine)
                {
                    var Parts = CodeLine.Split(" ");

                    foreach (var partx in Parts)
                    {
                        for (int i = 0; i < Parts.Length; i++)
                        {
                            if (Parts[i].Contains(")."))
                            {
                                if (Parts[i].StartsWith("$("))
                                {
                                    List<string> OPTS = new List<string>();

                                    OPTS = Parts[i].Split(".").ToList();

                                    var tCmd = OPTS[1];

                                    //foreach (var item in OPTS)
                                    //{
                                    //    Console.WriteLine(item);
                                    //}

                                    List<string> TotalOpts =
                                    [
                                        "toupper",  // implemented
                                "tolower",  // implemented

                                "tostring", // implemented
                                "toint",    // not implemented :: fooling user
                                "tofloat",  // not implemented :: fooling user
                                "todouble", // not implemented :: fooling user
                                "tolong",   // not implemented :: fooling user
                            ];

                                    if (TotalOpts.Contains(tCmd))
                                    {
                                        if (tCmd.ToLower() == "toupper")
                                        {
                                            //Console.WriteLine($"{tCmd}: Found!");
                                            {
                                                Parts[i] = Parts[i].Replace("$(", "");
                                                Parts[i] = Parts[i].Replace(").", "");
                                                Parts[i] = Parts[i].Replace($"{tCmd}", "");

                                                //Console.WriteLine($":: {Parts[i]}");

                                                //novaOutput.result(EnvironmentVariables[Parts[i]]);
                                                try
                                                {
                                                    Parts[i] = Parts[i].Replace(Parts[i], CommandEnv.EnvironmentVariables[Parts[i]]);
                                                    Parts[i] = Parts[i].ToUpper();
                                                }
                                                catch (Exception exept)
                                                {
                                                    //errs.New(exept.ToString());
                                                    errs.New($"The given key '{Parts[i]}' was not present in the env dictionary.");
                                                    errs.ListThem();
                                                    errs.CacheClean();
                                                    return;
                                                }
                                            }
                                        }
                                        else if (tCmd.ToLower() == "tolower")
                                        {
                                            //Console.WriteLine($"{tCmd}: Found!");
                                            {
                                                Parts[i] = Parts[i].Replace("$(", "");
                                                Parts[i] = Parts[i].Replace(").", "");
                                                Parts[i] = Parts[i].Replace($"{tCmd}", "");

                                                //Console.WriteLine($":: {Parts[i]}");

                                                //novaOutput.result(EnvironmentVariables[Parts[i]]);
                                                try
                                                {
                                                    Parts[i] = Parts[i].Replace(Parts[i], CommandEnv.EnvironmentVariables[Parts[i]]);
                                                    Parts[i] = Parts[i].ToLower();
                                                }
                                                catch (Exception exept)
                                                {
                                                    //errs.New(exept.ToString());
                                                    errs.New($"The given key '{Parts[i]}' was not present in the env dictionary.");
                                                    errs.ListThem();
                                                    errs.CacheClean();
                                                    return;
                                                }
                                            }
                                        }
                                        else if (tCmd.ToLower().StartsWith("to"))
                                        {
                                            Parts[i] = Parts[i].Replace("$(", "");
                                            Parts[i] = Parts[i].Replace(").", "");
                                            Parts[i] = Parts[i].Replace($"{tCmd}", "");

                                            //Console.WriteLine($":: {Parts[i]}");

                                            //novaOutput.result(EnvironmentVariables[Parts[i]]);

                                            try
                                            {
                                                Parts[i] = Parts[i].Replace(Parts[i], CommandEnv.EnvironmentVariables[Parts[i]]);
                                            }
                                            catch (Exception exept)
                                            {
                                                //errs.New(exept.ToString());
                                                errs.New($"The given key '{Parts[i]}' was not present in the env dictionary.");
                                                errs.ListThem();
                                                errs.CacheClean();
                                                return;
                                            }

                                            if (tCmd.ToLower() == "tostring")
                                            {
                                                Parts[i] = Parts[i].ToString();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        errs.CacheClean();
                                        errs.New($"`{tCmd}` is not a real command in `{Parts[i]}`");
                                        errs.ListThem();
                                        return;
                                    }
                                }

                            }
                            else
                            {
                                if (Parts[i].StartsWith("$("))
                                {
                                    Parts[i] = Parts[i].Replace("$(", "");
                                    Parts[i] = Parts[i].Replace(")", "");

                                    //Console.WriteLine($":: {Parts[i]}");

                                    //novaOutput.result(EnvironmentVariables[Parts[i]]);
                                    try
                                    {
                                        Parts[i] = Parts[i].Replace(Parts[i], CommandEnv.EnvironmentVariables[Parts[i]]);
                                    }
                                    catch (Exception exept)
                                    {
                                        //errs.New(exept.ToString());
                                        errs.New($"The given key '{Parts[i]}' was not present in the env dictionary.");
                                        errs.ListThem();
                                        errs.CacheClean();
                                        return;
                                    }
                                }

                            }
                        }

                        for (int i = 0; i < Parts.Length; i++)
                        {
                            if (Parts[i].StartsWith("\\"))
                            {
                                Parts[i] = Parts[i].Replace("\\n", "\n");
                                Parts[i] = Parts[i].Replace("\\t", "\t");
                            }
                        }

                        var part = partx.Replace(" ", "");

                        if (part == ".start")
                        {
                            if (!IsStarted)
                            {
                                IsStarted = true;
                                Console.WriteLine("* Starting the program...");
                            }
                            else
                            {
                                errs.New($"The program cannot be started without ending the olderone: `{CodeLine}`\nhint: program is already running, to start anathor one, end the previous one first!");
                            }
                        }
                        else if (part == ".end")
                        {
                            if (IsStarted)
                            {
                                IsStarted = false;
                                Console.WriteLine("* Ending the program...");
                            }
                            else
                            {
                                errs.New($"The program cannot be ended without starting: `{CodeLine}`\nhint: start the program first by typing `START` at the top of the code!");
                            }
                        }
                        else if (part == "print")
                        {
                            if (IsStarted)
                            {
                                for (int i = 0; i < Parts.Length; i++)
                                {
                                    if ((Parts[i] == Parts[0]))
                                    {

                                    }
                                    else
                                    {
                                        Console.Write($" {Parts[i]}");
                                    }
                                }
                            }
                            else
                            {
                                errs.New($"PLEASE START THE PROGRAM FIRST TO RUN ANY TYPE OF COMMAND: `{CodeLine}`\nhint: start the program first by typing `START` at the top of the code!");
                            }
                        }
                        else if (part == "input")
                        {
                            if (IsStarted)
                            {
                                for (int i = 1; i < Parts.Length; i++)
                                {
                                    if ((Parts[i] == Parts[1]) || (Parts[i] == Parts[0]))
                                    {

                                    }
                                    else
                                    {
                                        Console.Write($" {Parts[i]}");
                                    }
                                }
                                string InptValue = Console.ReadLine();
                                CommandEnv.EnvironmentVariables[Parts[1]] = (InptValue);
                                CommandEnv.SaveEnvironmentVariables();
                            }
                            else
                            {
                                errs.New($"PLEASE START THE PROGRAM FIRST TO RUN ANY TYPE OF COMMAND: `{CodeLine}`\nhint: start the program first by typing `START` at the top of the code!");
                            }
                        }
                        else if (part == "set")
                        {
                            if (IsStarted)
                            {
                                string wholething = "";
                                for (int i = 1; i < Parts.Length; i++)
                                {
                                    if ((Parts[i] == Parts[1]) || (Parts[i] == Parts[0]))
                                    {

                                    }
                                    else
                                    {
                                        wholething = wholething + " " + Parts[i];
                                    }
                                }
                                CommandEnv.EnvironmentVariables[Parts[1]] = (wholething);
                                CommandEnv.SaveEnvironmentVariables();
                            }
                            else
                            {
                                errs.New($"PLEASE START THE PROGRAM FIRST TO RUN ANY TYPE OF COMMAND: `{CodeLine}`\nhint: start the program first by typing `START` at the top of the code!");
                            }
                        }
                        else if (Parts[0].Equals("do"))
                        {
                            if (IsStarted)
                            {
                                int loopCount;

                                // Try parsing the loop count from the second part
                                if (int.TryParse(Parts[1], out loopCount) && loopCount > 0 && loopCount <= 1_000_000_000_000)
                                {
                                    // Extract the code block inside the curly braces
                                    int startIndex = CodeLine.IndexOf("{") + 1;
                                    int endIndex = CodeLine.LastIndexOf("}");

                                    if (startIndex > 0 && endIndex > startIndex)
                                    {
                                        // Clean up the code block by trimming unnecessary spaces
                                        string loopCode = CodeLine.Substring(startIndex, endIndex - startIndex).Trim();

                                        // Execute the code block for the specified number of times
                                        for (int i = 0; i < loopCount; i++)
                                        {
                                            // Run only the code inside the block
                                            Interpreter.Interpret.main.ThisCodeLineOnly(loopCode);
                                        }
                                    }
                                    else
                                    {
                                        errs.New("Error: Missing or improperly formatted code block. Make sure to use `{}` around the code to execute.");
                                    }
                                }
                                else
                                {
                                    errs.New($"Invalid loop count '{Parts[1]}'. It should be a number from 1 to `1_000_000_000_000`.");
                                }
                            }
                            else
                            {
                                errs.New($"PLEASE START THE PROGRAM FIRST TO RUN ANY TYPE OF COMMAND: `{CodeLine}`\nhint: start the program first by typing `START` at the top of the code!");
                            }
                        }
                        //else
                        //{
                        //    errs.New($"WHAT IS THIS: `{CodeLine}`");
                        //}
                    }
                }
            }
        }
    }
}
