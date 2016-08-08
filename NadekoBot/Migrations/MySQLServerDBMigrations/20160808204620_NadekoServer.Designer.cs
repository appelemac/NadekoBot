using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using NadekoBot.Database;

namespace NadekoBot.Migrations.MySQLServerDBMigrations
{
    [DbContext(typeof(MySQLServerDB))]
    [Migration("20160808204620_NadekoServer")]
    partial class NadekoServer
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("NadekoBot.Models.DB.CommandModel", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ChannelId");

                    b.Property<string>("CommandContent");

                    b.Property<DateTime>("DateAdded");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Commands");
                });

            modelBuilder.Entity("NadekoBot.Models.DB.ServerModel", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ChannelCount");

                    b.Property<DateTime>("DateAdded");

                    b.Property<int>("MemberCount");

                    b.Property<long>("ServerId");

                    b.Property<string>("ServerName");

                    b.Property<long>("ServerOwnerId");

                    b.HasKey("Id");

                    b.ToTable("Servers");
                });
        }
    }
}
