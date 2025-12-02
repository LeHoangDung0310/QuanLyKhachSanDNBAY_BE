using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnTotNghiep_KS_BE.Migrations
{
    /// <inheritdoc />
    public partial class Thementities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhGia_Phong_MaPhong",
                table: "DanhGia");

            migrationBuilder.DropForeignKey(
                name: "FK_HinhAnhPhong_Phong_MaPhong",
                table: "HinhAnhPhong");

            migrationBuilder.DropForeignKey(
                name: "FK_HoanTien_HuyDatPhong_MaHuyDatPhong",
                table: "HoanTien");

            migrationBuilder.DropForeignKey(
                name: "FK_HuyDatPhong_DatPhong_MaDatPhong",
                table: "HuyDatPhong");

            migrationBuilder.DropForeignKey(
                name: "FK_HuyDatPhong_NguoiDung_MaKhachHang",
                table: "HuyDatPhong");

            migrationBuilder.DropIndex(
                name: "IX_HuyDatPhong_MaKhachHang",
                table: "HuyDatPhong");

            migrationBuilder.DropIndex(
                name: "IX_HoanTien_MaHuyDatPhong",
                table: "HoanTien");

            migrationBuilder.DropColumn(
                name: "GiaMoiDem",
                table: "Phong");

            migrationBuilder.DropColumn(
                name: "HuongNhin",
                table: "Phong");

            migrationBuilder.DropColumn(
                name: "TenLoai",
                table: "Phong");

            migrationBuilder.DropColumn(
                name: "MaKhachHang",
                table: "HuyDatPhong");

            migrationBuilder.DropColumn(
                name: "SoTaiKhoan",
                table: "HuyDatPhong");

            migrationBuilder.DropColumn(
                name: "TenChuTaiKhoan",
                table: "HuyDatPhong");

            migrationBuilder.DropColumn(
                name: "TenNganHang",
                table: "HuyDatPhong");

            migrationBuilder.RenameColumn(
                name: "DienTich",
                table: "Phong",
                newName: "MaLoaiPhong");

            migrationBuilder.RenameColumn(
                name: "GhiChuNguoiDuyet",
                table: "HuyDatPhong",
                newName: "GhiChu");

            migrationBuilder.RenameColumn(
                name: "MaPhong",
                table: "HinhAnhPhong",
                newName: "MaLoaiPhong");

            migrationBuilder.RenameIndex(
                name: "IX_HinhAnhPhong_MaPhong",
                table: "HinhAnhPhong",
                newName: "IX_HinhAnhPhong_MaLoaiPhong");

            migrationBuilder.RenameColumn(
                name: "MaPhong",
                table: "DanhGia",
                newName: "MaLoaiPhong");

            migrationBuilder.RenameIndex(
                name: "IX_DanhGia_MaPhong",
                table: "DanhGia",
                newName: "IX_DanhGia_MaLoaiPhong");

            migrationBuilder.AddColumn<string>(
                name: "GioiTinh",
                table: "NguoiDung",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayCapCCCD",
                table: "NguoiDung",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgaySinh",
                table: "NguoiDung",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NoiCapCCCD",
                table: "NguoiDung",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SoCCCD",
                table: "NguoiDung",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaDatPhong",
                table: "DanhGia",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LoaiPhong",
                columns: table => new
                {
                    MaLoaiPhong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoaiPhong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SoNguoiToiDa = table.Column<int>(type: "int", nullable: true),
                    SoGiuong = table.Column<int>(type: "int", nullable: true),
                    DienTich = table.Column<int>(type: "int", nullable: true),
                    GiaMoiDem = table.Column<decimal>(type: "decimal(12,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiPhong", x => x.MaLoaiPhong);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoanNganHang",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    NganHang = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SoTaiKhoan = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    TenChuTK = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoanNganHang", x => x.MaTaiKhoan);
                    table.ForeignKey(
                        name: "FK_TaiKhoanNganHang_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Phong_MaLoaiPhong",
                table: "Phong",
                column: "MaLoaiPhong");

            migrationBuilder.CreateIndex(
                name: "IX_HoanTien_MaHuyDatPhong",
                table: "HoanTien",
                column: "MaHuyDatPhong",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DanhGia_MaDatPhong",
                table: "DanhGia",
                column: "MaDatPhong");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoanNganHang_MaNguoiDung",
                table: "TaiKhoanNganHang",
                column: "MaNguoiDung");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhGia_DatPhong_MaDatPhong",
                table: "DanhGia",
                column: "MaDatPhong",
                principalTable: "DatPhong",
                principalColumn: "MaDatPhong",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DanhGia_LoaiPhong_MaLoaiPhong",
                table: "DanhGia",
                column: "MaLoaiPhong",
                principalTable: "LoaiPhong",
                principalColumn: "MaLoaiPhong",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HinhAnhPhong_LoaiPhong_MaLoaiPhong",
                table: "HinhAnhPhong",
                column: "MaLoaiPhong",
                principalTable: "LoaiPhong",
                principalColumn: "MaLoaiPhong",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HoanTien_HuyDatPhong_MaHuyDatPhong",
                table: "HoanTien",
                column: "MaHuyDatPhong",
                principalTable: "HuyDatPhong",
                principalColumn: "MaHuyDatPhong",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HuyDatPhong_DatPhong_MaDatPhong",
                table: "HuyDatPhong",
                column: "MaDatPhong",
                principalTable: "DatPhong",
                principalColumn: "MaDatPhong",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Phong_LoaiPhong_MaLoaiPhong",
                table: "Phong",
                column: "MaLoaiPhong",
                principalTable: "LoaiPhong",
                principalColumn: "MaLoaiPhong",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhGia_DatPhong_MaDatPhong",
                table: "DanhGia");

            migrationBuilder.DropForeignKey(
                name: "FK_DanhGia_LoaiPhong_MaLoaiPhong",
                table: "DanhGia");

            migrationBuilder.DropForeignKey(
                name: "FK_HinhAnhPhong_LoaiPhong_MaLoaiPhong",
                table: "HinhAnhPhong");

            migrationBuilder.DropForeignKey(
                name: "FK_HoanTien_HuyDatPhong_MaHuyDatPhong",
                table: "HoanTien");

            migrationBuilder.DropForeignKey(
                name: "FK_HuyDatPhong_DatPhong_MaDatPhong",
                table: "HuyDatPhong");

            migrationBuilder.DropForeignKey(
                name: "FK_Phong_LoaiPhong_MaLoaiPhong",
                table: "Phong");

            migrationBuilder.DropTable(
                name: "LoaiPhong");

            migrationBuilder.DropTable(
                name: "TaiKhoanNganHang");

            migrationBuilder.DropIndex(
                name: "IX_Phong_MaLoaiPhong",
                table: "Phong");

            migrationBuilder.DropIndex(
                name: "IX_HoanTien_MaHuyDatPhong",
                table: "HoanTien");

            migrationBuilder.DropIndex(
                name: "IX_DanhGia_MaDatPhong",
                table: "DanhGia");

            migrationBuilder.DropColumn(
                name: "GioiTinh",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "NgayCapCCCD",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "NgaySinh",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "NoiCapCCCD",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "SoCCCD",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "MaDatPhong",
                table: "DanhGia");

            migrationBuilder.RenameColumn(
                name: "MaLoaiPhong",
                table: "Phong",
                newName: "DienTich");

            migrationBuilder.RenameColumn(
                name: "GhiChu",
                table: "HuyDatPhong",
                newName: "GhiChuNguoiDuyet");

            migrationBuilder.RenameColumn(
                name: "MaLoaiPhong",
                table: "HinhAnhPhong",
                newName: "MaPhong");

            migrationBuilder.RenameIndex(
                name: "IX_HinhAnhPhong_MaLoaiPhong",
                table: "HinhAnhPhong",
                newName: "IX_HinhAnhPhong_MaPhong");

            migrationBuilder.RenameColumn(
                name: "MaLoaiPhong",
                table: "DanhGia",
                newName: "MaPhong");

            migrationBuilder.RenameIndex(
                name: "IX_DanhGia_MaLoaiPhong",
                table: "DanhGia",
                newName: "IX_DanhGia_MaPhong");

            migrationBuilder.AddColumn<decimal>(
                name: "GiaMoiDem",
                table: "Phong",
                type: "decimal(12,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HuongNhin",
                table: "Phong",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenLoai",
                table: "Phong",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaKhachHang",
                table: "HuyDatPhong",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SoTaiKhoan",
                table: "HuyDatPhong",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenChuTaiKhoan",
                table: "HuyDatPhong",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenNganHang",
                table: "HuyDatPhong",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HuyDatPhong_MaKhachHang",
                table: "HuyDatPhong",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_HoanTien_MaHuyDatPhong",
                table: "HoanTien",
                column: "MaHuyDatPhong");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhGia_Phong_MaPhong",
                table: "DanhGia",
                column: "MaPhong",
                principalTable: "Phong",
                principalColumn: "MaPhong");

            migrationBuilder.AddForeignKey(
                name: "FK_HinhAnhPhong_Phong_MaPhong",
                table: "HinhAnhPhong",
                column: "MaPhong",
                principalTable: "Phong",
                principalColumn: "MaPhong",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HoanTien_HuyDatPhong_MaHuyDatPhong",
                table: "HoanTien",
                column: "MaHuyDatPhong",
                principalTable: "HuyDatPhong",
                principalColumn: "MaHuyDatPhong",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HuyDatPhong_DatPhong_MaDatPhong",
                table: "HuyDatPhong",
                column: "MaDatPhong",
                principalTable: "DatPhong",
                principalColumn: "MaDatPhong",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HuyDatPhong_NguoiDung_MaKhachHang",
                table: "HuyDatPhong",
                column: "MaKhachHang",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
