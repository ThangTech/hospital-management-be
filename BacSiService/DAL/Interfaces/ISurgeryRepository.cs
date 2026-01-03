using BacSiService.DTOs;
using System;

namespace BacSiService.DAL.Interfaces
{
    public interface ISurgeryRepository
    {
        List<SurgeryScheduleDto> GetAll();
        PagedResult<SurgeryScheduleDto> Search(Guid? bacSiId, int pageNumber, int pageSize, string? searchTerm);
        SurgeryScheduleDto? Create(SurgeryScheduleDto dto, string? auditUser = null);
        SurgeryScheduleDto? Update(Guid id, SurgeryScheduleDto dto, string? auditUser = null);
        bool Delete(Guid id, string? auditUser = null);
    }
}
