using NadekoBot.Services.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NadekoBot.Services.Database.Repositories.Impl
{
    public class ServerReactionRepository : Repository<CustomServerReaction>, IServerReactionRepository
    {
        public ServerReactionRepository(DbContext context) : base(context)
        {
        }
    }
}
