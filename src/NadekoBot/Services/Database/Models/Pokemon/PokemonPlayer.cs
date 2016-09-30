using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NadekoBot.Services.Database.Models
{
    public class PokemonPlayer : DbEntity
    {
        public long UserId { get; set; }
        public List<PokemonPlayer> Attacked { get; set; } 
        public PokemonPlayer Duelist { get; set; }
        public Dictionary<PokemonMove, int> Moves { get; set; } 
        //public List<PokemonMove> Moves { get; set; } = new List<PokemonMove>();
        public PokemonType PokemonType { get; set; }
        //todo make this a nice format
        public override string ToString()
        {
            return base.ToString();
        }

        public override bool Equals(object obj)
        {
            var pl = obj as PokemonPlayer;
            //this could allow 'equal' players from different servers
            return pl != null && pl.UserId == UserId;
        }

        public override int GetHashCode() => UserId.GetHashCode();

        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Strength { get; set; }
        public int Agility { get; set; }
    }
}
