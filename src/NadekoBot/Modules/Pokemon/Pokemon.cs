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
using System.Text;
using System.Reflection;
using Discord.WebSocket;
using NadekoBot.Services.Database.Models;
using NadekoBot.Services.Database.Repositories;

namespace NadekoBot.Modules.Pokemon
{
    [Module(">", AppendSpace = false)]
    public class PokemonModule : DiscordModule
    {
        private static List<PokemonType> _types;
        private static List<PokemonMove> _moves;
        public ConcurrentDictionary<PokemonPlayer, PokemonPlayer> DuelProposals;
        private Random _rand;




        public PokemonModule(ILocalization loc, CommandService cmds, DiscordSocketClient client) : base(loc, cmds, client)
        {
            try
            {
                using (var uow = DbHandler.UnitOfWork())
                {
                    //I'm loading the types and units here, 
                    //as that prevents a lot of load, 
                    //and those are not changing anyway
                    var typeSet = uow.PokemonSets as IRepository<PokemonType>;
                    _types = typeSet.GetAll();
                    if (_types.Count == 0)
                    {
                        //import from JSON
                        _types = JsonConvert.DeserializeObject<List<PokemonType>>(File.ReadAllText("pokemontypes.json"));
                        typeSet.AddRange(_types.ToArray());
                    }
                    var moveSet = uow.PokemonSets as IRepository<PokemonMove>;
                    _moves = moveSet.GetAll();
                    if (_moves.Count == 0)
                    {
                        //import from JSON
                        _moves = JsonConvert.DeserializeObject<List<PokemonMove>>(File.ReadAllText("pokemontypes.json"));
                        moveSet.AddRange(_moves.ToArray());
                    }
                }

                //var DataDir = Path.Combine(Directory.GetParent(typeof(NadekoBot).GetTypeInfo().Assembly.Location).FullName, "data");
                DuelProposals = new ConcurrentDictionary<PokemonPlayer, PokemonPlayer>();
                _rand = new Random();
            }
            catch (Exception e)
            {
                _log.Warn("Failed to load pokemon: " + e.Message);
            }
        }


