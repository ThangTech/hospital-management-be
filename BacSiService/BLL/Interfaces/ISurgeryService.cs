using BacSiService.DTOs;
using System;

namespace BacSiService.BLL.Interfaces
{
    public interface ISurgeryService
    {
        PagedResult<SurgeryScheduleDto> Search(SearchRequestDTO request);
        SurgeryScheduleDto? Create(SurgeryScheduleDto dto);
        SurgeryScheduleDto? Update(Guid id, SurgeryScheduleDto dto);
        bool Delete(Guid id);
    }
}
