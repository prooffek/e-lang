using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Lang.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAttemptCurrentStageRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttemptStages_Attempts_Id",
                table: "AttemptStages");

            migrationBuilder.DropIndex(
                name: "IX_AttemptStages_Id",
                table: "AttemptStages");

            migrationBuilder.AddColumn<Guid>(
                name: "AttemptId",
                table: "AttemptStages",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttemptStages_AttemptId",
                table: "AttemptStages",
                column: "AttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_AttemptStages_Id",
                table: "AttemptStages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttemptStages_Attempts_AttemptId",
                table: "AttemptStages",
                column: "AttemptId",
                principalTable: "Attempts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttemptStages_Attempts_AttemptId",
                table: "AttemptStages");

            migrationBuilder.DropIndex(
                name: "IX_AttemptStages_AttemptId",
                table: "AttemptStages");

            migrationBuilder.DropIndex(
                name: "IX_AttemptStages_Id",
                table: "AttemptStages");

            migrationBuilder.DropColumn(
                name: "AttemptId",
                table: "AttemptStages");

            migrationBuilder.CreateIndex(
                name: "IX_AttemptStages_Id",
                table: "AttemptStages",
                column: "Id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AttemptStages_Attempts_Id",
                table: "AttemptStages",
                column: "Id",
                principalTable: "Attempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
