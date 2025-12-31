using BacSiService.BLL.Interfaces;
using BacSiService.DAL.Interfaces;
using BacSiService.DTOs;

namespace BacSiService.BLL.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _repo;

        public PatientService(IPatientRepository repo)
        {
            _repo = repo;
        }

        public PagedResult<PatientLookupDto> Lookup(string? term, int pageNumber, int pageSize)
        {
            return _repo.Lookup(term, pageNumber, pageSize);
        }
    }
}
