﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NadekoBot.Migrations
{
    public partial class first : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BotConfig",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    BufferSize = table.Column<ulong>(nullable: false),
                    CurrencyName = table.Column<string>(nullable: true),
                    CurrencyPluralName = table.Column<string>(nullable: true),
                    CurrencySign = table.Column<string>(nullable: true),
                    DontJoinServers = table.Column<bool>(nullable: false),
                    ForwardMessages = table.Column<bool>(nullable: false),
                    ForwardToAllOwners = table.Column<bool>(nullable: false),
                    RemindMessageFormat = table.Column<string>(nullable: true),
                    RotatingStatuses = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClashOfClans",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    ChannelId = table.Column<ulong>(nullable: false),
                    EnemyClan = table.Column<string>(nullable: true),
                    GuildId = table.Column<ulong>(nullable: false),
                    Size = table.Column<int>(nullable: false),
                    StartedAt = table.Column<DateTime>(nullable: false),
                    WarState = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClashOfClans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    Amount = table.Column<long>(nullable: false),
                    UserId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.Id);
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
                    AutoDeleteSelfAssignedRoleMessages = table.Column<bool>(nullable: false),
                    ByeMessageChannelId = table.Column<ulong>(nullable: false),
                    ChannelByeMessageText = table.Column<string>(nullable: true),
                    ChannelGreetMessageText = table.Column<string>(nullable: true),
                    DefaultMusicVolume = table.Column<float>(nullable: false),
                    DeleteMessageOnCommand = table.Column<bool>(nullable: false),
                    DmGreetMessageText = table.Column<string>(nullable: true),
                    ExclusiveSelfAssignedRoles = table.Column<bool>(nullable: false),
                    GreetMessageChannelId = table.Column<ulong>(nullable: false),
                    GuildId = table.Column<ulong>(nullable: false),
                    SendChannelByeMessage = table.Column<bool>(nullable: false),
                    SendChannelGreetMessage = table.Column<bool>(nullable: false),
                    SendDmGreetMessage = table.Column<bool>(nullable: false),
                    VoicePlusTextEnabled = table.Column<bool>(nullable: false)
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
                name: "Reminders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    ChannelId = table.Column<ulong>(nullable: false),
                    IsPrivate = table.Column<bool>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    ServerId = table.Column<ulong>(nullable: false),
                    UserId = table.Column<ulong>(nullable: false),
                    When = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reminders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Repeaters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    ChannelId = table.Column<ulong>(nullable: false),
                    GuildId = table.Column<ulong>(nullable: false),
                    Interval = table.Column<TimeSpan>(nullable: false),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repeaters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SelfAssignableRoles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    GuildId = table.Column<ulong>(nullable: false),
                    RoleId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelfAssignableRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlacklistItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    BotConfigId = table.Column<int>(nullable: true),
                    ItemId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlacklistItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlacklistItem_BotConfig_BotConfigId",
                        column: x => x.BotConfigId,
                        principalTable: "BotConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EightBallResponse",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    BotConfigId = table.Column<int>(nullable: true),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EightBallResponse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EightBallResponse_BotConfig_BotConfigId",
                        column: x => x.BotConfigId,
                        principalTable: "BotConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ModulePrefix",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    BotConfigId = table.Column<int>(nullable: true),
                    ModuleName = table.Column<string>(nullable: true),
                    Prefix = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModulePrefix", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModulePrefix_BotConfig_BotConfigId",
                        column: x => x.BotConfigId,
                        principalTable: "BotConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayingStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    BotConfigId = table.Column<int>(nullable: true),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayingStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayingStatus_BotConfig_BotConfigId",
                        column: x => x.BotConfigId,
                        principalTable: "BotConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RaceAnimal",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    BotConfigId = table.Column<int>(nullable: true),
                    Icon = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaceAnimal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RaceAnimal_BotConfig_BotConfigId",
                        column: x => x.BotConfigId,
                        principalTable: "BotConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClashCallers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    BaseDestroyed = table.Column<bool>(nullable: false),
                    CallUser = table.Column<string>(nullable: true),
                    ClashWarId = table.Column<int>(nullable: false),
                    Stars = table.Column<int>(nullable: false),
                    TimeAdded = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClashCallers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClashCallers_ClashOfClans_ClashWarId",
                        column: x => x.ClashWarId,
                        principalTable: "ClashOfClans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FollowedStream",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    ChannelId = table.Column<ulong>(nullable: false),
                    GuildConfigId = table.Column<int>(nullable: true),
                    GuildId = table.Column<ulong>(nullable: false),
                    LastStatus = table.Column<bool>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Username = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowedStream", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FollowedStream_GuildConfigs_GuildConfigId",
                        column: x => x.GuildConfigId,
                        principalTable: "GuildConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlacklistItem_BotConfigId",
                table: "BlacklistItem",
                column: "BotConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ClashCallers_ClashWarId",
                table: "ClashCallers",
                column: "ClashWarId");

            migrationBuilder.CreateIndex(
                name: "IX_Currency_UserId",
                table: "Currency",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Donators_UserId",
                table: "Donators",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EightBallResponse_BotConfigId",
                table: "EightBallResponse",
                column: "BotConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowedStream_GuildConfigId",
                table: "FollowedStream",
                column: "GuildConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildConfigs_GuildId",
                table: "GuildConfigs",
                column: "GuildId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModulePrefix_BotConfigId",
                table: "ModulePrefix",
                column: "BotConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayingStatus_BotConfigId",
                table: "PlayingStatus",
                column: "BotConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_RaceAnimal_BotConfigId",
                table: "RaceAnimal",
                column: "BotConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_Repeaters_ChannelId",
                table: "Repeaters",
                column: "ChannelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SelfAssignableRoles_GuildId_RoleId",
                table: "SelfAssignableRoles",
                columns: new[] { "GuildId", "RoleId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlacklistItem");

            migrationBuilder.DropTable(
                name: "ClashCallers");

            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropTable(
                name: "Donators");

            migrationBuilder.DropTable(
                name: "EightBallResponse");

            migrationBuilder.DropTable(
                name: "FollowedStream");

            migrationBuilder.DropTable(
                name: "ModulePrefix");

            migrationBuilder.DropTable(
                name: "PlayingStatus");

            migrationBuilder.DropTable(
                name: "Quotes");

            migrationBuilder.DropTable(
                name: "RaceAnimal");

            migrationBuilder.DropTable(
                name: "Reminders");

            migrationBuilder.DropTable(
                name: "Repeaters");

            migrationBuilder.DropTable(
                name: "SelfAssignableRoles");

            migrationBuilder.DropTable(
                name: "ClashOfClans");

            migrationBuilder.DropTable(
                name: "GuildConfigs");

            migrationBuilder.DropTable(
                name: "BotConfig");
        }
    }
}
