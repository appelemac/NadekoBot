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
using System.Reflection;
using System.Text;

namespace NadekoBot.Modules.CustomReactions
{
    [Module(".", AppendSpace = false)]
    public class CustomReactions : DiscordModule
    {
        private ReactionFormatProvider formatProvider;
        private HashSet<CustomGlobalReaction> _globalReactions;
        private HashSet<CustomServerReaction> _serverReactions;
        private static Random _rand = new Random();
        private ulong currentUserId = 0;
        private Regex _creationCommandRegex;
        public const string SERVER_PREFIX = "server ";

        public CustomReactions(ILocalization loc, CommandService cmds, DiscordSocketClient client) : base(loc, cmds, client)
        {
          
            //if (config.EnableCustomReactions) //EnableCustomReactions
            //{
                _log.Info("Initializing Custom Reactions");
                InitReactions();
                client.MessageReceived += CustomReactionsChecker;
                _creationCommandRegex = new Regex($"^(?<serverCommand>{SERVER_PREFIX}\\s)?(?<trigger>(?<regexTrigger>/.*?/)|(?<normalTrigger>\".*?\")|(?<basictrigger>\\S*)) (?<responses>(?<response>.*?(?=\\||$))+)$", RegexOptions.Compiled);
            //}
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

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task CustomReactionsChecker(IMessage arg)
#pragma warning restore CS1998
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(async () =>
            {
                if (arg.Author.Id != currentUserId && arg.Channel is IGuildChannel)
                {
                    var msg = arg as IUserMessage;
                    if (msg == null) return;

                    //first check for global
                    foreach (var reaction in _globalReactions)
                    {
                        if (reaction.IsRegex)
                        {
                            Match m;
                            if ((m = reaction.Regex.Match(arg.Content)) != null)
                            {
                                _log.Info($"CustomCommand: {arg.Content}");
                                await React(msg, reaction, m);
                            }
                        }
                        else if (arg.Content.StartsWith(reaction.Trigger))
                        {
                            _log.Info($"CustomCommand: {arg.Content}");
                            await React(msg, reaction);
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
                                await React(msg, reaction, m);
                            }
                        }
                        else if (arg.Content.StartsWith(reaction.Trigger))
                        {
                            _log.Info($"CustomCommand: {arg.Content}");
                            await React(msg, reaction);
                        }
                    }
                }
            });
#pragma warning restore CS4014
        }

