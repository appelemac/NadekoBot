using Microsoft.EntityFrameworkCore;
using NadekoBot.Services.Database.Models;

namespace NadekoBot.Services.Database.Repositories.Impl
{
    public class CustomReactionsRepository : Repository<CustomReaction>, ICustomReactionRepository
    {
        public CustomReactionsRepository(DbContext context) : base(context)
        {
        }
    }
}