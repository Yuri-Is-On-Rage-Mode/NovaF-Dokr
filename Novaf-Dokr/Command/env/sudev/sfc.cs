using nova.Command;
using nova.Command.env;
using nova.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static nova.Command.env.shel_scripting_lang.ByteCode.ByteCodeVM.Process.ByteCode;
using static nova_vm.Command.env.sudev.SudevConsole.Configurations;

namespace nova_vm.Command.env.sudev
{
    public class sfc // Sudev Framework Console
    {
        public static void doit(string[] parts)
        {
            SudevConsole.Configurations.AbortOrContinue(
                SudevConsole.Configurations.Process(
                    SudevConsole.Configurations.AreAllOkay()
                )
            );

            SudevConsole.PasswordForSudev.AbortOrContinue(
                SudevConsole.PasswordForSudev.Process(
                    SudevConsole.PasswordForSudev.Take()
                )
            );

            if (parts.Length > 1)
            {
                if (parts[1] == "conf")
                {
                    SudevConsole.Configurations.Manager.CommandEnvEnvCommand(parts);
                }
                else if (parts[1] == "do")
                {
                    SudevConsole.Works.CEEC(parts);
                }
            }

        }
    }
    public class SudevConsole
    {
        public static bool IsLoggedInPermentently = true;
        public static string CurrectSudevPassword = "passcodeispassword";

        public static bool IsPasswdNeeded = true;

        private static string UserHomeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        public static string PathToConf_g_dir = Path.Combine(UserHomeDir, "vin_env", "conf.g.dir");
        public static string PathToConf_g = Path.Combine(PathToConf_g_dir, "conf.g");

        private static Dictionary<string, string> EnvironmentConfigurations = new Dictionary<string, string>();

        public class Configurations
        {
            public static bool AreAllOkay()
            { 
                bool StatusOnline = false;

                try
                {
                    if (!Directory.Exists(PathToConf_g_dir))
                    {
                        errs.New("Error: Sudev directory does not exist. Creating directory at " + PathToConf_g_dir);
                        Directory.CreateDirectory(PathToConf_g_dir);
                    }

                    if (!File.Exists(PathToConf_g))
                    {
                        errs.New("Error: `@sudev/conf.g` file not present. Creating file.");
                        File.WriteAllText(PathToConf_g, "{}");
                    }

                    try
                    {
                        LoadEnvironmentConfigurations();
                    }
                    catch (Exception exxp)
                    {
                        errs.New($"Error: While loading `conf.g`: {exxp}");
                    }

                    try
                    {
                        SaveEnvironmentConfigurations();
                    }
                    catch (Exception exxp)
                    {
                        errs.New($"Error: While saving `conf.g`: {exxp}");
                    }

                    errs.ListThem();
                    errs.CacheClean();
                }
                catch (Exception ex)
                {
                    errs.New($"Error creating sudev environment setup: {ex.Message}");
                    errs.ListThem();
                    errs.CacheClean();
                }

                return IsPasswdNeeded; 
            }
            private static void SaveEnvironmentConfigurations()
            {
                File.WriteAllText(PathToConf_g, JsonSerializer.Serialize(EnvironmentConfigurations, new JsonSerializerOptions { WriteIndented = true }));
            }
            private static void LoadEnvironmentConfigurations()
            {
                try
                {
                    if (File.Exists(PathToConf_g))
                    {
                        string json = File.ReadAllText(PathToConf_g);
                        EnvironmentConfigurations = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                    }
                }
                catch (Exception ex)
                {
                    errs.New($"Error loading environment configurations: {ex.Message}");
                    errs.ListThem();
                    errs.CacheClean();
                }
            }
            public static bool Process(bool okay)
            {
                bool isOkay = okay;


                return isOkay;
            }
            public static void AbortOrContinue(bool isOkay)
            {
                if (isOkay)
                {
                    novaOutput.stdout("+++ Sudev Env Setup: Successfull");
                }
                else
                {
                    novaOutput.stdoutfailed("--- Sudev Env Setup: Failed");

                }
            }

