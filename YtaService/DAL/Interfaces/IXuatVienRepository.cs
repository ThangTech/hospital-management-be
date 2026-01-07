using System;
using System.Collections.Generic;
using YtaService.DTO;

namespace YtaService.DAL.Interfaces
{
    public interface IXuatVienRepository
    {
        int XuatVien(XuatVienDTO dto);
        List<SanSangXuatVienDTO> LayDanhSachSanSangXuatVien();
        List<LichSuXuatVienDTO> LayLichSuXuatVien();
        XuatVienPreviewDTO XemTruocXuatVien(Guid nhapVienId);
    }
}
