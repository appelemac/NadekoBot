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

namespace NadekoBot.Modules.CustomReactions
{
    public class CustomReactionsModule : DiscordModule
    {
        public List<CustomGlobalReaction> _globalReactions;
        public List<CustomServerReaction> _serverReactions;

        public CustomReactionsModule(ILocalization loc, CommandService cmds, IBotConfiguration config, DiscordSocketClient client) : base(loc, cmds, config, client)
        {
            //todo initialize that shit :D
            //including creating compiled regexes!
            if (config.DontJoinServers) //EnableCustomReactions
            {

                client.MessageReceived += CustomReactionsChecker;
            }
        }
        private async Task CustomReactionsChecker(IMessage arg)
        {
            if (arg.Channel is IGuildChannel)
            {
                //first check for global
                foreach (var reaction in _globalReactions)
                {
                    if (reaction.IsRegex)
                    {
                        Match m;
                        if ((m = reaction.Regex.Match(arg.Content)) != null)
                        {
                            //todo send reaction
                        }
                    }
                    else if (arg.Content.StartsWith(reaction.Trigger))
                    {
                        //todo send reaction
                    }
                }
                //same for serverReactions
                foreach (var reaction in _serverReactions)
                {
                    if (reaction.IsRegex)
                    {
                        Match m;
                        if ((m = reaction.Regex.Match(arg.Content)) != null)
                        {
                            //todo send reaction
                        }
                    }
                    else if (arg.Content.StartsWith(reaction.Trigger))
                    {
                        //todo send reaction
                    }
                }
            }
        }
    }
}