            public class Manager
            {
                public static void CommandEnvEnvCommand(string[] parts)
                {
                    //if (parts.Length < 3) Console.WriteLine("Not Enough Arguments"); return;

                    switch (parts[2].ToLower())
                    {
                        case "set":
                            if (parts.Length >= 5)
                            {
                                EnvironmentConfigurations[parts[3]] = string.Join(" ", parts.Skip(4));
                                SaveEnvironmentConfigurations();
                                //Console.WriteLine($"Environment variable '{parts[2]}' set.");
                                novaOutput.result($"Environment variable '{parts[3]}' set.");
                            }
                            break;
                        case "get":
                            if (parts.Length >= 4 && EnvironmentConfigurations.ContainsKey(parts[3]))
                            {
                                novaOutput.result(EnvironmentConfigurations[parts[2]]);
                            }
                            else
                            {
                                errs.New($"Environment variable '{parts[3]}' not found.");
                                errs.ListThem();
                                errs.CacheClean();
                            }
                            break;
                        case "ls":
                            if (EnvironmentConfigurations.Count == 0)
                            {
                                errs.New("No environment configurations set.");
                                errs.ListThem();
                                errs.CacheClean();
                            }
                            else
                            {
                                foreach (var kvp in EnvironmentConfigurations)
                                {
                                    Console.WriteLine($" {kvp.Key} ==> {kvp.Value}");
                                }
                            }
                            break;
                        case "rm":
                            if (parts.Length >= 4)
                            {
                                if (EnvironmentConfigurations.Remove(parts[3]))
                                {
                                    SaveEnvironmentConfigurations();
                                    Console.WriteLine($"Environment variable '{parts[3]}' removed.");
                                }
                                else
                                {
                                    errs.New($"Environment variable '{parts[3]}' not Found.");
                                    errs.ListThem();
                                    errs.CacheClean();
                                }
                            }
                            break;
                        default:
                            errs.New("Invalid `@sudev conf` command. Use 'set', 'get', 'ls', or 'rm'.");
                            errs.ListThem();
                            errs.CacheClean();
                            break;
                    }
                }
            }
        }
        public class PasswordForSudev
        {
            public static string Take()
            {
                if (!IsLoggedInPermentently)
                {
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.Write("Enter password for sudev:");

                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Black;
                    string passwd = Console.ReadLine();

                    Console.ForegroundColor = ConsoleColor.White;

                    return passwd;
                }
                else 
                {
                    return CurrectSudevPassword;
                }
            }
            public static bool Process(string password)
            {
                bool isOkay = false;

                if (password == CurrectSudevPassword)
                {
                    isOkay = true;
                    IsLoggedInPermentently = true;
                }

                return isOkay;
            }
            public static void AbortOrContinue(bool isOkay)
            {
                if (!IsLoggedInPermentently)
                {
                    if (isOkay)
                    {
                        novaOutput.stdout("+++ Sudev login: Successfull");
                    }
                    else
                    {
                        novaOutput.stdoutfailed("--- Sudev login: Failed");

                    }
                }
                else
                {

                }
            }
        }
        public class Works
        {
            public static void CEEC(string[] parts)
            {
                //if (parts.Length < 3) Console.WriteLine("Not Enough Arguments"); return;

                switch (parts[2].ToLower())
                {
                    case "run":
                        string thing = parts[3];
                        if (thing.StartsWith("sys:") || thing.StartsWith("s:"))
                        {
                            RunSomeStuff.Scripts.Run(thing.Replace("sys:", "").Replace("s:", ""));
                        }
                        else if (thing.StartsWith("nova:") || thing.StartsWith("c:"))
                        {
                            RunSomeStuff.novaScripts.Run(thing.Replace("nova:", "").Replace("c:", ""));
                        }
                        else if (thing.StartsWith("dokr:") || thing.StartsWith("d:"))
                        {
                            try
                            {
                                doker_lang.Interpreter.Interpret.Helper.ThisCode(File.ReadAllText(thing.Replace("dokr:", "").Replace("d:", "")));
                            }
                            catch (Exception exx)
                            {
                                errs.CacheClean();
                                errs.New($"Error reading file `{thing.Replace("dokr:", "").Replace("d:", "")}`");
                                errs.ListThem();
                                errs.CacheClean();
                            }    
                        }
                        else
                        {
                            if (!File.Exists(thing))
                            {
                                errs.CacheClean();
                                errs.New($"Error: `{thing}` file not present.");
                                errs.ListThem();
                                errs.CacheClean();
                            }
                            else
                            {
                                {
                                    IdentifyCommand.CacheClean();

                                    List<string> commandsz = UserInput.Prepare(File.ReadAllText(thing));

                                    IdentifyCommand.Identify(commandsz);
                                    List<string> parsed_commandsz = IdentifyCommand.ReturnThemPlease();

                                    //foreach (string command in parsed_commands) { Console.WriteLine(command); }

                                    PleaseCommandEnv.TheseCommands(parsed_commandsz);

                                    IdentifyCommand.CacheClean();
                                }
                            }
                        }
                        break;
                    default:
                        errs.New("Invalid `@sudev do` command. Use 'run'.");
                        errs.ListThem();
                        errs.CacheClean();
                        break;
                }
            }

            public class RunSomeStuff
            {
                public class Scripts
                { 
                    public static void Run(string path_to_script)
                    {
                        if (!File.Exists(path_to_script))
                        {
                            errs.CacheClean();
                            errs.New($"Error: `{path_to_script}` file not present.");
                            errs.ListThem();
                            errs.CacheClean();
                        }
                        else
                        {
                            IdentifyCommand.CacheClean();

                            List<string> commands = UserInput.Prepare(File.ReadAllText(path_to_script));

                            IdentifyCommand.Identify(commands);
                            List<string> parsed_commands = IdentifyCommand.ReturnThemPlease();

                            //foreach (string command in parsed_commands) { Console.WriteLine(command); }

                            PleaseCommandEnv.TheseCommands(parsed_commands);

                            IdentifyCommand.CacheClean();
                        }
                    }
                }
                public class novaScripts
                {
                    public static void Run(string path_to_script)
                    {
                        if (!File.Exists(path_to_script))
                        {
                            errs.CacheClean();
                            errs.New($"Error: `{path_to_script}` file not present.");
                            errs.ListThem();
                            errs.CacheClean();
                        }
                        else
                        {
                            int x = 0;
                            foreach (var line in (nova_lang.Compiler.Bytecode.Generate.From.RichText(File.ReadAllText(path_to_script))))
                            {
                                Console.WriteLine($"{x}: {line}");
                                x++;
                            }
                        }
                    }
                }
            }
        }
    }
}
