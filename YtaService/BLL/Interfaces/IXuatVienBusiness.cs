using System;
using System.Collections.Generic;
using YtaService.DTO;

namespace YtaService.BLL.Interfaces
{
    public interface IXuatVienBusiness
    {
        string XuatVien(XuatVienDTO dto);
        List<SanSangXuatVienDTO> LayDanhSachSanSangXuatVien();
        List<LichSuXuatVienDTO> LayLichSuXuatVien();
        XuatVienPreviewDTO XemTruocXuatVien(Guid nhapVienId);
    }
}
