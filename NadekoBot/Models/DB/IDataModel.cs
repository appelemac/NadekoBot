using System;

namespace NadekoBot.Models.DB
{
    internal abstract class IDataModel
    {
        public int? Id { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;

        public IDataModel()
        {
        }
    }
}