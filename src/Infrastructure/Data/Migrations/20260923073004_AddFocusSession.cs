using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FocusPocuss.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFocusSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FocusSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    TaskItemId = table.Column<int>(type: "integer", nullable: false),
                    TaskStartPlanId = table.Column<int>(type: "integer", nullable: false),
                    Action = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    PlannedDurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    StartedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CompletedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FocusSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FocusSessions_TaskItems_TaskItemId",
                        column: x => x.TaskItemId,
                        principalTable: "TaskItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FocusSessions_TaskStartPlans_TaskStartPlanId",
                        column: x => x.TaskStartPlanId,
                        principalTable: "TaskStartPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FocusSessions_TaskItemId",
                table: "FocusSessions",
                column: "TaskItemId");

            migrationBuilder.CreateIndex(
                name: "IX_FocusSessions_TaskStartPlanId",
                table: "FocusSessions",
                column: "TaskStartPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_FocusSessions_UserId_Active",
                table: "FocusSessions",
                column: "UserId",
                unique: true,
                filter: "\"CompletedAtUtc\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FocusSessions");
        }
    }
}
