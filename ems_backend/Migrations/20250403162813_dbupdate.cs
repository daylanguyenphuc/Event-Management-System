using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ems_backend.Migrations
{
    /// <inheritdoc />
    public partial class dbupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discussions_Users_UserId1",
                table: "Discussions");

            migrationBuilder.DropIndex(
                name: "IX_Discussions_UserId1",
                table: "Discussions");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Discussions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Discussions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Discussions_UserId1",
                table: "Discussions",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Discussions_Users_UserId1",
                table: "Discussions",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
