﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TestWithEF;

#nullable disable

namespace TestWithEF.Migrations
{
    [DbContext(typeof(TestDbContext))]
    partial class TestDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TestWithEF.Entities.Author", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("TestWithEF.Entities.OrderProduct", b =>
                {
                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("ProductId", "OrderId");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderProduct");
                });

            modelBuilder.Entity("TestWithEF.Entities.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<byte>("ProductType")
                        .HasColumnType("tinyint");

                    b.HasKey("Id");

                    b.ToTable("Product");

                    b.HasDiscriminator<byte>("ProductType");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("TestWithEF.Features.Orders.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uniqueidentifier");

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

                    b.HasIndex("AuthorId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("TestWithEF.Entities.FeaturedProduct", b =>
                {
                    b.HasBaseType("TestWithEF.Entities.Product");

                    b.Property<DateTimeOffset>("End")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("Start")
                        .HasColumnType("datetimeoffset");

                    b.HasDiscriminator().HasValue((byte)2);
                });

            modelBuilder.Entity("TestWithEF.Entities.StandardProduct", b =>
                {
                    b.HasBaseType("TestWithEF.Entities.Product");

                    b.HasDiscriminator().HasValue((byte)1);
                });

            modelBuilder.Entity("TestWithEF.Entities.Author", b =>
                {
                    b.OwnsOne("TestWithEF.ValueObjects.ContactDetails", "ContactDetails", b1 =>
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

            modelBuilder.Entity("TestWithEF.Entities.OrderProduct", b =>
                {
                    b.HasOne("TestWithEF.Features.Orders.Order", null)
                        .WithMany("OrderProducts")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TestWithEF.Entities.Product", null)
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TestWithEF.Features.Orders.Order", b =>
                {
                    b.HasOne("TestWithEF.Entities.Author", null)
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TestWithEF.Features.Orders.Order", b =>
                {
                    b.Navigation("OrderProducts");
                });
#pragma warning restore 612, 618
        }
    }
}
