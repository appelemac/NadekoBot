using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace NadekoBot.Modules.Utility
{
    [Module(".")]
    public class Utilities
    {
        
        [Command("stats")]
        public static async Task StatsCommand(IMessage msg)
        {
            await msg.Channel.SendMessageAsync(NadekoBot.NadekoClient.NadekoStats.ToString());
        }
       
        [Command("rename")]
        
        public static async Task RenameCommand(IMessage msg, [Remainder] string newName)
        {
            if (!NadekoClient.IsOwner(msg.Author.Id))
            {
                await msg.Channel.SendMessageAsync(NadekoBot.Strings.Utilities_RenameCommand_OwnerOnlyCommand);
                return;
            }
            if (string.IsNullOrWhiteSpace(newName) || newName.Length > 20) //username resstrictions should be here
            {
                await msg.Channel.SendMessageAsync(NadekoBot.Strings.Utilities_RenameCommand_NewNameMustBeAllowed);
                return; 
            }
            var current = await NadekoClient.SocketClient.GetCurrentUserAsync();
           await current.ModifyAsync(x => x.Username = newName);
            await msg.Channel.SendMessageAsync(string.Format(NadekoBot.Strings.Utilities_RenameCommand_SuccessChangedUsernameTo0, newName));
        }
    }
}