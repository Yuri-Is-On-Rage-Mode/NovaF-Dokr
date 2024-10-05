using System;
using System.Collections.Generic;

namespace nova.Utils
{
    internal class Warnings
    {
        // No changes needed for now
    }

    public class warns
    {
        // List to store warning messages
        public static List<string> WarningsAre = new List<string>();

        // Add a new warning message to the list
        public static void New(string warn_msg)
        {
            WarningsAre.Add(warn_msg);
        }

        // Clear the warning message cache
        public static void CacheClean()
        {
            WarningsAre.Clear();
        }

        // List warnings, use warnlast for the last warning and regular warnings for others
        public static void ListThem()
        {
            if (WarningsAre.Count > 0)
            {
                if (WarningsAre.Count == 1)
                {
                    novaOutput.warningoutputs.warninfo($"Found a Warning!");

                    for (int i = 0; i < WarningsAre.Count - 1; i++)
                    {
                        novaOutput.warningoutputs.warn(WarningsAre[i], "wrn"); // Regular warnings
                    }

                    // Use warnlast for the last warning
                    warnlast(WarningsAre[^1]);
                }
                else if (WarningsAre.Count <= 20)
                {
                    novaOutput.warningoutputs.warninfo($"Found {WarningsAre.Count} Warnings: Listing {WarningsAre.Count} out of {WarningsAre.Count} Warnings!");

                    for (int i = 0; i < WarningsAre.Count - 1; i++)
                    {
                        novaOutput.warningoutputs.warn(WarningsAre[i], "wrn"); // Regular warnings
                    }

                    // Use warnlast for the last warning
                    warnlast(WarningsAre[^1]);
                }
                else
                {
                    novaOutput.warningoutputs.warninfo($"Found {WarningsAre.Count} Warnings: Listing 20 out of {WarningsAre.Count} Warnings!");

                    for (int i = 0; i < 19; i++) // List the first 19 as regular warnings
                    {
                        novaOutput.warningoutputs.warn(WarningsAre[i], "wrn");
                    }

                    // Use warnlast for the 20th warning
                    warnlast(WarningsAre[19]);
                }
            }
            else
            {
                //novaOutput.warningoutputs.warninfo("0 Warnings Found!");
            }
        }

        // List all warnings and use warnlast for the last warning
        public static void ListThemAll()
        {
            if (WarningsAre.Count == 1)
            {
                novaOutput.warningoutputs.warninfo($"Found a Warning!");

                for (int i = 0; i < WarningsAre.Count - 1; i++)
                {
                    novaOutput.warningoutputs.warn(WarningsAre[i], "wrn"); // Regular warnings
                }

                // Use warnlast for the last warning
                warnlast(WarningsAre[^1]);
            }
            else if (WarningsAre.Count > 0)
            {
                novaOutput.warningoutputs.warninfo($"Found {WarningsAre.Count} Warnings: Listing {WarningsAre.Count} out of {WarningsAre.Count} Warnings!");

                for (int i = 0; i < WarningsAre.Count - 1; i++) // Print all but the last warning
                {
                    novaOutput.warningoutputs.warn(WarningsAre[i], "wrn");
                }

                // Use warnlast for the last warning
                warnlast(WarningsAre[^1]);
            }
            else
            {
                //novaOutput.warningoutputs.warninfo("0 Warnings Found!");
            }
        }

        // Print the last warning using warnlast
        public static void warnlast(string lastWarning)
        {
            novaOutput.warningoutputs.warnlast(lastWarning, "wrn");
        }
    }
}
