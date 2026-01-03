using BacSiService.BLL.Interfaces;
using BacSiService.DAL.Interfaces;
using BacSiService.DTOs;
using System;

namespace BacSiService.BLL.Services
{
    public class LabTestService : ILabTestService
    {
        private readonly ILabTestRepository _repo;

        public LabTestService(ILabTestRepository repo)
        {
            _repo = repo;
        }

        public PagedResult<LabTestDto> Search(SearchRequestDTO request)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;
            Guid? nhapVienId = null;
            if (Guid.TryParse(request.SearchTerm, out var g)) nhapVienId = g;
            return _repo.Search(nhapVienId, pageNumber, pageSize, request.SearchTerm);
        }


        public LabTestDto? Create(LabTestDto dto) => _repo.Create(dto);
        public LabTestDto? Update(Guid id, LabTestDto dto) => _repo.Update(id, dto);
        public bool Delete(Guid id) => _repo.Delete(id);

        public List<LabTestDto> GetAll()
        {
            return _repo.GetAll();
        }
    }
}
