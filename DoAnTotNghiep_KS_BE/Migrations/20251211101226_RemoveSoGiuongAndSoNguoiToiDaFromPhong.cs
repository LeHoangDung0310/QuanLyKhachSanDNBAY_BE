using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnTotNghiep_KS_BE.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSoGiuongAndSoNguoiToiDaFromPhong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaiKhoanNganHang_NguoiDung_MaNguoiDung",
                table: "TaiKhoanNganHang");

            migrationBuilder.DropColumn(
                name: "SoGiuong",
                table: "Phong");

            migrationBuilder.DropColumn(
                name: "SoNguoiToiDa",
                table: "Phong");

            migrationBuilder.AddForeignKey(
                name: "FK_TaiKhoanNganHang_NguoiDung_MaNguoiDung",
                table: "TaiKhoanNganHang",
                column: "MaNguoiDung",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaiKhoanNganHang_NguoiDung_MaNguoiDung",
                table: "TaiKhoanNganHang");

            migrationBuilder.AddColumn<int>(
                name: "SoGiuong",
                table: "Phong",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SoNguoiToiDa",
                table: "Phong",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TaiKhoanNganHang_NguoiDung_MaNguoiDung",
                table: "TaiKhoanNganHang",
                column: "MaNguoiDung",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
