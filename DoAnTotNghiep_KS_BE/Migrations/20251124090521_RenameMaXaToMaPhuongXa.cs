using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnTotNghiep_KS_BE.Migrations
{
    /// <inheritdoc />
    public partial class RenameMaXaToMaPhuongXa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NguoiDung_PhuongXa_MaXa",
                table: "NguoiDung");

            migrationBuilder.RenameColumn(
                name: "TenXa",
                table: "PhuongXa",
                newName: "TenPhuongXa");

            migrationBuilder.RenameColumn(
                name: "MaXa",
                table: "PhuongXa",
                newName: "MaPhuongXa");

            migrationBuilder.RenameColumn(
                name: "MaXa",
                table: "NguoiDung",
                newName: "MaPhuongXa");

            migrationBuilder.RenameIndex(
                name: "IX_NguoiDung_MaXa",
                table: "NguoiDung",
                newName: "IX_NguoiDung_MaPhuongXa");

            migrationBuilder.AddForeignKey(
                name: "FK_NguoiDung_PhuongXa_MaPhuongXa",
                table: "NguoiDung",
                column: "MaPhuongXa",
                principalTable: "PhuongXa",
                principalColumn: "MaPhuongXa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NguoiDung_PhuongXa_MaPhuongXa",
                table: "NguoiDung");

            migrationBuilder.RenameColumn(
                name: "TenPhuongXa",
                table: "PhuongXa",
                newName: "TenXa");

            migrationBuilder.RenameColumn(
                name: "MaPhuongXa",
                table: "PhuongXa",
                newName: "MaXa");

            migrationBuilder.RenameColumn(
                name: "MaPhuongXa",
                table: "NguoiDung",
                newName: "MaXa");

            migrationBuilder.RenameIndex(
                name: "IX_NguoiDung_MaPhuongXa",
                table: "NguoiDung",
                newName: "IX_NguoiDung_MaXa");

            migrationBuilder.AddForeignKey(
                name: "FK_NguoiDung_PhuongXa_MaXa",
                table: "NguoiDung",
                column: "MaXa",
                principalTable: "PhuongXa",
                principalColumn: "MaXa");
        }
    }
}
