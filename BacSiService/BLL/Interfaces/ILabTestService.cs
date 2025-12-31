using BacSiService.DTOs;

namespace BacSiService.BLL.Interfaces
{
    public interface ILabTestService
    {
        PagedResult<LabTestDto> Search(SearchRequestDTO request);
        LabTestDto? Create(LabTestDto dto);
        LabTestDto? Update(System.Guid id, LabTestDto dto);
        bool Delete(System.Guid id);
    }
}
