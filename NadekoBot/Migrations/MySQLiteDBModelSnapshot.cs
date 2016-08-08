using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using NadekoBot.Database;

namespace NadekoBot.Migrations
{
    [DbContext(typeof(MySQLiteDB))]
    partial class MySQLiteDBModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("NadekoBot.Models.DB.CommandModel", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("ChannelId");

                    b.Property<string>("CommandContent");

                    b.Property<DateTime>("DateAdded");

                    b.Property<ulong>("UserId");

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

                    b.Property<ulong>("ServerId");

                    b.Property<string>("ServerName");

                    b.Property<ulong>("ServerOwnerId");

                    b.HasKey("Id");

                    b.ToTable("servers");
                });
        }
    }
}
