using System;
using System.Collections.Generic;
using YtaService.BLL.Interfaces;
using YtaService.DAL.Interfaces;
using YtaService.DTO;

namespace YtaService.BLL.Interfaces
{
    public interface INhapVienBusiness
    {
        bool NhapVienMoi(NhapVienCreateDTO dto);
        List<NhapVienViewDTO> LayDanhSachNoiTru();
        string CapNhatNhapVien(NhapVienUpdateDTO dto);
        string XoaNhapVien(Guid id);
        string ChuyenGiuong(ChuyenGiuongDTO dto);
        NhapVienViewDTO LayChiTietNhapVien(Guid id);
        List<NhapVienViewDTO> TimKiem(NhapVienSearchDTO dto);
    }
}
