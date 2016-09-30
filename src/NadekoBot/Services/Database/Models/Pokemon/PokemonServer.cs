using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NadekoBot.Services.Database.Models
{
    public class PokemonServer : DbEntity
    {
        public long ServerId { get; set; }
        public List<PokemonPlayer> Players { get; set; } = new List<PokemonPlayer>();
    }
}
