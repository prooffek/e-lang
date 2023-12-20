﻿// <auto-generated />
using System;
using E_Lang.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace E_Lang.Persistence.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("E_Lang.Domain.Entities.Attempt", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CollectionId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("CurrentStageId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IncludeMeanings")
                        .HasColumnType("boolean");

                    b.Property<int>("MaxFlashcardsPerStage")
                        .HasColumnType("integer");

                    b.Property<int>("MaxQuizTypesPerFlashcard")
                        .HasColumnType("integer");

                    b.Property<int>("MinCompletedQuizzesPerCent")
                        .HasColumnType("integer");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Order")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CollectionId");

                    b.HasIndex("CurrentStageId");

                    b.ToTable("Attempts");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.AttemptProperty", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AttemptId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CustomPropertyId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("AttemptProperties");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.AttemptQuizType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AttemptId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("QuiTypeId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("AttemptQuizTypes");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.AttemptStage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("AttemptStages");

                    b.HasDiscriminator<string>("Discriminator").HasValue("AttemptStage");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.AttemptStageFlashcardState", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AttemptStageId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("FlashcardStateId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("AttemptStageFlashcardStates");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.Collection", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(120)
                        .HasColumnType("character varying(120)");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ParentId");

                    b.HasIndex(new[] { "Id", "ParentId" }, "Collection_Id_ParentId");

                    b.ToTable("Collections");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.CompletedQuizType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("FlashcardStateId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("QuizTypeId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("CompletedQuizTypes");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.CustomProperty", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("AttemptId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.HasKey("Id");

                    b.HasIndex("AttemptId");

                    b.ToTable("CustomProperties");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.CustomPropertyRelation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("Property1Id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("Property2Id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RelationTypeId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Property2Id");

                    b.HasIndex("RelationTypeId");

                    b.ToTable("CustomPropertyRelation");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.Flashcard", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("AttemptId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("CollectionId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("FlashcardBaseId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("LastSeenOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastStatusChangedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AttemptId");

                    b.HasIndex("CollectionId");

                    b.HasIndex("FlashcardBaseId");

                    b.HasIndex(new[] { "Id", "OwnerId" }, "Flashcard_Id_OwnerId");

                    b.ToTable("Flashcards");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.FlashcardBase", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("WordOrPhrase")
                        .IsRequired()
                        .HasMaxLength(10000)
                        .HasColumnType("character varying(10000)");

                    b.HasKey("Id");

                    b.ToTable("FlashcardBase");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.FlashcardState", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("AttemptStageId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("CurrentQuizTypeId")
                        .HasColumnType("uuid");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("FlashcardId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("ShowAgainOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AttemptStageId");

                    b.HasIndex("CurrentQuizTypeId");

                    b.HasIndex("FlashcardId");

                    b.ToTable("FlashcardStates");

                    b.HasDiscriminator<string>("Discriminator").HasValue("FlashcardState");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.Meaning", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("FlashcardBaseId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(10000)
                        .HasColumnType("character varying(10000)");

                    b.HasKey("Id");

                    b.HasIndex("FlashcardBaseId");

                    b.ToTable("Meaning");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.MeaningsRelation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("Meaning1Id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("Meaning2Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("RelationTypeId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Meaning2Id");

                    b.HasIndex("RelationTypeId");

                    b.ToTable("MeaningsRelations");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.QuizType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("AttemptId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("FlashcardStateId")
                        .HasColumnType("uuid");

                    b.Property<string>("Instruction")
                        .IsRequired()
                        .HasMaxLength(10000)
                        .HasColumnType("character varying(10000)");

                    b.Property<bool>("IsArrange")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsFillInBlank")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsInput")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsMatch")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsMultiselect")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsSelect")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsSelectCorrect")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsSelectMissing")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.HasKey("Id");

                    b.HasIndex("AttemptId");

                    b.HasIndex("FlashcardStateId");

                    b.ToTable("QuizTypes");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.RelationType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.HasKey("Id");

                    b.ToTable("RelationTypes");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "Email" }, "User_Email")
                        .IsUnique();

                    b.HasIndex(new[] { "Id" }, "User_Id");

                    b.HasIndex(new[] { "UserName" }, "User_UserName")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.InitAttemptStage", b =>
                {
                    b.HasBaseType("E_Lang.Domain.Entities.AttemptStage");

                    b.HasDiscriminator().HasValue("InitAttemptStage");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.InitFlashcardState", b =>
                {
                    b.HasBaseType("E_Lang.Domain.Entities.FlashcardState");

                    b.HasDiscriminator().HasValue("InitFlashcardState");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.Attempt", b =>
                {
                    b.HasOne("E_Lang.Domain.Entities.Collection", "Collection")
                        .WithMany("Attempts")
                        .HasForeignKey("CollectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("E_Lang.Domain.Entities.AttemptStage", "CurrentStage")
                        .WithMany()
                        .HasForeignKey("CurrentStageId");

                    b.Navigation("Collection");

                    b.Navigation("CurrentStage");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.Collection", b =>
                {
                    b.HasOne("E_Lang.Domain.Entities.Collection", "Parent")
                        .WithMany("Subcollections")
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.CustomProperty", b =>
                {
                    b.HasOne("E_Lang.Domain.Entities.Attempt", null)
                        .WithMany("Properties")
                        .HasForeignKey("AttemptId");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.CustomPropertyRelation", b =>
                {
                    b.HasOne("E_Lang.Domain.Entities.Flashcard", null)
                        .WithMany("PropertyRelaions")
                        .HasForeignKey("Property2Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("E_Lang.Domain.Entities.RelationType", "RelationType")
                        .WithMany()
                        .HasForeignKey("RelationTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RelationType");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.Flashcard", b =>
                {
                    b.HasOne("E_Lang.Domain.Entities.Attempt", null)
                        .WithMany("CompletedFlashcards")
                        .HasForeignKey("AttemptId");

                    b.HasOne("E_Lang.Domain.Entities.Collection", "Collection")
                        .WithMany("Flashcards")
                        .HasForeignKey("CollectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("E_Lang.Domain.Entities.FlashcardBase", "FlashcardBase")
                        .WithMany()
                        .HasForeignKey("FlashcardBaseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Collection");

                    b.Navigation("FlashcardBase");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.FlashcardState", b =>
                {
                    b.HasOne("E_Lang.Domain.Entities.AttemptStage", null)
                        .WithMany("Flashcards")
                        .HasForeignKey("AttemptStageId");

                    b.HasOne("E_Lang.Domain.Entities.QuizType", "CurrentQuizType")
                        .WithMany()
                        .HasForeignKey("CurrentQuizTypeId");

                    b.HasOne("E_Lang.Domain.Entities.Flashcard", "Flashcard")
                        .WithMany()
                        .HasForeignKey("FlashcardId");

                    b.Navigation("CurrentQuizType");

                    b.Navigation("Flashcard");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.Meaning", b =>
                {
                    b.HasOne("E_Lang.Domain.Entities.FlashcardBase", "FlashcardBase")
                        .WithMany("Meanings")
                        .HasForeignKey("FlashcardBaseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FlashcardBase");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.MeaningsRelation", b =>
                {
                    b.HasOne("E_Lang.Domain.Entities.Flashcard", null)
                        .WithMany("MeaningRelations")
                        .HasForeignKey("Meaning2Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("E_Lang.Domain.Entities.RelationType", "RelationType")
                        .WithMany()
                        .HasForeignKey("RelationTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RelationType");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.QuizType", b =>
                {
                    b.HasOne("E_Lang.Domain.Entities.Attempt", null)
                        .WithMany("QuizTypes")
                        .HasForeignKey("AttemptId");

                    b.HasOne("E_Lang.Domain.Entities.FlashcardState", null)
                        .WithMany("CompletedQuizTypes")
                        .HasForeignKey("FlashcardStateId");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.Attempt", b =>
                {
                    b.Navigation("CompletedFlashcards");

                    b.Navigation("Properties");

                    b.Navigation("QuizTypes");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.AttemptStage", b =>
                {
                    b.Navigation("Flashcards");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.Collection", b =>
                {
                    b.Navigation("Attempts");

                    b.Navigation("Flashcards");

                    b.Navigation("Subcollections");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.Flashcard", b =>
                {
                    b.Navigation("MeaningRelations");

                    b.Navigation("PropertyRelaions");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.FlashcardBase", b =>
                {
                    b.Navigation("Meanings");
                });

            modelBuilder.Entity("E_Lang.Domain.Entities.FlashcardState", b =>
                {
                    b.Navigation("CompletedQuizTypes");
                });
#pragma warning restore 612, 618
        }
    }
}
