using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using NadekoBot.Services.Database.Models;
using System.Text.RegularExpressions;

namespace NadekoBot.TypeReaders
{
    /// <summary>
    /// Reads in the setup of a *new* custom reaction
    /// This way uses regex, recommend improvement
    /// </summary>
    public class CustomReactionReader : TypeReader
    {
        private const string SERVER_PREFIX = "server ";
        private Regex commandRegex = new Regex($"^(?<serverCommand>{SERVER_PREFIX}\\s)?(?<trigger>(?<regexTrigger>/.*?/)|(?<normalTrigger>\".*?\")|(?<basictrigger>\\S*)) (?<responses>(?<response>.*?(?=\\||$))+)$", RegexOptions.Compiled);
        public override async Task<TypeReaderResult> Read(IUserMessage context, string input)
        {
            var match = commandRegex.Match(input);
            if (!match.Success) return TypeReaderResult.FromError(CommandError.ParseFailed, "match failed");
            var reaction = new CustomReaction();
            if (match.Groups["serverCommand"].Success) reaction.ServerId = (long)(context.Channel as IGuildChannel).Id;
            if (match.Groups["regexTrigger"].Success)
            {
                reaction.IsRegex = true;
                reaction.Trigger = match.Groups["regexTrigger"].Value;
                reaction.Regex = new Regex(reaction.Trigger, RegexOptions.Compiled);
            }
            else
            {
                reaction.IsRegex = false;
                reaction.Trigger = match.Groups["normalTrigger"].Value;

            }
            reaction.Responses = new List<Response>();
            foreach (Group response in match.Groups["responses"].Captures)
            {
                reaction.Responses.Add(new Response { Text = response.Value });
            }
            return TypeReaderResult.FromSuccess(reaction);
        }
    }
}
