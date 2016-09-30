using NadekoBot.Services.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace NadekoBot.Modules.Pokemon
{
    public static class Extensions
    {
        public static PokemonPlayer GetOrAdd(this List<PokemonPlayer> list, ulong playerId, System.Func<long, PokemonPlayer> generator)
        {
            var player = list.FirstOrDefault(u => u.UserId == (long)playerId);
            if (player != null) return player;
            player = generator.Invoke((long)playerId);
            list.Add(player);
            return player;
        }

        public static int CalculateDamage(this PokemonMove move, PokemonPlayer attacker, PokemonPlayer defender)
        {
            //TODO make this a nice formula~
            double mod = 1;
            if (defender.PokemonType.Weaknesses.Any(x => x == move.MoveType)) mod = 2;
            else if (defender.PokemonType.Strenghts.Any(x => x == move.MoveType)) mod = 0.5;
            var rand = new Random();
            //TODO factor in strength?
            return (int)Math.Round(rand.Next(80, 120) * mod, 0);
        }
        public static bool IsFainted(this PokemonPlayer player) => player.Health <= 0;

        public static string Reward(this PokemonPlayer attacker)
        {
            var random = new Random().Next(1, 101) / 10;
            string reward = "yet to fill";
            switch (random) //TODO think of some nice rewards like an increase in agility
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                case 7:
                    break;
                case 8:
                    break;
                case 9:
                    break;
                default:
                    break;
            }
            return reward;
        }
    }
}

