using System;
using System.Collections.Generic;
using YtaService.DTO;

namespace YtaService.DAL.Interfaces
{
    public interface INhapVienRepository
    {
        bool TaoPhieuNhapVien(NhapVienCreateDTO dto);
        List<NhapVienViewDTO> LayDanhSachDangDieuTri();
        bool CapNhatNhapVien(NhapVienUpdateDTO dto);
        int XoaNhapVien(Guid id);
        bool ChuyenGiuong(ChuyenGiuongDTO dto);
        NhapVienViewDTO GetById(Guid id);
        List<NhapVienViewDTO> TimKiem(NhapVienSearchDTO dto);
    }
}
