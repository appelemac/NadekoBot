using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using System;
using NadekoBot.Classes;
using NadekoBot.Attributes;
using System.Text;
using System.Reflection;

namespace NadekoBot.Modules.Utility
{
    [Module(".")]
    //TODO REMOVE APPENDSPACE
    public class UtilityModule
    {
        [LocalizedCommand, LocalizedDescription, LocalizedSummary, RequireContext(ContextType.Guild)]
        public async Task WhoPlays(IMessage msg, [Remainder] string game)
        {
            var chnl = (IGuildChannel)msg.Channel;
            game = game.Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(game))
                return;
            var arr = (await chnl.Guild.GetUsersAsync())
                    .Where(u => u.Game?.Name.ToUpperInvariant() == game)
                    .Select(u => u.Username)
                    .ToList();

            int i = 0;
            if (!arr.Any())
                await msg.Channel.SendMessageAsync("`Noone is playing that game.`").ConfigureAwait(false);
            else
                await msg.Channel.SendMessageAsync("```xl\n" + string.Join("\n", arr.GroupBy(item => (i++) / 3).Select(ig => string.Concat(ig.Select(el => $"• {el,-35}")))) + "\n```").ConfigureAwait(false);

        }
        [LocalizedCommand, LocalizedDescription, LocalizedSummary, RequireContext(ContextType.Guild)]
        public async Task InRole(IMessage msg, [Remainder] string roles)
        {
            if (string.IsNullOrWhiteSpace(roles))
                return;
            var channel = msg.Channel as IGuildChannel;
            var arg = roles.Split(',').Select(r => r.Trim().ToUpperInvariant());
            StringBuilder send = new StringBuilder($"`Here is a list of users in a specfic role:`");
            foreach (var roleStr in arg.Where(str => !string.IsNullOrWhiteSpace(str) && str != "@EVERYONE" && str != "EVERYONE"))
            {
                var role = channel.Guild.Roles.Where(r => r.Name.ToUpperInvariant() == roleStr).FirstOrDefault();
                if (role == null) continue;
                send.Append($"\n`{role.Name}`\n");
                send.Append(string.Join(", ", (await channel.Guild.GetUsersAsync()).Where(u => u.Roles.Contains(role)).Select(u => u.ToString())));
            }
            await msg.ReplyLong(send.ToString(), ",");
        }
        [LocalizedCommand, LocalizedDescription, LocalizedSummary, RequireContext(ContextType.Guild)]
        public async Task CheckMyPerms(IMessage msg)
        {

            StringBuilder builder = new StringBuilder("```\n");
            var user = msg.Author as IGuildUser;
            var perms = user.GetPermissions(msg.Channel as IGuildChannel);
            foreach (var p in perms.GetType().GetProperties().Where(p => !p.GetGetMethod().GetParameters().Any())) {
                builder.AppendLine($"{p.Name} : {p.GetValue(perms, null).ToString()}");
            }
            builder.Append("```");
            await msg.Reply(builder.ToString());
        }
        [LocalizedCommand, LocalizedDescription, LocalizedSummary, RequireContext(ContextType.Guild)]
        public async Task UserId(IMessage msg, IGuildUser target = null)
        {
            var usr = target ?? msg.Author;
            await msg.Reply($"Id of the user { usr.Username } is { usr.Id })");
        }
        [LocalizedCommand, LocalizedDescription, LocalizedSummary]
        public async Task ChannelId(IMessage msg)
        {
            await msg.Reply($"This Channel's ID is {msg.Channel.Id}");
        }
        [LocalizedCommand, LocalizedDescription, LocalizedSummary, RequireContext(ContextType.Guild)]
        public async Task ServerId(IMessage msg)
        {
            await msg.Reply($"This server's ID is {(msg.Channel as IGuildChannel).Guild.Id}");
        }
        [LocalizedCommand, LocalizedDescription, LocalizedSummary, RequireContext(ContextType.Guild)]
        public async Task Roles(IMessage msg, IGuildUser target = null)
        {
            if (target != null)
            {
                await msg.Reply($"`List of roles for **{target.Username}**:` \n• " + string.Join("\n• ", target.Roles));
            } else
            {
                await msg.Reply("`List of roles:` \n• " + string.Join("\n• ", (msg.Channel as IGuildChannel).Guild.Roles));
            }
        }
        [LocalizedCommand, LocalizedDescription, LocalizedSummary, RequireContext(ContextType.Guild)]
        public async Task Remind(IMessage msg, string meOrChannel, string time, [Remainder] string message)
        {
           await Commands.Reminders.CreateReminder(msg, meOrChannel, time, message);
        }
    }
}