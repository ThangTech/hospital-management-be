using BacSiService.DTOs;
using System;

namespace BacSiService.DAL.Interfaces
{
    public interface IMedicalRecordRepository
    {
        List<MedicalRecordDto> GetAll();
        PagedResult<MedicalRecordDto> GetByAdmission(Guid? patientId, int pageNumber, int pageSize, string? searchTerm);
        MedicalRecordDto? Create(MedicalRecordDto dto, string? auditUser = null);
        MedicalRecordDto? Update(Guid id, MedicalRecordDto dto, string? auditUser = null);
        bool Delete(Guid id, string? auditUser = null);
    }
}
