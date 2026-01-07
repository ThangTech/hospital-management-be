using BacSiService.DTOs;
using System;

namespace BacSiService.DAL.Interfaces
{
    public interface ILabTestRepository
    {
        List<LabTestDto> GetAll();
        PagedResult<LabTestDto> Search(Guid? nhapVienId, int pageNumber, int pageSize, string? searchTerm);
        LabTestDto? Create(LabTestDto dto, Guid? nguoiDungId = null, string? auditUser = null);
        LabTestDto? Update(Guid id, LabTestDto dto, Guid? nguoiDungId = null, string? auditUser = null);
        bool Delete(Guid id, Guid? nguoiDungId = null, string? auditUser = null);
    }
}
