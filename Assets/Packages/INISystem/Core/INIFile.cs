using System.Collections.Generic;
using System.IO;

namespace CorruptedSmileStudio.INI
{
    /// <summary>
    /// Handles reading and writing of INI files.
    /// </summary>
    public static class INIFile
    {
        /// <summary>
        /// Saves the current values and identities to file.
        /// </summary>
        /// <param name="filePath">The path to the file where it must be saved.</param>
        /// <param name="contents">The INIContent object that contains INI entries.</param>
        /// <returns>True if successful or false if unsuccessful</returns>
        public static bool Write(string filePath, INIContent contents)
        {
            // Creates a sorted dictionary of the elements, this ensures that elements get stored under their correct section.
            SortedDictionary<string, string> sortedElements = new SortedDictionary<string, string>(contents.elements);
            string output = "";
            string currentSection = "";
            string id = "";

            foreach (KeyValuePair<string, string> pair in sortedElements)
            {
                id = pair.Key.Substring(pair.Key.IndexOf(".") + 1);
                if (pair.Key.Substring(0, pair.Key.IndexOf(".")) != currentSection)
                {
                    currentSection = pair.Key.Substring(0, pair.Key.IndexOf("."));
                    output += string.Format("[{0}]\n", currentSection);
                }
                output += string.Format("{0}={1}\n", id, pair.Value);
            }

            // If the directory for storing the file doesn't exist, creates it.
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                }
                catch
                {
                    return false;
                }
            }

            // Writes to the file.
            try
            {
                File.WriteAllText(filePath, output);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Parses the file into identities and sections.
        /// </summary>
        /// <param name="content">The contents of the file as a string array</param>
        private static INIContent ParseFile(string[] content)
        {
            INIContent ini = new INIContent();
            string currentSection = "";
            string id = "";
            string value = "";
            foreach (string line in content)
            {
                // Determines if this line is a Section or element line.
                if (line.StartsWith("["))
                {
                    currentSection = line.Substring(1, line.Length - 2);
                }
                else if (line != "")
                {
                    id = line.Substring(0, line.IndexOf("="));
                    value = line.Substring(line.IndexOf("=") + 1);
                    if (!ini.elements.ContainsKey(string.Format("{0}.{1}", currentSection, id)))
                    {
                        ini.elements.Add(string.Format("{0}.{1}", currentSection, id), value);
                    }
                    else
                    {
                        ini.elements[string.Format("{0}.{1}", currentSection, id)] = value;
                    }
                }
            }
            return ini;
        }
        /// <summary>
        /// Load an elements file.
        /// </summary>
        /// <param name="filePath">The path to the INI file</param>
        /// <returns>A INIContent object that contains the elements.</returns>
        /// <exception cref="System.IO.FileNotFoundException">When the INI file could not be found raises a File Not Found Exception</exception>
        public static INIContent Read(string filePath)
        {
            if (File.Exists(filePath))
            {
                return ParseFile(File.ReadAllLines(filePath));
            }
            throw new FileNotFoundException("The INI file at: " + filePath + " could not be found!");
        }
    }
}