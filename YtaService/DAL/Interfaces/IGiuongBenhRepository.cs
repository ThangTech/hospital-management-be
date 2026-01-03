using System.Collections.Generic;
using YtaService.DTO;
using YtaService.Models;

namespace YtaService.DAL.Interfaces
{
    public interface IGiuongBenhRepository
    {
        List<GiuongBenh> GetAll();
        void Create(GiuongBenh giuong);
        GiuongBenhDetailDTO GetById(Guid id);
    }
}