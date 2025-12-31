using BacSiService.BLL.Interfaces;
using BacSiService.DAL.Interfaces;
using BacSiService.Models;
using BacSiService.DTOs;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BacSiService.BLL.Services
{
    public class DoctorBusiness : IDoctorBusiness
    {
        private readonly IDoctorRepository _repository;

        public DoctorBusiness(IDoctorRepository repository)
        {
            _repository = repository;
        }

        // Doctor
        public IEnumerable<DoctorDto> GetAllDtos()
        {
            var doctors = _repository.GetAll();
            return doctors.Select(d => new DoctorDto
            {
                Id = d.Id,
                HoTen = d.HoTen,
                ChuyenKhoa = d.ChuyenKhoa,
                ThongTinLienHe = d.ThongTinLienHe
            });
        }

        public DoctorDto? CreateDoctor(DoctorDto doctorDto)
        {
            var created = _repository.CreateDoctor(doctorDto);
            if (created == null) return null;
            return new DoctorDto
            {
                Id = created.Id,
                HoTen = created.HoTen,
                ChuyenKhoa = created.ChuyenKhoa,
                ThongTinLienHe = created.ThongTinLienHe
            };
        }

        public bool DeleteDoctor(Guid id)
        {
            return _repository.DeleteDoctor(id);
        }

        public DoctorDto? GetDoctorByID(Guid id)
        {
            var d = _repository.GetById(id);
            if (d == null) return null;
            return new DoctorDto
            {
                Id = d.Id,
                HoTen = d.HoTen,
                ChuyenKhoa = d.ChuyenKhoa,
                ThongTinLienHe = d.ThongTinLienHe
            };
        }

        public DoctorUpdateDTO? UpdateDTO(Guid id, DoctorUpdateDTO doctorUpdateDTO)
        {
            var updated = _repository.UpdateDoctor(id, doctorUpdateDTO);
            if (updated == null) return null;
            return new DoctorUpdateDTO
            {
                HoTen = updated.HoTen,
                ChuyenKhoa = updated.ChuyenKhoa,
                ThongTinLienHe = updated.ThongTinLienHe
            };
        }

        public PagedResult<BacSi> SearchDoctors(SearchRequestDTO request)
        {
            if (request == null)
                request = new SearchRequestDTO();

            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize < 1) request.PageSize = 10;
            if (request.PageSize > 100) request.PageSize = 100;

            return _repository.SearchDoctors(request);
        }

    }
}

