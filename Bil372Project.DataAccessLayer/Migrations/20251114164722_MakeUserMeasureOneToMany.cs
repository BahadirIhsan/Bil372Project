using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bil372Project.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class MakeUserMeasureOneToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Önce FK'yi düşür
            migrationBuilder.DropForeignKey(
                name: "FK_UserMeasures_Users_UserId",
                table: "UserMeasures");

            // 2) Sonra eski UNIQUE index'i düşür
            migrationBuilder.DropIndex(
                name: "IX_UserMeasures_UserId",
                table: "UserMeasures");

            // 3) Aynı isimle NON-UNIQUE index oluştur
            migrationBuilder.CreateIndex(
                name: "IX_UserMeasures_UserId",
                table: "UserMeasures",
                column: "UserId");

            // 4) FK'yi tekrar ekle (artık bu index'i kullanacak ama unique değil)
            migrationBuilder.AddForeignKey(
                name: "FK_UserMeasures_Users_UserId",
                table: "UserMeasures",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Geri almada tam tersi yapılır

            migrationBuilder.DropForeignKey(
                name: "FK_UserMeasures_Users_UserId",
                table: "UserMeasures");

            migrationBuilder.DropIndex(
                name: "IX_UserMeasures_UserId",
                table: "UserMeasures");

            // Eski hali unique ise geriye dönmek için:
            migrationBuilder.CreateIndex(
                name: "IX_UserMeasures_UserId",
                table: "UserMeasures",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMeasures_Users_UserId",
                table: "UserMeasures",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

    }
}
