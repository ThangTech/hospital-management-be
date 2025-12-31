using BacSiService.DTOs;
using BacSiService.Models;
using System.Collections.Generic;
using System;

namespace BacSiService.DAL.Interfaces
{
    public interface IDoctorRepository
    {
        // ===== QU?N LÝ BÁC S? =====
        IEnumerable<BacSi> GetAll();
        BacSi? GetById(Guid id);
        BacSi? CreateDoctor(DoctorDto doctorDto);
        BacSi? UpdateDoctor(Guid id, DoctorUpdateDTO doctorUpdateDTO);
        bool DeleteDoctor(Guid id);
        PagedResult<BacSi> SearchDoctors(SearchRequestDTO request);

        // ===== H? S? B?NH ÁN =====
        PagedResult<MedicalRecordDto> GetMedicalRecordsByPatient(Guid? patientId, int pageNumber, int pageSize, string? searchTerm);
        MedicalRecordDto? CreateMedicalRecord(MedicalRecordDto dto, string? auditUser = null);
        MedicalRecordDto? UpdateMedicalRecord(Guid id, MedicalRecordDto dto, string? auditUser = null);
        bool DeleteMedicalRecord(Guid id, string? auditUser = null);

        // ===== L?CH PH?U THU?T =====
        PagedResult<SurgeryScheduleDto> SearchSurgeries(Guid? bacSiId, int pageNumber, int pageSize, string? searchTerm);
        SurgeryScheduleDto? CreateSurgery(SurgeryScheduleDto dto, string? auditUser = null);
        SurgeryScheduleDto? UpdateSurgery(Guid id, SurgeryScheduleDto dto, string? auditUser = null);
        bool DeleteSurgery(Guid id, string? auditUser = null);

        // ===== XÉT NGHI?M =====
        PagedResult<LabTestDto> SearchLabTests(Guid? nhapVienId, int pageNumber, int pageSize, string? searchTerm);
        LabTestDto? CreateLabTest(LabTestDto dto, string? auditUser = null);
        LabTestDto? UpdateLabTest(Guid id, LabTestDto dto, string? auditUser = null);
        bool DeleteLabTest(Guid id, string? auditUser = null);

        // ===== TRA C?U B?NH NHÂN =====
        PagedResult<PatientLookupDto> LookupPatients(string? term, int pageNumber, int pageSize);
    }
}
