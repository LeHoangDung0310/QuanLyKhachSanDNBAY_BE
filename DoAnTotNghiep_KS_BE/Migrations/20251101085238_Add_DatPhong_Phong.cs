using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnTotNghiep_KS_BE.Migrations
{
    /// <inheritdoc />
    public partial class Add_DatPhong_Phong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatPhong_Phong_MaPhong",
                table: "DatPhong");

            migrationBuilder.DropIndex(
                name: "IX_DatPhong_MaPhong",
                table: "DatPhong");

            migrationBuilder.DropColumn(
                name: "NganHang",
                table: "HoanTien");

            migrationBuilder.DropColumn(
                name: "SoTaiKhoan",
                table: "HoanTien");

            migrationBuilder.DropColumn(
                name: "SoTienHoan",
                table: "HoanTien");

            migrationBuilder.DropColumn(
                name: "TenChuTaiKhoan",
                table: "HoanTien");

            migrationBuilder.DropColumn(
                name: "MaPhong",
                table: "DatPhong");

            migrationBuilder.DropColumn(
                name: "TongTien",
                table: "DatPhong");

            migrationBuilder.AddColumn<int>(
                name: "SoNguoiToiDa",
                table: "Phong",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DatPhong_Phong",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDatPhong = table.Column<int>(type: "int", nullable: false),
                    MaPhong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatPhong_Phong", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DatPhong_Phong_DatPhong_MaDatPhong",
                        column: x => x.MaDatPhong,
                        principalTable: "DatPhong",
                        principalColumn: "MaDatPhong",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DatPhong_Phong_Phong_MaPhong",
                        column: x => x.MaPhong,
                        principalTable: "Phong",
                        principalColumn: "MaPhong",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DatPhong_Phong_MaDatPhong",
                table: "DatPhong_Phong",
                column: "MaDatPhong");

            migrationBuilder.CreateIndex(
                name: "IX_DatPhong_Phong_MaPhong",
                table: "DatPhong_Phong",
                column: "MaPhong");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatPhong_Phong");

            migrationBuilder.DropColumn(
                name: "SoNguoiToiDa",
                table: "Phong");

            migrationBuilder.AddColumn<string>(
                name: "NganHang",
                table: "HoanTien",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SoTaiKhoan",
                table: "HoanTien",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SoTienHoan",
                table: "HoanTien",
                type: "decimal(12,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenChuTaiKhoan",
                table: "HoanTien",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaPhong",
                table: "DatPhong",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TongTien",
                table: "DatPhong",
                type: "decimal(12,2)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DatPhong_MaPhong",
                table: "DatPhong",
                column: "MaPhong");

            migrationBuilder.AddForeignKey(
                name: "FK_DatPhong_Phong_MaPhong",
                table: "DatPhong",
                column: "MaPhong",
                principalTable: "Phong",
                principalColumn: "MaPhong",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
