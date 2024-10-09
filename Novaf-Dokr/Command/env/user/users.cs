using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;
using nova.Command;
using nova.Utils;

namespace Novaf_Dokr.Command.env.user
{
    public class Users
    {
        public const string USER_FILE_PATH = @"C:\Users\Hamza\vin_env\third_party\nova\users\uygfiyiTFVYTFyvfvrTYR8Y5r6rHFtyrf65RTfyTFVr.json";
        public const string DELETED_USER_FILE_PATH = @"C:\Users\Hamza\vin_env\third_party\nova\users\yGU(T67t67RT76r86R86r8TRVytRCTrv65RV865rvT.json";
        public const int MAX_ROOT_USERS = 150;
        public const int MAX_TMP_USERS = 5000;

        public static string current_user = CommandEnv.CURRENT_USER_NAME;
        public static string current_node = CommandEnv.CURRENT_NODE_NAME;

        public static List<User> users = new List<User>();
        public static List<User> deletedUsers = new List<User>();

        static Users()
        {
            InitializeUserSystem();
        }

        public static void InitializeUserSystem()
        {
            try
            {
                EnsureFileExists(USER_FILE_PATH);
                EnsureFileExists(DELETED_USER_FILE_PATH);
                LoadUsers();
                LoadDeletedUsers();
                RemoveDuplicateUsers();
            }
            catch (Exception ex)
            {
                errs.CacheClean();
                errs.New($"Error initializing user system: {ex.Message}");
                errs.ListThem();
                errs.CacheClean();
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
                errs.New($"Error ensuring file exists: {filePath}: {ex}");
                errs.ListThem();
                errs.CacheClean();
            }
        }

        public static void LoadUsers()
        {
            try
            {
                string json = File.ReadAllText(USER_FILE_PATH);
                users = JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
            }
            catch (Exception ex)
            {
                errs.New($"Error loading users: {ex}");
                errs.ListThem();
                errs.CacheClean();
            }
        }

        public static void LoadDeletedUsers()
        {
            try
            {
                string json = File.ReadAllText(DELETED_USER_FILE_PATH);
                deletedUsers = JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
            }
            catch (Exception ex)
            {
                errs.New($"Error loading deleted users: {ex}");
                errs.ListThem();
                errs.CacheClean();
            }
        }

        public static void SaveUsers()
        {
            try
            {
                string json = JsonConvert.SerializeObject(users, Formatting.Indented);
                File.WriteAllText(USER_FILE_PATH, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving users: {ex.Message}");
            }
        }

        public static void SaveDeletedUsers()
        {
            try
            {
                string json = JsonConvert.SerializeObject(deletedUsers, Formatting.Indented);
                File.WriteAllText(DELETED_USER_FILE_PATH, json);
            }
            catch (Exception ex)
            {
                errs.New($"Error saving deleted users: {ex.Message}");
                errs.ListThem();
                errs.CacheClean();
            }
        }

        public static string HashPassword(string password)
        {
            try
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                    return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                }
            }
            catch (Exception ex)
            {
                errs.New($"Error hashing password: {ex}");
                errs.ListThem();
                errs.CacheClean();

                return "ERROR CREATING HASH OF THAT PASSWORD";
            }
        }

