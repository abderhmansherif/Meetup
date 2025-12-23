using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetupWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddingTransactionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "RefundId",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "RefundStatus",
                table: "Transactions",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Transactions",
                newName: "PaymentAt");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Transactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PaymentType",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Transactions",
                newName: "RefundStatus");

            migrationBuilder.RenameColumn(
                name: "PaymentAt",
                table: "Transactions",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefundId",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
