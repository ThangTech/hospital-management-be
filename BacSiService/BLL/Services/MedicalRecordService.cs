using BacSiService.BLL.Interfaces;
using BacSiService.DAL.Interfaces;
using BacSiService.DTOs;
using System;

namespace BacSiService.BLL.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IMedicalRecordRepository _repo;

        public MedicalRecordService(IMedicalRecordRepository repo)
        {
            _repo = repo;
        }

        public PagedResult<MedicalRecordDto> GetByPatient(Guid? patientId, int pageNumber, int pageSize, string? searchTerm)
        {
            return _repo.GetByPatient(patientId, pageNumber, pageSize, searchTerm);
        }

        public MedicalRecordDto? Create(MedicalRecordDto dto)
        {
            // minimal validation
            return _repo.Create(dto);
        }

        public MedicalRecordDto? Update(Guid id, MedicalRecordDto dto)
        {
            return _repo.Update(id, dto);
        }

        public bool Delete(Guid id)
        {
            return _repo.Delete(id);
        }
    }
}
