using BacSiService.BLL.Interfaces;
using BacSiService.DTOs;
using BacSiService.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

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

        // LIST - explicit path to avoid collision with GET by id
        // GET api/bacsi/doctors
        [HttpGet("doctors")]
        public ActionResult<ApiResponse<IEnumerable<DoctorDto>>> GetAll()
        {
            var dtos = _doctorBusiness.GetAllDtos();
            return Ok(new ApiResponse<IEnumerable<DoctorDto>>
            {
                Success = true,
                Data = dtos,
                Message = "OK"
            });
        }

        // GET by GUID id - route constraint prevents non-GUID strings being bound
        // GET api/bacsi/{id}
        [HttpGet("{id:guid}")]
        public ActionResult<ApiResponse<DoctorDto>> GetById(Guid id)
        {
            var doc = _doctorBusiness.GetDoctorByID(id);
            if (doc == null)
                return NotFound(new ApiResponse { Success = false, Message = "Not found" });

            return Ok(new ApiResponse<DoctorDto> { Success = true, Data = doc, Message = "OK" });
        }

        // CREATE
        // POST api/bacsi
        [HttpPost]
        public ActionResult<ApiResponse<DoctorDto>> Create(DoctorDto dto)
        {
            var doc = _doctorBusiness.CreateDoctor(dto);
            if (doc == null)
                return BadRequest(new ApiResponse { Success = false, Message = "Create failed" });

            return Ok(new ApiResponse<DoctorDto> { Success = true, Data = doc, Message = "Created" });
        }

        // UPDATE
        // PUT api/bacsi/{id}
        [HttpPut("{id:guid}")]
        public ActionResult<ApiResponse<DoctorUpdateDTO>> Update(Guid id, DoctorUpdateDTO dto)
        {
            var doc = _doctorBusiness.UpdateDTO(id, dto);
            if (doc == null)
                return BadRequest(new ApiResponse { Success = false, Message = "Update failed" });

            return Ok(new ApiResponse<DoctorUpdateDTO> { Success = true, Data = doc, Message = "Updated" });
        }

        // DELETE
        // DELETE api/bacsi/{id}
        [HttpDelete("{id:guid}")]
        public ActionResult<ApiResponse> Delete(Guid id)
        {
            var ok = _doctorBusiness.DeleteDoctor(id);
            return Ok(new ApiResponse { Success = ok, Message = ok ? "Deleted" : "Delete failed" });
        }

        // SEARCH doctors - prefer POST
        // POST api/bacsi/search
        [HttpPost("search")]
        public ActionResult<ApiResponse<PagedResult<DoctorDto>>> Search(SearchRequestDTO request)
        {
            var resultModel = _doctorBusiness.SearchDoctors(request);

            var dtoPaged = new PagedResult<DoctorDto>
            {
                Data = resultModel.Data.Select(d => new DoctorDto { Id = d.Id, HoTen = d.HoTen, ChuyenKhoa = d.ChuyenKhoa, ThongTinLienHe = d.ThongTinLienHe }).ToList(),
                PageNumber = resultModel.PageNumber,
                PageSize = resultModel.PageSize,
                TotalPages = resultModel.TotalPages,
                TotalRecords = resultModel.TotalRecords
            };

            return Ok(new ApiResponse<PagedResult<DoctorDto>> { Success = true, Data = dtoPaged, Message = "OK" });
        }
    }
}

