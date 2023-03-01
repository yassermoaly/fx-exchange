﻿// <auto-generated />
using System;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataAccessLayer.Migrations
{
    [DbContext(typeof(FxExchangeDBContext))]
    [Migration("20230301151243_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Models.Data.Account", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<double>("Balance")
                        .HasColumnType("float");

                    b.Property<short>("CurrencyId")
                        .HasColumnType("smallint");

                    b.Property<long>("HolderId")
                        .HasColumnType("bigint");

                    b.Property<TimeSpan?>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("time");

                    b.HasKey("Id");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("HolderId");

                    b.ToTable("Accounts", t =>
                        {
                            t.HasCheckConstraint("AccountBalanceGreaterThanOrEqualZero", "[Balance] >= 0");
                        });

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            Balance = 1000.0,
                            CurrencyId = (short)1,
                            HolderId = 1L
                        },
                        new
                        {
                            Id = 2L,
                            Balance = 1000.0,
                            CurrencyId = (short)2,
                            HolderId = 1L
                        },
                        new
                        {
                            Id = 3L,
                            Balance = 1000.0,
                            CurrencyId = (short)3,
                            HolderId = 1L
                        },
                        new
                        {
                            Id = 4L,
                            Balance = 1000.0,
                            CurrencyId = (short)4,
                            HolderId = 1L
                        });
                });

            modelBuilder.Entity("Models.Data.Currency", b =>
                {
                    b.Property<short>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<short>("Id"));

                    b.Property<string>("ISOCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Currencies");

                    b.HasData(
                        new
                        {
                            Id = (short)1,
                            ISOCode = "EUR",
                            Name = "EURO"
                        },
                        new
                        {
                            Id = (short)2,
                            ISOCode = "USD",
                            Name = "American Dollar"
                        },
                        new
                        {
                            Id = (short)3,
                            ISOCode = "CAD",
                            Name = "Canadian Dollar"
                        },
                        new
                        {
                            Id = (short)4,
                            ISOCode = "EGP",
                            Name = "Egyptian Pound"
                        });
                });

            modelBuilder.Entity("Models.Data.FxTransaction", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<double>("FxRate")
                        .HasColumnType("float");

                    b.Property<short>("FxTransactionFixedSideId")
                        .HasColumnType("smallint");

                    b.Property<long>("HolderId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("FxTransactionFixedSideId");

                    b.HasIndex("HolderId");

                    b.ToTable("FxTransactions");
                });

            modelBuilder.Entity("Models.Data.FxTransactionDetail", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<double>("AccountBalancePost")
                        .HasColumnType("float");

                    b.Property<double>("AccountBalancePre")
                        .HasColumnType("float");

                    b.Property<long>("AccountId")
                        .HasColumnType("bigint");

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<short>("FxTransactionDetailTypeId")
                        .HasColumnType("smallint");

                    b.Property<long>("FxTransactionId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("FxTransactionDetailTypeId");

                    b.HasIndex("FxTransactionId");

                    b.ToTable("FxTransactionDetails");
                });

            modelBuilder.Entity("Models.Data.FxTransactionDetailType", b =>
                {
                    b.Property<short>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<short>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("FxTransactionDetailTypes");

                    b.HasData(
                        new
                        {
                            Id = (short)1,
                            Name = "Sell"
                        },
                        new
                        {
                            Id = (short)2,
                            Name = "Buy"
                        });
                });

            modelBuilder.Entity("Models.Data.FxTransactionFixedSide", b =>
                {
                    b.Property<short>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<short>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("FxTransactionFixedSides");

                    b.HasData(
                        new
                        {
                            Id = (short)1,
                            Name = "Buy"
                        },
                        new
                        {
                            Id = (short)2,
                            Name = "Sell"
                        });
                });

            modelBuilder.Entity("Models.Data.Holder", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PassportId")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.HasKey("Id");

                    b.ToTable("Holders");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            Address = "Zayed, Egypt",
                            FirstName = "Yasser",
                            LastName = "Moaly",
                            PassportId = "A27344511"
                        });
                });

            modelBuilder.Entity("Models.Data.Account", b =>
                {
                    b.HasOne("Models.Data.Currency", "Currency")
                        .WithMany("Accounts")
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Models.Data.Holder", "Holder")
                        .WithMany("Accounts")
                        .HasForeignKey("HolderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Currency");

                    b.Navigation("Holder");
                });

            modelBuilder.Entity("Models.Data.FxTransaction", b =>
                {
                    b.HasOne("Models.Data.FxTransactionFixedSide", "FxTransactionFixedSide")
                        .WithMany()
                        .HasForeignKey("FxTransactionFixedSideId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Models.Data.Holder", "Holder")
                        .WithMany()
                        .HasForeignKey("HolderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FxTransactionFixedSide");

                    b.Navigation("Holder");
                });

            modelBuilder.Entity("Models.Data.FxTransactionDetail", b =>
                {
                    b.HasOne("Models.Data.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Models.Data.FxTransactionDetailType", "FxTransactionDetailType")
                        .WithMany()
                        .HasForeignKey("FxTransactionDetailTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Models.Data.FxTransaction", "FxTransaction")
                        .WithMany("FxTransactionDetails")
                        .HasForeignKey("FxTransactionId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("FxTransaction");

                    b.Navigation("FxTransactionDetailType");
                });

            modelBuilder.Entity("Models.Data.Currency", b =>
                {
                    b.Navigation("Accounts");
                });

            modelBuilder.Entity("Models.Data.FxTransaction", b =>
                {
                    b.Navigation("FxTransactionDetails");
                });

            modelBuilder.Entity("Models.Data.Holder", b =>
                {
                    b.Navigation("Accounts");
                });
#pragma warning restore 612, 618
        }
    }
}