﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Xpense.API.Data;
using Xpense.Persistence;

#nullable disable

namespace Xpense.Persistence.Migrations
{
    [DbContext(typeof(XpenseDbContext))]
    [Migration("20231006233329_Tags_AlterNameIndex_IsNotUnique")]
    partial class Tags_AlterNameIndex_IsNotUnique
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AccountTag", b =>
                {
                    b.Property<int>("AccountsId")
                        .HasColumnType("int");

                    b.Property<int>("TagsId")
                        .HasColumnType("int");

                    b.HasKey("AccountsId", "TagsId");

                    b.HasIndex("TagsId");

                    b.ToTable("AccountTag", "Xpense");
                });

            modelBuilder.Entity("TagTransaction", b =>
                {
                    b.Property<int>("TagsId")
                        .HasColumnType("int");

                    b.Property<int>("TransactionsId")
                        .HasColumnType("int");

                    b.HasKey("TagsId", "TransactionsId");

                    b.HasIndex("TransactionsId");

                    b.ToTable("TagTransaction", "Xpense");
                });

            modelBuilder.Entity("Xpense.API.Data.Models.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Balance")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<int>("CurrencyId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDefaultAccount")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsTransactionLocked")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Number")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nchar(10)")
                        .IsFixedLength();

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("Number")
                        .IsUnique();

                    b.ToTable("Accounts", "Xpense");
                });

            modelBuilder.Entity("Xpense.API.Data.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoryLevelId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryLevelId");

                    b.ToTable("Categories", "Xpense");
                });

            modelBuilder.Entity("Xpense.API.Data.Models.CategoryLevel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("Weight")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("CategoryLevels", "Xpense");
                });

            modelBuilder.Entity("Xpense.API.Data.Models.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<int>("CurrencyId")
                        .HasColumnType("int");

                    b.Property<string>("DialCode")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Country", "Xpense");
                });

            modelBuilder.Entity("Xpense.API.Data.Models.Currency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("IsoCode")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("nvarchar(3)");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("IsoCode")
                        .IsUnique();

                    b.ToTable("Currencies", "Currency");
                });

            modelBuilder.Entity("Xpense.API.Data.Models.CurrencyExchangeRate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<int>("ForeignCurrencyId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<int>("PrincipalCurrencyId")
                        .HasColumnType("int");

                    b.Property<decimal>("Rate")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("ForeignCurrencyId");

                    b.HasIndex("PrincipalCurrencyId");

                    b.ToTable("CurrencyExchangeRate", "Currency");
                });

            modelBuilder.Entity("Xpense.API.Data.Models.CurrencyExchangeRateAudit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<int>("ExchangeRateId")
                        .HasColumnType("int");

                    b.Property<int>("ForeignCurrencyId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<int>("PrincipalCurrencyId")
                        .HasColumnType("int");

                    b.Property<decimal>("Rate")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("ExchangeRateId");

                    b.HasIndex("ForeignCurrencyId");

                    b.HasIndex("PrincipalCurrencyId");

                    b.ToTable("CurrencyExchangeRateAudits", "Currency");
                });

            modelBuilder.Entity("Xpense.API.Data.Models.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("BgColorHex")
                        .HasMaxLength(6)
                        .HasColumnType("nchar(6)")
                        .IsFixedLength();

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("FgColorHex")
                        .HasMaxLength(6)
                        .HasColumnType("nchar(6)")
                        .IsFixedLength();

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.ToTable("Tag", "Xpense");
                });

            modelBuilder.Entity("Xpense.API.Data.Models.Transaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Amount")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<int>("CurrencyExchangeRateAuditId")
                        .HasColumnType("int");

                    b.Property<int>("FromAccountId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Reason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ToAccountId")
                        .HasColumnType("int");

                    b.Property<int>("TransactionType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("CurrencyExchangeRateAuditId");

                    b.HasIndex("FromAccountId");

                    b.HasIndex("ToAccountId");

                    b.ToTable("Transactions", "Xpense");
                });

            modelBuilder.Entity("AccountTag", b =>
                {
                    b.HasOne("Xpense.API.Data.Models.Account", null)
                        .WithMany()
                        .HasForeignKey("AccountsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Xpense.API.Data.Models.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TagTransaction", b =>
                {
                    b.HasOne("Xpense.API.Data.Models.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Xpense.API.Data.Models.Transaction", null)
                        .WithMany()
                        .HasForeignKey("TransactionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Xpense.API.Data.Models.Account", b =>
                {
                    b.HasOne("Xpense.API.Data.Models.Currency", "Currency")
                        .WithMany("LinkedAccounts")
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Currency");
                });

            modelBuilder.Entity("Xpense.API.Data.Models.Category", b =>
                {
                    b.HasOne("Xpense.API.Data.Models.CategoryLevel", "CategoryLevel")
                        .WithMany("Categories")
                        .HasForeignKey("CategoryLevelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CategoryLevel");
                });

            modelBuilder.Entity("Xpense.API.Data.Models.Country", b =>
                {
                    b.HasOne("Xpense.API.Data.Models.Currency", "Currency")
                        .WithMany("Countries")
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Currency");
                });

            modelBuilder.Entity("Xpense.API.Data.Models.CurrencyExchangeRate", b =>
                {
                    b.HasOne("Xpense.API.Data.Models.Currency", "ForeignCurrency")
                        .WithMany("ForeignCurrencyExchangeRates")
                        .HasForeignKey("ForeignCurrencyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Xpense.API.Data.Models.Currency", "PrincipalCurrency")
                        .WithMany("PrincipalExchangeRates")
                        .HasForeignKey("PrincipalCurrencyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ForeignCurrency");

                    b.Navigation("PrincipalCurrency");
                });

            modelBuilder.Entity("Xpense.API.Data.Models.CurrencyExchangeRateAudit", b =>
                {
                    b.HasOne("Xpense.API.Data.Models.CurrencyExchangeRate", "ExchangeRate")
                        .WithMany("Audits")
                        .HasForeignKey("ExchangeRateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Xpense.API.Data.Models.Currency", "ForeignCurrency")
                        .WithMany("ForeignCurrencyExchangeRateAudits")
                        .HasForeignKey("ForeignCurrencyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Xpense.API.Data.Models.Currency", "PrincipalCurrency")
                        .WithMany("PrincipalExchangeRateAudits")
                        .HasForeignKey("PrincipalCurrencyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ExchangeRate");

                    b.Navigation("ForeignCurrency");

                    b.Navigation("PrincipalCurrency");
                });

            modelBuilder.Entity("Xpense.API.Data.Models.Transaction", b =>
                {
                    b.HasOne("Xpense.API.Data.Models.Category", "Category")
                        .WithMany("Transactions")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Xpense.API.Data.Models.CurrencyExchangeRateAudit", "CurrencyExchangeRateAudit")
                        .WithMany("Transactions")
                        .HasForeignKey("CurrencyExchangeRateAuditId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Xpense.API.Data.Models.Account", "FromAccount")
                        .WithMany("WithdrawTransactions")
                        .HasForeignKey("FromAccountId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Xpense.API.Data.Models.Account", "ToAccount")
                        .WithMany("DepositTransactions")
                        .HasForeignKey("ToAccountId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("CurrencyExchangeRateAudit");

                    b.Navigation("FromAccount");

                    b.Navigation("ToAccount");
                });

            modelBuilder.Entity("Xpense.API.Data.Models.Account", b =>
                {
                    b.Navigation("DepositTransactions");

                    b.Navigation("WithdrawTransactions");
                });

            modelBuilder.Entity("Xpense.API.Data.Models.Category", b =>
                {
                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("Xpense.API.Data.Models.CategoryLevel", b =>
                {
                    b.Navigation("Categories");
                });

            modelBuilder.Entity("Xpense.API.Data.Models.Currency", b =>
                {
                    b.Navigation("Countries");

                    b.Navigation("ForeignCurrencyExchangeRateAudits");

                    b.Navigation("ForeignCurrencyExchangeRates");

                    b.Navigation("LinkedAccounts");

                    b.Navigation("PrincipalExchangeRateAudits");

                    b.Navigation("PrincipalExchangeRates");
                });

            modelBuilder.Entity("Xpense.API.Data.Models.CurrencyExchangeRate", b =>
                {
                    b.Navigation("Audits");
                });

            modelBuilder.Entity("Xpense.API.Data.Models.CurrencyExchangeRateAudit", b =>
                {
                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
