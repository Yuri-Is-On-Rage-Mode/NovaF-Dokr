using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using Novaf_Dokr.Command.env.user;
using nova.Command;
using System.Xml.Linq;

using RestSharp;
using System;
using RestSharp.Authenticators;
using nova.Utils;
using Novaf_Dokr.Command.env.u_f_net.utils;
using System.Globalization;
using System.Threading.Tasks.Dataflow;

namespace Novaf_Dokr.Command.env.u_f_net
{
    public class fri3ndly_network
    {
        public const string NODES_FILE_PATH = @"C:\Users\Hamza\vin_env\third_party\nova\f_net\nodes.json";
        public static List<Node> nodes = new List<Node>();

        static fri3ndly_network()
        {
            InitializeNetworkSystem();
        }

        public static void InitializeNetworkSystem()
        {
            try
            {
                EnsureFileExists(NODES_FILE_PATH);
                LoadNodes();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing network system: {ex.Message}");
                Environment.Exit(1);
            }
        }

        public static void EnsureFileExists(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    File.WriteAllText(filePath, "[]");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error ensuring file exists: {filePath}", ex);
            }
        }

        public static void LoadNodes()
        {
            try
            {
                string json = File.ReadAllText(NODES_FILE_PATH);
                nodes = JsonConvert.DeserializeObject<List<Node>>(json) ?? new List<Node>();
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading nodes", ex);
            }
        }

