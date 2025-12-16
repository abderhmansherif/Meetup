using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetupWebApp.Migrations
{
    /// <inheritdoc />
    public partial class ModifingtheEventEntitywithpaymentfeaturesproperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasCost",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Refundable",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "TicketPrice",
                table: "Events",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasCost",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Refundable",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "TicketPrice",
                table: "Events");
        }
    }
}
