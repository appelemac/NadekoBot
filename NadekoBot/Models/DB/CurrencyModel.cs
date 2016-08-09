using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NadekoBot.Models.DB
{
    public class CurrencyModel : IDataModel
    {
        public long UserId { get; set; }
        public long ServerId { get; set; }
        public int CurrencyAmount { get; set; }
    }
}