        public static void SaveNodes()
        {
            try
            {
                string json = JsonConvert.SerializeObject(nodes, Formatting.Indented);
                File.WriteAllText(NODES_FILE_PATH, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving nodes: {ex.Message}");
            }
        }

        public static void entrypoint(string[] parts)
        {
            try
            {
                if (parts.Length < 2)
                {
                    ShowHelp();
                    return;
                }

                switch (parts[1].ToLower())
                {
                    case "list":
                        ListNodes();
                        break;
                    case "add":
                        if (parts.Length < 4)
                        {
                            ShowHelp("add");
                            return;
                        }
                        AddNode(parts[2], parts[3], int.Parse(parts[4]));
                        break;
                    case "remove":
                        if (parts.Length != 3)
                        {
                            ShowHelp("remove");
                            return;
                        }
                        RemoveNode(parts[2]);
                        break;
                    case "login":
                        if (parts.Length <= 3)
                        {
                            ShowHelp("login");
                            return;
                        }
                        LoginToNode(parts[2], parts[3], parts[4]);
                        break;
                    case "logout":
                        LogoutFromNode();
                        break;
                    case "node":
                        if (parts.Length < 4)
                        {
                            ShowHelp("node");
                            return;
                        }
                        HandleNodeCommand(parts);
                        break;
                    case "req":
                        if (parts.Length < 3)
                        {
                            ShowHelp("req");
                            return;
                        }
                        routes_things.HandlReqRequest(parts);
                        break;
                    default:
                        ShowHelp();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static void ListNodes()
        {
            if (nodes.Count == 0)
            {
                Console.WriteLine("No nodes found.");
                return;
            }

            Console.WriteLine("List of nodes:");
            PrintTableHeader();

            int isup = 0;
            int isdown = 0;

            //foreach (var node in nodes)
            //{
            //    if (routes_things.PossibleRequests.InsureGetRequest(Path.Join($"http://{node.IPAddress}:{node.Port}", "/")) == 200)
            //    {
            //        node.IsLive = true;
            //        isup++;
            //    }
            //    else
            //    {
            //        node.IsLive = false;
            //        isdown++;
            //    }
            //}

            foreach (var node in nodes)
            {
                //if (routes_things.PossibleRequests.InsureGetRequest(Path.Join($"http://{node.IPAddress}:{node.Port}", "/")) == 200)
                //{
                //    isup++;
                //    node.IsLive = true;
                //}
                //else
                //{
                //    isdown++;
                //    node.IsLive = false;
                //}
                PrintNodeRow(node);
            }
            PrintTableFooter();

            //Console.WriteLine($"\nTotal `{nodes.Count}` node(s), `{isup}` node(s) are online, `{isdown}` node(s) are offline!");


        }

        public static void PrintTableHeader()
        {
            Console.WriteLine("+------------------+-----------------+----------+------------+---------------------+");
            Console.WriteLine("| Node Name        | IP Address      | Port     | Admin User | Last Update         |");
            Console.WriteLine("+------------------+-----------------+----------+------------+---------------------+");
        }

        public static void PrintNodeRow(Node node)
        {
            
            Console.WriteLine($"| {node.Name,-16} | {node.IPAddress,-15} | {node.Port,-8} | {node.AdminUsername,-10} | {node.Last_Code_Update:yyyy-MM-dd HH:mm:ss} |");
        }

        public static void PrintTableFooter()
        {
            Console.WriteLine("+------------------+-----------------+----------+------------+---------------------+");
        }

        public static void ShowHelp(string command = null)
        {
            if (string.IsNullOrEmpty(command))
            {
                Console.WriteLine("Usage: @fnet <command> [arguments]");
                Console.WriteLine("Commands:");
                Console.WriteLine("  list                         - List all running nodes in your network");
                Console.WriteLine("  add <nodename> <path> <port> - Add a new node in your system");
                Console.WriteLine("  remove <nodename>            - Remove a node from your system");
                Console.WriteLine("  login <nodename> <username> <password>     - Login to a node in your system or network");
                Console.WriteLine("  logout                       - Logout from the current node and login to 127.0.0.1");
                Console.WriteLine("  node <subcommand>            - Manage node operations (go-live, shutdown, update, stats)");
                Console.WriteLine("  req  <subcommand>            - Manage requests operations, type `list` for list, type `help` for `help`");
            }
            else
            {
                switch (command.ToLower())
                {
                    case "add":
                        Console.WriteLine("Usage: @fnet add <nodename> <path> <port>");
                        Console.WriteLine("  <nodename> - Name of the new node to add");
                        Console.WriteLine("  <path>     - Path to the Python Flask API script, type `$basic` for basic script");
                        Console.WriteLine("  <port>     - Port to run the service on");
                        break;
                    case "remove":
                        Console.WriteLine("Usage: @fnet remove <nodename>");
                        Console.WriteLine("  <nodename> - Name of the node to remove");
                        break;
                    case "login":
                        Console.WriteLine("Usage: @fnet login <nodename> <username> <password>");
                        Console.WriteLine("  <nodename> - Name of the node to login to");
                        Console.WriteLine("  <username> - Username for auth");
                        Console.WriteLine("  <password> - Username's password for auth");
                        break;
                    case "node":
                        Console.WriteLine("Usage: @fnet node <subcommand> [arguments]");
                        Console.WriteLine("Subcommands:");
                        Console.WriteLine("  go-live <nodename>  - Publish a specific node in your network");      // type `$all` for all nodes
                        Console.WriteLine("  shutdown <nodename> - Take down a node that you own in your network");// type `$all` for all nodes
                        Console.WriteLine("  update <nodename>   - Update a node that you published or own");      // type `$all` for all nodes
                        Console.WriteLine("  stats <nodename>    - View the stats of a specific node, type `$all` for all nodes");
                        break;
                    case "req":
                        Console.WriteLine("Usage: @fnet req <method> <url> [options]");
                        Console.WriteLine("Methods:");
                        Console.WriteLine("  get    - Perform a GET request");
                        Console.WriteLine("  post   - Perform a POST request");
                        Console.WriteLine("  put    - Perform a PUT request");
                        Console.WriteLine("  delete - Perform a DELETE request");
                        Console.WriteLine("  list   - List all possible paths");
                        Console.WriteLine("Options:");
                        Console.WriteLine("  --show-content       - Display the response content");
                        Console.WriteLine("  --threads <number>   - Number of concurrent requests");
                        Console.WriteLine("  --retry-on-fail <number> - Number of retry attempts on failure");
                        //Console.WriteLine("\nExamples:");
                        //Console.WriteLine("  @fnet req get http://example.com");
                        //Console.WriteLine("  @fnet req post http://api.example.com/data --show-content");
                        //Console.WriteLine("  @fnet req get http://test.com --threads 5 --retry-on-fail 3");
                        //Console.WriteLine("  @fnet req put http://api.example.com/update --show-content");
                        //Console.WriteLine("  @fnet req delete http://api.example.com/resource/123");

                        //Console.WriteLine("  @fnet req get $all");
                        //Console.WriteLine("  @fnet req list");
                        break;
                    default:
                        ShowHelp();
                        break;
                }
            }
        }

        public static void AddNode(string nodeName, string scriptPath, int portNum)
        {
            if (nodes.Any(n => n.Name.Equals(nodeName, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Node already exists. Please choose a different name.");
                return;
            }

            if (nodes.Any(n => n.Port.Equals(portNum)))
            {
                Console.WriteLine($"A node is already using port {portNum}. Please choose a different port.");
                return;
            }

            // Request password
            Console.Write($"Password for {CommandEnv.CURRENT_USER_NAME}: ");
            string password = Console.ReadLine();

            // Verify password
            if (Users.Login(CommandEnv.CURRENT_USER_NAME, password) != 1)
            {
                Console.WriteLine("Login failed. Incorrect password.");
                return;
            }

            if (scriptPath == "$basic")
            {
                scriptPath = "./basic_api_serv.py";
                // Check if script exists
                if (!File.Exists(scriptPath))
                {
                    Console.WriteLine($"Error: Basic script file does not exist at {scriptPath}.");
                    return;
                }

                Node newNode = new Node
                {
                    Name = nodeName,
                    IPAddress = GetLocalIPAddress(),
                    IsLive = false,

                    Port = portNum,

                    Last_Code_Update = DateTime.Now,
                    ScriptPath = scriptPath,

                    AdminUsername = CommandEnv.CURRENT_USER_NAME,
                    Creation_Time = DateTime.Now,
                    Last_Up = DateTime.Now,
                };

                nodes.Add(newNode);
                SaveNodes();

                Console.WriteLine($"Node '{nodeName}' added successfully.");
            }
            else
            {
                // Check if script exists
                if (!File.Exists(scriptPath))
                {
                    Console.WriteLine($"Error: Script file does not exist at {scriptPath}.");
                    return;
                }

                Node newNode = new Node
                {
                    Name = nodeName,
                    IPAddress = GetLocalIPAddress(),
                    IsLive = false,

                    Port = portNum,

                    Last_Code_Update = DateTime.Now,
                    ScriptPath = scriptPath,

                    AdminUsername = CommandEnv.CURRENT_USER_NAME,
                    Creation_Time = DateTime.Now,
                    Last_Up = DateTime.Now,
                };

                nodes.Add(newNode);
                SaveNodes();

                Console.WriteLine($"Node '{nodeName}' added successfully.");
            }
        }

        public static void RemoveNode(string nodeName)
        {
            Node nodeToRemove = nodes.FirstOrDefault(n => n.Name.Equals(nodeName, StringComparison.OrdinalIgnoreCase));
            if (nodeToRemove == null)
            {
                Console.WriteLine($"Error: Node '{nodeName}' not found.");
                return;
            }

            nodes.Remove(nodeToRemove);
            SaveNodes();
            Console.WriteLine($"Node '{nodeName}' removed successfully.");
        }

        public static string CurrentNodeAccessToken = "";

        public static void LoginToNode(string nodeName,string username1, string password1)
        {
            Node nodeToLogin = nodes.FirstOrDefault(n => n.Name.Equals(nodeName, StringComparison.OrdinalIgnoreCase));
            if (nodeToLogin == null)
            {
                Console.WriteLine($"Error: Node '{nodeName}' not found.");
                return;
            }

            //PYTHON SCRIOT FOR LOGIN: G:\fri3nds\v-category-projects\Developer-Grade-Virtual-OS\Novaf-Dokr\Novaf-Dokr\bin\Debug\net8.0\login_me.py

            try
            {
                CurrentNodeAccessToken = RunOnWindows.RunPythonFileAndReturn(["G:\\fri3nds\\v-category-projects\\Developer-Grade-Virtual-OS\\Novaf-Dokr\\Novaf-Dokr\\bin\\Debug\\net8.0\\login_me.py", $"http://{nodeToLogin.IPAddress}:{nodeToLogin.Port}", username1, password1]);
                if (!CurrentNodeAccessToken.Contains("###error_occured###"))
                {
                    if (!CurrentNodeAccessToken.Contains("###not_logined###"))
                    {
                        Console.WriteLine($"Login to node {nodeToLogin.Name} with username {username1} successfull.");
                        Console.Write($"Do you want to start a ssh connection in an interactive shell? [y/n]");

                        string yorn = Console.ReadLine();

                        if (yorn != string.Empty)
                        {
                            if (yorn.ToLower() == "y")
                            {
                            Bshell:

                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write($"(");

                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.Write($"{nodeToLogin.IPAddress}");

                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write($":");

                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.Write($"{nodeToLogin.Port}");

                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write($")");

                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write($" $ ");

                                Console.ForegroundColor = ConsoleColor.White;
                                string cmd = Console.ReadLine();

                                if (cmd != "exit")
                                {
                                    string outputfromserver = RunOnWindows.RunPythonFileAndReturn(["G:\\fri3nds\\v-category-projects\\Developer-Grade-Virtual-OS\\Novaf-Dokr\\Novaf-Dokr\\bin\\Debug\\net8.0\\command_me.py", $"http://{nodeToLogin.IPAddress}:{nodeToLogin.Port}", cmd, CurrentNodeAccessToken]);

                                    try
                                    {
                                        if (outputfromserver != string.Empty)
                                        {
                                            Console.WriteLine(outputfromserver);
                                        }
                                        else
                                        {
                                            Console.WriteLine("<return>");
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        errs.New($"An error occured!");
                                        errs.ListThem();
                                        errs.CacheClean();
                                    }
                                    goto Bshell;
                                }
                            }
                        }
                    }
                    else
                    {
                        novaOutput.starerror($"{CurrentNodeAccessToken}");
                    }
                }
                else
                {
                    novaOutput.starerror($"###error_occured###:{CurrentNodeAccessToken.Replace("###error_occured###", "")}");
                }
            }
            catch (Exception)
            {
                errs.New($"An error occured!");
                errs.ListThem();
                errs.CacheClean();
            }
            
        }

        public static void LogoutFromNode()
        {
            if (CurrentNodeAccessToken != string.Empty || CurrentNodeAccessToken != "")
            {
                CurrentNodeAccessToken = "";
                Console.WriteLine("Logged out successfully.");
            }
            else
            {
                Console.WriteLine("No node to logout.");
            }
        }

        public static void HandleNodeCommand(string[] parts)
        {
            try
            {
                // Check if enough parts are provided for the command
                if (parts.Length < 3)
                {
                    Console.WriteLine("Error: Insufficient arguments. A valid subcommand is required.");
                    ShowHelp("node");
                    return;
                }

                string subcommand = parts[2].ToLower();
                string nodeName = parts.Length > 3 ? parts[3] : null; // Check if nodeName is provided

                switch (subcommand)
                {
                    case "go-live":
                        if (nodeName != null)
                        {
                            GoLiveNode(nodeName);
                        }
                        else
                        {
                            errs.New("Error: Node name required for 'go-live' command.");
                            errs.ListThem();
                            errs.CacheClean();
                        }
                        break;

                    case "shutdown":
                        if (nodeName != null)
                        {
                            ShutdownNode(nodeName);
                        }
                        else
                        {
                            errs.New("Error: Node name required for 'shutdown' command.");
                            errs.ListThem();
                            errs.CacheClean();
                        }
                        break;

                    case "update":
                        if (nodeName != null)
                        {
                            UpdateNode(nodeName);
                        }
                        else
                        {
                            errs.New("Error: Node name required for 'update' command.");
                            errs.ListThem();
                            errs.CacheClean();
                        }
                        break;

                    case "stats":
                        if (nodeName != null)
                        {
                            ShowNodeStats(nodeName);
                        }
                        else
                        {
                            // If no node name is given, show stats for all nodes
                            errs.New("Error: Node name required for 'stats' command.");
                            errs.ListThem();
                            errs.CacheClean();
                        }
                        break;

                    default:
                        errs.New($"Error: Unknown subcommand '{subcommand}'.");
                        errs.ListThem();
                        errs.CacheClean();
                        ShowHelp("node");
                        break;
                }
            }
            catch (Exception ex)
            {
                // Log the error and prevent the app from shutting down
                errs.New($"An error occurred: {ex.Message}");
                errs.ListThem();
                errs.CacheClean();
            }
        }


        public static void GoLiveNode(string nodeName)
        {
            Node nodeToGoLive = nodes.FirstOrDefault(n => n.Name.Equals(nodeName, StringComparison.OrdinalIgnoreCase));
            if (nodeToGoLive == null)
            {
                Console.WriteLine($"Error: Node '{nodeName}' not found.");
                return;
            }

            // Request password
            Console.Write($"Password for {CommandEnv.CURRENT_USER_NAME}: ");
            string password = Console.ReadLine();

            // Verify password
            if (Users.Login(CommandEnv.CURRENT_USER_NAME, password) != 1)
            {
                Console.WriteLine("Login failed. Incorrect password.");
                return;
            }

            // Start the node's Python script in a new process
            Process.Start(new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{nodeToGoLive.ScriptPath}\" {nodeToGoLive.Name} {CommandEnv.CURRENT_USER_NAME} {password} {nodeToGoLive.Port} ", // Ensures the script path is correctly escaped
                UseShellExecute = false, // False to use standard input/output
                RedirectStandardOutput = true, // Optionally redirect output
                CreateNoWindow = true // Do not create a window for the process
                //WorkingDirectory = Path.GetDirectoryName(nodeToGoLive.ScriptPath) // Optional: Set the working directory
            });
            nodeToGoLive.IsLive = true;
            nodeToGoLive.Last_Code_Update = DateTime.Now;
            SaveNodes();

            if (routes_things.PossibleRequests.InsureGetRequest(Path.Join($"http://{nodeToGoLive.IPAddress}:{nodeToGoLive.Port}", "/home/json")) == 200)
            {
                nodeToGoLive.IsLive = true;

                Console.WriteLine($"Node '{nodeName}' has been updated and is now live.");
            }
            else
            {
                nodeToGoLive.IsLive = false;

                Console.WriteLine($"Node '{nodeName}' has been updated but it is not live, type `@fnet node go-live {nodeName}`.");
            }
        }

        public static void ShutdownNode(string nodeName)
        {
            Node nodeToShutdown = nodes.FirstOrDefault(n => n.Name.Equals(nodeName, StringComparison.OrdinalIgnoreCase));
            if (nodeToShutdown == null)
            {
                Console.WriteLine($"Error: Node '{nodeName}' not found.");
                return;
            }

            // Send POST request to nodeToShutdown.IPAddress/shutdown
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string shutdownUrl = $"http://{nodeToShutdown.IPAddress}:{nodeToShutdown.Port}/f/system/shutdown";
                    HttpResponseMessage response = client.GetAsync(shutdownUrl).Result; // Block and wait for the result

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = response.Content.ReadAsStringAsync().Result; // Block and read the response synchronously
                        Console.WriteLine($"Shutdown response from node: {responseBody}");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to shut down node remotely. Status code: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred while sending shutdown request: {ex.Message}");
                    return; // If shutdown fails remotely, do not proceed with killing the local process
                }
            }

            // Shutdown the process running the Python script (Assumes single instance)
            var processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(nodeToShutdown.ScriptPath));
            foreach (var process in processes)
            {
                process.Kill();
            }

            // Update the node status and save
            nodeToShutdown.IsLive = false;
            SaveNodes();
            
            if (routes_things.PossibleRequests.InsureGetRequest(Path.Join($"http://{nodeToShutdown.IPAddress}:{nodeToShutdown.Port}", "/home/json")) == 200)
            {
                nodeToShutdown.IsLive = true;

                Console.WriteLine($"Node '{nodeName}' could not be shutdown.");
            }
            else
            {
                nodeToShutdown.IsLive = false;

                Console.WriteLine($"Node '{nodeName}' has been shut down.");
            }
        }

        public static void UpdateNode(string nodeName)
        {
            Node nodeToUpdate = nodes.FirstOrDefault(n => n.Name.Equals(nodeName, StringComparison.OrdinalIgnoreCase));
            if (nodeToUpdate == null)
            {
                Console.WriteLine($"Error: Node '{nodeName}' not found.");
                return;
            }

            ShutdownNode(nodeName); // Shutdown the node first

            // Request password
            Console.Write($"Password for {CommandEnv.CURRENT_USER_NAME}: ");
            string password = Console.ReadLine();

            // Verify password
            if (Users.Login(CommandEnv.CURRENT_USER_NAME, password) != 1)
            {
                Console.WriteLine("Login failed. Incorrect password.");
                return;
            }

            // Start the node's Python script in a new process
            Process.Start(new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{nodeToUpdate.ScriptPath}\" {nodeToUpdate.Name} {CommandEnv.CURRENT_USER_NAME} {password} {nodeToUpdate.Port}", // Ensures the script path is correctly escaped
                UseShellExecute = false, // False to use standard input/output
                RedirectStandardOutput = true, // Optionally redirect output
                CreateNoWindow = true // Do not create a window for the process
                //WorkingDirectory = Path.GetDirectoryName(nodeToUpdate.ScriptPath) // Optional: Set the working directory
            });
            nodeToUpdate.Last_Code_Update = DateTime.Now;
            SaveNodes();

            if (routes_things.PossibleRequests.InsureGetRequest(Path.Join($"http://{nodeToUpdate.IPAddress}:{nodeToUpdate.Port}", "/home/json")) == 200)
            {
                nodeToUpdate.IsLive = true;

                Console.WriteLine($"Node '{nodeName}' has been updated and is now live.");
            }
            else
            {
                nodeToUpdate.IsLive = false;

                Console.WriteLine($"Node '{nodeName}' has been updated but it is not live, type `@fnet node go-live {nodeName}`.");
            }

        }

        public static void ShowNodeStats(string nodeName)
        {
            try
            {
                if (nodeName == "$all")
                {
                    PrintAllNodesTable();
                }
                else
                {
                    PrintSingleNodeInfo(nodeName);
                }
            }
            catch (Exception)
            {
                PrintAllNodesTable();
            }
        }

        public static void PrintAllNodesTable()
        {
            if (nodes == null || nodes.Count == 0)
            {
                Console.WriteLine("No nodes available.");
                return;
            }

            const string separator = "+----------------+----------------+--------------------------------------------+------------------+--------+----------+---------------------------+---------------------------+";
            const string format = "| {0,-14} | {1,-14} | {2,-41}  | {3,-16} | {4,-6} | {5,-8} | {6,-25} | {7,-25} |";

            Console.WriteLine(separator);
            Console.WriteLine(string.Format(format, "Node Name", "Admin User", "Script Path", "IP Address", "Port", "Is Live", "Last Up", "Last Update"));
            Console.WriteLine(separator);

            int isup = 0;
            int isdown = 0;

            //foreach (var node in nodes)
            //{
            //    if (routes_things.PossibleRequests.InsureGetRequest(Path.Join($"http://{node.IPAddress}:{node.Port}", "/")) == 200)
            //    {
            //        node.IsLive = true;
            //        isup++;
            //    }
            //    else
            //    {
            //        node.IsLive = false;
            //        isdown++;
            //    }
            //}

            foreach (var node in nodes)
            {
                if (routes_things.PossibleRequests.InsureGetRequest(Path.Join($"http://{node.IPAddress}:{node.Port}", "/")) == 200)
                {
                    node.IsLive = true;
                    isup++;
                }
                else
                {
                    node.IsLive = false;
                    isdown++;
                }
                Console.WriteLine(string.Format(format,
                 TruncateString(node.Name, 14),
                 TruncateString(node.AdminUsername, 14),
                 TruncateString(node.ScriptPath, 41),
                 TruncateString(node.IPAddress, 16),
                 node.Port,
                 (node.IsLive ? "Yes" : "No"),
                 TruncateString(node.Last_Up.ToString(), 25),
                 TruncateString(node.Last_Code_Update.ToString(), 25)));
            }

            Console.WriteLine(separator);

            Console.WriteLine($"\nTotal `{nodes.Count}` node(s), `{isup}` nodes(s) are online, `{isdown}` node(s) are offline!");
        }

        public static void PrintSingleNodeInfo(string nodeName)
        {
            Node node = nodes.FirstOrDefault(n => n.Name.Equals(nodeName, StringComparison.OrdinalIgnoreCase));
            if (node == null)
            {
                Console.WriteLine($"Error: Node '{nodeName}' not found.");
                return;
            }

            //Console.WriteLine($"Node Name  : {nodeToShow.Name}");
            //Console.WriteLine($"Node Script: {nodeToShow.ScriptPath}");
            //Console.WriteLine($"IP Address : {nodeToShow.IPAddress}");
            //Console.WriteLine($"Port       : {nodeToShow.Port}");
            //Console.WriteLine($"Is Live    : {(nodeToShow.IsLive ? "Yes" : "No")}");
            //Console.WriteLine($"Last Update: {nodeToShow.Last_Code_Update}");

            //if (node == null)
            //{
            //    Console.WriteLine("No nodes available.");
            //    return;
            //}

            const string separator = "+----------------+----------------+--------------------------------------------+------------------+--------+----------+---------------------------+---------------------------+";
            const string format = "| {0,-14} | {1,-14} | {2,-41}  | {3,-16} | {4,-6} | {5,-8} | {6,-25} | {7,-25} |";

            Console.WriteLine(separator);
            Console.WriteLine(string.Format(format, "Node Name", "Admin User", "Script Path", "IP Address", "Port", "Is Live", "Last Up", "Last Update"));
            Console.WriteLine(separator);

            if (routes_things.PossibleRequests.InsureGetRequest(Path.Join($"http://{node.IPAddress}:{node.Port}", "/")) == 200)
            {
                node.IsLive = true;
            }
            else
            {
                node.IsLive = false;
            }

            Console.WriteLine(string.Format(format,
                TruncateString(node.Name, 14),
                TruncateString(node.AdminUsername, 14),
                TruncateString(node.ScriptPath, 41),
                TruncateString(node.IPAddress, 16),
                node.Port,
                (node.IsLive ? "Yes" : "No"),
                TruncateString(node.Last_Up.ToString(), 25),
                TruncateString(node.Last_Code_Update.ToString(), 25)));

            Console.WriteLine(separator);
        }

        public static string TruncateString(string input, int maxLength)
        {
            return input.Length <= maxLength ? input : input.Substring(0, maxLength - 3) + "...";
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var localIP = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            return localIP != null ? localIP.ToString() : "127.0.0.1";
        }
    }

    public class Node
    {
        public string Name { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public bool IsLive { get; set; }
        public DateTime Last_Code_Update { get; set; }
        public string ScriptPath { get; set; }

        public string AdminUsername { get; set; }
        public DateTime Creation_Time { get; set; }
        public DateTime Last_Up { get; set; }
    }
}
