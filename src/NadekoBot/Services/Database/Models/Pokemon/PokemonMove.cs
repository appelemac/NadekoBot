using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NadekoBot.Services.Database.Models
{
    public class PokemonMove : DbEntity
    {
        public string Name { get; set; }
        //Not sure whether these'll be used
       
        public PokemonType MoveType { get; set; }
        public short PP { get; set; }
        public int Power { get; set; }
    }
}
