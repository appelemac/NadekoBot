using NadekoBot.Services.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NadekoBot.Services.Database.Repositories.Impl
{
    public class PokemonRepository : Repository<PokemonMove>, IPokemonRepository
    {
        private DbSet<PokemonMove> _moves;
        private DbSet<PokemonPlayer> _players;
        private DbSet<PokemonServer> _servers;
        private DbSet<PokemonType> _types;

        public PokemonRepository(DbContext context) : base(context)
        {
            _moves = context.Set<PokemonMove>();
            _servers = context.Set<PokemonServer>();
            _types = context.Set<PokemonType>();
            _players = context.Set<PokemonPlayer>();
        }

        public void Add(PokemonType obj)
        {
            _types.Add(obj);
        }

        public void Add(PokemonServer obj)
        {
            _servers.Add(obj);
        }

        public void Add(PokemonPlayer obj)
        {
            _players.Add(obj);
        }

        public void AddRange(params PokemonType[] objs)
        {
            _types.AddRange(objs);
        }

        public void AddRange(params PokemonServer[] objs)
        {
            _servers.AddRange(objs);
        }

        public void AddRange(params PokemonPlayer[] objs)
        {
            _players.AddRange(objs);
        }

        public PokemonServer GetCurrentServer(long id)
        {
            return _servers.FirstOrDefault(x => x.ServerId == id);
        }

        public void Remove(PokemonType obj)
        {
            _types.Remove(obj);
        }

        public void Remove(PokemonServer obj)
        {
            _servers.Remove(obj);
        }

        public void Remove(PokemonPlayer obj)
        {
            _players.Remove(obj);
        }

        public void RemoveRange(params PokemonType[] objs)
        {
            _types.RemoveRange(objs);
        }

        public void RemoveRange(params PokemonServer[] objs)
        {
            _servers.RemoveRange(objs);
        }

        public void RemoveRange(params PokemonPlayer[] objs)
        {
            _players.RemoveRange(objs);
        }

        public void Update(PokemonServer obj)
        {
            _servers.Update(obj);
        }

        public void Update(PokemonType obj)
        {
            _types.Update(obj);
        }

        public void Update(PokemonPlayer obj)
        {
            _players.Update(obj);
        }

        public void UpdateRange(params PokemonServer[] objs)
        {
            _servers.UpdateRange(objs);
        }

        public void UpdateRange(params PokemonType[] objs)
        {
            _types.UpdateRange(objs);
        }

        public void UpdateRange(params PokemonPlayer[] objs)
        {
            _players.UpdateRange(objs);
        }

        PokemonServer IRepository<PokemonServer>.Get(int id)
        {
            return _servers.FirstOrDefault(x => x.Id == id);
        }

        PokemonType IRepository<PokemonType>.Get(int id)
        {
            return _types.FirstOrDefault(x => x.Id == id);
        }

        PokemonPlayer IRepository<PokemonPlayer>.Get(int id)
        {
            return _players.FirstOrDefault(x => x.Id == id);
        }

        List<PokemonType> IRepository<PokemonType>.GetAll()
        {
            return _types.ToList();
        }

        List<PokemonServer> IRepository<PokemonServer>.GetAll()
        {
            return _servers.ToList();
        }

        List<PokemonPlayer> IRepository<PokemonPlayer>.GetAll()
        {
            return _players.ToList();
        }
    }
}
