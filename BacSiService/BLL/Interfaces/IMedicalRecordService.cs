using BacSiService.DTOs;
using System;

namespace BacSiService.BLL.Interfaces
{
    public interface IMedicalRecordService
    {
        PagedResult<MedicalRecordDto> GetByPatient(Guid? patientId, int pageNumber, int pageSize, string? searchTerm);
        MedicalRecordDto? Create(MedicalRecordDto dto);
        MedicalRecordDto? Update(Guid id, MedicalRecordDto dto);
        bool Delete(Guid id);
    }
}
