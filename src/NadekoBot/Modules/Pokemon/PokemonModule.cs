using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using NadekoBot.Services;
using NadekoBot.Attributes;
using System.Collections.Concurrent;
using NadekoBot.Extensions;
using Newtonsoft.Json;
using System.IO;

namespace NadekoBot.Modules.Pokemon
{
    [Module(">", AppendSpace = false)]
    public class PokemonModule : DiscordModule
    {
        public ConcurrentDictionary<long, PokemonServer> Dictionary;
        public readonly List<PokemonType> PokemonTypes;
        private Random _rand;
        public PokemonModule(ILocalization loc, CommandService cmds, IBotConfiguration config, IDiscordClient client) : base(loc, cmds, config, client)
        {
            Dictionary = new ConcurrentDictionary<long, PokemonServer>();
            _rand = new Random();
            //TODO load Dictionary
            try
            {
                //TODO change this to loading from DB I guess?
                PokemonTypes = JsonConvert.DeserializeObject<List<PokemonType>>(File.ReadAllText(Path.Combine(NadekoBot.DataDir, "pokemontypes.json")));
            }
            catch (Exception e)
            {
                //TODO log exception, disable module or load default
                Console.WriteLine(e.Message);
            }
        }
        [LocalizedCommand, LocalizedDescription, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        public async Task Attack(IMessage msg, [Remainder] string attack)
        {

            var channel = msg.Channel as IGuildChannel;
            var guild = channel.Guild;
            var serverPlayers = Dictionary.GetOrAdd((long)guild.Id, new PokemonServer() { ServerId = (long)guild.Id }).Players;
            if (msg.MentionedUsers.Count == 1 && msg.MentionedUsers.FirstOrDefault().Id != (await _client.GetCurrentUserAsync()).Id)
            {
                var targetUser = msg.MentionedUsers.First();
                attack = attack.Replace(targetUser.Mention, "").Replace(targetUser.Mention.Insert(2, "!"), "").Trim();
                //now get both players
                var attacker = serverPlayers.GetOrAdd(msg.Author.Id, GenerateNewPlayer);
                var defender = serverPlayers.GetOrAdd(targetUser.Id, GenerateNewPlayer);
                //check if any of the two is fainted
                if (attacker.IsFainted())
                {
                    await msg.Reply("{0} is fainted and can't move!", attacker);
                    return;
                }
                if (defender.IsFainted())
                {
                    await msg.Reply("{0} has already fainted!", defender);
                    return;
                }
                //check if in duel
                if (!((attacker.Duelist == null && defender.Duelist == null) || (attacker.Duelist == defender)))
                {
                    await msg.Reply("One of the players is in a duel, and cannot engage other targets");
                    return;
                }
                //check if the move was a valid one :D
                if (!attacker.Moves.Any(kvp => kvp.Key.Name == attack))
                {
                    //TODO change this to moveslist + add prefix
                    await msg.Reply(string.Format("Move {0} not valid, use {1} to get your moves", attack, nameof(Attack)));
                    return;
                }
                var move = attacker.Moves.FirstOrDefault(m => m.Key.Name == attack);
                //check PP
                if (move.Value < 1)
                {
                    await msg.Reply(string.Format("Move {0} has no PP left, try using {1}", move.Key.Name, nameof(Attack)));
                    return;
                }
                //check agility for evasion~
                var diff = defender.Stats.Agility - attacker.Stats.Agility;
                //TODO mod this chance to make it purrfect
                var evaded = _rand.Next(1, 101) + diff > 85;
                if (evaded)
                {
                    await msg.Reply(string.Format("{0} evaded {1}'s {2}!", targetUser.Username, msg.Author.Username, attack));
                    return;
                }
                //execute move on target
                var damage = move.Key.CalculateDamage(attacker, defender);
                defender.Stats.Health -= damage;
                //todo add icon to things (toString method?)
                string message = string.Format("{0} attacked {1} with {2} for {3} HP!\n", attacker, defender, attack, damage);
                if (defender.IsFainted())
                {
                    message += string.Format("{0} has fainted!");
                    message += string.Format("{0} receives {1} for defeating {2}", attacker, attacker.Reward(), defender);
                    if (attacker.Duelist == defender)
                    {
                        message += "\nThe Duel has ended!";
                        attacker.Duelist = null;
                        defender.Duelist = null;
                        //todo decide whether we heal after duel
                    }
                }
                else
                {
                    message += string.Format("{0} has {1} HP left.", defender, defender.Stats.Health);
                }
                var atkIndex = serverPlayers.IndexOf(attacker);
                var dfndIndex = serverPlayers.IndexOf(defender);
                serverPlayers[atkIndex] = attacker;
                serverPlayers[dfndIndex] = defender;
                var newServer = new PokemonServer() { Players = serverPlayers, ServerId = (long)guild.Id };
                Dictionary.TryUpdate((long)guild.Id, newServer, newServer);
            }
        }

        public PokemonPlayer GenerateNewPlayer(long id)
        {
            return new PokemonPlayer()
            {
                UserId = id,
                Stats = new PokemonStats()
                {
                    Health = _rand.Next(80, 120),
                    Agility = _rand.Next(10, 50),
                    Strength = _rand.Next(80, 120)
                },
                Moves = generateRandomMoves()
            };
        }

        private Dictionary<PokemonMove, int> generateRandomMoves()
        {
            throw new NotImplementedException();
        }
    }
}