        [LocalizedCommand, LocalizedDescription, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        public async Task ListCustReact(IUserMessage msg)
        {
            var channel = msg.Channel as IGuildChannel;
            //todo add pagify
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Global Reactions: ```xl");
            foreach (var reaction in _globalReactions)
            {
                builder.AppendLine(reaction.ToString());
            }
            builder.AppendLine("```\nServer Reactions:```xl");
            foreach (var reaction in _serverReactions)
            {
                builder.AppendLine(reaction.ToString());
            }
            await msg.ReplyLong(builder.ToString());
        }

        [LocalizedCommand, LocalizedDescription, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        public async Task DelCustReact(IUserMessage msg, [Remainder] string content)
        {
            var m = Regex.Match(content, $@"({SERVER_PREFIX})?(.*?(?= \d+|$))\s*(\d+)?$");
            if (!m.Success) return;
            string name = m.Groups[2].Value;
            int indexToRemove = -1;
            var channel = msg.Channel as IGuildChannel;
            if (NadekoBot.Credentials.OwnerIds.Contains(msg.Author.Id) && !m.Groups[1].Success)
            {
                //global command
                var reaction = _globalReactions.FirstOrDefault(s => s.Trigger.Equals(name));
                if (reaction == null)
                {
                    await msg.Reply(string.Format("Could not find a trigger corresponding to the given key: \"{0}\" in the global reactions list, try prefixing with \"{1}\"", name, SERVER_PREFIX));
                    return;
                }
                if (m.Groups[3].Success)
                {
                    int index = int.Parse(m.Groups[3].Value);
                    if (reaction.Responses.Count < index)
                    {
                        await msg.Reply("Index too large");
                        return;
                    }
                }
                using (var uow = DbHandler.Instance.GetUnitOfWork())
                {
                    if (indexToRemove >= 0)
                    {
                        reaction.Responses.RemoveAt(indexToRemove);
                        uow.GlobalReactions.Update(reaction);
                        //it's a reference value, so should be updated already
                        await msg.Reply("removed response");
                    } else
                    {
                        uow.GlobalReactions.Remove(reaction.Id);
                        _globalReactions.Remove(reaction);
                        await msg.Reply(string.Format("Succesfully removed reaction from db: {0}", reaction.ToString()));
                    }
                    uow.Complete();
                }
                return;
            }
            else if (name.StartsWith(SERVER_PREFIX))
            {
                name = name.Remove(0, SERVER_PREFIX.Length);
            }
            //server reaction
            var serverReaction = _serverReactions.FirstOrDefault(s => s.Trigger.Equals(name));
            if (serverReaction == null)
            {
                await msg.Reply(string.Format("Could not find a trigger corresponding to the given key: \"{0}\" in the server reactions list", name));
                return;
            }
            if (m.Groups[3].Success)
            {
                int index = int.Parse(m.Groups[3].Value);
                if (serverReaction.Responses.Count < index)
                {
                    await msg.Reply("Index too large");
                    return;
                }

            }
            using (var uow = DbHandler.Instance.GetUnitOfWork())
            {
                if (indexToRemove >= 0)
                {
                    serverReaction.Responses.RemoveAt(indexToRemove);
                    uow.ServerReactions.Update(serverReaction);
                    //it's a reference value, so should be updated already
                    await msg.Reply("removed response");
                }
                else
                {
                    uow.GlobalReactions.Remove(serverReaction.Id);
                    _serverReactions.Remove(serverReaction);
                    await msg.Reply(string.Format("Succesfully removed reaction from db: {0}", serverReaction.ToString()));
                }

            }
            _serverReactions.Remove(serverReaction);
            await msg.Reply(string.Format("Succesfully removed reaction from db: {0}", serverReaction.ToString()));
            return;

        }
        [LocalizedCommand, LocalizedDescription, LocalizedSummary]
        public async Task AddCustReact(IUserMessage msg, [Remainder] string message)
        {
            if (_creationCommandRegex == null)
            {
                _log.Error("Custom Reactions have been disabled");
                return;
            }
            Match m = _creationCommandRegex.Match(message);
            if (!m.Success)
            {
                _log.Debug("Match failed");
                await msg.Reply("Could not Match to format");
                return;
            }
            string trigger = m.Groups["regexTrigger"].Success ? m.Groups["regexTrigger"].Value : m.Groups["normalTrigger"].Success ? m.Groups["normalTrigger"].Value : m.Groups["basictrigger"].Value;
            List<string> responses = new List<string>();
            foreach (Group capture in m.Groups["responses"].Captures)
            {
                responses.Add(capture.Value);
            }
            ICustomReaction reaction;
            if (m.Groups["serverCommand"].Success || !NadekoBot.Credentials.OwnerIds.Contains(msg.Author.Id))
            {
                //create server command
                reaction = CreateReaction(trigger, m.Groups["regexTrigger"].Success, responses, serverId: (long)(msg.Channel as IGuildChannel).Id);
            }
            else
            {
                //global command
                reaction = CreateReaction(trigger, m.Groups["regexTrigger"].Success, responses);
            }
            using (var uow = DbHandler.Instance.GetUnitOfWork())
            {
                if (reaction is CustomGlobalReaction)
                    uow.GlobalReactions.Add((CustomGlobalReaction)reaction);
                else
                    uow.ServerReactions.Add((CustomServerReaction)reaction);
                uow.Complete();
            }
            await msg.Reply("Created custom reaction: \n" + reaction.ToString());

        }

        private ICustomReaction CreateReaction(string trigger, bool isRegex, IEnumerable<string> responses, long serverId = 0)
        {
            ICustomReaction reaction;
            if (serverId != 0)
                reaction = new CustomServerReaction() { ServerId = serverId };
            else
                reaction = new CustomGlobalReaction();

            reaction.IsRegex = isRegex;
            reaction.Trigger = trigger;
            reaction.Responses = responses.Select(s => new Response() { Text = s }).ToList();
            if (isRegex)
                reaction.Regex = new Regex(trigger, RegexOptions.Compiled);

            return reaction;
        }


        private async Task<IMessage> React<T>(IUserMessage msg, T customReact, Match m = null) where T : ICustomReaction => await msg.Reply(string.Format(formatProvider, customReact.Responses[_rand.Next(0, customReact.Responses.Count)].Text, msg.Author, msg.MentionedUsers, _rand)); //todo add possible objects here

        

    }
    
}

