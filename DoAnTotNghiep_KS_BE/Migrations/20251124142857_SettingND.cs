using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnTotNghiep_KS_BE.Migrations
{
    /// <inheritdoc />
    public partial class SettingND : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HuyDatPhong_NguoiDung_MaNhanVienDuyet",
                table: "HuyDatPhong");

            migrationBuilder.RenameColumn(
                name: "MaNhanVienDuyet",
                table: "HuyDatPhong",
                newName: "MaNguoiDuyet");

            migrationBuilder.RenameColumn(
                name: "GhiChuLeTan",
                table: "HuyDatPhong",
                newName: "GhiChuNguoiDuyet");

            migrationBuilder.RenameIndex(
                name: "IX_HuyDatPhong_MaNhanVienDuyet",
                table: "HuyDatPhong",
                newName: "IX_HuyDatPhong_MaNguoiDuyet");

            migrationBuilder.AddForeignKey(
                name: "FK_HuyDatPhong_NguoiDung_MaNguoiDuyet",
                table: "HuyDatPhong",
                column: "MaNguoiDuyet",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HuyDatPhong_NguoiDung_MaNguoiDuyet",
                table: "HuyDatPhong");

            migrationBuilder.RenameColumn(
                name: "MaNguoiDuyet",
                table: "HuyDatPhong",
                newName: "MaNhanVienDuyet");

            migrationBuilder.RenameColumn(
                name: "GhiChuNguoiDuyet",
                table: "HuyDatPhong",
                newName: "GhiChuLeTan");

            migrationBuilder.RenameIndex(
                name: "IX_HuyDatPhong_MaNguoiDuyet",
                table: "HuyDatPhong",
                newName: "IX_HuyDatPhong_MaNhanVienDuyet");

            migrationBuilder.AddForeignKey(
                name: "FK_HuyDatPhong_NguoiDung_MaNhanVienDuyet",
                table: "HuyDatPhong",
                column: "MaNhanVienDuyet",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
