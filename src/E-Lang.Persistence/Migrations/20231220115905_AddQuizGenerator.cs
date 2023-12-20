using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Lang.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizGenerator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AttemptId",
                table: "Flashcards",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AttemptProperties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomPropertyId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttemptId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttemptProperties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttemptQuizTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuiTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttemptId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttemptQuizTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttemptStageFlashcardStates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AttemptStageId = table.Column<Guid>(type: "uuid", nullable: false),
                    FlashcardStateId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttemptStageFlashcardStates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttemptStages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttemptStages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompletedQuizTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuizTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    FlashcardStateId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompletedQuizTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RelationTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Attempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaxFlashcardsPerStage = table.Column<int>(type: "integer", nullable: false),
                    MaxQuizTypesPerFlashcard = table.Column<int>(type: "integer", nullable: false),
                    MinCompletedQuizzesPerCent = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    IncludeMeanings = table.Column<bool>(type: "boolean", nullable: false),
                    CollectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentStageId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attempts_AttemptStages_CurrentStageId",
                        column: x => x.CurrentStageId,
                        principalTable: "AttemptStages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Attempts_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomPropertyRelation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RelationTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Property1Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Property2Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomPropertyRelation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomPropertyRelation_Flashcards_Property2Id",
                        column: x => x.Property2Id,
                        principalTable: "Flashcards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomPropertyRelation_RelationTypes_RelationTypeId",
                        column: x => x.RelationTypeId,
                        principalTable: "RelationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeaningsRelations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RelationTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Meaning1Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Meaning2Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeaningsRelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeaningsRelations_Flashcards_Meaning2Id",
                        column: x => x.Meaning2Id,
                        principalTable: "Flashcards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeaningsRelations_RelationTypes_RelationTypeId",
                        column: x => x.RelationTypeId,
                        principalTable: "RelationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomProperties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    AttemptId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomProperties_Attempts_AttemptId",
                        column: x => x.AttemptId,
                        principalTable: "Attempts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FlashcardStates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentQuizTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    FlashcardId = table.Column<Guid>(type: "uuid", nullable: true),
                    ShowAgainOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AttemptStageId = table.Column<Guid>(type: "uuid", nullable: true),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlashcardStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlashcardStates_AttemptStages_AttemptStageId",
                        column: x => x.AttemptStageId,
                        principalTable: "AttemptStages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FlashcardStates_Flashcards_FlashcardId",
                        column: x => x.FlashcardId,
                        principalTable: "Flashcards",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "QuizTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Instruction = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    IsSelect = table.Column<bool>(type: "boolean", nullable: false),
                    IsMultiselect = table.Column<bool>(type: "boolean", nullable: false),
                    IsSelectCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsSelectMissing = table.Column<bool>(type: "boolean", nullable: false),
                    IsMatch = table.Column<bool>(type: "boolean", nullable: false),
                    IsArrange = table.Column<bool>(type: "boolean", nullable: false),
                    IsInput = table.Column<bool>(type: "boolean", nullable: false),
                    IsFillInBlank = table.Column<bool>(type: "boolean", nullable: false),
                    AttemptId = table.Column<Guid>(type: "uuid", nullable: true),
                    FlashcardStateId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizTypes_Attempts_AttemptId",
                        column: x => x.AttemptId,
                        principalTable: "Attempts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuizTypes_FlashcardStates_FlashcardStateId",
                        column: x => x.FlashcardStateId,
                        principalTable: "FlashcardStates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Flashcards_AttemptId",
                table: "Flashcards",
                column: "AttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_Attempts_CollectionId",
                table: "Attempts",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Attempts_CurrentStageId",
                table: "Attempts",
                column: "CurrentStageId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomProperties_AttemptId",
                table: "CustomProperties",
                column: "AttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomPropertyRelation_Property2Id",
                table: "CustomPropertyRelation",
                column: "Property2Id");

            migrationBuilder.CreateIndex(
                name: "IX_CustomPropertyRelation_RelationTypeId",
                table: "CustomPropertyRelation",
                column: "RelationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FlashcardStates_AttemptStageId",
                table: "FlashcardStates",
                column: "AttemptStageId");

            migrationBuilder.CreateIndex(
                name: "IX_FlashcardStates_CurrentQuizTypeId",
                table: "FlashcardStates",
                column: "CurrentQuizTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FlashcardStates_FlashcardId",
                table: "FlashcardStates",
                column: "FlashcardId");

            migrationBuilder.CreateIndex(
                name: "IX_MeaningsRelations_Meaning2Id",
                table: "MeaningsRelations",
                column: "Meaning2Id");

            migrationBuilder.CreateIndex(
                name: "IX_MeaningsRelations_RelationTypeId",
                table: "MeaningsRelations",
                column: "RelationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizTypes_AttemptId",
                table: "QuizTypes",
                column: "AttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizTypes_FlashcardStateId",
                table: "QuizTypes",
                column: "FlashcardStateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flashcards_Attempts_AttemptId",
                table: "Flashcards",
                column: "AttemptId",
                principalTable: "Attempts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FlashcardStates_QuizTypes_CurrentQuizTypeId",
                table: "FlashcardStates",
                column: "CurrentQuizTypeId",
                principalTable: "QuizTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flashcards_Attempts_AttemptId",
                table: "Flashcards");

            migrationBuilder.DropForeignKey(
                name: "FK_Attempts_AttemptStages_CurrentStageId",
                table: "Attempts");

            migrationBuilder.DropForeignKey(
                name: "FK_FlashcardStates_AttemptStages_AttemptStageId",
                table: "FlashcardStates");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizTypes_Attempts_AttemptId",
                table: "QuizTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_FlashcardStates_QuizTypes_CurrentQuizTypeId",
                table: "FlashcardStates");

            migrationBuilder.DropTable(
                name: "AttemptProperties");

            migrationBuilder.DropTable(
                name: "AttemptQuizTypes");

            migrationBuilder.DropTable(
                name: "AttemptStageFlashcardStates");

            migrationBuilder.DropTable(
                name: "CompletedQuizTypes");

            migrationBuilder.DropTable(
                name: "CustomProperties");

            migrationBuilder.DropTable(
                name: "CustomPropertyRelation");

            migrationBuilder.DropTable(
                name: "MeaningsRelations");

            migrationBuilder.DropTable(
                name: "RelationTypes");

            migrationBuilder.DropTable(
                name: "AttemptStages");

            migrationBuilder.DropTable(
                name: "Attempts");

            migrationBuilder.DropTable(
                name: "QuizTypes");

            migrationBuilder.DropTable(
                name: "FlashcardStates");

            migrationBuilder.DropIndex(
                name: "IX_Flashcards_AttemptId",
                table: "Flashcards");

            migrationBuilder.DropColumn(
                name: "AttemptId",
                table: "Flashcards");
        }
    }
}
