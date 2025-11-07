using DoAnTotNghiep_KS_BE.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DoAnTotNghiep_KS_BE.Data
{
	public class MyDbContext : DbContext
	{
		public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
		{
		}

		// DbSets
		public DbSet<NguoiDung> NguoiDungs { get; set; }
		public DbSet<OTP> OTPs { get; set; }
		public DbSet<Tang> Tangs { get; set; }
		public DbSet<Phong> Phongs { get; set; }
		public DbSet<TienNghi> TienNghis { get; set; }
		public DbSet<Phong_TienNghi> Phong_TienNghis { get; set; }
		public DbSet<HinhAnhPhong> HinhAnhPhongs { get; set; }
		public DbSet<DatPhong> DatPhongs { get; set; }
		public DbSet<DatPhong_Phong> DatPhong_Phongs { get; set; }
		public DbSet<ThanhToan> ThanhToans { get; set; }
		public DbSet<HuyDatPhong> HuyDatPhongs { get; set; }
		public DbSet<HoanTien> HoanTiens { get; set; }
		public DbSet<DanhGia> DanhGias { get; set; }
		public DbSet<RefreshToken> RefreshTokens { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Cấu hình bảng RefreshToken
			modelBuilder.Entity<RefreshToken>(entity =>
			{
				entity.ToTable("RefreshToken"); // ← Thêm dòng này để đặt tên bảng

				entity.HasKey(e => e.MaRefreshToken);

				entity.Property(e => e.Token)
					.IsRequired()
					.HasMaxLength(500);

				entity.Property(e => e.DiaChi)
					.HasMaxLength(100);

				entity.HasOne(e => e.NguoiDung)
					.WithMany(n => n.RefreshTokens)
					.HasForeignKey(e => e.MaNguoiDung)
					.OnDelete(DeleteBehavior.Cascade);
			});

			// Cấu hình unique constraint cho Email
			modelBuilder.Entity<NguoiDung>()
				.HasIndex(u => u.Email)
				.IsUnique();

			// Cấu hình unique constraint cho SoPhong
			modelBuilder.Entity<Phong>()
				.HasIndex(p => p.SoPhong)
				.IsUnique();

			// Cấu hình bảng trung gian DatPhong_Phong
			modelBuilder.Entity<DatPhong_Phong>(entity =>
			{
				entity.HasKey(e => e.MaDatPhong_Phong);

				entity.HasOne(e => e.DatPhong)
					.WithMany(d => d.DatPhong_Phongs)
					.HasForeignKey(e => e.MaDatPhong)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasOne(e => e.Phong)
					.WithMany(p => p.DatPhong_Phongs)
					.HasForeignKey(e => e.MaPhong)
					.OnDelete(DeleteBehavior.Restrict);

				// Tạo unique constraint cho cặp MaDatPhong + MaPhong (tránh trùng)
				entity.HasIndex(e => new { e.MaDatPhong, e.MaPhong })
					.IsUnique();
			});

			// Cấu hình quan hệ HuyDatPhong với NguoiDung (tránh multiple cascade paths)
			modelBuilder.Entity<HuyDatPhong>()
				.HasOne(h => h.KhachHang)
				.WithMany(n => n.HuyDatPhongsKhachHang)
				.HasForeignKey(h => h.MaKhachHang)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<HuyDatPhong>()
				.HasOne(h => h.NhanVienDuyet)
				.WithMany(n => n.HuyDatPhongsNhanVien)
				.HasForeignKey(h => h.MaNhanVienDuyet)
				.OnDelete(DeleteBehavior.Restrict);

			// Cấu hình quan hệ HoanTien với NguoiDung
			modelBuilder.Entity<HoanTien>()
				.HasOne(h => h.QuanTri)
				.WithMany(n => n.HoanTiens)
				.HasForeignKey(h => h.MaQuanTri)
				.OnDelete(DeleteBehavior.Restrict);

			// Cấu hình quan hệ DatPhong với NguoiDung
			modelBuilder.Entity<DatPhong>()
				.HasOne(d => d.KhachHang)
				.WithMany(n => n.DatPhongs)
				.HasForeignKey(d => d.MaKhachHang)
				.OnDelete(DeleteBehavior.Restrict);

			// Cấu hình quan hệ DanhGia với NguoiDung
			modelBuilder.Entity<DanhGia>()
				.HasOne(d => d.KhachHang)
				.WithMany(n => n.DanhGias)
				.HasForeignKey(d => d.MaKhachHang)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
