using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bil372Project.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddMlRelatedTables1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Allergies",
                table: "UserMeasures");

            migrationBuilder.DropColumn(
                name: "DislikedFoods",
                table: "UserMeasures");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Allergies",
                table: "UserMeasures",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DislikedFoods",
                table: "UserMeasures",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
