using BacSiService.DTOs;
using System;

namespace BacSiService.DAL.Interfaces
{
    public interface ILabTestRepository
    {
        PagedResult<LabTestDto> Search(Guid? nhapVienId, int pageNumber, int pageSize, string? searchTerm);
        LabTestDto? Create(LabTestDto dto, string? auditUser = null);
        LabTestDto? Update(Guid id, LabTestDto dto, string? auditUser = null);
        bool Delete(Guid id, string? auditUser = null);
    }
}
