using Microsoft.EntityFrameworkCore;
using NadekoBot.Database;
using NadekoBot.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NadekoBot.Classes
{
    public static class CurrencyHandler
    {
        public static async Task<int> GetMoney(this ADataBase DB, ulong userId, ulong serverId)
        {
            var currencyModel = await DB.Currency.FirstOrDefaultAsync(x => (ulong)x.UserId == userId && (ulong)x.ServerId == serverId);

            if (currencyModel == null)
            {
                currencyModel = new CurrencyModel()
                {
                    ServerId = (long)serverId,
                    UserId = (long)userId,
                    CurrencyAmount = 0
                };
                DB.Currency.Add(currencyModel);
                //DB.SaveChanges();
            }
            return currencyModel.CurrencyAmount;
        }
        /// <summary>
        /// Change the CurrencyAmount for the given user in given server in given DbContext with given amount
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="userId"></param>
        /// <param name="serverId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static async Task ChangeMoney(this ADataBase DB, ulong userId, ulong serverId, int amount)
        {
            var currencyModel = await DB.Currency.FirstOrDefaultAsync(x => (ulong)x.UserId == userId && (ulong)x.ServerId == serverId);
            //EntityState state;
            if (currencyModel == null)
            {
                currencyModel = new CurrencyModel()
                {
                    ServerId = (long)serverId,
                    UserId = (long)userId,
                    CurrencyAmount = amount
                };
                //state = EntityState.Added;
                DB.Currency.Add(currencyModel);
            }
            else
            {
                //state = EntityState.Modified;
                var attached = DB.Currency.Attach(currencyModel).Property("CurrencyAmount");
                attached.CurrentValue = (int)attached.OriginalValue + amount;
                attached.IsModified = true;
            }
            //DB.Entry(currencyModel).State = state;
        }
    }
}
