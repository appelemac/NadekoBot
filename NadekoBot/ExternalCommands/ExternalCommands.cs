using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NadekoBot.ExternalCommands
{
    public class ExternalCommands
    {
        private bool _compiled;
        public List<string> CommandFiles = new List<string>();
        internal string _path;
        private ExternalCommandsBuilder _builder;
        public ExternalCommands(string directoryPath)
        {
            
            _path = directoryPath;
            var files = Directory.EnumerateFiles(directoryPath);
            foreach (var file in files)
            {
                CommandFiles.Add(File.ReadAllText(file));
            }
            _builder = new ExternalCommandsBuilder();
            CommandFiles.ForEach(x => _builder.AddCode(x));
            _builder.LoadReferences(Path.Combine(_path, "References"));
        }
        public Assembly getResultingAssembly()
        {
            _compiled = true;
            return _builder.Compile();
        }

    }
}