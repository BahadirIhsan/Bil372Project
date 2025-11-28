using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bil372Project.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddMlRelatedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bmi",
                table: "UserMeasures");

            migrationBuilder.AddColumn<string>(
                name: "ActivityLevel",
                table: "UserMeasures",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DietaryPreference",
                table: "UserMeasures",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MeasurementsForMl",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserMeasureId = table.Column<int>(type: "int", nullable: false),
                    Bmi = table.Column<double>(type: "double", nullable: false),
                    DailyCalorieTarget = table.Column<double>(type: "double", nullable: false),
                    ProteinGrams = table.Column<double>(type: "double", nullable: false),
                    FatGrams = table.Column<double>(type: "double", nullable: false),
                    SugarGrams = table.Column<double>(type: "double", nullable: false),
                    SodiumMg = table.Column<double>(type: "double", nullable: false),
                    CalculatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementsForMl", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeasurementsForMl_UserMeasures_UserMeasureId",
                        column: x => x.UserMeasureId,
                        principalTable: "UserMeasures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserDietPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserMeasureId = table.Column<int>(type: "int", nullable: false),
                    Breakfast = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Lunch = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Dinner = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Snack = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GeneratedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModelVersion = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDietPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDietPlans_UserMeasures_UserMeasureId",
                        column: x => x.UserMeasureId,
                        principalTable: "UserMeasures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementsForMl_UserMeasureId",
                table: "MeasurementsForMl",
                column: "UserMeasureId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDietPlans_UserMeasureId",
                table: "UserDietPlans",
                column: "UserMeasureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeasurementsForMl");

            migrationBuilder.DropTable(
                name: "UserDietPlans");

            migrationBuilder.DropColumn(
                name: "ActivityLevel",
                table: "UserMeasures");

            migrationBuilder.DropColumn(
                name: "DietaryPreference",
                table: "UserMeasures");

            migrationBuilder.AddColumn<double>(
                name: "Bmi",
                table: "UserMeasures",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
