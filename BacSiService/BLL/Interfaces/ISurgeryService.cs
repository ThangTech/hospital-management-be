using BacSiService.DTOs;
using System;

namespace BacSiService.BLL.Interfaces
{
    public interface ISurgeryService
    {
        List<SurgeryScheduleDto> GetAll();
        PagedResult<SurgeryScheduleDto> Search(SearchRequestDTO request);
        SurgeryScheduleDto? Create(SurgeryScheduleDto dto, Guid? nguoiDungId = null, string? auditUser = null);
        SurgeryScheduleDto? Update(Guid id, SurgeryScheduleDto dto, Guid? nguoiDungId = null, string? auditUser = null);
        bool Delete(Guid id, Guid? nguoiDungId = null, string? auditUser = null);
    }
}
