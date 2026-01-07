using System;
using System.Collections.Generic;
using YtaService.DTO;

namespace YtaService.BLL.Interfaces
{
    public interface IHoaDonBusiness
    {
        string TaoHoaDonMoi(HoaDonCreateDTO dto);
        List<HoaDonViewDTO> LayToanBoHoaDon();
        List<HoaDonViewDTO> LayDanhSachHoaDon(Guid? benhNhanId, Guid? nhapVienId);
        HoaDonViewDTO LayChiTietHoaDon(Guid id);
        string ThanhToanHoaDon(HoaDonThanhToanDTO dto);
        string XoaHoaDon(Guid id);
        HoaDonPreviewDTO LayPreviewGoiY(Guid nhapVienId);
    }
}