        [LocalizedCommand, LocalizedDescription, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        public async Task Attack(IUserMessage msg, [Remainder] string attack)
        {

            var channel = msg.Channel as IGuildChannel;
            var guild = channel.Guild;


            if (msg.MentionedUsers.Count == 1 /*&& msg.MentionedUsers.FirstOrDefault().Id != (await _client.GetCurrentUserAsync()).Id*/)

            {
                using (var uow = DbHandler.UnitOfWork())
                {
                    var saving = false;
                    var serverPlayerSet = uow.PokemonSets.GetCurrentServer((long)guild.Id);
                    if (serverPlayerSet == null)
                    {
                        saving = true;
                        (uow.PokemonSets as IRepository<PokemonServer>).Add(new PokemonServer()
                        {
                           ServerId = (long) guild.Id,
                        });
                    }
                    var targetUser = msg.MentionedUsers.First();
                    attack = attack.Replace(targetUser.Mention, "").Replace(targetUser.Mention.Insert(2, "!"), "").Trim();
                    //now get both players
                    var uid = (long)msg.Author.Id;
                    var attacker = serverPlayerSet.Players.Find(x => x.UserId == uid) ?? GenerateNewPlayer(uid);
                    var tid = (long)targetUser.Id;
                    var defender = serverPlayerSet.Players.Find(x => x.UserId == tid) ?? GenerateNewPlayer(tid);
                    //check if any of the two is fainted
                    if (attacker.IsFainted())
                    {
                        await msg.Reply(string.Format("{0} is fainted and can't move!", attacker));
                        if (saving) uow.Complete();
                        return;
                    }
                    if (defender.IsFainted())
                    {
                        await msg.Reply(string.Format("{0} has already fainted!", defender));
                        return;
                    }
                    //check if in duel
                    if (!((attacker.Duelist == null && defender.Duelist == null) || (attacker.Duelist == defender)))
                    {
                        await msg.Reply("One of the players is in a duel, and cannot engage other targets");
                        return;
                    }
                    //Check if already attacked
                    if (attacker.Attacked.Contains(defender))
                    {
                        await msg.Reply("Already attacked {0}");
                    }
                    //check if the move was a valid one :D
                    if (!attacker.Moves.Any(kvp => kvp.Key.Name == attack))
                    {
                        //TODO change this to moveslist + add prefix
                        await msg.Reply(string.Format("Move {0} not valid, use {1} to get your moves", attack, nameof(MovesList)));
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
                    var diff = defender.Agility - attacker.Agility;
                    //TODO mod this chance to make it purrfect add accuracy?

                    var evaded = _rand.Next(1, 101) + diff > 85;
                    if (evaded)
                    {
                        await msg.Reply(string.Format("{0} evaded {1}'s {2}!", targetUser.Username, msg.Author.Username, attack));
                        return;
                    }
                    //execute move on target
                    var damage = move.Key.CalculateDamage(attacker, defender);
                    defender.Health -= damage;
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
                        message += string.Format("{0} has {1} HP left.", defender, defender.Health);
                    }
                    defender.Attacked.Remove(attacker);
                    attacker.Attacked.Add(defender);
                    //update Data
                    (uow.PokemonSets as IRepository<PokemonServer>).Update(serverPlayerSet);
                }

            }
        }

        [LocalizedCommand, LocalizedDescription, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        public async Task Stats(IUserMessage msg, IGuildUser arg = null)
        {
            var guild = (msg.Channel as IGuildChannel).Guild;
            var serverPlayers = Dictionary.GetOrAdd((long)guild.Id, new PokemonServer() { ServerId = (long)guild.Id }).Players;
            StringBuilder builder = new StringBuilder();
            PokemonPlayer player = serverPlayers.GetOrAdd((arg ?? msg.Author).Id, GenerateNewPlayer);
            builder.AppendLine(string.Format("{0}'s Stats: ```xl", player));

            builder.AppendLine(string.Format("Type : {0}", player.PokemonType));
            if (player.Duelist != null)
                builder.AppendLine(string.Format("Duelist : {0}", player.Duelist));
            await msg.Reply(builder.AppendLine("```").ToString());
        }

        [LocalizedCommand, LocalizedDescription, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        public async Task Heal(IMessage msg, IGuildUser arg = null)
        {
            var guild = (msg.Channel as IGuildChannel).Guild;
            var serverPlayers = Dictionary.GetOrAdd((long)guild.Id, new PokemonServer() { ServerId = (long)guild.Id }).Players;
            var player = serverPlayers.GetOrAdd(arg?.Id ?? msg.Author.Id, GenerateNewPlayer);
            //TODO add pay

        }

        [LocalizedCommand, LocalizedDescription, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        public async Task MovesList(IUserMessage msg, IGuildUser arg = null)
        {
            var channel = msg.Channel as IGuildChannel;
            var guild = channel.Guild;
            var serverPlayers = Dictionary.GetOrAdd((long)guild.Id, new PokemonServer() { ServerId = (long)guild.Id }).Players;
            StringBuilder builder = new StringBuilder();
            PokemonPlayer player = serverPlayers.GetOrAdd((arg ?? msg.Author).Id, GenerateNewPlayer);
            builder.AppendLine(string.Format("{0}'s moves: ```xl", player));
            foreach (var kvp in player.Moves)
            {
                var moveT = kvp.Key.MoveType;
                //todo should probably make this a table, but that's irritating
                builder.AppendLine(string.Format("{0}{1} | PP: {2}", kvp.Key.Name, moveT.Icon, kvp.Value));
            }
            builder.Append("```");
            await msg.Reply(builder.ToString());
        }

        [LocalizedCommand, LocalizedDescription, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        public async Task StartDuel(IUserMessage msg, IGuildUser arg)
        {
            var guild = (msg.Channel as IGuildChannel).Guild;
            var serverPlayers = Dictionary.GetOrAdd((long)guild.Id, new PokemonServer() { ServerId = (long)guild.Id }).Players;
            PokemonPlayer player = serverPlayers.GetOrAdd(msg.Author.Id, GenerateNewPlayer);
            PokemonPlayer target = serverPlayers.GetOrAdd(arg.Id, GenerateNewPlayer);

        }


        public static PokemonPlayer GenerateNewPlayer(long id)
        {
            var _rand = new Random();
            var t = generateRandomType(id);
            var hp = _rand.Next(80, 120);
            var player = new PokemonPlayer()
            {
                UserId = id,
                Health = hp,
                MaxHealth = hp,
                Agility = _rand.Next(10, 50),
                Strength = _rand.Next(80, 120),
                PokemonType = t,
                Moves = generateRandomMoves(t, _rand),
            };
            using (var uow = DbHandler.UnitOfWork())
            {
                (uow.PokemonSets as IRepository<PokemonPlayer>).Add(player);
                uow.Complete();
            }
        }

        private static PokemonType generateRandomType(long id) => _types[(int)(id % _types.Count)];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="focus">Moves will have at least 1 move of this type and will not have a move of a weakness of the type</param>
        /// <returns></returns>
        private static Dictionary<PokemonMove, int> generateRandomMoves(PokemonType focus, Random _rand)
        {
            var dict = new Dictionary<PokemonMove, int>();
            //2 steps: select a move with type Focus
            var selectRange = _moves.Where(m => m.MoveType == focus).ToList();
            var selection = selectRange[_rand.Next(0, selectRange.Count)];
            dict.Add(selection, selection.PP);
            selectRange.AddRange(_moves.Where(m => !focus.Weaknesses.Contains(m.MoveType)));

            for (int i = 0; i < 3; i++)
            {
                selection = selectRange[_rand.Next(0, selectRange.Count)];
                if (dict.ContainsKey(selection)) i--;
                else dict.Add(selection, selection.PP);
            }
            return dict;
        }

    }

}