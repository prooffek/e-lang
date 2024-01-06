using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Lang.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAttempt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizTypes_FlashcardStates_FlashcardStateId",
                table: "QuizTypes");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AttemptStages");

            migrationBuilder.RenameColumn(
                name: "FlashcardStateId",
                table: "QuizTypes",
                newName: "InProgressFlashcardStateId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizTypes_FlashcardStateId",
                table: "QuizTypes",
                newName: "IX_QuizTypes_InProgressFlashcardStateId");

            migrationBuilder.AddColumn<int>(
                name: "Stage",
                table: "AttemptStages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Attempts",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizTypes_FlashcardStates_InProgressFlashcardStateId",
                table: "QuizTypes",
                column: "InProgressFlashcardStateId",
                principalTable: "FlashcardStates",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizTypes_FlashcardStates_InProgressFlashcardStateId",
                table: "QuizTypes");

            migrationBuilder.DropColumn(
                name: "Stage",
                table: "AttemptStages");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Attempts");

            migrationBuilder.RenameColumn(
                name: "InProgressFlashcardStateId",
                table: "QuizTypes",
                newName: "FlashcardStateId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizTypes_InProgressFlashcardStateId",
                table: "QuizTypes",
                newName: "IX_QuizTypes_FlashcardStateId");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AttemptStages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizTypes_FlashcardStates_FlashcardStateId",
                table: "QuizTypes",
                column: "FlashcardStateId",
                principalTable: "FlashcardStates",
                principalColumn: "Id");
        }
    }
}
