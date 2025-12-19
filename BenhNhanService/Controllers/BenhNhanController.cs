using BenhNhanService.BLL.Interfaces;
using QuanLyBenhNhan.Models;
using Microsoft.AspNetCore.Mvc;

namespace BenhNhanService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BenhNhanController : ControllerBase
    {
        private IBenhNhanBusiness _bus;

        public BenhNhanController(IBenhNhanBusiness bus)
        {
            _bus = bus;
        }

        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(_bus.GetListBenhNhan());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] BenhNhan model)
        {
            try
            {
                var result = _bus.AddBenhNhan(model);
                return Ok(new { success = result, message = "Thêm thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}