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

        [HttpGet("doctors/{id}")]
        public ActionResult<DoctorDto> GetById([FromRoute] Guid id)
        {
            var doctor = _doctorBusiness.GetDoctorByID(id);
            if(doctor == null)
            {
                return NotFound();
            }
            return Ok(doctor);
        }
        [HttpPut("updateDoctors/{id}")]

        public ActionResult<DoctorUpdateDTO> Update([FromRoute] Guid id, [FromBody] DoctorUpdateDTO doctorUpdateDTO)
        {
            var doctor = _doctorBusiness.UpdateDTO(id, doctorUpdateDTO);
            if(doctor == null)
            {
                return NotFound();
            }
            return Ok(doctor);
        }

        [HttpPost("createDoctors")]
        public ActionResult<DoctorDto> Create([FromBody] DoctorDto doctorDTO)
        {
            var doctor = _doctorBusiness.CreateDoctor(doctorDTO);
            if(doctor == null)
            {
                return BadRequest();
            }
            return CreatedAtAction(nameof(GetById), new {ID = doctor.Id}, doctor);
        }
        [HttpDelete("doctors/{id}")]
        public ActionResult<bool> Delete(Guid id)
        {
            var doctor = _doctorBusiness.GetDoctorByID(id);
            if(doctor == null)
            {
                return NotFound();
            }
            _doctorBusiness.DeleteDoctor(id);
            return Ok("Xóa thành công");

        }

        [HttpPost("doctors/search")]
        public ActionResult<PagedResult<DoctorDto>> SearchDoctors([FromBody] SearchRequestDTO request)
        {
            var result = _doctorBusiness.SearchDoctors(request);
            return Ok(result);
        }
    }
}

