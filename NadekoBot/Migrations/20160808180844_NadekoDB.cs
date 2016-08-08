using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NadekoBot.Migrations
{
    public partial class NadekoDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Commands",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    ChannelId = table.Column<ulong>(nullable: false),
                    CommandContent = table.Column<string>(nullable: true),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "servers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    ChannelCount = table.Column<int>(nullable: false),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    MemberCount = table.Column<int>(nullable: false),
                    ServerId = table.Column<ulong>(nullable: false),
                    ServerName = table.Column<string>(nullable: true),
                    ServerOwnerId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_servers", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Commands");

            migrationBuilder.DropTable(
                name: "servers");
        }
    }
}
