using Discord;
using Discord.Commands;
using NadekoBot.Attributes;
using NadekoBot.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NadekoBot.Modules.CustomCommands
{
    [Module]
    public class CCManager
    {
        [LocalizedCommand, LocalizedDescription, LocalizedSummary]
        public static async Task CreateCommand(IMessage msg, string name, [Remainder] string response)
        {
            //First word == name
            CCBuilder builder = new CCBuilder(name, response);
            builder.Save(NadekoClient.ExternalCommandsDir);
            await NadekoClient.InitializeExternalCommands();
            await msg.Reply("Done");
        }
        [LocalizedCommand, LocalizedDescription, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        public async Task DeleteCommand(IMessage msg, string name)
        {
            var channel = msg.Channel as IGuildChannel;
            var files = Directory.EnumerateFiles(NadekoClient.ExternalCommandsDir);
            var toDelete = files.Where(s => s.StartsWith(name)).ToList();
            if (toDelete.Count == 0)
            {
                await msg.Reply("No command to delete found.");
                
            }
            else 
            {
                File.Delete(Path.Combine(NadekoClient.ExternalCommandsDir, toDelete.First()));
            }
            //should not be otherwise
        }
    }
}
