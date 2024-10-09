using nova.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace nova.Command
{
    internal class RunOnWindows
    {
        public static void runpy(string[] parts)
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
                //    errs.New($"{x}: Expected: `python:abc.py` Found: `{item}`");
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
                    //    errs.New($"{x}: Expected: `python:abc.py` Found: `{item}`");
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
                    errs.New($"Error: File '{fileName}' not Found in {filePath}");
                    errs.ListThem();
                    errs.CacheClean();
                    return;
                }

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
            catch (Exception ex)
            {
                // Handle general errors during process execution
                errs.New($"Error starting process: {ex.Message}");
                errs.ListThem();
                errs.CacheClean();
            }
        }
        public static void RunPythonFile(List<string> commands)
        {
            // Ensure the commands list has at least two elements (Python path and script path)
            if (commands.Count <= 0)
            {
                Console.WriteLine("Please provide both the Python path and the script path.");
                return;
            }

            string pythonPath = "powershell";  // Python executable path
            string scriptPath = commands[0];  // Python script path

            // Collect the remaining elements (arguments) as a string
            string args = string.Join(" ", commands.GetRange(1, commands.Count - 1));  // All arguments after the script path

            // Set up the process to execute the Python script with the given arguments
            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = pythonPath,
                Arguments = $"python {scriptPath} {args}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            try
            {
                // Start the process and capture the output and errors
                using (Process process = Process.Start(start))
                {
                    // Read the standard output stream
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        Console.WriteLine("\n" + result);
                    }

                    // Read the error stream if any errors occur
                    using (StreamReader reader = process.StandardError)
                    {
                        string errors = reader.ReadToEnd();
                        if (!string.IsNullOrEmpty(errors))
                        {
                            Console.WriteLine("\n" + errors);
                        }
                    }

                    // Ensure the process completes before continuing
                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while running the command: " + ex.Message);
            }
        }
        public static string RunPythonFileAndReturn(List<string> commands)
        {
            // Ensure the commands list has at least two elements (Python path and script path)
            if (commands.Count <= 0)
            {
                Console.WriteLine("Please provide both the Python path and the script path.");
                return "###error_occured###";
            }

            string pythonPath = "powershell";  // Python executable path
            string scriptPath = commands[0];  // Python script path


            string result = "";
            // Collect the remaining elements (arguments) as a string
            string args = string.Join(" ", commands.GetRange(1, commands.Count - 1));  // All arguments after the script path

            // Set up the process to execute the Python script with the given arguments
            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = pythonPath,
                Arguments = $"python {scriptPath} {args}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            try
            {
                
                // Start the process and capture the output and errors
                using (Process process = Process.Start(start))
                {
                    // Read the standard output stream
                    using (StreamReader reader = process.StandardOutput)
                    {
                        result = reader.ReadToEnd();
                        //Console.WriteLine("\n" + result);
                    }

                    // Read the error stream if any errors occur
                    using (StreamReader reader = process.StandardError)
                    {
                        string errors = reader.ReadToEnd();
                        if (!string.IsNullOrEmpty(errors))
                        {
                            Console.WriteLine("\n" + errors);
                        }
                    }

                    // Ensure the process completes before continuing
                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while running the command: " + ex.Message);
            }

            return result;
        }
    }
}
