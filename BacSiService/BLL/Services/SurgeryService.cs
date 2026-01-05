using BacSiService.BLL.Interfaces;
using BacSiService.DAL.Interfaces;
using BacSiService.DTOs;
using System;

namespace BacSiService.BLL.Services
{
    public class SurgeryService : ISurgeryService
    {
        private readonly ISurgeryRepository _repo;

        public SurgeryService(ISurgeryRepository repo)
        {
            _repo = repo;
        }

        public List<SurgeryScheduleDto> GetAll()
        {
            return _repo.GetAll();
        }

        public PagedResult<SurgeryScheduleDto> Search(SearchRequestDTO request)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;
            Guid? bacSiId = null;
            if (Guid.TryParse(request.SearchTerm, out var g)) bacSiId = g;
            return _repo.Search(bacSiId, pageNumber, pageSize, request.SearchTerm);
        }

        public SurgeryScheduleDto? Create(SurgeryScheduleDto dto, Guid? nguoiDungId = null, string? auditUser = null)
        {
            return _repo.Create(dto, nguoiDungId, auditUser);
        }

        public SurgeryScheduleDto? Update(Guid id, SurgeryScheduleDto dto, Guid? nguoiDungId = null, string? auditUser = null)
        {
            return _repo.Update(id, dto, nguoiDungId, auditUser);
        }

        public bool Delete(Guid id, Guid? nguoiDungId = null, string? auditUser = null)
        {
            return _repo.Delete(id, nguoiDungId, auditUser);
        }
    }
}
