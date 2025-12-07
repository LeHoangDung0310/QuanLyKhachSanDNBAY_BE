using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnTotNghiep_KS_BE.Migrations
{
    /// <inheritdoc />
    public partial class AddNguoiTaoToDatPhong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LoaiDatPhong",
                table: "DatPhong",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MaNguoiTao",
                table: "DatPhong",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DatPhong_MaNguoiTao",
                table: "DatPhong",
                column: "MaNguoiTao");

            migrationBuilder.AddForeignKey(
                name: "FK_DatPhong_NguoiDung_MaNguoiTao",
                table: "DatPhong",
                column: "MaNguoiTao",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatPhong_NguoiDung_MaNguoiTao",
                table: "DatPhong");

            migrationBuilder.DropIndex(
                name: "IX_DatPhong_MaNguoiTao",
                table: "DatPhong");

            migrationBuilder.DropColumn(
                name: "LoaiDatPhong",
                table: "DatPhong");

            migrationBuilder.DropColumn(
                name: "MaNguoiTao",
                table: "DatPhong");
        }
    }
}
