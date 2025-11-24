using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnTotNghiep_KS_BE.Migrations
{
    /// <inheritdoc />
    public partial class TinhHuyenPhuongXa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiaChi",
                table: "NguoiDung",
                newName: "DiaChiChiTiet");

            migrationBuilder.AddColumn<int>(
                name: "MaXa",
                table: "NguoiDung",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tinh",
                columns: table => new
                {
                    MaTinh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenTinh = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tinh", x => x.MaTinh);
                });

            migrationBuilder.CreateTable(
                name: "Huyen",
                columns: table => new
                {
                    MaHuyen = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenHuyen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaTinh = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Huyen", x => x.MaHuyen);
                    table.ForeignKey(
                        name: "FK_Huyen_Tinh_MaTinh",
                        column: x => x.MaTinh,
                        principalTable: "Tinh",
                        principalColumn: "MaTinh",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhuongXa",
                columns: table => new
                {
                    MaXa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenXa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaHuyen = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhuongXa", x => x.MaXa);
                    table.ForeignKey(
                        name: "FK_PhuongXa_Huyen_MaHuyen",
                        column: x => x.MaHuyen,
                        principalTable: "Huyen",
                        principalColumn: "MaHuyen",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_MaXa",
                table: "NguoiDung",
                column: "MaXa");

            migrationBuilder.CreateIndex(
                name: "IX_Huyen_MaTinh",
                table: "Huyen",
                column: "MaTinh");

            migrationBuilder.CreateIndex(
                name: "IX_PhuongXa_MaHuyen",
                table: "PhuongXa",
                column: "MaHuyen");

            migrationBuilder.AddForeignKey(
                name: "FK_NguoiDung_PhuongXa_MaXa",
                table: "NguoiDung",
                column: "MaXa",
                principalTable: "PhuongXa",
                principalColumn: "MaXa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NguoiDung_PhuongXa_MaXa",
                table: "NguoiDung");

            migrationBuilder.DropTable(
                name: "PhuongXa");

            migrationBuilder.DropTable(
                name: "Huyen");

            migrationBuilder.DropTable(
                name: "Tinh");

            migrationBuilder.DropIndex(
                name: "IX_NguoiDung_MaXa",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "MaXa",
                table: "NguoiDung");

            migrationBuilder.RenameColumn(
                name: "DiaChiChiTiet",
                table: "NguoiDung",
                newName: "DiaChi");
        }
    }
}
