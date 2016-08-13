using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
namespace NadekoBot.Classes
{
    public class PermsHandler
    {
        public enum PermLevel
        {
            BotOwner,
            ServerOwner,
            ServerManager,
            RoleManager,
        }
    }
    public class OwnerOnly : PreconditionAttribute
    {
       
        public override Task<PreconditionResult> CheckPermissions(IMessage context, Command executingCommand, object moduleInstance)
        {
            if (NadekoClient.IsOwner(context.Author.Id)) return Task.FromResult(PreconditionResult.FromSuccess());
            return Task.FromResult(PreconditionResult.FromError("Not owner"));
        }
    }

    public class CustomPre : PreconditionAttribute
    {
        private Func<IMessage, bool> _function;
        public CustomPre(Func<IMessage, bool> func) : base()
        {
            _function = func;
        }
        public override Task<PreconditionResult> CheckPermissions(IMessage context, Command executingCommand, object moduleInstance) => Task.FromResult(_function.Invoke(context) ? PreconditionResult.FromSuccess() : PreconditionResult.FromError("Did not pass condition"));
    }
}
