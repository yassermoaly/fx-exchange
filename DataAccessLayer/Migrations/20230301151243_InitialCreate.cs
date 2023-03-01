using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISOCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FxTransactionDetailTypes",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FxTransactionDetailTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FxTransactionFixedSides",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FxTransactionFixedSides", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Holders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PassportId = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HolderId = table.Column<long>(type: "bigint", nullable: false),
                    Balance = table.Column<double>(type: "float", nullable: false),
                    CurrencyId = table.Column<short>(type: "smallint", nullable: false),
                    RowVersion = table.Column<TimeSpan>(type: "time", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.CheckConstraint("AccountBalanceGreaterThanOrEqualZero", "[Balance] >= 0");
                    table.ForeignKey(
                        name: "FK_Accounts_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Accounts_Holders_HolderId",
                        column: x => x.HolderId,
                        principalTable: "Holders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FxTransactions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HolderId = table.Column<long>(type: "bigint", nullable: false),
                    FxTransactionFixedSideId = table.Column<short>(type: "smallint", nullable: false),
                    FxRate = table.Column<double>(type: "float", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FxTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FxTransactions_FxTransactionFixedSides_FxTransactionFixedSideId",
                        column: x => x.FxTransactionFixedSideId,
                        principalTable: "FxTransactionFixedSides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FxTransactions_Holders_HolderId",
                        column: x => x.HolderId,
                        principalTable: "Holders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FxTransactionDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FxTransactionId = table.Column<long>(type: "bigint", nullable: false),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    FxTransactionDetailTypeId = table.Column<short>(type: "smallint", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    AccountBalancePre = table.Column<double>(type: "float", nullable: false),
                    AccountBalancePost = table.Column<double>(type: "float", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FxTransactionDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FxTransactionDetails_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FxTransactionDetails_FxTransactionDetailTypes_FxTransactionDetailTypeId",
                        column: x => x.FxTransactionDetailTypeId,
                        principalTable: "FxTransactionDetailTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FxTransactionDetails_FxTransactions_FxTransactionId",
                        column: x => x.FxTransactionId,
                        principalTable: "FxTransactions",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "ISOCode", "Name" },
                values: new object[,]
                {
                    { (short)1, "EUR", "EURO" },
                    { (short)2, "USD", "American Dollar" },
                    { (short)3, "CAD", "Canadian Dollar" },
                    { (short)4, "EGP", "Egyptian Pound" }
                });

            migrationBuilder.InsertData(
                table: "FxTransactionDetailTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { (short)1, "Sell" },
                    { (short)2, "Buy" }
                });

            migrationBuilder.InsertData(
                table: "FxTransactionFixedSides",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { (short)1, "Buy" },
                    { (short)2, "Sell" }
                });

            migrationBuilder.InsertData(
                table: "Holders",
                columns: new[] { "Id", "Address", "FirstName", "LastName", "PassportId" },
                values: new object[] { 1L, "Zayed, Egypt", "Yasser", "Moaly", "A27344511" });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Balance", "CurrencyId", "HolderId" },
                values: new object[,]
                {
                    { 1L, 1000.0, (short)1, 1L },
                    { 2L, 1000.0, (short)2, 1L },
                    { 3L, 1000.0, (short)3, 1L },
                    { 4L, 1000.0, (short)4, 1L }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CurrencyId",
                table: "Accounts",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_HolderId",
                table: "Accounts",
                column: "HolderId");

            migrationBuilder.CreateIndex(
                name: "IX_FxTransactionDetails_AccountId",
                table: "FxTransactionDetails",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_FxTransactionDetails_FxTransactionDetailTypeId",
                table: "FxTransactionDetails",
                column: "FxTransactionDetailTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FxTransactionDetails_FxTransactionId",
                table: "FxTransactionDetails",
                column: "FxTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_FxTransactions_FxTransactionFixedSideId",
                table: "FxTransactions",
                column: "FxTransactionFixedSideId");

            migrationBuilder.CreateIndex(
                name: "IX_FxTransactions_HolderId",
                table: "FxTransactions",
                column: "HolderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FxTransactionDetails");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "FxTransactionDetailTypes");

            migrationBuilder.DropTable(
                name: "FxTransactions");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "FxTransactionFixedSides");

            migrationBuilder.DropTable(
                name: "Holders");
        }
    }
}
