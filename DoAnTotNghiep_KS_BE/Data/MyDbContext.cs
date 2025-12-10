using Microsoft.EntityFrameworkCore;
using DoAnTotNghiep_KS_BE.Data.Entities;

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
		public DbSet<LoaiPhong> LoaiPhongs { get; set; }
		public DbSet<Phong> Phongs { get; set; }
		public DbSet<TienNghi> TienNghis { get; set; }
		public DbSet<Phong_TienNghi> Phong_TienNghis { get; set; }
		public DbSet<HinhAnhLPhong> HinhAnhLPhongs { get; set; }
		public DbSet<DatPhong> DatPhongs { get; set; }
		public DbSet<DatPhong_Phong> DatPhong_Phongs { get; set; }
		public DbSet<ThanhToan> ThanhToans { get; set; }
		public DbSet<HuyDatPhong> HuyDatPhongs { get; set; }
		public DbSet<HoanTien> HoanTiens { get; set; }
		public DbSet<DanhGia> DanhGias { get; set; }
		public DbSet<TaiKhoanNganHang> TaiKhoanNganHangs { get; set; }
		public DbSet<RefreshToken> RefreshTokens { get; set; }
		public DbSet<Tinh> Tinhs { get; set; }
		public DbSet<Huyen> Huyens { get; set; }
		public DbSet<PhuongXa> PhuongXas { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Cấu hình bảng RefreshToken
			modelBuilder.Entity<RefreshToken>(entity =>
			{
				entity.ToTable("RefreshToken");

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

				entity.HasIndex(e => new { e.MaDatPhong, e.MaPhong })
					.IsUnique();
			});

			// Cấu hình quan hệ HuyDatPhong với NguoiDung (NguoiDuyet)
			modelBuilder.Entity<HuyDatPhong>()
				.HasOne(h => h.NguoiDuyet)
				.WithMany(n => n.HuyDatPhongs_NguoiDuyet)
				.HasForeignKey(h => h.MaNguoiDuyet)
				.OnDelete(DeleteBehavior.Restrict);

			// Cấu hình quan hệ HuyDatPhong với DatPhong
			modelBuilder.Entity<HuyDatPhong>()
				.HasOne(h => h.DatPhong)
				.WithMany(d => d.HuyDatPhongs)
				.HasForeignKey(h => h.MaDatPhong)
				.OnDelete(DeleteBehavior.Restrict);

			// Cấu hình quan hệ 1-1 giữa HuyDatPhong và HoanTien
			modelBuilder.Entity<HuyDatPhong>()
				.HasOne(h => h.HoanTien)
				.WithOne(ht => ht.HuyDatPhong)
				.HasForeignKey<HoanTien>(ht => ht.MaHuyDatPhong)
				.OnDelete(DeleteBehavior.Restrict);

			// Cấu hình quan hệ HoanTien với NguoiDung (QuanTri)
			modelBuilder.Entity<HoanTien>()
				.HasOne(h => h.QuanTri)
				.WithMany(n => n.HoanTiens_QuanTri)
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

			// Cấu hình quan hệ DanhGia với LoaiPhong
			modelBuilder.Entity<DanhGia>()
				.HasOne(d => d.LoaiPhong)
				.WithMany(l => l.DanhGias)
				.HasForeignKey(d => d.MaLoaiPhong)
				.OnDelete(DeleteBehavior.Restrict);

			// Cấu hình quan hệ DanhGia với DatPhong
			modelBuilder.Entity<DanhGia>()
				.HasOne(d => d.DatPhong)
				.WithMany(dp => dp.DanhGias)
				.HasForeignKey(d => d.MaDatPhong)
				.OnDelete(DeleteBehavior.Restrict);

			// Cấu hình quan hệ HinhAnhLPhong với LoaiPhong
			modelBuilder.Entity<HinhAnhLPhong>()
				.HasOne(h => h.LoaiPhong)
				.WithMany(l => l.HinhAnhLPhongs)
				.HasForeignKey(h => h.MaLoaiPhong)
				.OnDelete(DeleteBehavior.Restrict);

			// Cấu hình quan hệ Phong với LoaiPhong
			modelBuilder.Entity<Phong>()
				.HasOne(p => p.LoaiPhong)
				.WithMany(l => l.Phongs)
				.HasForeignKey(p => p.MaLoaiPhong)
				.OnDelete(DeleteBehavior.Restrict);

			// Cấu hình quan hệ TaiKhoanNganHang với NguoiDung
			modelBuilder.Entity<TaiKhoanNganHang>()
				.HasOne(t => t.NguoiDung)
				.WithMany(n => n.TaiKhoanNganHangs)
				.HasForeignKey(t => t.MaNguoiDung)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
