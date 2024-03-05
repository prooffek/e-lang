using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Lang.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyingAttemptEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttemptStages_Attempts_AttemptId",
                table: "AttemptStages");

            migrationBuilder.DropTable(
                name: "CompletedFlashcards");

            migrationBuilder.AlterColumn<Guid>(
                name: "AttemptId",
                table: "AttemptStages",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AttemptStages_Attempts_AttemptId",
                table: "AttemptStages",
                column: "AttemptId",
                principalTable: "Attempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttemptStages_Attempts_AttemptId",
                table: "AttemptStages");

            migrationBuilder.AlterColumn<Guid>(
                name: "AttemptId",
                table: "AttemptStages",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateTable(
                name: "CompletedFlashcards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AttemptId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FlashcardId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompletedFlashcards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompletedFlashcards_Attempts_AttemptId",
                        column: x => x.AttemptId,
                        principalTable: "Attempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompletedFlashcards_Flashcards_FlashcardId",
                        column: x => x.FlashcardId,
                        principalTable: "Flashcards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompletedFlashcards_AttemptId",
                table: "CompletedFlashcards",
                column: "AttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_CompletedFlashcards_FlashcardId",
                table: "CompletedFlashcards",
                column: "FlashcardId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttemptStages_Attempts_AttemptId",
                table: "AttemptStages",
                column: "AttemptId",
                principalTable: "Attempts",
                principalColumn: "Id");
        }
    }
}
