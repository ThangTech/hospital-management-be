using BacSiService.BLL.Interfaces;
using BacSiService.DAL.Interfaces;
using BacSiService.Models;
using BacSiService.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace BacSiService.BLL.Services
{
    public class DoctorBusiness : IDoctorBusiness
    {
        private readonly IDoctorRepository _repository;

        public DoctorBusiness(IDoctorRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<BacSi> GetAll()
        {
            return _repository.GetAll();
        }

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
    }
}
