using BacSiService.DTOs;

namespace BacSiService.DAL.Interfaces
{
    public interface IPatientRepository
    {
        PagedResult<PatientLookupDto> Lookup(string? term, int pageNumber, int pageSize);
    }
}
