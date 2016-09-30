using Discord;
using Discord.Commands;
using NadekoBot.Attributes;
using NadekoBot.Extensions;
using NadekoBot.Services;
using NadekoBot.Services.Database.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;

namespace NadekoBot.Modules.CustomReactions
{
    [NadekoModule("Custom reactions", ".", AppendSpace = false)]
    public class CustomReactions : DiscordModule
    {
        private HashSet<CustomReaction> _globalReactions;
        private HashSet<CustomReaction> _serverReactions;
        private ReactionFormatProvider _formatProvider = new ReactionFormatProvider();
        private NadekoRandom _random = new NadekoRandom();

        public CustomReactions(ILocalization loc, CommandService cmds, ShardedDiscordClient client) : base(loc, cmds, client)
        {
            using (var uow = DbHandler.UnitOfWork())
            {
                var crs = uow.CustomReactions.GetAll();
                //init regex fro those that have it
                foreach (var cr in crs.Where(x => x.IsRegex))
                {
                    cr.Regex = new Regex(cr.Trigger, RegexOptions.Compiled);
                }
                _globalReactions = new HashSet<CustomReaction>(crs.Where(x => x.ServerId == null));
                _serverReactions = new HashSet<CustomReaction>(crs.Except(_globalReactions));
            }
            client.MessageReceived += OnMessageReceived;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task OnMessageReceived(IMessage arg)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (arg is IUserMessage)
            {
                var t = Task.Run(() => CustomReactionChecker((IUserMessage)arg));
            }

        }

        private async Task CustomReactionChecker(IUserMessage msg)
        {
            foreach (var reaction in _globalReactions)
            {
                if (reaction.IsRegex)
                {
                    Match m;
                    if ((m = reaction.Regex.Match(msg.Content)) != null)
                    {
                        _log.Info($"CustomCommand: {msg.Content}");
                        await React(msg, reaction, m);
                    }
                }
                else if (msg.Content.StartsWith(reaction.Trigger))
                {
                    _log.Info($"CustomCommand: {msg.Content}");
                    await React(msg, reaction);
                }
            }
            var guildId = (long)(msg.Channel as IGuildChannel).Guild.Id;
            //same for serverReactions
            foreach (var reaction in _serverReactions.Where(s => s.ServerId == guildId))
            {
                if (reaction.IsRegex)
                {
                    Match m;
                    if ((m = reaction.Regex.Match(msg.Content)) != null)
                    {
                        _log.Info($"CustomCommand: {msg.Content}");
                        await React(msg, reaction, m);
                    }
                }
                else if (msg.Content.StartsWith(reaction.Trigger))
                {
                    _log.Info($"CustomCommand: {msg.Content}");
                    await React(msg, reaction);
                }
            }
        }

        private async Task<IMessage[]> React(IUserMessage msg, CustomReaction reaction, Match m = null) => (await msg.ReplyLong(string.Format(_formatProvider, reaction.Responses[_random.Next(0, reaction.Responses.Count - 1)].Text, msg.Author, msg.MentionedUsers, m, _random)));

        [LocalizedCommand, LocalizedAlias, LocalizedRemarks, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        //inputting this perm for now
        [RequirePermission(GuildPermission.ManageGuild)]
        public async Task AddCustReact(IUserMessage msg, [Remainder] CustomReaction reaction)
        {
            //ascertain that only the owner makes a global reaction
            if (!NadekoBot.Credentials.IsOwner(msg.Author) && reaction.ServerId == null)
                reaction.ServerId = (long)(msg.Channel as IGuildChannel).Guild.Id;
            if (reaction.ServerId == null)
            {
                //global reaction
                _globalReactions.Add(reaction);
            }
            else
            {
                //server reaction
                _serverReactions.Add(reaction);
            }
            using (var uow = DbHandler.UnitOfWork())
            {
                uow.CustomReactions.Add(reaction);
                await uow.CompleteAsync();
            }
            _log.Info("Added new custom reaction: " + reaction.ToString());
            await msg.Reply("Added reaction: " + reaction.ToString());
        }
        public int PageSize { get; } = 5;
        [LocalizedCommand, LocalizedAlias, LocalizedRemarks, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        public async Task ListCustReact(IUserMessage msg, [Remainder] string triggerOrPage = null)
        {
            int page = -1;
            if (triggerOrPage != null && !int.TryParse(triggerOrPage, out page))
            {
                var item = _serverReactions.FirstOrDefault(x => x.Trigger == triggerOrPage) ?? _globalReactions.FirstOrDefault(x => x.Trigger == triggerOrPage);

                if (item == null)
                {
                    await msg.Reply("Reaction not found");
                    return;
                }
                await msg.ReplyLong("Found reaction:\n" + item.ToString());
                return;
            }

            //Page of the thing
            page = page < 0 ? 0 : page;
            List<CustomReaction> crs;
            crs = new List<CustomReaction>();
            crs.AddRange(_globalReactions);
            crs.AddRange(_serverReactions);

            var items = crs.Skip(PageSize * page).Take(PageSize);
            StringBuilder sb = new StringBuilder($"Custom reactions on page {page}/{System.Math.Ceiling((double)crs.Count / PageSize)}:\n");
            foreach (var item in items)
            {
                sb.AppendLine(item.ToStringWithoutResponses());
            }
            await msg.ReplyLong(sb.ToString());
        }

        [LocalizedCommand, LocalizedAlias, LocalizedRemarks, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        //inputting this perm for now
        [RequirePermission(GuildPermission.ManageGuild)]
        public async Task DelCustReact(IUserMessage msg, [Remainder] string trigger)
        {
            var items = _globalReactions.Where(x => x.Trigger == trigger).Union(_serverReactions.Where(x => x.Trigger == trigger)).ToList();

            if (items.Count == 0)
            {
                await msg.Reply("reaction not found");
                return;
            }
            else if (items.Count > 1)
            {
                //What do we do on multiple matches?

            }
            else
            {
                var item = items[0];
                if (item.ServerId == null && !NadekoBot.Credentials.IsOwner(msg.Author))
                {
                    await msg.Reply("Only the bot owner can delete global reactions, you should disable using the permissions system if wanted");
                    return;
                }
                if (item.ServerId == null)
                {
                    _globalReactions.Remove(item);
                }
                else
                {
                    _serverReactions.Remove(item);
                }
                using (var uow = DbHandler.UnitOfWork())
                {
                    //does this work if the ID is not known?
                    uow.CustomReactions.Remove(item);
                }
            }
        }
    }
}