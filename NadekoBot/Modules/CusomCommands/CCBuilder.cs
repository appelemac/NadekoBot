using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NadekoBot.Modules.CusomCommands
{
    public class CCBuilder
    {
        private string _builder;
        private string _name;
        public CCBuilder(string name, string response)
        {
            _name = name;
            var rand = new Random();
            _builder = init(name, "custom reaction", rand);
            _builder += @"{
        await msg.Channel.SendMessageAsync(" + $"\"{response}\"" + @"); 
        } 
    } 
}";

        }

        private static string init(string name, string description, Random rand) => $@"
using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Text;

namespace NadekoBot.CC.Section{rand.Next()}" + @" {
[Module]
public class ExternalCmd {
" + $"[Command(\"{name}\")]\n [Description(\"{description}\")]\n public static async Task {name}_{rand.Next()}(IMessage msg) ";

        public CCBuilder(string name, string description, string code)
        {
            var rand = new Random();
           _builder = init(name, description, rand);
            _builder += "{" + code + "}}}";
        }
        
        public void Save(string path)
        {
            using (var write = File.CreateText(Path.Combine(path, _name + Directory.EnumerateFiles(path).Count() + ".cs")))
            {
                write.Write(_builder);
            }


              
        }
    }
}
