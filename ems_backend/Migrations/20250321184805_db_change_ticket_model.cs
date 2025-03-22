using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ems_backend.Migrations
{
    /// <inheritdoc />
    public partial class db_change_ticket_model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Events_EventId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_EventId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Payments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EventId",
                table: "Payments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Payments_EventId",
                table: "Payments",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Events_EventId",
                table: "Payments",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");
        }
    }
}
