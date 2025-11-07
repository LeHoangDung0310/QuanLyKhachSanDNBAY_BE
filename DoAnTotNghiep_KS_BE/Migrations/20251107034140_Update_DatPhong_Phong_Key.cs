using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnTotNghiep_KS_BE.Migrations
{
    /// <inheritdoc />
    public partial class Update_DatPhong_Phong_Key : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatPhong_Phong_Phong_MaPhong",
                table: "DatPhong_Phong");

            migrationBuilder.DropIndex(
                name: "IX_DatPhong_Phong_MaDatPhong",
                table: "DatPhong_Phong");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "DatPhong_Phong",
                newName: "MaDatPhong_Phong");

            migrationBuilder.CreateIndex(
                name: "IX_DatPhong_Phong_MaDatPhong_MaPhong",
                table: "DatPhong_Phong",
                columns: new[] { "MaDatPhong", "MaPhong" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DatPhong_Phong_Phong_MaPhong",
                table: "DatPhong_Phong",
                column: "MaPhong",
                principalTable: "Phong",
                principalColumn: "MaPhong",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatPhong_Phong_Phong_MaPhong",
                table: "DatPhong_Phong");

            migrationBuilder.DropIndex(
                name: "IX_DatPhong_Phong_MaDatPhong_MaPhong",
                table: "DatPhong_Phong");

            migrationBuilder.RenameColumn(
                name: "MaDatPhong_Phong",
                table: "DatPhong_Phong",
                newName: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DatPhong_Phong_MaDatPhong",
                table: "DatPhong_Phong",
                column: "MaDatPhong");

            migrationBuilder.AddForeignKey(
                name: "FK_DatPhong_Phong_Phong_MaPhong",
                table: "DatPhong_Phong",
                column: "MaPhong",
                principalTable: "Phong",
                principalColumn: "MaPhong",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
