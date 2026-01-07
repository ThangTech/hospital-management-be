using BacSiService.BLL.Interfaces;
using BacSiService.DTOs;
using BacSiService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BacSiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Yêu cầu đăng nhập cho tất cả endpoints
    public class BacSiController : ControllerBase
    {
        private readonly IDoctorBusiness _doctorBusiness;

        public BacSiController(IDoctorBusiness doctorBusiness)
        {
            _doctorBusiness = doctorBusiness;
        }

        /// <summary>
        /// Lấy danh sách tất cả bác sĩ
        /// Quyền: Admin, BacSi
        /// </summary>
        [HttpGet("doctors")]
        [Authorize(Roles = "Admin,BacSi")]
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

        /// <summary>
        /// Lấy thông tin bác sĩ theo ID
        /// Quyền: Admin, BacSi
        /// </summary>
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin,BacSi")]
        public ActionResult<ApiResponse<DoctorDto>> GetById(Guid id)
        {
            var doc = _doctorBusiness.GetDoctorByID(id);
            if (doc == null)
                return NotFound(new ApiResponse { Success = false, Message = "Not found" });

            return Ok(new ApiResponse<DoctorDto> { Success = true, Data = doc, Message = "OK" });
        }

        /// <summary>
        /// Thêm bác sĩ mới
        /// Quyền: Chỉ Admin
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<ApiResponse<DoctorDto>> Create(DoctorDto dto)
        {
            var doc = _doctorBusiness.CreateDoctor(dto);
            if (doc == null)
                return BadRequest(new ApiResponse { Success = false, Message = "Create failed" });

            return Ok(new ApiResponse<DoctorDto> { Success = true, Data = doc, Message = "Created" });
        }

        /// <summary>
        /// Cập nhật thông tin bác sĩ
        /// Quyền: Admin hoặc BacSi (sửa thông tin của mình)
        /// </summary>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin,BacSi")]
        public ActionResult<ApiResponse<DoctorUpdateDTO>> Update(Guid id, DoctorUpdateDTO dto)
        {
            var doc = _doctorBusiness.UpdateDTO(id, dto);
            if (doc == null)
                return BadRequest(new ApiResponse { Success = false, Message = "Update failed" });

            return Ok(new ApiResponse<DoctorUpdateDTO> { Success = true, Data = doc, Message = "Updated" });
        }

        /// <summary>
        /// Xóa bác sĩ
        /// Quyền: Chỉ Admin
        /// </summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<ApiResponse> Delete(Guid id)
        {
            var ok = _doctorBusiness.DeleteDoctor(id);
            return Ok(new ApiResponse { Success = ok, Message = ok ? "Deleted" : "Delete failed" });
        }

        /// <summary>
        /// Tìm kiếm bác sĩ
        /// Quyền: Admin, BacSi
        /// </summary>
        [HttpPost("search")]
        [Authorize(Roles = "Admin,BacSi")]
        public ActionResult<ApiResponse<PagedResult<DoctorDto>>> Search(SearchRequestDTO request)
        {
            var resultModel = _doctorBusiness.SearchDoctors(request);

            var dtoPaged = new PagedResult<DoctorDto>
            {
                Data = resultModel.Data.Select(d => new DoctorDto { Id = d.Id, HoTen = d.HoTen, ChuyenKhoa = d.ChuyenKhoa, ThongTinLienHe = d.ThongTinLienHe, KhoaId = d.KhoaId }).ToList(),
                PageNumber = resultModel.PageNumber,
                PageSize = resultModel.PageSize,
                TotalPages = resultModel.TotalPages,
                TotalRecords = resultModel.TotalRecords
            };

            return Ok(new ApiResponse<PagedResult<DoctorDto>> { Success = true, Data = dtoPaged, Message = "OK" });
        }
    }
}
