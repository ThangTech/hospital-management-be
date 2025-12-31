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

        // CREATE - T?o bác s? m?i
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

        // DELETE - Xóa bác s?
        public bool DeleteDoctor(Guid id)
        {

            return _repository.DeleteDoctor(id);
        }

        // GET - L?y t?t c? bác s?
        public IEnumerable<BacSi> GetAll()
        {
            return _repository.GetAll();
        }

        // GET BY ID - L?y 1 bác s? theo ID
        public BacSi? GetById(Guid id)
        {
            if (id == Guid.Empty)
                return null;
            return _repository.GetById(id);
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

        // UPDATE - C?p nh?t thông tin bác s?
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

        // SEARCH & PAGING - Tìm ki?m bác s? v?i phân trang
        public PagedResult<BacSi> SearchDoctors(SearchRequestDTO request)
        {
            if (request == null)
                request = new SearchRequestDTO();

            // Validate paging
            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize < 1) request.PageSize = 10;
            if (request.PageSize > 100) request.PageSize = 100; // Max 100 records per page

            return _repository.SearchDoctors(request);
        }
    }
    
}

