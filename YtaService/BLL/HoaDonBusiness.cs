using System;
using System.Collections.Generic;
using YtaService.BLL.Interfaces;
using YtaService.DAL.Interfaces;
using YtaService.DTO;

namespace YtaService.BLL
{
    public class HoaDonBusiness : IHoaDonBusiness
    {
        private readonly IHoaDonRepository _repo;

        public HoaDonBusiness(IHoaDonRepository repo)
        {
            _repo = repo;
        }

        public string TaoHoaDonMoi(HoaDonCreateDTO dto)
        {
            if (dto.BenhNhanId == Guid.Empty || dto.NhapVienId == Guid.Empty)
                return "L?i: ID B?nh nhân ho?c ID Nh?p vi?n không du?c d? tr?ng.";

            if (dto.TongTien <= 0)
                return "L?i: T?ng ti?n hóa don ph?i l?n hon 0.";

            bool result = _repo.TaoHoaDon(dto);
            return result ? "T?o hóa don thành công." : "L?i: Không tìm th?y phi?u nh?p vi?n ho?c d? li?u không h?p l?.";
        }

        public List<HoaDonViewDTO> LayToanBoHoaDon()
        {
            return _repo.LayDanhSach(null, null);
        }

        public List<HoaDonViewDTO> LayDanhSachHoaDon(Guid? benhNhanId, Guid? nhapVienId)
        {
            return _repo.LayDanhSach(benhNhanId, nhapVienId);
        }

        public HoaDonViewDTO LayChiTietHoaDon(Guid id)
        {
            return _repo.GetById(id);
        }

        public string ThanhToanHoaDon(HoaDonThanhToanDTO dto)
        {
            if (dto.Id == Guid.Empty)
                return "L?i: Mã hóa don không h?p l?.";
            
            if (dto.SoTien <= 0)
                return "L?i: S? ti?n thanh toán ph?i l?n hon 0.";

            // ? dây tôi có th? g?i repo.ThanhToan và nh?n k?t qu? chi ti?t hon n?u s?a Repository
            // Hi?n t?i tôi s? gi? nguyên c?u trúc Repository nhung c?i thi?n logic x? lý k?t qu?
            bool result = _repo.ThanhToan(dto);
            return result ? "Thanh toán thành công." : "L?i: Không tìm th?y hóa don ho?c hóa don dã du?c thanh toán tru?c dó.";
        }

        public string XoaHoaDon(Guid id)
        {
            bool result = _repo.XoaHoaDon(id);
            return result ? "Xóa hóa don thành công." : "Xóa hóa don th?t b?i (Có th? hóa don dã thanh toán ho?c không t?n t?i).";
        }
        public HoaDonPreviewDTO LayPreviewGoiY(Guid nhapVienId)
        {
            return _repo.LayGoiYVienPhi(nhapVienId);
        }
    }
}
