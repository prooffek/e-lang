using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Lang.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ExcludeQuizTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExcludedQuizTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InProgressFlashcardStateId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuizTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExcludedQuizTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExcludedQuizTypes_FlashcardStates_InProgressFlashcardStateId",
                        column: x => x.InProgressFlashcardStateId,
                        principalTable: "FlashcardStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExcludedQuizTypes_QuizTypes_QuizTypeId",
                        column: x => x.QuizTypeId,
                        principalTable: "QuizTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExcludedQuizTypes_InProgressFlashcardStateId",
                table: "ExcludedQuizTypes",
                column: "InProgressFlashcardStateId");

            migrationBuilder.CreateIndex(
                name: "IX_ExcludedQuizTypes_QuizTypeId",
                table: "ExcludedQuizTypes",
                column: "QuizTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExcludedQuizTypes");
        }
    }
}
