using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amusoft.PCR.Int.UI.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Endpoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM BearerTokens");
            
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "BearerTokens",
                newName: "EndpointAccountId");

            migrationBuilder.AlterColumn<long>(
                name: "Time",
                table: "LogEntries",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AddColumn<long>(
                name: "IssuedAt",
                table: "BearerTokens",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Endpoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Endpoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EndpointAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    EndpointId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EndpointAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EndpointAccounts_Endpoints_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "Endpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BearerTokens_EndpointAccountId",
                table: "BearerTokens",
                column: "EndpointAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_EndpointAccounts_Email",
                table: "EndpointAccounts",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_EndpointAccounts_EndpointId_Email",
                table: "EndpointAccounts",
                columns: new[] { "EndpointId", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Endpoints_Address",
                table: "Endpoints",
                column: "Address",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BearerTokens_EndpointAccounts_EndpointAccountId",
                table: "BearerTokens",
                column: "EndpointAccountId",
                principalTable: "EndpointAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BearerTokens_EndpointAccounts_EndpointAccountId",
                table: "BearerTokens");

            migrationBuilder.DropTable(
                name: "EndpointAccounts");

            migrationBuilder.DropTable(
                name: "Endpoints");

            migrationBuilder.DropIndex(
                name: "IX_BearerTokens_EndpointAccountId",
                table: "BearerTokens");

            migrationBuilder.DropColumn(
                name: "IssuedAt",
                table: "BearerTokens");

            migrationBuilder.RenameColumn(
                name: "EndpointAccountId",
                table: "BearerTokens",
                newName: "Address");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Time",
                table: "LogEntries",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER");
        }
    }
}
