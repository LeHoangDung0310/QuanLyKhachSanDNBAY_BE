using DoAnTotNghiep_KS_BE.Data;
using DoAnTotNghiep_KS_BE.Data.Entities;
using DoAnTotNghiep_KS_BE.Interfaces.dto.ThanhToan;
using DoAnTotNghiep_KS_BE.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DoAnTotNghiep_KS_BE.Interfaces.Repositories
{
    public class ThanhToanRepository : IThanhToanRepository
    {
        private readonly MyDbContext _context;

        public ThanhToanRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<(bool success, string message, ThanhToanResponseDTO? data)> CreateThanhToanAsync(
            CreateThanhToanDTO createDTO,
            int nguoiThucHien)
        {
            // 1. Kiểm tra đặt phòng tồn tại
            var datPhong = await _context.DatPhongs
                .Include(dp => dp.DatPhong_Phongs)
                    .ThenInclude(dpp => dpp.Phong)
                    .ThenInclude(p => p!.LoaiPhong)
                .Include(dp => dp.ThanhToans)
                .FirstOrDefaultAsync(dp => dp.MaDatPhong == createDTO.MaDatPhong);

            if (datPhong == null)
            {
                return (false, "Đặt phòng không tồn tại", null);
            }

            // 2. Kiểm tra trạng thái đặt phòng
            if (datPhong.TrangThai == "DaHuy")
            {
                return (false, "Không thể thanh toán cho đặt phòng đã hủy", null);
            }

            if (datPhong.TrangThai == "HoanThanh")
            {
                return (false, "Đặt phòng đã hoàn thành", null);
            }

            // 3. Tính tổng tiền
            var soNgayO = (datPhong.NgayTraPhong - datPhong.NgayNhanPhong).Days;
            var tongTien = datPhong.DatPhong_Phongs?.Sum(dpp =>
                (dpp.Phong?.LoaiPhong?.GiaMoiDem ?? 0) * soNgayO) ?? 0;

            // 4. Tính số tiền đã thanh toán
            var daThanhToan = datPhong.ThanhToans?
                .Where(tt => tt.TrangThai == "ThanhCong")
                .Sum(tt => tt.SoTien ?? 0) ?? 0;

            var conLai = tongTien - daThanhToan;

            // 5. Validate số tiền thanh toán
            if (createDTO.SoTien <= 0)
            {
                return (false, "Số tiền thanh toán phải lớn hơn 0", null);
            }

            if (createDTO.SoTien > conLai)
            {
                return (false, $"Số tiền thanh toán ({createDTO.SoTien:N0}đ) vượt quá số tiền còn lại ({conLai:N0}đ)", null);
            }

            // 6. Validate phương thức thanh toán
            var phuongThucHopLe = new[] { "TienMat", "ChuyenKhoan", "TheATM", "MoMo", "ZaloPay", "VNPay" };
            if (!phuongThucHopLe.Contains(createDTO.PhuongThuc))
            {
                return (false, "Phương thức thanh toán không hợp lệ", null);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 7. Tạo thanh toán
                var thanhToan = new ThanhToan
                {
                    MaDatPhong = createDTO.MaDatPhong,
                    SoTien = createDTO.SoTien,
                    PhuongThuc = createDTO.PhuongThuc,
                    ThoiGian = DateTime.Now,
                    TrangThai = createDTO.PhuongThuc == "TienMat" ? "ThanhCong" : "DangCho", // Tiền mặt = thanh toán ngay
                    NgayTao = DateTime.Now
                };

                _context.ThanhToans.Add(thanhToan);
                await _context.SaveChangesAsync();

                // 8. Cập nhật trạng thái đặt phòng nếu thanh toán đủ
                var tongDaThanhToan = daThanhToan + createDTO.SoTien;
                if (tongDaThanhToan >= tongTien && datPhong.TrangThai == "ChoDuyet")
                {
                    datPhong.TrangThai = "DaDuyet";
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                var response = new ThanhToanResponseDTO
                {
                    MaThanhToan = thanhToan.MaThanhToan,
                    MaDatPhong = createDTO.MaDatPhong,
                    SoTienThanhToan = createDTO.SoTien,
                    TongTienDatPhong = tongTien,
                    DaThanhToan = tongDaThanhToan,
                    ConLai = tongTien - tongDaThanhToan,
                    TrangThai = thanhToan.TrangThai,
                    Message = thanhToan.TrangThai == "ThanhCong"
                        ? "Thanh toán thành công!"
                        : "Đang chờ xác nhận thanh toán"
                };

                return (true, response.Message!, response);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Lỗi khi tạo thanh toán: {ex.Message}");
                return (false, "Có lỗi xảy ra khi thanh toán", null);
            }
        }

        public async Task<ThongTinThanhToanDTO?> GetThongTinThanhToanAsync(int maDatPhong)
        {
            var datPhong = await _context.DatPhongs
                .Include(dp => dp.DatPhong_Phongs)
                    .ThenInclude(dpp => dpp.Phong)
                    .ThenInclude(p => p!.LoaiPhong)
                .Include(dp => dp.ThanhToans)
                .FirstOrDefaultAsync(dp => dp.MaDatPhong == maDatPhong);

            if (datPhong == null)
            {
                return null;
            }

            var soNgayO = (datPhong.NgayTraPhong - datPhong.NgayNhanPhong).Days;
            var tongTien = datPhong.DatPhong_Phongs?.Sum(dpp =>
                (dpp.Phong?.LoaiPhong?.GiaMoiDem ?? 0) * soNgayO) ?? 0;

            var daThanhToan = datPhong.ThanhToans?
                .Where(tt => tt.TrangThai == "ThanhCong")
                .Sum(tt => tt.SoTien ?? 0) ?? 0;

            var danhSachThanhToan = datPhong.ThanhToans?
                .OrderByDescending(tt => tt.ThoiGian)
                .Select(tt => new ThanhToanDTO
                {
                    MaThanhToan = tt.MaThanhToan,
                    MaDatPhong = tt.MaDatPhong,
                    SoTien = tt.SoTien,
                    PhuongThuc = tt.PhuongThuc,
                    TrangThai = tt.TrangThai,
                    ThoiGian = tt.ThoiGian,
                    NgayTao = tt.NgayTao
                })
                .ToList() ?? new List<ThanhToanDTO>();

            return new ThongTinThanhToanDTO
            {
                MaDatPhong = maDatPhong,
                TongTien = tongTien,
                DaThanhToan = daThanhToan,
                ConLai = tongTien - daThanhToan,
                DanhSachThanhToan = danhSachThanhToan
            };
        }

        public async Task<List<ThanhToanDTO>> GetLichSuThanhToanAsync(int maDatPhong)
        {
            var thanhToans = await _context.ThanhToans
                .Where(tt => tt.MaDatPhong == maDatPhong)
                .OrderByDescending(tt => tt.ThoiGian)
                .Select(tt => new ThanhToanDTO
                {
                    MaThanhToan = tt.MaThanhToan,
                    MaDatPhong = tt.MaDatPhong,
                    SoTien = tt.SoTien,
                    PhuongThuc = tt.PhuongThuc,
                    TrangThai = tt.TrangThai,
                    ThoiGian = tt.ThoiGian,
                    NgayTao = tt.NgayTao
                })
                .ToListAsync();

            return thanhToans;
        }

        public async Task<(bool success, string message)> XacNhanThanhToanOnlineAsync(int maThanhToan, string transactionId)
        {
            var thanhToan = await _context.ThanhToans
                .Include(tt => tt.DatPhong)
                .FirstOrDefaultAsync(tt => tt.MaThanhToan == maThanhToan);

            if (thanhToan == null)
            {
                return (false, "Thanh toán không tồn tại");
            }

            if (thanhToan.TrangThai != "DangCho")
            {
                return (false, $"Trạng thái thanh toán hiện tại: {thanhToan.TrangThai}");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Cập nhật trạng thái thanh toán
                thanhToan.TrangThai = "ThanhCong";
                thanhToan.ThoiGian = DateTime.Now;

                // Kiểm tra nếu thanh toán đủ → Chuyển trạng thái đặt phòng
                if (thanhToan.DatPhong != null && thanhToan.DatPhong.TrangThai == "ChoDuyet")
                {
                    var datPhong = await _context.DatPhongs
                        .Include(dp => dp.DatPhong_Phongs)
                            .ThenInclude(dpp => dpp.Phong)
                            .ThenInclude(p => p!.LoaiPhong)
                        .Include(dp => dp.ThanhToans)
                        .FirstOrDefaultAsync(dp => dp.MaDatPhong == thanhToan.MaDatPhong);

                    if (datPhong != null)
                    {
                        var soNgayO = (datPhong.NgayTraPhong - datPhong.NgayNhanPhong).Days;
                        var tongTien = datPhong.DatPhong_Phongs?.Sum(dpp =>
                            (dpp.Phong?.LoaiPhong?.GiaMoiDem ?? 0) * soNgayO) ?? 0;

                        var daThanhToan = datPhong.ThanhToans?
                            .Where(tt => tt.TrangThai == "ThanhCong")
                            .Sum(tt => tt.SoTien ?? 0) ?? 0;

                        if (daThanhToan >= tongTien)
                        {
                            datPhong.TrangThai = "DaDuyet";
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, "Xác nhận thanh toán thành công");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Lỗi khi xác nhận thanh toán: {ex.Message}");
                return (false, "Có lỗi xảy ra khi xác nhận thanh toán");
            }
        }

        public async Task<(bool success, string message)> HuyThanhToanAsync(int maThanhToan)
        {
            var thanhToan = await _context.ThanhToans.FindAsync(maThanhToan);

            if (thanhToan == null)
            {
                return (false, "Thanh toán không tồn tại");
            }

            if (thanhToan.TrangThai == "ThanhCong")
            {
                return (false, "Không thể hủy thanh toán đã thành công");
            }

            thanhToan.TrangThai = "DaHuy";
            await _context.SaveChangesAsync();

            return (true, "Hủy thanh toán thành công");
        }
    }
}