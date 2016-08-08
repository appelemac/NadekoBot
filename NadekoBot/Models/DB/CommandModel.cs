using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NadekoBot.Models.DB
{
    
    public class CommandModel : IDataModel
    {
        public long UserId { get; set; }
        public long ChannelId { get; set; }
        public string CommandContent { get; set; }
    }
}
