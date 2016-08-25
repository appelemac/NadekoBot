using NadekoBot.Services.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NadekoBot.Services.Database.Repositories.Impl
{
    public class GlobalReactionRepository : Repository<CustomGlobalReaction>, IGlobalReactionRepository
    {
        public GlobalReactionRepository(DbContext context) : base(context)
        {
        }
    }
}
