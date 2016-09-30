using NadekoBot.Services.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NadekoBot.Services.Database.Models
{
    public class PokemonType : DbEntity
    {
        
        public string Name { get; set; }
        public string Icon { get; set; }
       
        //change this to virtual PokemonTypes?
        public List<PokemonType> Weaknesses { get; set; } = new List<PokemonType>();
        public List<PokemonType> Strenghts { get; set; } = new List<PokemonType>();

        public override bool Equals(object other)
        {
            if (other is PokemonType)
                return GetHashCode().Equals(other.GetHashCode());
            return false;
        }
        public override int GetHashCode() => Name.GetHashCode();
    }
}
