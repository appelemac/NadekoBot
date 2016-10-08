using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using NadekoBot.Modules.CustomReactions;
using NadekoBot.Modules.Permissions;
using NadekoBot.Services.Database.Models;

namespace NadekoBot.TypeReaders
{
    public class PermissionCommandTypeReader : TypeReader
    {
        TypeReaders.CommandTypeReader permCommandReader { get; set; }

        /// <summary>
        /// rough and riddled implementation
        /// </summary>
        /// <param name="context"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<TypeReaderResult> Read(IUserMessage context, string input)
        {
            if (permCommandReader == null)
                permCommandReader = new CommandTypeReader();
            var permResult = await permCommandReader.Read(context, input);
            if (permResult.IsSuccess) return TypeReaderResult.FromSuccess(new PermissionCommand { Command = (Command)permResult.Values.FirstOrDefault().Value, IsCommand = true });
            if (input.StartsWith("CR")) input = input.Substring(2).Trim();
            else return TypeReaderResult.FromError(permResult);
            int id;
            if (!int.TryParse(input, out id))
            {
                return TypeReaderResult.FromError(CommandError.ParseFailed, "Custom reactions identified by id");
            }
            var reactions = CustomReactions.GlobalReactions;
            reactions.UnionWith(CustomReactions.ServerReactions);
            var found = reactions.FirstOrDefault(x => x.Id == id);
            if (found != null)
            {
                return TypeReaderResult.FromSuccess(new PermissionCommand { CustomReaction = found, IsCommand = false });
            }
            return TypeReaderResult.FromError(CommandError.ObjectNotFound, "id not found");
        }
    }
}
