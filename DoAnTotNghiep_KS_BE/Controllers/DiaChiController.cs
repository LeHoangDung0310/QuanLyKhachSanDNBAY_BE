using DoAnTotNghiep_KS_BE.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnTotNghiep_KS_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiaChiController : ControllerBase
    {
        private readonly MyDbContext _context;

        public DiaChiController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/DiaChi/Tinh
        [HttpGet("Tinh")]
        public async Task<ActionResult> GetTinhs()
        {
            var tinhs = await _context.Tinhs
                .OrderBy(t => t.TenTinh)
                .Select(t => new { t.MaTinh, t.TenTinh })
                .ToListAsync();

            return Ok(new { success = true, data = tinhs });
        }

        // GET: api/DiaChi/Huyen?maTinh=1
        [HttpGet("Huyen")]
        public async Task<ActionResult> GetHuyens([FromQuery] int maTinh)
        {
            var huyens = await _context.Huyens
                .Where(h => h.MaTinh == maTinh)
                .OrderBy(h => h.TenHuyen)
                .Select(h => new { h.MaHuyen, h.TenHuyen, h.MaTinh })
                .ToListAsync();

            return Ok(new { success = true, data = huyens });
        }

        // GET: api/DiaChi/PhuongXa?maHuyen=1
        [HttpGet("PhuongXa")]
        public async Task<ActionResult> GetPhuongXas([FromQuery] int maHuyen)
        {
            var phuongXas = await _context.PhuongXas
                .Where(x => x.MaHuyen == maHuyen)
                .OrderBy(x => x.TenPhuongXa)
                .Select(x => new { x.MaPhuongXa, x.TenPhuongXa, x.MaHuyen })
                .ToListAsync();

            return Ok(new { success = true, data = phuongXas });
        }
    }
}