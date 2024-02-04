using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Lang.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFlashcrdState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attempts_AttemptStages_CurrentStageId",
                table: "Attempts");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomProperties_Attempts_AttemptId",
                table: "CustomProperties");

            migrationBuilder.DropForeignKey(
                name: "FK_Flashcards_Attempts_AttemptId",
                table: "Flashcards");

            migrationBuilder.DropForeignKey(
                name: "FK_FlashcardStates_AttemptStages_AttemptStageId",
                table: "FlashcardStates");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizTypes_Attempts_AttemptId",
                table: "QuizTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizTypes_FlashcardStates_InProgressFlashcardStateId",
                table: "QuizTypes");

            migrationBuilder.DropTable(
                name: "AttemptStageFlashcardStates");

            migrationBuilder.DropIndex(
                name: "IX_QuizTypes_AttemptId",
                table: "QuizTypes");

            migrationBuilder.DropIndex(
                name: "IX_QuizTypes_InProgressFlashcardStateId",
                table: "QuizTypes");

            migrationBuilder.DropIndex(
                name: "IX_Flashcards_AttemptId",
                table: "Flashcards");

            migrationBuilder.DropIndex(
                name: "IX_CustomProperties_AttemptId",
                table: "CustomProperties");

            migrationBuilder.DropIndex(
                name: "IX_Attempts_CurrentStageId",
                table: "Attempts");

            migrationBuilder.DropColumn(
                name: "AttemptId",
                table: "QuizTypes");

            migrationBuilder.DropColumn(
                name: "InProgressFlashcardStateId",
                table: "QuizTypes");

            migrationBuilder.DropColumn(
                name: "AttemptId",
                table: "Flashcards");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Flashcards");

            migrationBuilder.DropColumn(
                name: "AttemptId",
                table: "CustomProperties");

            migrationBuilder.DropColumn(
                name: "CurrentStageId",
                table: "Attempts");

            migrationBuilder.RenameColumn(
                name: "IsMultiselect",
                table: "QuizTypes",
                newName: "IsFirst");

            migrationBuilder.RenameColumn(
                name: "FlashcardStateId",
                table: "CompletedQuizTypes",
                newName: "InProgressFlashcardStateId");

            migrationBuilder.RenameColumn(
                name: "QuiTypeId",
                table: "AttemptQuizTypes",
                newName: "QuizTypeId");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "QuizTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxAnswersToSelect",
                table: "QuizTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SeenQuizTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuizTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    InProgressFlashcardStateId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeenQuizTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeenQuizTypes_FlashcardStates_InProgressFlashcardStateId",
                        column: x => x.InProgressFlashcardStateId,
                        principalTable: "FlashcardStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeenQuizTypes_QuizTypes_QuizTypeId",
                        column: x => x.QuizTypeId,
                        principalTable: "QuizTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizTypes_Id",
                table: "QuizTypes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_QuizTypes_OwnerId",
                table: "QuizTypes",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_FlashcardStates_Id",
                table: "FlashcardStates",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CompletedQuizTypes_InProgressFlashcardStateId",
                table: "CompletedQuizTypes",
                column: "InProgressFlashcardStateId");

            migrationBuilder.CreateIndex(
                name: "IX_CompletedQuizTypes_QuizTypeId",
                table: "CompletedQuizTypes",
                column: "QuizTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompletedFlashcards_AttemptId",
                table: "CompletedFlashcards",
                column: "AttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_CompletedFlashcards_FlashcardId",
                table: "CompletedFlashcards",
                column: "FlashcardId");

            migrationBuilder.CreateIndex(
                name: "IX_AttemptStages_Id",
                table: "AttemptStages",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attempts_Id",
                table: "Attempts",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AttemptQuizTypes_AttemptId",
                table: "AttemptQuizTypes",
                column: "AttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_AttemptQuizTypes_QuizTypeId",
                table: "AttemptQuizTypes",
                column: "QuizTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AttemptProperties_AttemptId",
                table: "AttemptProperties",
                column: "AttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_AttemptProperties_CustomPropertyId",
                table: "AttemptProperties",
                column: "CustomPropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_SeenQuizTypes_InProgressFlashcardStateId",
                table: "SeenQuizTypes",
                column: "InProgressFlashcardStateId");

            migrationBuilder.CreateIndex(
                name: "IX_SeenQuizTypes_QuizTypeId",
                table: "SeenQuizTypes",
                column: "QuizTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttemptProperties_Attempts_AttemptId",
                table: "AttemptProperties",
                column: "AttemptId",
                principalTable: "Attempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttemptProperties_CustomProperties_CustomPropertyId",
                table: "AttemptProperties",
                column: "CustomPropertyId",
                principalTable: "CustomProperties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttemptQuizTypes_Attempts_AttemptId",
                table: "AttemptQuizTypes",
                column: "AttemptId",
                principalTable: "Attempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttemptQuizTypes_QuizTypes_QuizTypeId",
                table: "AttemptQuizTypes",
                column: "QuizTypeId",
                principalTable: "QuizTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttemptStages_Attempts_Id",
                table: "AttemptStages",
                column: "Id",
                principalTable: "Attempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompletedFlashcards_Attempts_AttemptId",
                table: "CompletedFlashcards",
                column: "AttemptId",
                principalTable: "Attempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompletedFlashcards_Flashcards_FlashcardId",
                table: "CompletedFlashcards",
                column: "FlashcardId",
                principalTable: "Flashcards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompletedQuizTypes_FlashcardStates_InProgressFlashcardState~",
                table: "CompletedQuizTypes",
                column: "InProgressFlashcardStateId",
                principalTable: "FlashcardStates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompletedQuizTypes_QuizTypes_QuizTypeId",
                table: "CompletedQuizTypes",
                column: "QuizTypeId",
                principalTable: "QuizTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FlashcardStates_AttemptStages_AttemptStageId",
                table: "FlashcardStates",
                column: "AttemptStageId",
                principalTable: "AttemptStages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttemptProperties_Attempts_AttemptId",
                table: "AttemptProperties");

            migrationBuilder.DropForeignKey(
                name: "FK_AttemptProperties_CustomProperties_CustomPropertyId",
                table: "AttemptProperties");

            migrationBuilder.DropForeignKey(
                name: "FK_AttemptQuizTypes_Attempts_AttemptId",
                table: "AttemptQuizTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_AttemptQuizTypes_QuizTypes_QuizTypeId",
                table: "AttemptQuizTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_AttemptStages_Attempts_Id",
                table: "AttemptStages");

            migrationBuilder.DropForeignKey(
                name: "FK_CompletedFlashcards_Attempts_AttemptId",
                table: "CompletedFlashcards");

            migrationBuilder.DropForeignKey(
                name: "FK_CompletedFlashcards_Flashcards_FlashcardId",
                table: "CompletedFlashcards");

            migrationBuilder.DropForeignKey(
                name: "FK_CompletedQuizTypes_FlashcardStates_InProgressFlashcardState~",
                table: "CompletedQuizTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_CompletedQuizTypes_QuizTypes_QuizTypeId",
                table: "CompletedQuizTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_FlashcardStates_AttemptStages_AttemptStageId",
                table: "FlashcardStates");

            migrationBuilder.DropTable(
                name: "SeenQuizTypes");

            migrationBuilder.DropIndex(
                name: "IX_QuizTypes_Id",
                table: "QuizTypes");

            migrationBuilder.DropIndex(
                name: "IX_QuizTypes_OwnerId",
                table: "QuizTypes");

            migrationBuilder.DropIndex(
                name: "IX_FlashcardStates_Id",
                table: "FlashcardStates");

            migrationBuilder.DropIndex(
                name: "IX_CompletedQuizTypes_InProgressFlashcardStateId",
                table: "CompletedQuizTypes");

            migrationBuilder.DropIndex(
                name: "IX_CompletedQuizTypes_QuizTypeId",
                table: "CompletedQuizTypes");

            migrationBuilder.DropIndex(
                name: "IX_CompletedFlashcards_AttemptId",
                table: "CompletedFlashcards");

            migrationBuilder.DropIndex(
                name: "IX_CompletedFlashcards_FlashcardId",
                table: "CompletedFlashcards");

            migrationBuilder.DropIndex(
                name: "IX_AttemptStages_Id",
                table: "AttemptStages");

            migrationBuilder.DropIndex(
                name: "IX_Attempts_Id",
                table: "Attempts");

            migrationBuilder.DropIndex(
                name: "IX_AttemptQuizTypes_AttemptId",
                table: "AttemptQuizTypes");

            migrationBuilder.DropIndex(
                name: "IX_AttemptQuizTypes_QuizTypeId",
                table: "AttemptQuizTypes");

            migrationBuilder.DropIndex(
                name: "IX_AttemptProperties_AttemptId",
                table: "AttemptProperties");

            migrationBuilder.DropIndex(
                name: "IX_AttemptProperties_CustomPropertyId",
                table: "AttemptProperties");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "QuizTypes");

            migrationBuilder.DropColumn(
                name: "MaxAnswersToSelect",
                table: "QuizTypes");

            migrationBuilder.RenameColumn(
                name: "IsFirst",
                table: "QuizTypes",
                newName: "IsMultiselect");

            migrationBuilder.RenameColumn(
                name: "InProgressFlashcardStateId",
                table: "CompletedQuizTypes",
                newName: "FlashcardStateId");

            migrationBuilder.RenameColumn(
                name: "QuizTypeId",
                table: "AttemptQuizTypes",
                newName: "QuiTypeId");

            migrationBuilder.AddColumn<Guid>(
                name: "AttemptId",
                table: "QuizTypes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InProgressFlashcardStateId",
                table: "QuizTypes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AttemptId",
                table: "Flashcards",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Flashcards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "AttemptId",
                table: "CustomProperties",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CurrentStageId",
                table: "Attempts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AttemptStageFlashcardStates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AttemptStageId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FlashcardStateId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttemptStageFlashcardStates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizTypes_AttemptId",
                table: "QuizTypes",
                column: "AttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizTypes_InProgressFlashcardStateId",
                table: "QuizTypes",
                column: "InProgressFlashcardStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Flashcards_AttemptId",
                table: "Flashcards",
                column: "AttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomProperties_AttemptId",
                table: "CustomProperties",
                column: "AttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_Attempts_CurrentStageId",
                table: "Attempts",
                column: "CurrentStageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attempts_AttemptStages_CurrentStageId",
                table: "Attempts",
                column: "CurrentStageId",
                principalTable: "AttemptStages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomProperties_Attempts_AttemptId",
                table: "CustomProperties",
                column: "AttemptId",
                principalTable: "Attempts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Flashcards_Attempts_AttemptId",
                table: "Flashcards",
                column: "AttemptId",
                principalTable: "Attempts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FlashcardStates_AttemptStages_AttemptStageId",
                table: "FlashcardStates",
                column: "AttemptStageId",
                principalTable: "AttemptStages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizTypes_Attempts_AttemptId",
                table: "QuizTypes",
                column: "AttemptId",
                principalTable: "Attempts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizTypes_FlashcardStates_InProgressFlashcardStateId",
                table: "QuizTypes",
                column: "InProgressFlashcardStateId",
                principalTable: "FlashcardStates",
                principalColumn: "Id");
        }
    }
}
