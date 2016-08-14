using Discord;
using Discord.Commands;
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
        [Command("createCommand")]
        public static async Task CreateCommand(IMessage msg, string name, [Remainder] string response)
        {
            //First word == name
            CCBuilder builder = new CCBuilder(name, response);
            builder.Save(Path.Combine(NadekoClient.DataDir, "External Commands"));
            await NadekoClient.InitializeExternalCommands();
            await msg.Channel.SendMessageAsync("Done");
        }
    }
}
