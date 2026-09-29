using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FocusPocuss.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFocusSessionReflection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReflectedAtUtc",
                table: "FocusSessions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Reflection",
                table: "FocusSessions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_FocusSessions_ReflectionState",
                table: "FocusSessions",
                sql: "(\"Reflection\" IS NULL AND \"ReflectedAtUtc\" IS NULL) OR (\"Reflection\" BETWEEN 1 AND 3 AND \"ReflectedAtUtc\" IS NOT NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_FocusSessions_ReflectionState",
                table: "FocusSessions");

            migrationBuilder.DropColumn(
                name: "ReflectedAtUtc",
                table: "FocusSessions");

            migrationBuilder.DropColumn(
                name: "Reflection",
                table: "FocusSessions");
        }
    }
}
