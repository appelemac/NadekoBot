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
                var crs = uow.CustomReactions.GetList();
                //init regex fro those that have it
                foreach (var cr in crs.Where(x => x.IsRegex))
                {
                    cr.Regex = new Regex(cr.Trigger, RegexOptions.Compiled);
                }
                var comparer = new CustomComparer();
                _serverReactions = new HashSet<CustomReaction>(/*crs.Where(x=>x.ServerId.HasValue),*/ comparer);
                _globalReactions = new HashSet<CustomReaction>(/*crs.Where(x => !x.ServerId.HasValue),*/ comparer);
                if (crs.Any(x => x.ServerId.HasValue))
                    _serverReactions.UnionWith(crs.Where(x => x.ServerId.HasValue));
                if (crs.Any(x => !x.ServerId.HasValue))
                    _globalReactions.UnionWith(crs.Where(x => !x.ServerId.HasValue));
            }
            client.MessageReceived += OnMessageReceived;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

        private async Task OnMessageReceived(IMessage arg)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (arg is IUserMessage && arg.Author.Id != NadekoBot.Client.GetCurrentUser().Id)
            {
                var throwaway = Task.Run(() => CustomReactionChecker((IUserMessage)arg));
            }
        }

        private async Task CustomReactionChecker(IUserMessage msg)
        {
            foreach (var reaction in _globalReactions)
            {
                if (reaction.IsRegex)
                {
                    Match m;
                    if ((m = reaction.Regex.Match(msg.Content)).Success)
                    {
                        _log.Info($@"Custom reaction: ""{msg.Content}"" matched ""{reaction.Trigger}""");
                        await React(msg, reaction, m);
                    }
                }
                else if (msg.Content.StartsWith(reaction.Trigger))
                {
                    _log.Info($@"Custom reaction: ""{msg.Content}"" matched ""{reaction.Trigger}""");
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
                    if ((m = reaction.Regex.Match(msg.Content)).Success)
                    {
                        _log.Info($@"Custom reaction: ""{msg.Content}"" matched ""{reaction.Trigger}""");
                        await React(msg, reaction, m);
                    }
                }
                else if (msg.Content.StartsWith(reaction.Trigger))
                {
                    _log.Info($@"Custom reaction: ""{msg.Content}"" matched ""{reaction.Trigger}""");
                    await React(msg, reaction);
                }
            }
        }

        private async Task<IMessage[]> React(IUserMessage msg, CustomReaction reaction, Match m = null) => (await msg.ReplyLong(string.Format(_formatProvider, reaction.Responses[_random.Next(0, reaction.Responses.Count - 1)].Text, msg, m, _random)));

        [LocalizedCommand, LocalizedAlias, LocalizedRemarks, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        //inputting this perm for now
        [RequirePermission(GuildPermission.ManageGuild)]
        public async Task AddCustReact(IUserMessage msg, [Remainder] CustomReaction reaction)
        {
            //ascertain that only the owner makes a global reaction
            if (!NadekoBot.Credentials.IsOwner(msg.Author) && reaction.ServerId == null)
                reaction.ServerId = (long)(msg.Channel as IGuildChannel).Guild.Id;
            using (var uow = DbHandler.UnitOfWork())
            {
                uow.CustomReactions.Add(reaction);
                await uow.CompleteAsync();
            }
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

            _log.Info("Added new custom reaction: " + reaction.ToString());
            await msg.ReplyLong("Added reaction: " + reaction.ToString());
        }

        [LocalizedCommand, LocalizedAlias, LocalizedRemarks, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        //inputting this perm for now
        [RequirePermission(GuildPermission.ManageGuild)]
        public async Task AddCustReact(IUserMessage msg, int id, [Remainder] string reaction)
        {
            var item = _serverReactions.FirstOrDefault(x => x.Id == id) ?? _globalReactions.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                await msg.Reply("Could not find reaction with id " + id);
                return;
            }
            item.Responses.Add(new Response { Text = reaction });
            using (var uow = DbHandler.UnitOfWork())
            {
                uow.CustomReactions.Update(item);
                await uow.CompleteAsync();
            }
            if (item.ServerId.HasValue)
            {
                _serverReactions.RemoveWhere(x => x.Id == item.Id);
                _serverReactions.Add(item);
            } else
            {
                _globalReactions.RemoveWhere(x => x.Id == item.Id);
                _globalReactions.Add(item);
            }
            await msg.ReplyLong(string.Format("Updated reaction {0} to ```xl\n{1}```", id, item));
        }

        public int PageSize { get; } = 5;

        [LocalizedCommand, LocalizedAlias, LocalizedRemarks, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        public async Task ListCustReact(IUserMessage msg,string pageOrId = "page", [Remainder] int number = 0)
        {
            int page = -1;
            if (pageOrId == "id")
            {
                var item = _serverReactions.FirstOrDefault(x => x.Id == number) ?? _globalReactions.FirstOrDefault(x => x.Id == number);

                if (item == null)
                {
                    await msg.Reply("Reaction not found");
                    return;
                }
                await msg.ReplyLong("Found reaction: ```xl\n" + item.ToString() + "```");
                return;
            }
            page--;
            //Page of the thing

            List<CustomReaction> crs;
            crs = new List<CustomReaction>();
            crs.AddRange(_globalReactions);
            crs.AddRange(_serverReactions);
            var max = (int)System.Math.Ceiling((double)crs.Count / PageSize);
            page = page < 0 ? 0 : page;
            page = page > max - 1 ? max -1 : page;
            var items = crs.Skip(PageSize * page).Take(PageSize);
            StringBuilder sb = new StringBuilder($"Custom reactions on page {page + 1}/{max}:\n");
            foreach (var item in items)
            {
                sb.AppendLine("```json\n" + item.ToStringWithoutResponses() + "```");
            }
            await msg.ReplyLong(sb.ToString());
        }

        [LocalizedCommand, LocalizedAlias, LocalizedRemarks, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        //inputting this perm for now
        [RequirePermission(GuildPermission.ManageGuild)]
        public async Task DelCustReact(IUserMessage msg, [Remainder] int id)
        {
            var item = _serverReactions.FirstOrDefault(x => x.Id == id) ?? _globalReactions.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                await msg.Reply("Could not find reaction with id " + id);
                return;
            }

            if (item.ServerId == null && !NadekoBot.Credentials.IsOwner(msg.Author))
            {
                await msg.Reply("Only the bot owner can delete global reactions, you should disable using the permissions system if wanted");
                return;
            }
            if (!item.ServerId.HasValue)
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
                await uow.CompleteAsync();
            }
            await msg.ReplyLong("Successfully removed custom reaction: " + item.ToString());
        }
    }
}