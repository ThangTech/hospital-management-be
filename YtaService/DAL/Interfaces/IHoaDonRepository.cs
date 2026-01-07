using System;
using System.Collections.Generic;
using YtaService.DTO;

namespace YtaService.DAL.Interfaces
{
    public interface IHoaDonRepository
    {
        bool TaoHoaDon(HoaDonCreateDTO dto);
        List<HoaDonViewDTO> LayDanhSach(Guid? benhNhanId, Guid? nhapVienId);
        HoaDonViewDTO GetById(Guid id);
        bool ThanhToan(HoaDonThanhToanDTO dto);
        bool XoaHoaDon(Guid id);
        HoaDonPreviewDTO LayGoiYVienPhi(Guid nhapVienId);
    }
}
