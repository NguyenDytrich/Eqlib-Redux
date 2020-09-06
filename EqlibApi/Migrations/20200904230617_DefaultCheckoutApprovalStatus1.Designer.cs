﻿// <auto-generated />
using System;
using EqlibApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace EqlibApi.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20200904230617_DefaultCheckoutApprovalStatus1")]
    partial class DefaultCheckoutApprovalStatus1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("EqlibApi.Models.Db.Checkout", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("ApprovalStatus")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(1);

                    b.Property<DateTime?>("CheckoutDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp without time zone")
                        .HasDefaultValueSql("NOW()");

                    b.Property<int?>("CheckoutStatus")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(3);

                    b.Property<DateTime>("DueDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("ReturnDate")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("Checkouts");
                });

            modelBuilder.Entity("EqlibApi.Models.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("Availability")
                        .HasColumnType("integer");

                    b.Property<int?>("CheckoutId")
                        .HasColumnType("integer");

                    b.Property<int>("Condition")
                        .HasColumnType("integer");

                    b.Property<DateTime>("DateAcquired")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("ItemGroupId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("LastInspected")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Notes")
                        .HasColumnType("text");

                    b.Property<bool>("OffsiteEligable")
                        .HasColumnType("boolean");

                    b.Property<string>("SerialNumber")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CheckoutId");

                    b.HasIndex("ItemGroupId");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("EqlibApi.Models.ItemGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Category")
                        .HasColumnType("text");

                    b.Property<string>("Department")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Make")
                        .HasColumnType("text");

                    b.Property<string>("Model")
                        .HasColumnType("text");

                    b.Property<string>("Moniker")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ItemGroups");
                });

            modelBuilder.Entity("EqlibApi.Models.Item", b =>
                {
                    b.HasOne("EqlibApi.Models.Db.Checkout", null)
                        .WithMany("Items")
                        .HasForeignKey("CheckoutId");

                    b.HasOne("EqlibApi.Models.ItemGroup", null)
                        .WithMany("Inventory")
                        .HasForeignKey("ItemGroupId");
                });
#pragma warning restore 612, 618
        }
    }
}
