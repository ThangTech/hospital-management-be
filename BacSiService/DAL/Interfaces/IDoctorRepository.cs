using BacSiService.DTOs;
using BacSiService.Models;
using System.Collections.Generic;

namespace BacSiService.DAL.Interfaces
{
    public interface IDoctorRepository
    {
        IEnumerable<BacSi> GetAll();
        BacSi? GetById(Guid id);
        BacSi? UpdateDoctor(Guid id, DoctorUpdateDTO doctorUpdateDTO);
        BacSi? CreateDoctor(DoctorDto doctorDto);
        bool DeleteDoctor(Guid id);
    }
}
