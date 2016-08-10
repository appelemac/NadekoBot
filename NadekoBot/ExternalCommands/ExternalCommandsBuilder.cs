using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using NadekoBot.Classes;
using System.Runtime.Loader;
using Discord;
using Discord.Commands;

namespace NadekoBot.ExternalCommands
{
    public class ExternalCommandsBuilder
    {
        private List<PortableExecutableReference> _references;
        private List<string> _codeContents;
        private Logger _logger;
        private string _compiledLocation;

        internal ExternalCommandsBuilder()
        {
            _references = new List<PortableExecutableReference>();
            _codeContents = new List<string>();
            LoadDefaultReferences();
        }

        private void LoadDefaultReferences()
        {
            var temp = new List<Assembly>() {
                {typeof(object).GetTypeInfo().Assembly},
                {typeof(IMessage).GetTypeInfo().Assembly },
                {typeof(CommandAttribute).GetTypeInfo().Assembly },
                {typeof(NadekoClient).GetTypeInfo().Assembly }
            };
            _references.AddRange(temp.Select(x => MetadataReference.CreateFromFile(x.Location)));

            var dotnetAssemblies = new List<string>()
            {
                { "mscorlib.dll"},
                { "System.Runtime.dll"},
                { "System.Threading.Tasks.dll"}
            };
            var locationPath = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);
            foreach (var a in dotnetAssemblies)
            {
                var p = Path.Combine(locationPath, a);
                _references.Add(MetadataReference.CreateFromFile(p));
            }
        }

        internal Assembly Compile()
        {
            Reset();
            if (_references.Count == 0 || _codeContents.Count == 0) throw new InvalidOperationException("references or code empty");
            List<SyntaxTree> trees = new List<SyntaxTree>();
            foreach (var classContent in _codeContents)
            {
                trees.Add(CSharpSyntaxTree.ParseText(classContent));
            }
            var assemblyName = Path.GetRandomFileName();
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: trees,
                references: _references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                );
            using (MemoryStream ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);
                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
           diagnostic.IsWarningAsError ||
           diagnostic.Severity == DiagnosticSeverity.Error);
                    _logger.LogError(NadekoBot.Strings.ExternalCommandsBuilder_Compile_FailedCreatingCommandsFollowingErrorsOccurredN);
                    foreach (var error in failures) _logger.LogError($"{error.Id}: {error.GetMessage()}");
                    return null;
                }
                _logger.LogDebug(NadekoBot.Strings.ExternalCommandsBuilder_Compile_SuccesfullyCompiledCommands);
                ms.Seek(0, SeekOrigin.Begin);
                _compiledLocation = assemblyName;
                return AssemblyLoadContext.Default.LoadFromStream(ms);
            }
        }

        private void AddReference(PortableExecutableReference reference)
        {
            Reset();
            _references.Add(reference);
        }

        internal void AddReference(string refLocation) => AddReference(MetadataReference.CreateFromFile(refLocation));

        private void RemoveReference(PortableExecutableReference reference)
        {
            Reset();
            _references.Remove(reference);
        }

        internal void RemoveReference(string refLocation) => RemoveReference(MetadataReference.CreateFromFile(refLocation));

        internal void LoadReferences(string path)
        {
            var paths = Directory.GetFiles(path);
            foreach (var filepath in paths)
            {
                AddReference(filepath);
            }
        }

        internal void AddCode(string classOfCode)
        {
            _codeContents.Add(classOfCode);
        }

        internal void RemoveCode(string classOfCode)
        {
            _codeContents.Remove(classOfCode);
        }

        internal void Reset()
        {
            if (_compiledLocation != null && File.Exists(_compiledLocation)) File.Delete(_compiledLocation);
        }
    }
}