        public static void RemoveDuplicateUsers()
        {
            var uniqueUsers = users
                .GroupBy(u => u.Username.ToLower())
                .Select(g => g.OrderByDescending(u => u.LastActive).First())
                .ToList();

            users = uniqueUsers;
            SaveUsers();
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
                    case "name":
                        Console.WriteLine(CommandEnv.CURRENT_USER_NAME);
                        break;
                    case "node-name":
                        Console.WriteLine(CommandEnv.CURRENT_NODE_NAME);
                        break;
                    case "list":
                        ListUsers();
                        break;
                    case "add":
                        if (parts.Length != 5)
                        {
                            ShowHelp("add");
                            return;
                        }
                        AddUser(parts[2], parts[3], parts[4]);
                        break;
                    case "remove":
                        if (parts.Length != 3)
                        {
                            ShowHelp("remove");
                            return;
                        }
                        RemoveUser(parts[2]);
                        break;
                    case "login":
                        if (parts.Length != 4)
                        {
                            ShowHelp("login");
                            return;
                        }
                        Login(parts[2], parts[3]);
                        break;
                    case "logout":
                        Logout();
                        break;
                    default:
                        ShowHelp();
                        break;
                }
            }
            catch (Exception ex)
            {
                errs.New($"Error: {ex}");
                errs.ListThem();
                errs.CacheClean();
            }
        }

        public static void ListUsers()
        {
            if (users.Count == 0)
            {
                Console.WriteLine("No users found.");
                return;
            }

            Console.WriteLine("List of users:");
            PrintTableHeader();
            foreach (var user in users)
            {
                if (user.Username != CommandEnv.CURRENT_USER_NAME)
                {
                    user.IsActive = false;
                }
                else
                { 
                    user.IsActive = true;
                }
                PrintUserRow(user);
            }
            PrintTableFooter();
        }

        public static void PrintTableHeader()
        {
            Console.WriteLine("+--------------+---------------------+---------------------+----------------+----------+-------+");
            Console.WriteLine("| Username     | Creation Date       | Last Active         | Age            | Is Active| Group |");
            Console.WriteLine("+--------------+---------------------+---------------------+----------------+----------+-------+");
        }

        public static void PrintUserRow(User user)
        {
            Console.WriteLine($"| {user.Username,-12} | {user.CreationDate,-19:yyyy-MM-dd HH:mm:ss} | {user.LastActive,-19:yyyy-MM-dd HH:mm:ss} | {FormatTimeSpan(user.LastActive - user.CreationDate ),-14} | {(user.IsActive ? "Yes" : "No"),-8} | {user.Group,-5} |");
        }

        public static void PrintTableFooter()
        {
            Console.WriteLine("+--------------+---------------------+---------------------+----------------+----------+-------+");
        }

        public static string FormatTimeSpan(TimeSpan timeSpan)
        {
            return timeSpan.Days > 0
                ? $"{timeSpan.Days}d {timeSpan.Hours}h"
                : timeSpan.Hours > 0
                    ? $"{timeSpan.Hours}h {timeSpan.Minutes}m"
                    : $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
        }

        public static void ShowHelp(string command = null)
        {
            if (string.IsNullOrEmpty(command))
            {
                Console.WriteLine("Usage: user <command> [arguments]");
                Console.WriteLine("Commands:");
                Console.WriteLine("  name                     - Display current user name");
                Console.WriteLine("  node-name                - Display current node name");
                Console.WriteLine("  list                     - List all users");
                Console.WriteLine("  add <username> <password> <group> - Add a new user");
                Console.WriteLine("  remove <username>        - Remove a user");
                Console.WriteLine("  login <username> <password> - Log in as a user");
                Console.WriteLine("  logout                   - Log out current user");
            }
            else
            {
                switch (command.ToLower())
                {
                    case "add":
                        Console.WriteLine("Usage: user add <username> <password> <group>");
                        Console.WriteLine("  <username> - New user's username (3-20 characters)");
                        Console.WriteLine("  <password> - User's password (at least 8 characters)");
                        Console.WriteLine("  <group>    - User's group (root, guest, or tmp)");
                        break;
                    case "remove":
                        Console.WriteLine("Usage: user remove <username>");
                        Console.WriteLine("  <username> - Username of the user to remove");
                        break;
                    case "login":
                        Console.WriteLine("Usage: user login <username> <password>");
                        Console.WriteLine("  <username> - Username to log in");
                        Console.WriteLine("  <password> - User's password");
                        break;
                    default:
                        ShowHelp();
                        break;
                }
            }
        }

        public static void AddUser(string username, string password, string group)
        {
            if (!ValidateUsername(username) || !ValidatePassword(password) || !ValidateGroup(group))
            {
                return;
            }

            if (users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Username already exists. Please choose a different username.");
                return;
            }

            int rootUsersCount = users.Count(u => u.Group.Equals("root", StringComparison.OrdinalIgnoreCase));
            int tmpUsersCount = users.Count(u => u.Group.Equals("tmp", StringComparison.OrdinalIgnoreCase));

            if (group.Equals("root", StringComparison.OrdinalIgnoreCase) && rootUsersCount >= MAX_ROOT_USERS)
            {
                Console.WriteLine($"Cannot add more root users. Maximum limit of {MAX_ROOT_USERS} reached.");
                return;
            }

            if (group.Equals("tmp", StringComparison.OrdinalIgnoreCase) && tmpUsersCount >= MAX_TMP_USERS)
            {
                Console.WriteLine($"Cannot add more tmp users. Maximum limit of {MAX_TMP_USERS} reached.");
                return;
            }

            if (group.Equals("root", StringComparison.OrdinalIgnoreCase))
            {
                if (CommandEnv.CURRENT_USER_NAME != "root")
                {
                    Console.Write("Password for root: ");
                    string passwdFroot = Console.ReadLine();

                    if (Login("root", passwdFroot) == 0)
                    {
                        return;
                    }
                    else
                    {
                        Logout();
                    }
                }
            }

            User newUser = new User
            {
                Username = username,
                PasswordHash = HashPassword(password),
                CreationDate = DateTime.Now,
                LastActive = DateTime.Now,
                TimeSpent = TimeSpan.Zero,
                IsActive = true,
                Group = group.ToLower()
            };

            users.Add(newUser);
            SaveUsers();

            Console.WriteLine($"User {username} added successfully.");
            Console.Write($"Do you want to login as {username}? [y/n]");

            string yorn = Console.ReadLine();

            if (yorn != string.Empty)
            {
                if (yorn.ToLower() == "y")
                {
                    Login(username,password);
                }
            }
        }

        public static void RemoveUser(string targetUsername)
        {
            if (CommandEnv.CURRENT_USER_NAME == "Guest")
            {
                Console.WriteLine("You must be logged in to remove a user.");
                return;
            }

            User currentUser = users.FirstOrDefault(u => u.Username.Equals(CommandEnv.CURRENT_USER_NAME, StringComparison.OrdinalIgnoreCase));
            User targetUser = users.FirstOrDefault(u => u.Username.Equals(targetUsername, StringComparison.OrdinalIgnoreCase));

            if (targetUser == null)
            {
                Console.WriteLine("User not found. Operation cancelled.");
                return;
            }

            if (targetUser.Group == "root" && currentUser.Group != "root")
            {
                Console.WriteLine("You don't have permission to remove a root user.");
                return;
            }

            users.Remove(targetUser);
            targetUser.IsActive = false;
            deletedUsers.Add(targetUser);
            SaveUsers();
            SaveDeletedUsers();

            Console.WriteLine($"User {targetUsername} has been removed.");
        }

        public static int Login(string username, string password)
        {
            User user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && u.PasswordHash == HashPassword(password));

            if (user != null)
            {
                user.LastActive = DateTime.Now;
                user.IsActive = true;
                CommandEnv.CURRENT_USER_NAME = username;
                SaveUsers();
                Console.WriteLine($"Logged in as {username}");
                return 1;
            }
            else
            {
                Console.WriteLine("Invalid username or password.");
                return 0;
            }
        }

        public static void Logout()
        {
            User currentUser = users.FirstOrDefault(u => u.Username.Equals(CommandEnv.CURRENT_USER_NAME, StringComparison.OrdinalIgnoreCase));
            if (currentUser != null)
            {
                currentUser.TimeSpent += DateTime.Now - currentUser.LastActive;
                currentUser.IsActive = false;
                SaveUsers();
            }

            CommandEnv.CURRENT_USER_NAME = "Guest";
            Console.WriteLine("Logged out successfully.");
        }

        public static bool ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3 || username.Length > 20)
            {
                Console.WriteLine("Username must be between 3 and 20 characters.");
                return false;
            }
            return true;
        }

        public static bool ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            {
                Console.WriteLine("Password must be at least 8 characters long.");
                return false;
            }
            return true;
        }

        public static bool ValidateGroup(string group)
        {
            if (!new[] { "root", "guest", "tmp" }.Contains(group.ToLower()))
            {
                Console.WriteLine("Invalid group. Please enter 'root', 'guest', or 'tmp'.");
                return false;
            }
            return true;
        }
    }

    public class User
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastActive { get; set; }
        public TimeSpan TimeSpent { get; set; }
        public bool IsActive { get; set; }
        public string Group { get; set; }
    }
}