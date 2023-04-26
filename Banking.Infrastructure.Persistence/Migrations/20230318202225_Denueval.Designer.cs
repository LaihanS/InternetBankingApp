﻿// <auto-generated />
using System;
using Banking.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Banking.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20230318202225_Denueval")]
    partial class Denueval
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Banking.Core.Domain.Entities.Pagos", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"), 1L, 1);

                    b.Property<double>("PaymentAmount")
                        .HasColumnType("float");

                    b.Property<string>("PaymentFor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentFrom")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ProductID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("created")
                        .HasColumnType("datetime2");

                    b.Property<string>("createdBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("modifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("modifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("userID")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.HasIndex("ProductID");

                    b.ToTable("Pagos", (string)null);
                });

            modelBuilder.Entity("Banking.Core.Domain.Entities.Products", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"), 1L, 1);

                    b.Property<string>("BeneficiarioID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("DebtAmount")
                        .HasColumnType("float");

                    b.Property<bool>("IsPrincipalAccount")
                        .HasColumnType("bit");

                    b.Property<double?>("ProductAmount")
                        .HasColumnType("float");

                    b.Property<string>("ProductType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UnicDigitSequence")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("UserHasDebt")
                        .HasColumnType("bit");

                    b.Property<string>("UserID")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("created")
                        .HasColumnType("datetime2");

                    b.Property<string>("createdBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("modifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("modifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("Productos", (string)null);
                });

            modelBuilder.Entity("Banking.Core.Domain.Entities.Transacciones", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"), 1L, 1);

                    b.Property<int?>("ProductID")
                        .HasColumnType("int");

                    b.Property<double>("TransactAmount")
                        .HasColumnType("float");

                    b.Property<string>("TransactFor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TransactFrom")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("created")
                        .HasColumnType("datetime2");

                    b.Property<string>("createdBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("modifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("modifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("userID")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.HasIndex("ProductID");

                    b.ToTable("Transacciones", (string)null);
                });

            modelBuilder.Entity("Banking.Core.Domain.Entities.Pagos", b =>
                {
                    b.HasOne("Banking.Core.Domain.Entities.Products", "Product")
                        .WithMany("Pagos")
                        .HasForeignKey("ProductID")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Banking.Core.Domain.Entities.Transacciones", b =>
                {
                    b.HasOne("Banking.Core.Domain.Entities.Products", "Product")
                        .WithMany()
                        .HasForeignKey("ProductID");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Banking.Core.Domain.Entities.Products", b =>
                {
                    b.Navigation("Pagos");
                });
#pragma warning restore 612, 618
        }
    }
}