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

        public List<MedicalRecordDto> GetAll()
        {
            return _repo.GetAll();
        }

        public PagedResult<MedicalRecordDto> GetByAdmission(Guid? patientId, int pageNumber, int pageSize, string? searchTerm)
        {
            return _repo.GetByAdmission(patientId, pageNumber, pageSize, searchTerm);
        }

        public MedicalRecordDto? Create(MedicalRecordDto dto, Guid? nguoiDungId = null, string? auditUser = null)
        {
            return _repo.Create(dto, nguoiDungId, auditUser);
        }

        public MedicalRecordDto? Update(Guid id, MedicalRecordDto dto, Guid? nguoiDungId = null, string? auditUser = null)
        {
            return _repo.Update(id, dto, nguoiDungId, auditUser);
        }

        public bool Delete(Guid id, Guid? nguoiDungId = null, string? auditUser = null)
        {
            return _repo.Delete(id, nguoiDungId, auditUser);
        }
    }
}
