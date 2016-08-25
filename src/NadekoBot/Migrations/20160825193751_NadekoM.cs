using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NadekoBot.Migrations
{
    public partial class NadekoM : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GlobalReactions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    IsRegex = table.Column<bool>(nullable: false),
                    Trigger = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalReactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServerReactions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    IsRegex = table.Column<bool>(nullable: false),
                    ServerId = table.Column<long>(nullable: false),
                    Trigger = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerReactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Donators",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    Amount = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    UserId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Donators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GuildConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    AutoAssignRoleId = table.Column<ulong>(nullable: false),
                    AutoDeleteByeMessages = table.Column<bool>(nullable: false),
                    AutoDeleteGreetMessages = table.Column<bool>(nullable: false),
                    AutoDeleteGreetMessagesTimer = table.Column<int>(nullable: false),
                    ByeMessageChannelId = table.Column<ulong>(nullable: false),
                    ChannelByeMessageText = table.Column<string>(nullable: true),
                    ChannelGreetMessageText = table.Column<string>(nullable: true),
                    DeleteMessageOnCommand = table.Column<bool>(nullable: false),
                    DmGreetMessageText = table.Column<string>(nullable: true),
                    GreetMessageChannelId = table.Column<ulong>(nullable: false),
                    GuildId = table.Column<ulong>(nullable: false),
                    SendChannelByeMessage = table.Column<bool>(nullable: false),
                    SendChannelGreetMessage = table.Column<bool>(nullable: false),
                    SendDmGreetMessage = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Quotes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    AuthorId = table.Column<ulong>(nullable: false),
                    AuthorName = table.Column<string>(nullable: false),
                    GuildId = table.Column<ulong>(nullable: false),
                    Keyword = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Response",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    CustomGlobalReactionId = table.Column<int>(nullable: true),
                    CustomServerReactionId = table.Column<int>(nullable: true),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Response", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Response_GlobalReactions_CustomGlobalReactionId",
                        column: x => x.CustomGlobalReactionId,
                        principalTable: "GlobalReactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Response_ServerReactions_CustomServerReactionId",
                        column: x => x.CustomServerReactionId,
                        principalTable: "ServerReactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Donators_UserId",
                table: "Donators",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GuildConfigs_GuildId",
                table: "GuildConfigs",
                column: "GuildId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Response_CustomGlobalReactionId",
                table: "Response",
                column: "CustomGlobalReactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Response_CustomServerReactionId",
                table: "Response",
                column: "CustomServerReactionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Donators");

            migrationBuilder.DropTable(
                name: "GuildConfigs");

            migrationBuilder.DropTable(
                name: "Quotes");

            migrationBuilder.DropTable(
                name: "Response");

            migrationBuilder.DropTable(
                name: "GlobalReactions");

            migrationBuilder.DropTable(
                name: "ServerReactions");
        }
    }
}
