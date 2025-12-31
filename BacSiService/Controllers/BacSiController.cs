using BacSiService.BLL.Interfaces;
using BacSiService.DTOs;
using BacSiService.Models;
using Microsoft.AspNetCore.Mvc;
using System;
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

        // ===== BÁC SĨ =====
        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<DoctorDto>>> GetAll()
        {
            var dtos = _doctorBusiness.GetAllDtos();
            return Ok(new ApiResponse<IEnumerable<DoctorDto>> { Success = true, Data = dtos, Message = "OK" });
        }

        [HttpGet("{id}")]
        public ActionResult<ApiResponse<BacSi>> GetById(Guid id)
        {
            var doc = _doctorBusiness.GetById(id);
            if (doc == null) return NotFound(new ApiResponse { Success = false, Message = "Not found" });
            return Ok(new ApiResponse<BacSi> { Success = true, Data = doc, Message = "OK" });
        }

        [HttpPost]
        public ActionResult<ApiResponse<BacSi>> Create(DoctorDto dto)
        {
            var doc = _doctorBusiness.CreateDoctor(dto);
            if (doc == null) return BadRequest(new ApiResponse { Success = false, Message = "Create failed" });
            return Ok(new ApiResponse<BacSi> { Success = true, Data = doc, Message = "Created" });
        }

        [HttpPut("{id}")]
        public ActionResult<ApiResponse<BacSi>> Update(Guid id, DoctorUpdateDTO dto)
        {
            var doc = _doctorBusiness.UpdateDoctor(id, dto);
            if (doc == null) return BadRequest(new ApiResponse { Success = false, Message = "Update failed" });
            return Ok(new ApiResponse<BacSi> { Success = true, Data = doc, Message = "Updated" });
        }

        [HttpDelete("{id}")]
        public ActionResult<ApiResponse> Delete(Guid id)
        {
            var ok = _doctorBusiness.DeleteDoctor(id);
            return Ok(new ApiResponse { Success = ok, Message = ok ? "Deleted" : "Delete failed" });
        }

        [HttpPost("search")]
        public ActionResult<ApiResponse<PagedResult<BacSi>>> Search(SearchRequestDTO request)
        {
            var result = _doctorBusiness.SearchDoctors(request);
            return Ok(new ApiResponse<PagedResult<BacSi>> { Success = true, Data = result, Message = "OK" });
        }

        // ===== HỒ SƠ BỆNH ÁN =====
        [HttpGet("patients/{patientId}/medical-records")]
        public ActionResult<ApiResponse<PagedResult<MedicalRecordDto>>> GetMedicalRecords(Guid patientId, int pageNumber = 1, int pageSize = 10, string? q = null)
        {
            var data = _doctorBusiness.GetMedicalRecordsByPatient(patientId, pageNumber, pageSize, q);
            return Ok(new ApiResponse<PagedResult<MedicalRecordDto>> { Success = true, Data = data, Message = "OK" });
        }

        [HttpPost("medical-records")]
        public ActionResult<ApiResponse<MedicalRecordDto>> CreateMedicalRecord(MedicalRecordDto dto)
        {
            var created = _doctorBusiness.CreateMedicalRecord(dto);
            if (created == null) return BadRequest(new ApiResponse { Success = false, Message = "Create failed" });
            return Ok(new ApiResponse<MedicalRecordDto> { Success = true, Data = created, Message = "Created" });
        }

        [HttpPut("medical-records/{id}")]
        public ActionResult<ApiResponse<MedicalRecordDto>> UpdateMedicalRecord(Guid id, MedicalRecordDto dto)
        {
            var updated = _doctorBusiness.UpdateMedicalRecord(id, dto);
            if (updated == null) return BadRequest(new ApiResponse { Success = false, Message = "Update failed" });
            return Ok(new ApiResponse<MedicalRecordDto> { Success = true, Data = updated, Message = "Updated" });
        }

        [HttpDelete("medical-records/{id}")]
        public ActionResult<ApiResponse> DeleteMedicalRecord(Guid id)
        {
            var ok = _doctorBusiness.DeleteMedicalRecord(id);
            return Ok(new ApiResponse { Success = ok, Message = ok ? "Deleted" : "Delete failed" });
        }

        // ===== LỊCH PHẪU THUẬT =====
        [HttpPost("surgeries/search")]
        public ActionResult<ApiResponse<PagedResult<SurgeryScheduleDto>>> SearchSurgeries([FromBody] SearchRequestDTO request)
        {
            var data = _doctorBusiness.SearchSurgeries(request);
            return Ok(new ApiResponse<PagedResult<SurgeryScheduleDto>> { Success = true, Data = data, Message = "OK" });
        }

        [HttpPost("surgeries")]
        public ActionResult<ApiResponse<SurgeryScheduleDto>> CreateSurgery(SurgeryScheduleDto dto)
        {
            var created = _doctorBusiness.CreateSurgery(dto);
            if (created == null) return BadRequest(new ApiResponse { Success = false, Message = "Create failed" });
            return Ok(new ApiResponse<SurgeryScheduleDto> { Success = true, Data = created, Message = "Created" });
        }

        [HttpPut("surgeries/{id}")]
        public ActionResult<ApiResponse<SurgeryScheduleDto>> UpdateSurgery(Guid id, SurgeryScheduleDto dto)
        {
            var updated = _doctorBusiness.UpdateSurgery(id, dto);
            if (updated == null) return BadRequest(new ApiResponse { Success = false, Message = "Update failed" });
            return Ok(new ApiResponse<SurgeryScheduleDto> { Success = true, Data = updated, Message = "Updated" });
        }

        [HttpDelete("surgeries/{id}")]
        public ActionResult<ApiResponse> DeleteSurgery(Guid id)
        {
            var ok = _doctorBusiness.DeleteSurgery(id);
            return Ok(new ApiResponse { Success = ok, Message = ok ? "Deleted" : "Delete failed" });
        }

        // ===== XÉT NGHIỆM =====
        [HttpPost("labtests/search")]
        public ActionResult<ApiResponse<PagedResult<LabTestDto>>> SearchLabTests([FromBody] SearchRequestDTO request)
        {
            var data = _doctorBusiness.SearchLabTests(request);
            return Ok(new ApiResponse<PagedResult<LabTestDto>> { Success = true, Data = data, Message = "OK" });
        }

        [HttpPost("labtests")]
        public ActionResult<ApiResponse<LabTestDto>> CreateLabTest(LabTestDto dto)
        {
            var created = _doctorBusiness.CreateLabTest(dto);
            if (created == null) return BadRequest(new ApiResponse { Success = false, Message = "Create failed" });
            return Ok(new ApiResponse<LabTestDto> { Success = true, Data = created, Message = "Created" });
        }

        [HttpPut("labtests/{id}")]
        public ActionResult<ApiResponse<LabTestDto>> UpdateLabTest(Guid id, LabTestDto dto)
        {
            var updated = _doctorBusiness.UpdateLabTest(id, dto);
            if (updated == null) return BadRequest(new ApiResponse { Success = false, Message = "Update failed" });
            return Ok(new ApiResponse<LabTestDto> { Success = true, Data = updated, Message = "Updated" });
        }

        [HttpDelete("labtests/{id}")]
        public ActionResult<ApiResponse> DeleteLabTest(Guid id)
        {
            var ok = _doctorBusiness.DeleteLabTest(id);
            return Ok(new ApiResponse { Success = ok, Message = ok ? "Deleted" : "Delete failed" });
        }

        // ===== TRA CỨU BỆNH NHÂN =====
        [HttpGet("patients/lookup")]
        public ActionResult<ApiResponse<PagedResult<PatientLookupDto>>> LookupPatients(string? term, int pageNumber = 1, int pageSize = 10)
        {
            var result = _doctorBusiness.LookupPatients(term, pageNumber, pageSize);
            return Ok(new ApiResponse<PagedResult<PatientLookupDto>> { Success = true, Data = result, Message = "OK" });
        }
    }
}

