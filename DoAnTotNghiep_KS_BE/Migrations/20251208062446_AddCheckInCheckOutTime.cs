using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnTotNghiep_KS_BE.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckInCheckOutTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ThoiGianCheckIn",
                table: "DatPhong",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ThoiGianCheckOut",
                table: "DatPhong",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThoiGianCheckIn",
                table: "DatPhong");

            migrationBuilder.DropColumn(
                name: "ThoiGianCheckOut",
                table: "DatPhong");
        }
    }
}
