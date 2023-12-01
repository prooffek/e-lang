using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Lang.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Addflashcards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollectionFlashcard");

            migrationBuilder.DropIndex(
                name: "User_UserName",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "CollectionId",
                table: "Flashcards",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "FlashcardBaseId",
                table: "Flashcards",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSeenOn",
                table: "Flashcards",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastStatusChangedOn",
                table: "Flashcards",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Flashcards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Collections",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateTable(
                name: "FlashcardBase",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WordOrPhrase = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlashcardBase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Meaning",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    FlashcardId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meaning", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlashcardBaseMeaning",
                columns: table => new
                {
                    FlashcardBasesId = table.Column<Guid>(type: "uuid", nullable: false),
                    MeaningsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlashcardBaseMeaning", x => new { x.FlashcardBasesId, x.MeaningsId });
                    table.ForeignKey(
                        name: "FK_FlashcardBaseMeaning_FlashcardBase_FlashcardBasesId",
                        column: x => x.FlashcardBasesId,
                        principalTable: "FlashcardBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlashcardBaseMeaning_Meaning_MeaningsId",
                        column: x => x.MeaningsId,
                        principalTable: "Meaning",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "User_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "User_UserName",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Flashcards_CollectionId",
                table: "Flashcards",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Flashcards_FlashcardBaseId",
                table: "Flashcards",
                column: "FlashcardBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_OwnerId",
                table: "Collections",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_FlashcardBaseMeaning_MeaningsId",
                table: "FlashcardBaseMeaning",
                column: "MeaningsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flashcards_Collections_CollectionId",
                table: "Flashcards",
                column: "CollectionId",
                principalTable: "Collections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Flashcards_FlashcardBase_FlashcardBaseId",
                table: "Flashcards",
                column: "FlashcardBaseId",
                principalTable: "FlashcardBase",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flashcards_Collections_CollectionId",
                table: "Flashcards");

            migrationBuilder.DropForeignKey(
                name: "FK_Flashcards_FlashcardBase_FlashcardBaseId",
                table: "Flashcards");

            migrationBuilder.DropTable(
                name: "FlashcardBaseMeaning");

            migrationBuilder.DropTable(
                name: "FlashcardBase");

            migrationBuilder.DropTable(
                name: "Meaning");

            migrationBuilder.DropIndex(
                name: "User_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "User_UserName",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Flashcards_CollectionId",
                table: "Flashcards");

            migrationBuilder.DropIndex(
                name: "IX_Flashcards_FlashcardBaseId",
                table: "Flashcards");

            migrationBuilder.DropIndex(
                name: "IX_Collections_OwnerId",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "CollectionId",
                table: "Flashcards");

            migrationBuilder.DropColumn(
                name: "FlashcardBaseId",
                table: "Flashcards");

            migrationBuilder.DropColumn(
                name: "LastSeenOn",
                table: "Flashcards");

            migrationBuilder.DropColumn(
                name: "LastStatusChangedOn",
                table: "Flashcards");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Flashcards");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Collections",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120);

            migrationBuilder.CreateTable(
                name: "CollectionFlashcard",
                columns: table => new
                {
                    CollectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    FlashcardId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionFlashcard", x => new { x.CollectionId, x.FlashcardId });
                    table.ForeignKey(
                        name: "FK_CollectionFlashcard_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionFlashcard_Flashcards_FlashcardId",
                        column: x => x.FlashcardId,
                        principalTable: "Flashcards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "User_UserName",
                table: "Users",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionFlashcard_FlashcardId",
                table: "CollectionFlashcard",
                column: "FlashcardId");
        }
    }
}
