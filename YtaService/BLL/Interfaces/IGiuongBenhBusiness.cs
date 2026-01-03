using System.Collections.Generic;
using YtaService.DTO;
using YtaService.Models;

namespace YtaService.BLL.Interfaces
{
    public interface IGiuongBenhBusiness
    {
        List<GiuongBenh> GetAllGiuong();
        void CreateGiuong(GiuongBenhCreateDTO dto);
        GiuongBenhDetailDTO GetById(Guid id);
    }
}