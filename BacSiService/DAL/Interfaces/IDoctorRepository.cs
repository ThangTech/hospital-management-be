using BacSiService.DTOs;
using BacSiService.Models;
using System.Collections.Generic;
using System;

namespace BacSiService.DAL.Interfaces
{
    public interface IDoctorRepository
    {
        // ===== QUẢN LÝ BÁC SĨ =====
        IEnumerable<BacSi> GetAll();
        BacSi? GetById(Guid id);
        BacSi? CreateDoctor(DoctorDto doctorDto);
        BacSi? UpdateDoctor(Guid id, DoctorUpdateDTO doctorUpdateDTO);
        bool DeleteDoctor(Guid id);
        PagedResult<BacSi> SearchDoctors(SearchRequestDTO request);
    }
}
