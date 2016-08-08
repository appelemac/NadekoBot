using Microsoft.EntityFrameworkCore;
using NadekoBot.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;


namespace NadekoBot.Database
{
    
    /// <summary>
    /// Handles interaction with Databases
    /// </summary>
    public abstract class ADataBase : DbContext
    {
        public DbSet<CommandModel> Commands { get; set; }
        public DbSet<ServerModel> Servers { get; set; }
        
        protected abstract override void OnConfiguring(DbContextOptionsBuilder optionsBuilder);

    }

}