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
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace NadekoBot.Modules.Utility
{
    [Module(".")]
    //TODO REMOVE APPENDSPACE
    public class UtilityModule
    {
        [LocalizedCommand, LocalizedDescription, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        public async Task List(IMessage msg)
        {
            var channel = msg.Channel as IGuildChannel;
            await msg.Reply(string.Join("\n-", NadekoClient.CommandService.Commands.Select(cmd => cmd.Text)));
        }
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
            var perms = user.GetPermissions(msg.Channel as ITextChannel);
            foreach (var p in perms.GetType().GetProperties().Where(p => !p.GetGetMethod().GetParameters().Any()))
            {
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
            var guild = (msg.Channel as IGuildChannel).Guild;
            if (target != null)
            {
                await msg.Reply($"`List of roles for **{target.Username}**:` \n• " + string.Join("\n• ", target.Roles.Except(new[] { guild.EveryoneRole })));
            }
            else
            {
                await msg.Reply("`List of roles:` \n• " + string.Join("\n• ", (msg.Channel as IGuildChannel).Guild.Roles.Except(new[] { guild.EveryoneRole })));
            }
        }
        [LocalizedCommand, LocalizedDescription, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        public async Task Prune(IMessage msg, [Remainder] string target = null)
        {
            var channel = msg.Channel as IGuildChannel;

            var user = await channel.Guild.GetCurrentUserAsync();
            if (string.IsNullOrWhiteSpace(target))
            {

                var enumerable = (await msg.Channel.GetMessagesAsync(limit: 100)).Where(x => x.Author.Id == user.Id);
                await msg.Channel.DeleteMessagesAsync(enumerable);
                return;
            }
            target = target.Trim();
            //if (!user.GetPermissions(channel).ManageMessages)
            //{
            //    await msg.Reply("Don't have permissions to manage messages in channel");
            //    return;
            //}
            int count;
            if (int.TryParse(target, out count))
            {
                while (count > 0)
                {
                    int limit = (count < 100) ? count : 100;
                    var enumerable = (await msg.Channel.GetMessagesAsync(limit: limit));
                    await msg.Channel.DeleteMessagesAsync(enumerable);
                    if (enumerable.Count < limit) break;
                    count -= limit;
                }
            }
            else if (msg.MentionedUsers.Count > 0)
            {
                var toDel = new List<IMessage>();

                var match = Regex.Match(target, @"\s(\d+)\s");
                if (match.Success)
                {
                    int.TryParse(match.Groups[1].Value, out count);
                    var messages = new List<IMessage>(count);

                    while (count > 0)
                    {
                        var toAdd = await msg.Channel.GetMessagesAsync(limit: count < 100 ? count : 100);
                        messages.AddRange(toAdd);
                        count -= toAdd.Count;
                    }

                    foreach (var mention in msg.MentionedUsers)
                    {
                        toDel.AddRange(messages.Where(m => m.Author.Id == mention.Id));
                    }
                    //TODO check if limit == 100 or there is no limit
                    await msg.Channel.DeleteMessagesAsync(toDel);
                }

            }


        }
        [LocalizedCommand, LocalizedDescription, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        public async Task TestLong(IMessage msg)
        {
            var builder = new StringBuilder("Testing long message system\n```json\n");
            while (builder.Length < 20000)
            {
                builder.AppendLine($"letter {builder.Length}");
            }
            builder.Append("```");
          await   msg.ReplyLong(builder.ToString(), addToEnd:"```", addToStart: "```json\n");
        }
        [LocalizedCommand, LocalizedDescription, LocalizedSummary, RequireContext(ContextType.Guild)]
        public async Task Remind(IMessage msg, string meOrChannel, string time, [Remainder] string message)
        {
            await Commands.Reminders.CreateReminder(msg, meOrChannel, time, message);
        }
        [LocalizedCommand, LocalizedDescription, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        public async Task ServerInfo(IMessage msg, IGuild guild = null)
        {
            var channel = msg.Channel as IGuildChannel;
            var server = guild ?? channel.Guild;
            
            if (server == null)
                return;
            var createdAt = new DateTime(2015, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(server.Id >> 22);
            var sb = new StringBuilder();
            sb.AppendLine($"`Name:` **#{server.Name}**");
            sb.AppendLine($"`Owner:` **{await server.GetUserAsync(server.OwnerId)}**");
            sb.AppendLine($"`Id:` **{server.Id}**");
            sb.AppendLine($"`Icon Url:` **{ server.IconUrl}**");
            sb.AppendLine($"`TextChannels:` **{(await server.GetTextChannelsAsync()).Count()}** `VoiceChannels:` **{(await server.GetVoiceChannelsAsync()).Count()}**");
            var users = await server.GetUsersAsync();
            sb.AppendLine($"`Members:` **{users.Count}** `Online:` **{users.Count(u => u.Status == UserStatus.Online)}** (may be incorrect)");
            sb.AppendLine($"`Roles:` **{server.Roles.Count()}**");
            sb.AppendLine($"`Created At:` **{createdAt}**");
            if (server.Emojis.Count() > 0)
                sb.AppendLine($"`Custom Emojis:` **{string.Join(", ", server.Emojis)}**");
            if (server.Features.Count() > 0)
                sb.AppendLine($"`Features:` **{string.Join(", ", server.Features)}**");
            if (!string.IsNullOrWhiteSpace(server.SplashUrl))
                sb.AppendLine($"`Region:` **{server.VoiceRegionId}**");
            await msg.Reply(sb.ToString()).ConfigureAwait(false);
        }
        [LocalizedCommand, LocalizedDescription, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        public async Task ChannelInfo(IMessage msg, ITextChannel channel = null)
        {
          
            var ch = channel ?? msg.Channel as ITextChannel;
            if (ch == null)
                return;
            var createdAt = new DateTime(2015, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(ch.Id >> 22);
            var sb = new StringBuilder();
            sb.AppendLine($"`Name:` **#{ch.Name}**");
            sb.AppendLine($"`Id:` **{ch.Id}**");
            sb.AppendLine($"`Created At:` **{createdAt}**");
            sb.AppendLine($"`Topic:` **{ch.Topic}**");
            sb.AppendLine($"`Users:` **{(await ch.GetUsersAsync()).Count()}**");
            await msg.Reply(sb.ToString()).ConfigureAwait(false);
        }
        [LocalizedCommand, LocalizedDescription, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        public async Task UserInfo(IMessage msg, IGuildUser usr = null)
        {
            var channel = msg.Channel as IGuildChannel;
            var user = usr ?? msg.Author as IGuildUser;
            if (user == null)
                return;
            var sb = new StringBuilder();
            sb.AppendLine($"`Name#Discrim:` **#{user.Username}#{user.Discriminator}**");
            if (!string.IsNullOrWhiteSpace(user.Nickname))
                sb.AppendLine($"`Nickname:` **{user.Nickname}**");
            sb.AppendLine($"`Id:` **{user.Id}**");
            sb.AppendLine($"`Current Game:` **{(user.Game?.Name == null ? "-" : user.Game.Name)}**");
            //if (user. != null)
            //    sb.AppendLine($"`Last Online:` **{user.LastOnlineAt:HH:mm:ss}**");
            sb.AppendLine($"`Joined At:` **{user.JoinedAt}**");
            sb.AppendLine($"`Roles:` **({user.Roles.Count()}) - {string.Join(", ", user.Roles.Select(r => r.Name))}**");
            sb.AppendLine($"`AvatarUrl:` **{user.AvatarUrl}**");
            await msg.Reply(sb.ToString()).ConfigureAwait(false);
        }
    }
}