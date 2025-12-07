using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyBenhNhan.Models;

namespace QuanLyBenhNhan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BenhNhanController : ControllerBase
    {
        private readonly HospitalManageContext context;

        public BenhNhanController(HospitalManageContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public IActionResult GetAllBenhNhan()
        {
            var benhnhan = context.BenhNhans.ToList();
            return Ok(benhnhan);
        }

        [HttpGet]
        [Route("{id:guid}")]

        public IActionResult GetBenhNhan() { 
        
            var benhnhan = context.BenhNhans.FirstOrDefault();
            return Ok(benhnhan);
        
        }
    }
}
