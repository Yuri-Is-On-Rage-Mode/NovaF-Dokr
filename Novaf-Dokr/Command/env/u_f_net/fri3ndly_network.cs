using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Novaf_Dokr.Command.env.u_f_net
{
    public class fri3ndly_network
    {
        private const string NODES_FILE_PATH = @"C:\Users\Hamza\vin_env\third_party\nova\f_net\nodes.json";

        private static List<Node> nodes = new List<Node>();

        static fri3ndly_network()
        {
            InitializeNetworkSystem();
        }

        private static void InitializeNetworkSystem()
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

        private static void EnsureFileExists(string filePath)
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

        private static void LoadNodes()
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

        private static void SaveNodes()
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
                        if (parts.Length != 3)
                        {
                            ShowHelp("add");
                            return;
                        }
                        AddNode(parts[2]);
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
                        if (parts.Length != 3)
                        {
                            ShowHelp("login");
                            return;
                        }
                        LoginToNode(parts[2]);
                        break;
                    case "logout":
                        LogoutFromNode();
                        break;
                    case "node":
                        if (parts.Length < 3)
                        {
                            ShowHelp("node");
                            return;
                        }
                        HandleNodeCommand(parts);
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

        private static void ListNodes()
        {
            if (nodes.Count == 0)
            {
                Console.WriteLine("No nodes found.");
                return;
            }

            Console.WriteLine("List of nodes:");
            PrintTableHeader();
            foreach (var node in nodes)
            {
                PrintNodeRow(node);
            }
            PrintTableFooter();
        }

        private static void PrintTableHeader()
        {
            Console.WriteLine("┌──────────────┬─────────────┬──────────┬─────────────┐");
            Console.WriteLine("│ Node Name    │ IP Address  │ Is Live  │ Last Update │");
            Console.WriteLine("├──────────────┼─────────────┼──────────┼─────────────┤");
        }

        private static void PrintNodeRow(Node node)
        {
            Console.WriteLine($"│ {node.Name,-12} │ {node.IPAddress,-11} │ {(node.IsLive ? "Yes" : "No"),-8} │ {node.LastUpdate,-11:yyyy-MM-dd} │");
        }

        private static void PrintTableFooter()
        {
            Console.WriteLine("└──────────────┴─────────────┴──────────┴─────────────┘");
        }

        private static void ShowHelp(string command = null)
        {
            if (string.IsNullOrEmpty(command))
            {
                Console.WriteLine("Usage: @fnet <command> [arguments]");
                Console.WriteLine("Commands:");
                Console.WriteLine("  list                 - List all running nodes in your network");
                Console.WriteLine("  add <nodename>       - Add a new node in your system");
                Console.WriteLine("  remove <nodename>    - Remove a node from your system");
                Console.WriteLine("  login <nodename>     - Login to a node in your system or network");
                Console.WriteLine("  logout               - Logout from the current node and login to 127.0.0.1");
                Console.WriteLine("  node <subcommand>    - Manage node operations (go-live, shutdown, update, stats)");
            }
            else
            {
                switch (command.ToLower())
                {
                    case "add":
                        Console.WriteLine("Usage: @fnet add <nodename>");
                        Console.WriteLine("  <nodename> - Name of the new node to add");
                        break;
                    case "remove":
                        Console.WriteLine("Usage: @fnet remove <nodename>");
                        Console.WriteLine("  <nodename> - Name of the node to remove");
                        break;
                    case "login":
                        Console.WriteLine("Usage: @fnet login <nodename>");
                        Console.WriteLine("  <nodename> - Name of the node to login to");
                        break;
                    case "node":
                        Console.WriteLine("Usage: @fnet node <subcommand> [arguments]");
                        Console.WriteLine("Subcommands:");
                        Console.WriteLine("  go-live <nodename>  - Publish a specific node in your network");
                        Console.WriteLine("  shutdown <nodename> - Take down a node that you own in your network");
                        Console.WriteLine("  update <nodename>   - Update a node that you published or own");
                        Console.WriteLine("  stats <nodename>    - View the stats of a specific node");
                        break;
                    default:
                        ShowHelp();
                        break;
                }
            }
        }

        private static void AddNode(string nodeName)
        {
            if (nodes.Any(n => n.Name.Equals(nodeName, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Node already exists. Please choose a different name.");
                return;
            }

            Node newNode = new Node
            {
                Name = nodeName,
                IPAddress = GenerateRandomIP(),
                IsLive = false,
                LastUpdate = DateTime.Now
            };

            nodes.Add(newNode);
            SaveNodes();

            Console.WriteLine($"Node {nodeName} added successfully.");
        }

        private static void RemoveNode(string nodeName)
        {
            Node targetNode = nodes.FirstOrDefault(n => n.Name.Equals(nodeName, StringComparison.OrdinalIgnoreCase));

            if (targetNode == null)
            {
                Console.WriteLine("Node not found. Operation cancelled.");
                return;
            }

            nodes.Remove(targetNode);
            SaveNodes();

            Console.WriteLine($"Node {nodeName} has been removed.");
        }

        private static void LoginToNode(string nodeName)
        {
            Node targetNode = nodes.FirstOrDefault(n => n.Name.Equals(nodeName, StringComparison.OrdinalIgnoreCase));

            if (targetNode == null)
            {
                Console.WriteLine("Node not found. Login failed.");
                return;
            }

            Console.WriteLine($"Logged in to node: {nodeName} (IP: {targetNode.IPAddress})");
        }

        private static void LogoutFromNode()
        {
            Console.WriteLine("Logged out. Connected to 127.0.0.1");
        }

        private static void HandleNodeCommand(string[] parts)
        {
            if (parts.Length < 4)
            {
                ShowHelp("node");
                return;
            }

            string subCommand = parts[2].ToLower();
            string nodeName = parts[3];

            switch (subCommand)
            {
                case "go-live":
                    NodeGoLive(nodeName);
                    break;
                case "shutdown":
                    NodeShutdown(nodeName);
                    break;
                case "update":
                    NodeUpdate(nodeName);
                    break;
                case "stats":
                    NodeStats(nodeName);
                    break;
                default:
                    ShowHelp("node");
                    break;
            }
        }

        private static void NodeGoLive(string nodeName)
        {
            Node targetNode = nodes.FirstOrDefault(n => n.Name.Equals(nodeName, StringComparison.OrdinalIgnoreCase));

            if (targetNode == null)
            {
                Console.WriteLine("Node not found. Operation cancelled.");
                return;
            }

            targetNode.IsLive = true;
            targetNode.LastUpdate = DateTime.Now;
            SaveNodes();

            Console.WriteLine($"Node {nodeName} is now live.");
        }

        private static void NodeShutdown(string nodeName)
        {
            Node targetNode = nodes.FirstOrDefault(n => n.Name.Equals(nodeName, StringComparison.OrdinalIgnoreCase));

            if (targetNode == null)
            {
                Console.WriteLine("Node not found. Operation cancelled.");
                return;
            }

            targetNode.IsLive = false;
            targetNode.LastUpdate = DateTime.Now;
            SaveNodes();

            Console.WriteLine($"Node {nodeName} has been shut down.");
        }

        private static void NodeUpdate(string nodeName)
        {
            Node targetNode = nodes.FirstOrDefault(n => n.Name.Equals(nodeName, StringComparison.OrdinalIgnoreCase));

            if (targetNode == null)
            {
                Console.WriteLine("Node not found. Operation cancelled.");
                return;
            }

            targetNode.LastUpdate = DateTime.Now;
            SaveNodes();

            Console.WriteLine($"Node {nodeName} has been updated.");
        }

        private static void NodeStats(string nodeName)
        {
            Node targetNode = nodes.FirstOrDefault(n => n.Name.Equals(nodeName, StringComparison.OrdinalIgnoreCase));

            if (targetNode == null)
            {
                Console.WriteLine("Node not found. Operation cancelled.");
                return;
            }

            Console.WriteLine($"Stats for node: {nodeName}");
            Console.WriteLine($"IP Address: {targetNode.IPAddress}");
            Console.WriteLine($"Is Live: {(targetNode.IsLive ? "Yes" : "No")}");
            Console.WriteLine($"Last Update: {targetNode.LastUpdate:yyyy-MM-dd HH:mm:ss}");
        }

        private static string GenerateRandomIP()
        {
            Random rand = new Random();
            return $"{rand.Next(1, 256)}.{rand.Next(0, 256)}.{rand.Next(0, 256)}.{rand.Next(1, 256)}";
        }
    }

    public class Node
    {
        public string Name { get; set; }
        public string IPAddress { get; set; }
        public bool IsLive { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}