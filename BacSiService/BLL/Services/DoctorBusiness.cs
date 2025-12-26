using BacSiService.BLL.Interfaces;
using BacSiService.DAL.Interfaces;
using BacSiService.Models;
using BacSiService.DTOs;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BacSiService.BLL.Services
{
    public class DoctorBusiness : IDoctorBusiness
    {
        private readonly IDoctorRepository _repository;

        public DoctorBusiness(IDoctorRepository repository)
        {
            _repository = repository;
        }

        public DoctorDto CreateDoctor(DoctorDto doctorDto)
        {
            var doctor = _repository.CreateDoctor(doctorDto);

            if (doctor == null)
            {
                return null;
            }

            return new DoctorDto
            {
                Id = doctor.Id,
                HoTen = doctor.HoTen,
                ChuyenKhoa = doctor.ChuyenKhoa,
                ThongTinLienHe = doctor.ThongTinLienHe
            };
        }

        public bool DeleteDoctor(Guid id)
        {

            return _repository.DeleteDoctor(id);
        }

        //public IEnumerable<BacSi> GetAll()
        //{
        //    return _repository.GetAll();
        //}

        // Convenience method to return DTOs
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

        public DoctorDto GetDoctorByID(Guid id)
        {
            var doctor = _repository.GetById(id);
            if( doctor == null)
            {
                return null;
            }
            return new DoctorDto
            {
                Id = doctor.Id,
                HoTen = doctor.HoTen,
                ChuyenKhoa = doctor.ChuyenKhoa,
                ThongTinLienHe = doctor.ThongTinLienHe
            };
            
        }

        public PagedResult<DoctorDto> SearchDoctors(SearchRequestDTO searchRequestDTO)
        {
            var result = _repository.SearchDoctors(searchRequestDTO);

            var dtoList = new List<DoctorDto>();
            foreach (var d in result.Data)
            {
                dtoList.Add(new DoctorDto
                {
                    Id = d.Id,
                    HoTen = d.HoTen,
                    ChuyenKhoa = d.ChuyenKhoa,
                    ThongTinLienHe = d.ThongTinLienHe
                });
            }

            return new PagedResult<DoctorDto>
            {
                Data = dtoList,  // List<DoctorDto>
                TotalRecords = result.TotalRecords,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages
            };
        }

        public DoctorUpdateDTO UpdateDTO(Guid id, DoctorUpdateDTO doctorUpdateDTO)
        {
            var updatedDoctor = _repository.UpdateDoctor(id, doctorUpdateDTO);

            if (updatedDoctor == null)
            {
                return null;
            }

            return new DoctorUpdateDTO
            {
                HoTen = updatedDoctor.HoTen,
                ChuyenKhoa = updatedDoctor.ChuyenKhoa,
                ThongTinLienHe = updatedDoctor.ThongTinLienHe
            };
        }
    }
    
}

