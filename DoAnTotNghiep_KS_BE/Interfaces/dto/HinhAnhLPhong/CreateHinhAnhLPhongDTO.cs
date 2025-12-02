using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep_KS_BE.Interfaces.dto.HinhAnhLPhong
{
    public class CreateHinhAnhLPhongDTO
    {
        [Required(ErrorMessage = "Mã loại phòng không được để trống")]
        public int MaLoaiPhong { get; set; }

        [Required(ErrorMessage = "File hình ảnh không được để trống")]
        public IFormFile File { get; set; } = null!;
    }
}