using BacSiService.Models;
using System.Collections.Generic;

namespace BacSiService.DAL.Interfaces
{
    public interface IDoctorRepository
    {
        IEnumerable<BacSi> GetAll();
    }
}
