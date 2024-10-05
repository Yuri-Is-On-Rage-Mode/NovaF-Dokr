using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

using nova.Utils;

namespace nova.Command
{
    internal class Identifier
    {
        public static List<string> main() 
        {
            return ["this","is","not","an","actual","class","so","dont","use","this","use", "IdentifyCommand","instead","!"];
        }
    }

    public class IdentifyCommand
    {
        public static List<string> RegisteredCommands = ["SUDO","COFFEE"];
        public static List<string> StructuredCommands = [];
        public static void Identify(List<string> commands)
        {
            for (int i = 0; i < commands.Count; i++)
            {
                //Console.WriteLine($"{i}: {commands[i]}");
                PleaseIdentifyThisForMe(commands[i]);
            }
        }

        public static List<string> ReturnThemPlease()
        {
            return StructuredCommands;
        }

        public static void CacheClean()
        {

            StructuredCommands.Clear();
        
        }

        public static void PleaseIdentifyThisForMe(string command_name) 
        {
            if (string.IsNullOrEmpty(command_name)) { return; }
            else
            {
                if (RegisteredCommands.Contains(command_name.ToUpper()))
                {
                    //Console.WriteLine($"[+] Found!`{command_name.ToUpper()}` ");
                    StructuredCommands.Add("@"+ command_name);
                }
                else if (!RegisteredCommands.Contains(command_name.ToUpper()))
                {
                    //Console.WriteLine($"[-] Not Found! `{command_name.ToUpper()}`");
                    StructuredCommands.Add(command_name);
                }
            }
        }
    }
}
