using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NadekoBot.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace NadekoBot.Database
{
    public class MySQLiteDB : ADataBase
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./data/NadekoDB.db");  
        }
    }
}
