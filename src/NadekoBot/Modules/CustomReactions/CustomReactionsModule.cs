using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NadekoBot.Services;
using System.Text.RegularExpressions;
using NadekoBot.Services.Database.Models;
using NadekoBot.Extensions;
using NadekoBot.Attributes;

namespace NadekoBot.Modules.CustomReactions
{ 
    [Module(".", AppendSpace =false)]
    public class CustomReactionsModule : DiscordModule
    {
        private ReactionFormatProvider formatProvider;
        private HashSet<CustomGlobalReaction> _globalReactions;
        private HashSet<CustomServerReaction> _serverReactions;
        private static Random _rand = new Random();
        private ulong currentUserId = 0;

        public CustomReactionsModule(ILocalization loc, CommandService cmds, IBotConfiguration config, DiscordSocketClient client) : base(loc, cmds, config, client)
        {
            //todo initialize that shit :D
            //including creating compiled regexes!
            if (config.EnableCustomReactions) //EnableCustomReactions
            {
                _log.Info("Initializing Custom Reactions");
                InitReactions();
                client.MessageReceived += CustomReactionsChecker;
            }
        }

        private void InitReactions()
        {
            currentUserId = _client.GetCurrentUser().Id;
            _globalReactions = new HashSet<CustomGlobalReaction>();
            _serverReactions = new HashSet<CustomServerReaction>();
            formatProvider = new ReactionFormatProvider();
            using (var uow = DbHandler.Instance.GetUnitOfWork())
            {
                foreach (var gReaction in uow.GlobalReactions.GetAll())
                {
                    if (gReaction.IsRegex)
                    {
                        gReaction.Regex = new Regex(gReaction.Trigger, RegexOptions.Compiled);
                    }
                    _globalReactions.Add(gReaction);
                }
                foreach (var sReaction in uow.ServerReactions.GetAll())
                {
                    if (sReaction.IsRegex)
                    {
                        sReaction.Regex = new Regex(sReaction.Trigger, RegexOptions.Compiled);
                    }
                    _serverReactions.Add(sReaction);
                }
            }
        }

        private async Task CustomReactionsChecker(IMessage arg) //should this run a task!?
        {
            if (arg.Author.Id != currentUserId && arg.Channel is IGuildChannel)
            {
                //first check for global
                foreach (var reaction in _globalReactions)
                {
                    if (reaction.IsRegex)
                    {
                        Match m;
                        if ((m = reaction.Regex.Match(arg.Content)) != null)
                        {
                            _log.Info($"CustomCommand: {arg.Content}");
                            await React(arg, reaction, m);
                        }
                    }
                    else if (arg.Content.StartsWith(reaction.Trigger))
                    {
                        _log.Info($"CustomCommand: {arg.Content}");
                        await React(arg, reaction);
                    }
                }
                var guildId = (long)(arg.Channel as IGuildChannel).Guild.Id;
                //same for serverReactions
                foreach (var reaction in _serverReactions.Where(s => s.ServerId == guildId))
                {
                    if (reaction.IsRegex)
                    {
                        Match m;
                        if ((m = reaction.Regex.Match(arg.Content)) != null)
                        {
                            _log.Info($"CustomCommand: {arg.Content}");
                            await React(arg, reaction, m);
                        }
                    }
                    else if (arg.Content.StartsWith(reaction.Trigger))
                    {
                        _log.Info($"CustomCommand: {arg.Content}");
                        await React(arg, reaction);
                    }
                }
            }
        }
        [LocalizedCommand, LocalizedDescription, LocalizedSummary]
        public async Task AddCustReact(IMessage msg, [Remainder] string message)
        {
            if (NadekoBot.Credentials.OwnerIds.Contains(msg.Author.Id))
            {
                if (message.StartsWith("server "))
                {
                    message = message.Remove(0, "server ".Length /* -1 */);
                    //create server-only command
                } else
                {
                    //global command
                    var items = message.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (items.Length > 1) 
                    {
                        //regex
                        CustomGlobalReaction reaction = new CustomGlobalReaction()
                        {
                            Trigger = items[0],
                            IsRegex = true,
                            //todo decide if we want to split responses or do one at a time
                            Responses = new List<Response>() { new Response { Text = items[1] } },
                            Regex = new Regex(items[0],RegexOptions.Compiled)
                            
                        };
                        if (string.IsNullOrWhiteSpace(reaction.Trigger)) return;
                        _globalReactions.Add(reaction);
                        using (var uow = DbHandler.Instance.GetUnitOfWork())
                        {
                            uow.GlobalReactions.Add(reaction);
                        }
                        await msg.Reply("Created custom reaction: " + reaction.ToString());
                    }
                }
            }
        }

        private async Task<IMessage> React<T>(IMessage msg, T customReact, Match m = null) where T : ICustomReaction => await msg.Reply(string.Format(formatProvider, customReact.Responses[_rand.Next(0, customReact.Responses.Count)].Text, msg.Author, msg.MentionedUsers, msg.Content,_rand)); //todo add possible objects here

        public class ReactionFormatProvider : IFormatProvider, ICustomFormatter
        {
            public object GetFormat(Type formatType)
            {
                if (formatType == typeof(ReactionFormatProvider))
                    return this;
                return null;
            }
            public string Format(string format, object arg, IFormatProvider formatProvider)
            {
                if (!Equals(formatProvider)) return null;
                //return null;
                if (string.IsNullOrWhiteSpace(format)) return "";
                switch (format.ToUpperInvariant())
                {
                    case "NAME":
                        IUser user = arg as IUser;
                        if (user == null) return "";
                        return user.Username;
                    case "NICK":
                        IGuildUser guildUsr = arg as IGuildUser;
                        if (guildUsr == null) return "";
                        return guildUsr.Nickname;
                        
                   
                }
                //now for regex matches
                var match = Regex.Match(format, @"RANDOM\[(\d+)\-(\d+)\]");
                if (match.Success)
                {
                    return _rand.Next(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value)).ToString();
                }

                return string.Empty;
            }
        }

    }
}
