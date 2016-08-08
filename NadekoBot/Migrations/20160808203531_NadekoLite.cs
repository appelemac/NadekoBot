using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NadekoBot.Migrations
{
    public partial class NadekoLite : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_servers",
            //    table: "servers");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_Servers",
            //    table: "servers",
            //    column: "Id");

            //migrationBuilder.RenameTable(
            //    name: "servers",
            //    newName: "Servers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_Servers",
            //    table: "Servers");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_servers",
            //    table: "Servers",
            //    column: "Id");

            //migrationBuilder.RenameTable(
            //    name: "Servers",
            //    newName: "servers");
        }
    }
}
