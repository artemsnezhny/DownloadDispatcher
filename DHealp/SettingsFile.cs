using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHelp
{
    public static class SettingsFile
    {
        const string SETTINGS_FILE_NAME = @"settings";

        public static void Write(string settingName, string value)
        {
            using (StreamWriter sw = new StreamWriter(SETTINGS_FILE_NAME, true, Encoding.UTF8))
            {
                sw.Write(settingName + "=" + value);
                sw.WriteLine();
            }
        }

        public static void Change(string settingName, string newValue)
        {
            List<string> lines;
            lines = File.ReadAllLines(SETTINGS_FILE_NAME).ToList<string>();
            for (int i = 0; i < lines.Count; i++)
            {
                string[] line = lines[i].Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (line[0] == settingName)
                {
                    lines[i] = line[0] + "=" + newValue;
                    break;
                }
            }

            using (StreamWriter sw = new StreamWriter(SETTINGS_FILE_NAME, false, Encoding.UTF8))
            {
                foreach (var line in lines)
                    sw.WriteLine(line);
            }
        }

        public static string Read(string settingName)
        {
            if (!File.Exists(SETTINGS_FILE_NAME))
            {
                using (File.Create(SETTINGS_FILE_NAME))
                {}
                return null;
            }
            using (StreamReader sr = new StreamReader(SETTINGS_FILE_NAME, Encoding.UTF8))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] temp = line.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (temp[0] == settingName)
                        return temp[1];
                }
                return null;
            }
        }
    }
}
