using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Useful_Classes
{
    public static class LogSystem
    {
        public static void UpdateLog(string text, string currentDirectory)
        {
            try
            {
                string file = currentDirectory + ((!currentDirectory.EndsWith("\\")) ? "\\" : "") + "Log-" + DateTime.Now.ToString("dd-MM-yyyy") + ".log"; 

                List<string> lines = new List<string>();

                if (File.Exists(file))
                    lines.AddRange(File.ReadAllLines(file));

                if (text.Contains("\n"))
                {
                    string spl = "";
                    string mainS = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss ");
                    string[] data = text.Split(new string[] { "\n" }, StringSplitOptions.None);
                        lines.Add(mainS + "# " + data[0]);

                    for (int io = 0; io < mainS.Length; io++)
                        spl += " ";

                    for (int i = 1; i < data.Length; i++)
                    {
                        lines.Add(spl + "# " + data[i]);
                    }
                }
                else
                    lines.Add(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss # ") + text);

                File.WriteAllLines(file, lines.ToArray());
            }
            catch
            { }
        }
    }
}
