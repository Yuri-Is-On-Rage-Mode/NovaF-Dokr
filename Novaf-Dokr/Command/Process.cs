using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.IO;

using nova.Utils;
using nova.Command;

using System.Diagnostics;
using System.ComponentModel.Design;
using System.Text.RegularExpressions;
using Novaf_Dokr.Command.env.user;

namespace nova.Command
{

    internal class CommandEnv
    {
        public static string CURRENT_USER_NAME = "anonymous";
        public static string CURRENT_NODE_NAME = "127.0.0.1";


        public static string CurrentDirDest = UserHomeDir;

        public static string UserHomeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public static string novaEnvDir = Path.Combine(UserHomeDir, "vin_env", "vars");
        public static string EnvVarsFile = Path.Combine(novaEnvDir, "env_vars.json");
        public static string AliasesFile = Path.Combine(novaEnvDir, "aliases.json");

        public static List<string> DecoraterCommands = new List<string>();
        public static List<string> AlertCommands = new List<string>();
        public static Dictionary<string, string> EnvironmentVariables = new Dictionary<string, string>();
        public static Dictionary<string, string> Aliases = new Dictionary<string, string>();
        public static List<string> CommandHistory = new List<string>();

        static CommandEnv()
        {
            CurrentDirDest = Environment.CurrentDirectory;
            EnsureEnvironmentSetup();
            LoadEnvironmentVariables();
            LoadAliases();
        }
        // Ensure the directory and files exist, or create them after Listing an error
        // Ensure the directory and files exist, or create them after Listing an error
        public static void EnsureEnvironmentSetup()
        {
            try
            {
                if (!Directory.Exists(novaEnvDir))
                {
                    errs.New("Error: Environment directory does not exist. Creating directory at " + novaEnvDir);
                    Directory.CreateDirectory(novaEnvDir);
                }

                if (!File.Exists(EnvVarsFile))
                {
                    errs.New("Error: env_vars.json file not found. Creating file.");
                    File.WriteAllText(EnvVarsFile, "{}");
                }

                if (!File.Exists(AliasesFile))
                {
                    errs.New("Error: aliases.json file not found. Creating file.");
                    File.WriteAllText(AliasesFile, "{}");
                }

                errs.ListThem();
                errs.CacheClean();
            }
            catch (Exception ex)
            {
                errs.New($"Error creating environment setup: {ex.Message}");
                errs.ListThem();
                errs.CacheClean();
            }
        }
        public static void SaveEnvironmentVariables()
        {
            File.WriteAllText(EnvVarsFile, JsonSerializer.Serialize(EnvironmentVariables, new JsonSerializerOptions { WriteIndented = true }));
        }
        public static void LoadEnvironmentVariables()
        {
            try
            {
                if (File.Exists(EnvVarsFile))
                {
                    string json = File.ReadAllText(EnvVarsFile);
                    EnvironmentVariables = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                }
            }
            catch (Exception ex)
            {
                errs.New($"Error loading environment variables: {ex.Message}");
                errs.ListThem();
                errs.CacheClean();
            }
        }
        public static void SaveAliases()
        {
            File.WriteAllText(AliasesFile, JsonSerializer.Serialize(Aliases, new JsonSerializerOptions { WriteIndented = true }));
        }
        public static void LoadAliases()
        {
            try
            {
                if (File.Exists(AliasesFile))
                {
                    string json = File.ReadAllText(AliasesFile);
                    Aliases = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                }
            }
            catch (Exception ex)
            {
                errs.New($"Error loading aliases: {ex.Message}");
                errs.ListThem();
                errs.CacheClean();
            }
        }
        public static List<List<string>> DecoCommands(List<string> commands)
        {
            DecoraterCommands.Clear();

            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].StartsWith("@"))
                {
                    if (!commands[i].Equals("@"))
                    {
                        DecoraterCommands.Add(commands[i]);
                        commands.RemoveAt(i);
                        i--;
                    }
                    else if (commands[i].Equals("@"))
                    {
                        commands.RemoveAt(i);
                        i--;
                    }
                }
            }

            return new List<List<string>> { DecoraterCommands, commands };
        }
        public static List<string> SeperateThemCommands(List<string> commands)
        {
            List<string> SepCommands = new List<string>();
            StringBuilder currentCommand = new StringBuilder();

            foreach (var command in commands)
            {
                if (command == ";")
                {
                    SepCommands.Add(currentCommand.ToString().Trim());
                    currentCommand.Clear();
                }
                else
                {
                    if (currentCommand.Length > 0)
                    {
                        currentCommand.Append(" ");
                    }
                    currentCommand.Append(command);
                }
            }

            if (currentCommand.Length > 0)
            {
                SepCommands.Add(currentCommand.ToString().Trim());
            }

            return SepCommands;
        }
        public static void ProcessBinCommand(string[] parts)
        {
            if (parts[0] == "@bin")
            {
                if (parts.Length == 1 && parts[0] == "@bin")
                {
                    errs.New($"Usage: @bin <command> - Display file contents in binary format");
                    errs.ListThem();
                    errs.CacheClean();
                    return;
                }

                string executable = parts.Length > 1 ? parts[1] : parts[0];
                string[] args = parts.Skip(2).ToArray();

                string filePath = $"C:\\Users\\{Environment.UserName}\\vin_env\\bin\\{executable}\\{executable}.exe";
                try
                {
                    using (Process process = Process.Start(filePath, string.Join(" ", args)))
                    {
                        process.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    errs.New($"Error starting process: {ex.Message}");
                    errs.ListThem();
                    errs.CacheClean();
                }
            }
            else
            {
                string executable = parts.Length > 1 ? parts[0] : parts[0];
                string[] args = parts.Skip(1).ToArray();

                string filePath = $"C:\\Users\\{Environment.UserName}\\vin_env\\bin\\{executable}\\{executable}.exe";
                try
                {
                    using (Process process = Process.Start(filePath, string.Join(" ", args)))
                    {
                        process.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    errs.New($"Error starting process: {ex.Message}");
                    errs.ListThem();
                    errs.CacheClean();
                }
            }
        }
        public static void ProcessIbinCommand(string[] parts)
        {

            //parts[0] = "";
            // Print the command parts for debugging purposes
            //Console.WriteLine($"PARTS: len: {parts.Length}");
            //foreach (var abc in parts)
            //{
            //    Console.WriteLine(abc);
            //}

            // Ensure the first part (parts[0]) is in the <executable>:<file> format
            if (parts.Length < 2 || !parts[1].Contains(":"))
            {
                errs.New($"Usage: @ibin <executable>:<file> <args> - Run binary files with arguments");
                //errs.New($"YourCommand:");
                //int x = 0;
                //foreach (var item in parts)
                //{
                //    errs.New($"{x}: Expected: `python:abc.py` found: `{item}`");
                //    x = x + 1;
                //}
                errs.ListThem();
                errs.CacheClean();
                return;
            }

            try
            {
                // Split the executable and file part (e.g., python:abc.py)
                string[] execFileParts = parts[1].Split(':');
                if (execFileParts.Length != 2)
                {
                    errs.New("Invalid command format. Expected format: <executable>:<file>");
                    //errs.New($"YourCommand:");
                    //int x = 0;
                    //foreach (var item in parts)
                    //{
                    //    errs.New($"{x}: Expected: `python:abc.py` found: `{item}`");
                    //    x = x + 1;
                    //}
                    errs.ListThem();
                    errs.CacheClean();
                    return;
                }

                // Extract the executable (e.g., "python") and file (e.g., "abc.py")
                string executable = execFileParts[0];  // e.g., "python"
                string fileName = execFileParts[1];    // e.g., "abc.py"

                // Combine the rest of the arguments (if any) (e.g., arg1, arg2)
                string[] args = parts.Length > 2 ? parts.Skip(1).ToArray() : new string[1];  // Skip the first part

                // Construct the full path for the script in the ibin directory
                string filePath = Path.Combine($"C:\\Users\\{Environment.UserName}\\vin_env\\ibin\\{executable}", fileName);

                // Check if the file exists
                if (!File.Exists(filePath))
                {
                    errs.New($"Error: File '{fileName}' not found in {filePath}");
                    errs.ListThem();
                    errs.CacheClean();
                    return;
                }

                bool noshell = false;

                foreach (var arg in parts)
                {
                    if (arg.Contains("--no-shell"))
                    {
                        arg.Replace("--no-shell", "");
                        noshell = true;
                    }
                }

                if (noshell)
                {
                    //arg.Replace("--no-shell", "");
                    // Prepare the process to execute the file
                    ProcessStartInfo processStartInfo = new ProcessStartInfo
                    {
                        FileName = executable,  // e.g., "python"
                        Arguments = $"\"{filePath}\" " + string.Join(" ", args),  // Script path and arguments
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    // Start the process
                    using (Process process = Process.Start(processStartInfo))
                    {
                        // Capture the output and error streams
                        string output = process.StandardOutput.ReadToEnd();
                        string error = process.StandardError.ReadToEnd();
                        process.WaitForExit();

                        // Display the output or error
                        if (!string.IsNullOrEmpty(output))
                        {
                            Console.WriteLine(output);
                        }

                        if (!string.IsNullOrEmpty(error))
                        {
                            errs.New($"Error: {error}");
                            errs.ListThem();
                            errs.CacheClean();
                        }
                    }
                }
                else
                {
                    RunOnWindows.RunPythonFile([$"{filePath} " + string.Join(" ", args)]);
                }
            }
            catch (Exception ex)
            {
                // Handle general errors during process execution
                errs.New($"Error starting process: {ex.Message}");
                errs.ListThem();
                errs.CacheClean();
            }
        }
        public static string GetExtension(string executable)
        {
            switch (executable)
            {
                case "py":
                    return "py";
                case "roobi":
                    return "rb";
                default:
                    throw new ArgumentException("Unsupported executable");
            }
        }
        public static string ReplaceEnvironmentVariables(string input)
        {
            return Regex.Replace(input, @"\$(\w+)|\$\((\w+)\)", match =>
            {
                string key = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
                return EnvironmentVariables.TryGetValue(key, out string value) ? value : match.Value;
            });
        }
        public static void CommandEnvEach(List<string> commands)
        {
            if (!commands.Any()) { return; }

            foreach (var command in commands)
            {
                CommandHistory.Add(command);
                var parts = command.Split(' ');

                // Check if it is a get env arg value ` $(var_name) `
                //// List<string> parts = new List<string>();
                //// parts = ["@bin", "ls", "--path", "$(BinPath)"];
                for (int i = 0; i < parts.Length; i++)
                {
                    parts[i] = ReplaceEnvironmentVariables(parts[i]);
                    //if (parts[i].Contains(")."))
                    //{
                    //    if (parts[i].StartsWith("$("))
                    //    {
                    //        List<string> OPTS = new List<string>();

                    //        OPTS = parts[i].Split(".").ToList();

                    //        var tCmd = OPTS[1];

                    //        //foreach (var item in OPTS)
                    //        //{
                    //        //    Console.WriteLine(item);
                    //        //}

                    //        List<string> TotalOpts =
                    //        [
                    //            "toupper",  // implemented
                    //            "tolower",  // implemented

                    //            "tostring", // implemented
                    //            "toint",    // not implemented :: fooling user
                    //            "tofloat",  // not implemented :: fooling user
                    //            "todouble", // not implemented :: fooling user
                    //            "tolong",   // not implemented :: fooling user
                    //        ];

                    //        if (TotalOpts.Contains(tCmd))
                    //        {
                    //            if (tCmd.ToLower() == "toupper")
                    //            {
                    //                //Console.WriteLine($"{tCmd}: Found!");
                    //                {
                    //                    parts[i] = parts[i].Replace("$(", "");
                    //                    parts[i] = parts[i].Replace(").", "");
                    //                    parts[i] = parts[i].Replace($"{tCmd}", "");

                    //                    //Console.WriteLine($":: {parts[i]}");

                    //                    //novaOutput.result(EnvironmentVariables[parts[i]]);
                    //                    try
                    //                    {
                    //                        parts[i] = parts[i].Replace(parts[i], EnvironmentVariables[parts[i]]);
                    //                        parts[i] = parts[i].ToUpper();
                    //                    }
                    //                    catch (Exception exept)
                    //                    {
                    //                        //errs.New(exept.ToString());
                    //                        errs.New($"The given key '{parts[i]}' was not present in the env dictionary.");
                    //                        errs.ListThem();
                    //                        errs.CacheClean();
                    //                        return;
                    //                    }
                    //                }
                    //            }
                    //            else if (tCmd.ToLower() == "tolower")
                    //            {
                    //                //Console.WriteLine($"{tCmd}: Found!");
                    //                {
                    //                    parts[i] = parts[i].Replace("$(", "");
                    //                    parts[i] = parts[i].Replace(").", "");
                    //                    parts[i] = parts[i].Replace($"{tCmd}", "");

                    //                    //Console.WriteLine($":: {parts[i]}");

                    //                    //novaOutput.result(EnvironmentVariables[parts[i]]);
                    //                    try
                    //                    {
                    //                        parts[i] = parts[i].Replace(parts[i], EnvironmentVariables[parts[i]]);
                    //                        parts[i] = parts[i].ToLower();
                    //                    }
                    //                    catch (Exception exept)
                    //                    {
                    //                        //errs.New(exept.ToString());
                    //                        errs.New($"The given key '{parts[i]}' was not present in the env dictionary.");
                    //                        errs.ListThem();
                    //                        errs.CacheClean();
                    //                        return;
                    //                    }
                    //                }
                    //            }
                    //            else if (tCmd.ToLower().StartsWith("to"))
                    //            {
                    //                parts[i] = parts[i].Replace("$(", "");
                    //                parts[i] = parts[i].Replace(").", "");
                    //                parts[i] = parts[i].Replace($"{tCmd}", "");

                    //                //Console.WriteLine($":: {parts[i]}");

                    //                //novaOutput.result(EnvironmentVariables[parts[i]]);

                    //                try
                    //                {
                    //                    parts[i] = parts[i].Replace(parts[i], EnvironmentVariables[parts[i]]);
                    //                }
                    //                catch (Exception exept)
                    //                {
                    //                    //errs.New(exept.ToString());
                    //                    errs.New($"The given key '{parts[i]}' was not present in the env dictionary.");
                    //                    errs.ListThem();
                    //                    errs.CacheClean();
                    //                    return;
                    //                }

                    //                if (tCmd.ToLower() == "tostring")
                    //                {
                    //                    parts[i] = parts[i].ToString();
                    //                }
                    //            }
                    //        }
                    //        else
                    //        {
                    //            errs.CacheClean();
                    //            errs.New($"`{tCmd}` is not a real command in `{parts[i]}`");
                    //            errs.ListThem();
                    //            return;
                    //        }
                    //    }

                    //}
                    //else
                    //{
                    //    if (parts[i].StartsWith("$("))
                    //    {
                    //        parts[i] = parts[i].Replace("$(", "");
                    //        parts[i] = parts[i].Replace(")", "");

                    //        //Console.WriteLine($":: {parts[i]}");

                    //        //novaOutput.result(EnvironmentVariables[parts[i]]);
                    //        try
                    //        {
                    //            parts[i] = parts[i].Replace(parts[i], EnvironmentVariables[parts[i]]);
                    //        }
                    //        catch (Exception exept)
                    //        {
                    //            //errs.New(exept.ToString());
                    //            errs.New($"The given key '{parts[i]}' was not present in the env dictionary.");
                    //            errs.ListThem();
                    //            errs.CacheClean();
                    //            return;
                    //        }
                    //    }
                    //    else if (parts[i].StartsWith("$"))
                    //    {
                    //        parts[i] = parts[i].Replace("$", "");

                    //        //Console.WriteLine($":: {parts[i]}");

                    //        //novaOutput.result(EnvironmentVariables[parts[i]]);
                    //        try
                    //        {
                    //            parts[i] = parts[i].Replace(parts[i], EnvironmentVariables[parts[i]]);
                    //        }
                    //        catch (Exception exept)
                    //        {
                    //            //errs.New(exept.ToString());
                    //            errs.New($"The given key '{parts[i]}' was not present in the env dictionary.");
                    //            errs.ListThem();
                    //            errs.CacheClean();
                    //            return;
                    //        }
                    //    }
                    //    else if (parts[i].StartsWith("$"))
                    //    {
                    //        // Remove the '$' to get the variable name
                    //        var vname = parts[i].Replace("$", "");

                    //        // Attempt to retrieve the variable value from the environment variables
                    //        if (EnvironmentVariables.ContainsKey(vname))
                    //        {
                    //            var basePath = EnvironmentVariables[vname]; // Get the base path

                    //            // Check if additional path segments exist
                    //            if (parts.Length > 1 && parts[i + 1].StartsWith("/"))
                    //            {
                    //                // Combine base path with the additional segments
                    //                parts[i] = Path.Combine(basePath, parts[i + 1].Substring(1)); // Remove leading '/' for concatenation
                    //            }
                    //            else
                    //            {
                    //                parts[i] = basePath; // No additional path segments, just use the base path
                    //            }
                    //        }
                    //        else
                    //        {
                    //            // Handle the case where the variable is not found
                    //            errs.New($"The given key '{vname}' was not present in the env dictionary.");
                    //            errs.ListThem();
                    //            errs.CacheClean();
                    //            return;
                    //        }
                    //    }

                    //}
                }

                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i].StartsWith("\\"))
                    {
                        parts[i] = parts[i].Replace("\\n", "\n");
                        parts[i] = parts[i].Replace("\\t", "\t");
                    }
                }

                string mainCommand = parts[0].ToLower();

                // Check if the command is an alias
                if (Aliases.ContainsKey(mainCommand))
                {
                    var aliasedCommand = Aliases[mainCommand];
                    parts = (aliasedCommand + " " + string.Join(" ", parts.Skip(1))).Split(' ');
                    mainCommand = parts[0].ToLower();
                }

                Console.WriteLine("");

                switch (mainCommand)
                {
                    case "@help":
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("!: This is NovaF");

                        Console.WriteLine("\nNovaF Commands:");

                        Console.WriteLine(" @user     - Manage users and logins");
                        Console.WriteLine(" @fnet     - Manage nova network and fnet sessions");

                        Console.WriteLine("\nBasic Commands:");

                        Console.WriteLine(" @help     - Get this help message");
                        Console.WriteLine(" @run      - To run a command script");
                        Console.WriteLine(" @cls      - Clear the console");
                        Console.WriteLine(" @exit     - Exit the application");
                        Console.WriteLine(" @evars    - Manage environment variables ('set', 'get', 'list', 'unset')");
                        Console.WriteLine(" @alias    - Manage command aliases ('add', 'remove', 'list')");
                        Console.WriteLine(" @history  - View or clear command history");
                        Console.WriteLine(" @encrypt  - Encrypt a file or directory");
                        Console.WriteLine(" @decrypt  - Decrypt a file or directory");
                        Console.WriteLine(" @bin      - Display file contents in binary format");
                        Console.WriteLine(" @ibin     - Convert binary string back to file, use `--no-shell` to run the script in the shell, it may break your shell, so dont do it");
                        Console.WriteLine(" @cd       - Change the current directory");
                        Console.WriteLine(" @std      - Standard Shell Lib, type `@std #help` for help");
                        Console.ResetColor();

                        Console.WriteLine("\nAlias Commands:");

                        CommandEnvAliasCommand(["@alias","list"]);

                        Console.WriteLine("\nEnvirnment Variables:");

                        CommandEnvEnvCommand(["@evars", "list"]);

                        Console.WriteLine("\n!: You can add more commands and aliases! Just explore and have fun!");
                        Console.WriteLine("!: Need more help? Type '@help' anytime!");
                        Console.ResetColor();
                        break;

                    case "@std":
                        ProcessStdCommand(parts);
                        break;

                    case "@user":                           ////// NEW COMMAND //////
                        Users.entrypoint(parts);
                        break;

                    case "@fnet":                           ////// NEW COMMAND //////
                        Novaf_Dokr.Command.env.u_f_net.fri3ndly_network.entrypoint(parts);
                        break;

                    case "@sudev":
                        nova_vm.Command.env.sudev.sfc.doit(parts);
                        break;

                    case "@cls":
                        Console.Clear();
                        break;

                    case "@run":
                        if (!File.Exists(parts[1]))
                        {
                            errs.CacheClean();
                            errs.New($"Error: `{parts[1]}` file not present.");
                            errs.ListThem();
                            errs.CacheClean();
                        }
                        else
                        {
                            {
                                IdentifyCommand.CacheClean();

                                List<string> commandsz = UserInput.Prepare(File.ReadAllText(parts[1]));

                                IdentifyCommand.Identify(commandsz);
                                List<string> parsed_commandsz = IdentifyCommand.ReturnThemPlease();

                                //foreach (string command in parsed_commands) { Console.WriteLine(command); }

                                PleaseCommandEnv.TheseCommands(parsed_commandsz);

                                IdentifyCommand.CacheClean();
                            }
                        }
                        break;

                    case "@cd":
                        CommandEnvCdCommand(parts);
                        break;

                    case "@exit":
                        Console.WriteLine("Exiting the application...");
                        Environment.Exit(0);
                        break;

                    case "@evars":
                        CommandEnvEnvCommand(parts);
                        break;

                    case "@alias":
                        CommandEnvAliasCommand(parts);
                        break;

                    case "@history":
                        CommandEnvHistoryCommand(parts);
                        break;

                    case "@encrypt":
                    case "@decrypt":
                        CommandEnvEncryptionCommand(parts);
                        break;

                    case "@bin":
                        ProcessBinCommand(parts);
                        break;

                    case "@ibin":
                        ProcessIbinCommand(parts);
                        break;

                    default:
                        ProcessBinCommand(parts);
                        //errs.New($"Command: `{command}` is not a valid internal command, type `@help` for help!");
                        //errs.ListThem();
                        //errs.CacheClean();
                        break;
                }
            }
        }
        public static void CommandEnvCdCommand(string[] parts)
        {
            if (parts.Length < 2)
            {
                errs.New($"Usage: @cd <directory> - Change the current working directory.");
                errs.ListThem();
                errs.CacheClean();
                return;
            }

            string newDir = parts[1];

            try
            {
                if (newDir == "..")
                {
                    // Move up one directory level
                    newDir = Path.GetDirectoryName(CurrentDirDest);
                }
                else if (newDir == "~")
                {
                    newDir = UserHomeDir;
                }
                else if (!Path.IsPathRooted(newDir))
                {
                    newDir = Path.Combine(CurrentDirDest, newDir);
                }

                if (newDir == null)
                {
                    errs.New($"Error: Cannot navigate above the root directory.");
                    errs.ListThem();
                    errs.CacheClean();
                    return;
                }

                newDir = Path.GetFullPath(newDir);

                if (Directory.Exists(newDir))
                {
                    CurrentDirDest = newDir;
                    Environment.CurrentDirectory = newDir;  // Ensure the process working directory is also updated
                    //Console.WriteLine($"Changed directory to: {CurrentDirDest}");
                }
                else
                {
                    errs.New($"Error: Directory '{newDir}' does not exist.");
                    errs.ListThem();
                    errs.CacheClean();
                }
            }
            catch (Exception ex)
            {
                errs.New($"Error: {ex.Message}");
                errs.ListThem();
                errs.CacheClean();
            }
        }
        public static void ProcessStdCommand(string[] parts)
        {
            if (parts.Length < 2) return;

            try
            {
                // Get the current user's name
                string currentUser = Environment.UserName;

                // Base path to the nova std directory
                string basePath = $@"C:\Users\{currentUser}\vin_env\third_party\nova\std\";

                // Extract the command after "@std"
                string stdCommand = parts[1];
                string[] commandParts = stdCommand.Split('.'); // Split on dot to detect category, class, etc.

                // Start building the path
                string commandPath = basePath;
                foreach (var part in commandParts)
                {
                    commandPath = Path.Combine(commandPath, part); // Keep adding to the path for each command part
                }

                // The remaining parts are the command arguments (after the first two parts)
                string[] commandArgs = parts.Skip(2).ToArray();  // All arguments after the command

                // Check if the command is already an executable (like ls.exe)
                if (!commandPath.EndsWith(".exe"))
                {
                    // Try to add .exe to the command path to see if it's an executable
                    if (File.Exists(commandPath + ".exe"))
                    {
                        commandPath += ".exe";  // Append .exe and use it
                    }
                    else if (Directory.Exists(commandPath))
                    {
                        // If it's a directory, check if there is an executable inside the directory
                        string executableFile = Directory.EnumerateFiles(commandPath, "*.exe").FirstOrDefault();
                        if (executableFile != null)
                        {
                            commandPath = executableFile;  // Use the first found executable in the directory
                        }
                        else
                        {
                            Console.WriteLine($"Error: No executable found in {commandPath}");
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error: Command not found at {commandPath}");
                        return;
                    }
                }

                // Try to execute the command with the given arguments
                try
                {
                    ProcessStartInfo processInfo = new ProcessStartInfo
                    {
                        FileName = commandPath,
                        Arguments = string.Join(" ", commandArgs),  // Join the arguments into a single string
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using (Process process = Process.Start(processInfo))
                    {
                        using (StreamReader reader = process.StandardOutput)
                        {
                            string result = reader.ReadToEnd();
                            Console.WriteLine(result);  // Output the result from the executed command
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error executing command: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing command: {ex.Message}");
            }
        }
        public static void CommandEnvEnvCommand(string[] parts)
        {
            if (parts.Length < 2) return;

            switch (parts[1].ToLower())
            {
                case "set":
                    if (parts.Length >= 4)
                    {
                        EnvironmentVariables[parts[2]] = string.Join(" ", parts.Skip(3));
                        SaveEnvironmentVariables();
                        //Console.WriteLine($"Environment variable '{parts[2]}' set.");
                        novaOutput.result($"Environment variable '{parts[2]}' set.");
                    }
                    break;
                case "get":
                    if (parts.Length >= 3 && EnvironmentVariables.ContainsKey(parts[2]))
                    {
                        novaOutput.result(EnvironmentVariables[parts[2]]);
                    }
                    else
                    {
                        errs.New($"Environment variable '{parts[2]}' not found.");
                        errs.ListThem();
                        errs.CacheClean();
                    }
                    break;
                case "list":
                    if (EnvironmentVariables.Count == 0)
                    {
                        errs.New("No environment variables set.");
                        errs.ListThem();
                        errs.CacheClean();
                    }
                    else
                    {
                        foreach (var kvp in EnvironmentVariables)
                        {
                            Console.WriteLine($" {kvp.Key} ==> {kvp.Value}");
                        }
                    }
                    break;
                case "unset":
                    if (parts.Length >= 3)
                    {
                        if (EnvironmentVariables.Remove(parts[2]))
                        {
                            SaveEnvironmentVariables();
                            Console.WriteLine($"Environment variable '{parts[2]}' removed.");
                        }
                        else
                        {
                            errs.New($"Environment variable '{parts[2]}' not found.");
                            errs.ListThem();
                            errs.CacheClean();
                        }
                    }
                    break;
                default:
                    errs.New("Invalid @evars command. Use 'set', 'get', 'list', or 'unset'.");
                    errs.ListThem();
                    errs.CacheClean();
                    break;
            }
        }
        public static void CommandEnvAliasCommand(string[] parts)
        {
            if (parts.Length < 2) return;

            switch (parts[1].ToLower())
            {
                case "add":
                    if (parts.Length >= 4)
                    {
                        Aliases[parts[2]] = string.Join(" ", parts.Skip(3));
                        SaveAliases();
                        novaOutput.result($"Alias '{parts[2]}' added.");
                    }
                    else
                    {
                        errs.New("Invalid alias command. Use '@alias add [name] [command]'.");
                        errs.ListThem();
                        errs.CacheClean();
                    }
                    break;
                case "remove":
                    if (parts.Length >= 3)
                    {
                        if (Aliases.Remove(parts[2]))
                        {
                            SaveAliases();
                            novaOutput.result($"Alias '{parts[2]}' removed.");
                        }
                        else
                        {
                            errs.New($"Alias '{parts[2]}' not found.");
                            errs.ListThem();
                            errs.CacheClean();
                        }
                    }
                    else
                        Console.WriteLine("Invalid alias command. Use '@alias remove [name]'.");
                    break;
                case "list":
                    if (Aliases.Count == 0)
                    {
                        errs.New("No aliases defined.");
                        errs.ListThem();
                        errs.CacheClean();
                    }
                    else
                    {
                        foreach (var kvp in Aliases)
                        {
                            Console.WriteLine($" {kvp.Key} ==> {kvp.Value}");
                        }
                    }
                    break;
                default:
                    errs.New("Invalid @alias command. Use 'add', 'remove', or 'list'.");
                    errs.ListThem();
                    errs.CacheClean();
                    break;
            }
        }
        public static void CommandEnvHistoryCommand(string[] parts)
        {
            if (parts.Length > 1 && parts[1].ToLower() == "clear")
            {
                CommandHistory.Clear();
                novaOutput.result("Command history cleared.");
            }
            else
            {
                if (CommandHistory.Count == 0)
                {
                    errs.New("Command history is empty.");
                    errs.ListThem();
                    errs.CacheClean();
                }
                else
                    for (int i = 0; i < CommandHistory.Count; i++)
                        Console.WriteLine($"{i + 1}: {CommandHistory[i]}");
            }
        }
        public static void CommandEnvEncryptionCommand(string[] parts)
        {
            if (parts.Length < 2)
            {
                errs.New($"Usage: {parts[0]} [file_or_directory]");
                errs.ListThem();
                errs.CacheClean();
                return;
            }

            string path = parts[1];
            bool isEncrypt = parts[0].ToLower() == "@encrypt";

            if (File.Exists(path))
            {
                CommandEnvFileEncryption(path, isEncrypt);
            }
            else if (Directory.Exists(path))
            {
                CommandEnvDirectoryEncryption(path, isEncrypt);
            }
            else
            {
                errs.New($"File or directory not found: {path}");
                errs.ListThem();
                errs.CacheClean();
            }
        }
        public static void CommandEnvFileEncryption(string filePath, bool isEncrypt)
        {
            try
            {
                string outputPath = isEncrypt ? filePath + ".enc" : filePath.Replace(".enc", "");
                byte[] key = new byte[32]; // 256-bit key
                byte[] iv = new byte[16];  // 128-bit IV

                // In a real-world scenario, you'd want to securely generate and manage these
                new Random().NextBytes(key);
                new Random().NextBytes(iv);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;

                    using (FileStream inputFile = new FileStream(filePath, FileMode.Open))
                    using (FileStream outputFile = new FileStream(outputPath, FileMode.Create))
                    {
                        ICryptoTransform cryptoTransform = isEncrypt
                            ? aes.CreateEncryptor()
                            : aes.CreateDecryptor();

                        using (CryptoStream cryptoStream = new CryptoStream(outputFile, cryptoTransform, CryptoStreamMode.Write))
                        {
                            inputFile.CopyTo(cryptoStream);
                        }
                    }
                }

                novaOutput.result($"{(isEncrypt ? "Encrypted" : "Decrypted")} file: {outputPath}");
            }
            catch (Exception ex)
            {
                errs.New($"Error during {(isEncrypt ? "encryption" : "decryption")}: {ex.Message}");
                errs.ListThem();
                errs.CacheClean();
            }
        }
        public static void CommandEnvDirectoryEncryption(string dirPath, bool isEncrypt)
        {
            try
            {
                foreach (string filePath in Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories))
                {
                    CommandEnvFileEncryption(filePath, isEncrypt);
                }
                novaOutput.result($"Finished {(isEncrypt ? "encrypting" : "decrypting")} directory: {dirPath}");
            }
            catch (Exception ex)
            {
                errs.New($"Error during directory {(isEncrypt ? "encryption" : "decryption")}: {ex.Message}");
                errs.ListThem();
                errs.CacheClean();
            }
        }
    }

    public class PleaseCommandEnv
    {
        public static void TheseCommands(List<string> commands)
        {
            List<string> separatedCommands = CommandEnv.SeperateThemCommands(commands);
            CommandEnv.CommandEnvEach(separatedCommands);
        }
    }
}