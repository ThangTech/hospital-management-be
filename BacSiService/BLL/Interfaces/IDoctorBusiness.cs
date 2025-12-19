using BacSiService.Models;
using BacSiService.DTOs;
using System.Collections.Generic;

namespace BacSiService.BLL.Interfaces
{
    public interface IDoctorBusiness
    {
        IEnumerable<BacSi> GetAll();
        IEnumerable<DoctorDto> GetAllDtos();
    }
}
