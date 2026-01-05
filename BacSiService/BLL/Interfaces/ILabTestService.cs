using BacSiService.DTOs;
using System;

namespace BacSiService.BLL.Interfaces
{
    public interface ILabTestService
    {
        List<LabTestDto> GetAll();
        PagedResult<LabTestDto> Search(SearchRequestDTO request);
        LabTestDto? Create(LabTestDto dto, Guid? nguoiDungId = null, string? auditUser = null);
        LabTestDto? Update(Guid id, LabTestDto dto, Guid? nguoiDungId = null, string? auditUser = null);
        bool Delete(Guid id, Guid? nguoiDungId = null, string? auditUser = null);
    }
}
