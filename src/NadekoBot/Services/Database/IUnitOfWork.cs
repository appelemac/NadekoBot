using NadekoBot.Services.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NadekoBot.Services.Database
{
    public interface IUnitOfWork : IDisposable
    {
        IQuoteRepository Quotes { get; }
        IConfigRepository Configs { get; }
        IDonatorsRepository Donators { get; }
        IServerReactionRepository ServerReactions { get; }
        IGlobalReactionRepository GlobalReactions { get; }
        int Complete();
        Task<int> CompleteAsync();
    }
}
