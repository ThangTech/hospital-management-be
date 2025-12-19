using BacSiService.BLL.Interfaces;
using BacSiService.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BacSiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BacSiController : ControllerBase
    {
        private readonly IDoctorBusiness _doctorBusiness;

        public BacSiController(IDoctorBusiness doctorBusiness)
        {
            _doctorBusiness = doctorBusiness;
        }

        [HttpGet("doctors")]
        public ActionResult<IEnumerable<DoctorDto>> GetAll()
        {
            var doctors = _doctorBusiness.GetAllDtos();
            return Ok(doctors);
        }
    }
}
