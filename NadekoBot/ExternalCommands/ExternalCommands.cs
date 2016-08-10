using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NadekoBot.ExternalCommands
{
    public class ExternalCommands
    {
        private bool _compiled;
        public List<string> CommandFiles;

        public ExternalCommands(string directoryPath)
        {
            var files = Directory.EnumerateFiles(directoryPath);
            foreach (var file in files)
            {
                CommandFiles.Add(File.ReadAllText(file));
            }
        }
        public Assembly getResultingAssembly()
        {

        }
        
    }
}