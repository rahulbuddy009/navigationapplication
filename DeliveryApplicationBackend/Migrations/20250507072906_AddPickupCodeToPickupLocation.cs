using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryApplicationBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddPickupCodeToPickupLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PickupCode",
                table: "PickupLocations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PickupCode",
                table: "PickupLocations");
        }
    }
}
