using BacSiService.Models;
using BacSiService.DTOs;
using System;
using System.Collections.Generic;

namespace BacSiService.BLL.Interfaces
{
    public interface IDoctorBusiness
    {
        // ===== QU?N LÝ BÁC S? =====
        IEnumerable<BacSi> GetAll();
        BacSi? GetById(Guid id);
        BacSi? CreateDoctor(DoctorDto doctorDto);
        BacSi? UpdateDoctor(Guid id, DoctorUpdateDTO doctorUpdateDTO);
        bool DeleteDoctor(Guid id);
        PagedResult<BacSi> SearchDoctors(SearchRequestDTO request);
        IEnumerable<DoctorDto> GetAllDtos();
    }
}
