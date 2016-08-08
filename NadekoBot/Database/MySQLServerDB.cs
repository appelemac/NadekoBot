using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace NadekoBot.Database
{
    public class MySQLServerDB : ADataBase
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //System.Data.SqlClient.SqlConnectionStringBuilder builder =
            //    new System.Data.SqlClient.SqlConnectionStringBuilder();
            //builder.DataSource = "(local)\\test";
            //builder.IntegratedSecurity = true;
            //string dbFile = Path.Combine(NadekoClient.DataDir, "NadekoDB.mdf");
            ////if (!File.Exists(dbFile)) createDB(dbFile);
            ////builder.AttachDBFilename = System.IO.Path.Combine(NadekoClient.DataDir, "NadekoDB");
            //optionsBuilder.UseSqlServer(builder.ConnectionString);
        }

        //private void createDB(string dbFile)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
