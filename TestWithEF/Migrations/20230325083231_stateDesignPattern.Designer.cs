﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TestWithEF;

#nullable disable

namespace TestWithEF.Migrations
{
    [DbContext(typeof(TestContext))]
    [Migration("20230325083231_stateDesignPattern")]
    partial class stateDesignPattern
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TestWithEF.Entities.Author", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("TestWithEF.Entities.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CancelledAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ConfirmedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ProcessedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("State")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("TestWithEF.Entities.Author", b =>
                {
                    b.OwnsOne("TestWithEF.Entities.ContactDetails", "ContactDetails", b1 =>
                        {
                            b1.Property<Guid>("AuthorId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Phone")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("AuthorId");

                            b1.ToTable("Authors");

                            b1.ToJson("ContactDetails");

                            b1.WithOwner()
                                .HasForeignKey("AuthorId");

                            b1.OwnsOne("TestWithEF.Entities.Address", "Address", b2 =>
                                {
                                    b2.Property<Guid>("ContactDetailsAuthorId")
                                        .HasColumnType("uniqueidentifier");

                                    b2.Property<string>("City")
                                        .HasColumnType("nvarchar(max)");

                                    b2.Property<string>("Country")
                                        .HasColumnType("nvarchar(max)");

                                    b2.Property<string>("Postcode")
                                        .HasColumnType("nvarchar(max)");

                                    b2.Property<string>("Street")
                                        .HasColumnType("nvarchar(max)");

                                    b2.HasKey("ContactDetailsAuthorId");

                                    b2.ToTable("Authors");

                                    b2.WithOwner()
                                        .HasForeignKey("ContactDetailsAuthorId");
                                });

                            b1.Navigation("Address");
                        });

                    b.Navigation("ContactDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
