using BacSiService.Models;
using BacSiService.DTOs;
using System.Collections.Generic;

namespace BacSiService.BLL.Interfaces
{
    public interface IDoctorBusiness
    {
        //IEnumerable<BacSi> GetAll();
        IEnumerable<DoctorDto> GetAllDtos();
        DoctorDto GetDoctorByID(Guid id);
        DoctorUpdateDTO UpdateDTO(Guid id, DoctorUpdateDTO doctorUpdateDTO);

        DoctorDto CreateDoctor(DoctorDto doctorDto);
        bool DeleteDoctor(Guid id);

        PagedResult<DoctorDto> SearchDoctors(SearchRequestDTO searchRequestDTO);

    }
}
