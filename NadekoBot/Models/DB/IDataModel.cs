using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NadekoBot.Models.DB
{
    internal abstract class IDataModel
    {
        public int? Id { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
        public IDataModel() { }
    }
}
