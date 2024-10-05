using nova.Command;
using nova.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nova_vm.Command.env.nova_lang
{
    public class Compiler
    {
        public class GlitchInTheMatrix
        {
            public static void Show()
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.BackgroundColor = ConsoleColor.Red;

                Console.WriteLine("*** GLITCH IN THE MATRIX ***");

                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
            }
        }
        public class KeyWords
        {
            public static string _Start = ".start";
            public static string _End = ".end";

            public static string h_add = "#add";
        }
        public class Bytecode
        {
            public class Generate
            {
                public class From
                {
                    public static List<string> RichText( string code )
                    {
                        List<string> Bytecode = new List<string>();

                        List<string> CodeBlocks = code.Split(";").ToList();

                        Bytecode = Generate.Interpreter.doo(CodeBlocks);

                        return Bytecode;
                    }
                }

                public class Interpreter
                {
                    public static List<string> doo(List<string> CodeBlocks )
                    {
                        List<string> Bytecode = new List<string>();

                        bool isStarted = false;

                        int xx = 0;

                        foreach ( string cblock in CodeBlocks )
                        {

                            var parts = cblock.Split(' ');

                            // Check if it is a get env arg value ` $(var_name) `
                            //// List<string> parts = new List<string>();
                            //// parts = ["@bin", "ls", "--path", "$(BinPath)"];
                            for (int i = 0; i < parts.Length; i++)
                            {
                                if (parts[i].Contains(")."))
                                {
                                    if (parts[i].StartsWith("$("))
                                    {
                                        List<string> OPTS = new List<string>();

                                        OPTS = parts[i].Split(".").ToList();

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
                                                    parts[i] = parts[i].Replace("$(", "");
                                                    parts[i] = parts[i].Replace(").", "");
                                                    parts[i] = parts[i].Replace($"{tCmd}", "");

                                                    //Console.WriteLine($":: {parts[i]}");

                                                    //novaOutput.result(EnvironmentVariables[parts[i]]);
                                                    try
                                                    {
                                                        parts[i] = parts[i].Replace(parts[i], CommandEnv.EnvironmentVariables[parts[i]]);
                                                        parts[i] = parts[i].ToUpper();
                                                    }
                                                    catch (Exception exept)
                                                    {
                                                        //errs.New(exept.ToString());
                                                        errs.New($"The given key '{parts[i]}' was not present in the env dictionary.");
                                                        errs.ListThem();
                                                        errs.CacheClean();
                                                        //return;
                                                    }
                                                }
                                            }
                                            else if (tCmd.ToLower() == "tolower")
                                            {
                                                //Console.WriteLine($"{tCmd}: Found!");
                                                {
                                                    parts[i] = parts[i].Replace("$(", "");
                                                    parts[i] = parts[i].Replace(").", "");
                                                    parts[i] = parts[i].Replace($"{tCmd}", "");

                                                    //Console.WriteLine($":: {parts[i]}");

                                                    //novaOutput.result(EnvironmentVariables[parts[i]]);
                                                    try
                                                    {
                                                        parts[i] = parts[i].Replace(parts[i], CommandEnv.EnvironmentVariables[parts[i]]);
                                                        parts[i] = parts[i].ToLower();
                                                    }
                                                    catch (Exception exept)
                                                    {
                                                        //errs.New(exept.ToString());
                                                        errs.New($"The given key '{parts[i]}' was not present in the env dictionary.");
                                                        errs.ListThem();
                                                        errs.CacheClean();
                                                        //return;
                                                    }
                                                }
                                            }
                                            else if (tCmd.ToLower().StartsWith("to"))
                                            {
                                                parts[i] = parts[i].Replace("$(", "");
                                                parts[i] = parts[i].Replace(").", "");
                                                parts[i] = parts[i].Replace($"{tCmd}", "");

                                                //Console.WriteLine($":: {parts[i]}");

                                                //novaOutput.result(EnvironmentVariables[parts[i]]);

                                                try
                                                {
                                                    parts[i] = parts[i].Replace(parts[i], CommandEnv.EnvironmentVariables[parts[i]]);
                                                }
                                                catch (Exception exept)
                                                {
                                                    //errs.New(exept.ToString());
                                                    errs.New($"The given key '{parts[i]}' was not present in the env dictionary.");
                                                    errs.ListThem();
                                                    errs.CacheClean();
                                                    //return;
                                                }

                                                if (tCmd.ToLower() == "tostring")
                                                {
                                                    parts[i] = parts[i].ToString();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            errs.CacheClean();
                                            errs.New($"`{tCmd}` is not a real command in `{parts[i]}`");
                                            errs.ListThem();
                                            //return;
                                        }
                                    }

                                }
                                else
                                {
                                    if (parts[i].StartsWith("$("))
                                    {
                                        parts[i] = parts[i].Replace("$(", "");
                                        parts[i] = parts[i].Replace(")", "");

                                        //Console.WriteLine($":: {parts[i]}");

                                        //novaOutput.result(EnvironmentVariables[parts[i]]);
                                        try
                                        {
                                            parts[i] = parts[i].Replace(parts[i], CommandEnv.EnvironmentVariables[parts[i]]);
                                        }
                                        catch (Exception exept)
                                        {
                                            //errs.New(exept.ToString());
                                            errs.New($"The given key '{parts[i]}' was not present in the env dictionary.");
                                            errs.ListThem();
                                            errs.CacheClean();
                                            //return;
                                        }
                                    }

                                }
                            }

                            for (int i = 0; i < parts.Length; i++)
                            {
                                if (parts[i].StartsWith("\\"))
                                {
                                    parts[i] = parts[i].Replace("\\n", "\n");
                                    parts[i] = parts[i].Replace("\\t", "\t");
                                }
                            }

                            if (cblock == "\n" || cblock == "" || cblock == " " || cblock == "" || cblock == "\n" || cblock == string.Empty) { }
                            else if (cblock == KeyWords._Start)
                            {
                                if (isStarted)
                                {
                                    errs.New($"the program is started, cannot be started again!: \" {xx}: {cblock} \"");
                                    errs.ListThem();
                                    errs.CacheClean();
                                }
                                else if (!isStarted)
                                {
                                    isStarted = true;
                                }
                                else
                                {
                                    GlitchInTheMatrix.Show();
                                }
                            }
                            else if (cblock == KeyWords._End)
                            {
                                if (!isStarted)
                                {
                                    errs.New($"the program is not started, cannot be ended!: \" {xx}: {cblock} \"");
                                    errs.ListThem();
                                    errs.CacheClean();
                                }
                                else if (isStarted)
                                {
                                    isStarted = false;
                                }
                                else
                                {
                                    GlitchInTheMatrix.Show();
                                }
                            }
                            else
                            {
                                var Parts = cblock.Split(' ');

                                for (var i = 0; i < Parts.Length; i++)
                                {
                                    if (Parts[i] == KeyWords.h_add)
                                    {
                                        Parts[i].Remove(0);

                                        long total = 0;

                                        for (var j = 0; j <= Parts[i].Length; j++)
                                        {
                                            if (Parts[j].ToLower() == "ret")
                                            {
                                                Console.WriteLine(total);
                                            }
                                            else
                                            {
                                                total = total + 1; //Parts[i];
                                            }
                                        }

                                        Console.WriteLine(total);
                                    }
                                }
                            }
                            xx++;
                        }

                        return Bytecode;
                    }

                    /*public static List<string> gen(List<string> CodeBlocks )
                    {
                        List<string> Bytecode = new List<string>();

                        int inside = 0;
                        string insideblock = "";

                        foreach (string cblock in CodeBlocks)
                        {
                            if (cblock == "\n" || cblock == "" || cblock == " " || cblock == "" || cblock == "\n" || cblock == string.Empty)
                            {
                                //inside = inside + 1;
                                //insideblock = insideblock + "..";
                                //Bytecode.Add($"{insideblock}{cblock}");
                            }
                            else if (cblock.ToLower().StartsWith("{") || cblock.ToLower().StartsWith("func") || cblock.ToLower().StartsWith("conf"))
                            {
                                inside = inside + 1;

                                cblock.Replace("{","do");
                                
                                Bytecode.Add($"{insideblock}{cblock}");

                                insideblock = insideblock + "..";
                            }
                            else if (cblock.ToLower().StartsWith("}"))
                            {
                                inside = inside - 1;

                                
                                insideblock = "";

                                for (var i = 0; i <= inside - 1; i++)
                                {
                                    insideblock = insideblock + "..";
                                }

                                cblock.Replace("}", "rn");

                                Bytecode.Add($"{insideblock}{cblock}");


                            }
                            else
                            {
                                Bytecode.Add($"{insideblock}{cblock}");
                            }
                        }

                        return Bytecode;
                    }*/
                }
            }
        }
    }
}
