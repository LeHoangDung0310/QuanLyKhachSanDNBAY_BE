using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnTotNghiep_KS_BE.Migrations
{
    /// <inheritdoc />
    public partial class SettingHinhAnhLPhong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HinhAnhPhong");

            migrationBuilder.CreateTable(
                name: "HinhAnhLPhong",
                columns: table => new
                {
                    MaHinhAnh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLoaiPhong = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HinhAnhLPhong", x => x.MaHinhAnh);
                    table.ForeignKey(
                        name: "FK_HinhAnhLPhong_LoaiPhong_MaLoaiPhong",
                        column: x => x.MaLoaiPhong,
                        principalTable: "LoaiPhong",
                        principalColumn: "MaLoaiPhong",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HinhAnhLPhong_MaLoaiPhong",
                table: "HinhAnhLPhong",
                column: "MaLoaiPhong");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HinhAnhLPhong");

            migrationBuilder.CreateTable(
                name: "HinhAnhPhong",
                columns: table => new
                {
                    MaHinhAnh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLoaiPhong = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HinhAnhPhong", x => x.MaHinhAnh);
                    table.ForeignKey(
                        name: "FK_HinhAnhPhong_LoaiPhong_MaLoaiPhong",
                        column: x => x.MaLoaiPhong,
                        principalTable: "LoaiPhong",
                        principalColumn: "MaLoaiPhong",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HinhAnhPhong_MaLoaiPhong",
                table: "HinhAnhPhong",
                column: "MaLoaiPhong");
        }
    }
}
