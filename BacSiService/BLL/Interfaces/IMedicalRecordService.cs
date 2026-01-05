using BacSiService.DTOs;
using System;

namespace BacSiService.BLL.Interfaces
{
    public interface IMedicalRecordService
    {
        List<MedicalRecordDto> GetAll();
        PagedResult<MedicalRecordDto> GetByAdmission(Guid? patientId, int pageNumber, int pageSize, string? searchTerm);
        MedicalRecordDto? Create(MedicalRecordDto dto, Guid? nguoiDungId = null, string? auditUser = null);
        MedicalRecordDto? Update(Guid id, MedicalRecordDto dto, Guid? nguoiDungId = null, string? auditUser = null);
        bool Delete(Guid id, Guid? nguoiDungId = null, string? auditUser = null);
    }
}
