using BacSiService.DTOs;

namespace BacSiService.BLL.Interfaces
{
    public interface IPatientService
    {
        PagedResult<PatientLookupDto> Lookup(string? term, int pageNumber, int pageSize);
    }
}
