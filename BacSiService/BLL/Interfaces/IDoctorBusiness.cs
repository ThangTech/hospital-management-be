using BacSiService.Models;
using BacSiService.DTOs;
using System;
using System.Collections.Generic;

namespace BacSiService.BLL.Interfaces
{
    public interface IDoctorBusiness
    {
        // Doctor
        IEnumerable<DoctorDto> GetAllDtos();
        DoctorDto? CreateDoctor(DoctorDto doctorDto);
        bool DeleteDoctor(Guid id);
        DoctorDto? GetDoctorByID(Guid id);
        DoctorUpdateDTO? UpdateDTO(Guid id, DoctorUpdateDTO doctorUpdateDTO);
        PagedResult<BacSi> SearchDoctors(SearchRequestDTO request);
    }
}
