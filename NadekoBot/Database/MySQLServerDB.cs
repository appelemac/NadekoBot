using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NadekoBot.Database
{
    public class MySQLServerDB : ADataBase
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            System.Data.SqlClient.SqlConnectionStringBuilder builder =
                new System.Data.SqlClient.SqlConnectionStringBuilder();
            builder.DataSource = "(local)\\test";
            builder.AttachDBFilename = System.IO.Path.Combine(NadekoClient.DataDir, "NadekoDB.mdf");
            optionsBuilder.UseSqlServer(builder.ConnectionString);
        }
    }
}
