using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FocusPocuss.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRecoveryExecution : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FocusSessions_UserId_Active",
                table: "FocusSessions");

            migrationBuilder.DropIndex(
                name: "IX_DistractionEvents_FocusSessionId",
                table: "DistractionEvents");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EndedEarlyAtUtc",
                table: "FocusSessions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecoveryAction",
                table: "FocusSessions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecoveryRevision",
                table: "FocusSessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ActionAtDistraction",
                table: "DistractionEvents",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Choice",
                table: "DistractionEvents",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ClarificationUsed",
                table: "DistractionEvents",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "InterventionType",
                table: "DistractionEvents",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "DistractionEvents",
                type: "character varying(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParkedThought",
                table: "DistractionEvents",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProposedAction",
                table: "DistractionEvents",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Requirement",
                table: "DistractionEvents",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Resolution",
                table: "DistractionEvents",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ResolvedAtUtc",
                table: "DistractionEvents",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StrategyVersion",
                table: "DistractionEvents",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FocusSessions_UserId_Active",
                table: "FocusSessions",
                column: "UserId",
                unique: true,
                filter: "\"CompletedAtUtc\" IS NULL AND \"EndedEarlyAtUtc\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DistractionEvents_PendingRecovery",
                table: "DistractionEvents",
                column: "FocusSessionId",
                unique: true,
                filter: "\"InterventionType\" IS NOT NULL AND \"ResolvedAtUtc\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FocusSessions_UserId_Active",
                table: "FocusSessions");

            migrationBuilder.DropIndex(
                name: "IX_DistractionEvents_PendingRecovery",
                table: "DistractionEvents");

            migrationBuilder.DropColumn(
                name: "EndedEarlyAtUtc",
                table: "FocusSessions");

            migrationBuilder.DropColumn(
                name: "RecoveryAction",
                table: "FocusSessions");

            migrationBuilder.DropColumn(
                name: "RecoveryRevision",
                table: "FocusSessions");

            migrationBuilder.DropColumn(
                name: "ActionAtDistraction",
                table: "DistractionEvents");

            migrationBuilder.DropColumn(
                name: "Choice",
                table: "DistractionEvents");

            migrationBuilder.DropColumn(
                name: "ClarificationUsed",
                table: "DistractionEvents");

            migrationBuilder.DropColumn(
                name: "InterventionType",
                table: "DistractionEvents");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "DistractionEvents");

            migrationBuilder.DropColumn(
                name: "ParkedThought",
                table: "DistractionEvents");

            migrationBuilder.DropColumn(
                name: "ProposedAction",
                table: "DistractionEvents");

            migrationBuilder.DropColumn(
                name: "Requirement",
                table: "DistractionEvents");

            migrationBuilder.DropColumn(
                name: "Resolution",
                table: "DistractionEvents");

            migrationBuilder.DropColumn(
                name: "ResolvedAtUtc",
                table: "DistractionEvents");

            migrationBuilder.DropColumn(
                name: "StrategyVersion",
                table: "DistractionEvents");

            migrationBuilder.CreateIndex(
                name: "IX_FocusSessions_UserId_Active",
                table: "FocusSessions",
                column: "UserId",
                unique: true,
                filter: "\"CompletedAtUtc\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DistractionEvents_FocusSessionId",
                table: "DistractionEvents",
                column: "FocusSessionId");
        }
    }
}
