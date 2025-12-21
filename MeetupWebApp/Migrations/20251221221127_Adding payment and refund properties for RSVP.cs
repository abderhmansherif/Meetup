using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetupWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddingpaymentandrefundpropertiesforRSVP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentId",
                table: "RSVPs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "RSVPs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefundId",
                table: "RSVPs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefundStatus",
                table: "RSVPs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "RSVPs");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "RSVPs");

            migrationBuilder.DropColumn(
                name: "RefundId",
                table: "RSVPs");

            migrationBuilder.DropColumn(
                name: "RefundStatus",
                table: "RSVPs");
        }
    }
}
