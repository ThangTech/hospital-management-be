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
                return "Lỗi: ID Bệnh nhân hoặc ID Nhập viện không được để trống.";

            if (dto.TongTien <= 0)
                return "Lỗi: Tổng tiền hóa đơn phải lớn hơn 0.";

            bool result = _repo.TaoHoaDon(dto);
            return result ? "Tạo hóa đơn thành công." : "Lỗi: Không tìm thấy phiếu nhập viện hoặc dữ liệu không hợp lệ.";
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
                return "Lỗi: Mã hóa đơn không hợp lệ.";
            
            if (dto.SoTien <= 0)
                return "Lỗi: Số tiền thanh toán phải lớn hơn 0.";

            // Ở đây tôi có thể gọi repo.ThanhToan và nhận kết quả chi tiết hơn nếu sửa Repository
            // Hiện tại tôi sẽ giữ nguyên cấu trúc Repository nhưng cải thiện logic xử lý kết quả
            bool result = _repo.ThanhToan(dto);
            return result ? "Thanh toán thành công." : "Lỗi: Không tìm thấy hóa đơn hoặc hóa đơn đã được thanh toán trước đó.";
        }

        public string XoaHoaDon(Guid id)
        {
            bool result = _repo.XoaHoaDon(id);
            return result ? "Xóa hóa đơn thành công." : "Xóa hóa đơn thất bại (Có thể hóa đơn đã thanh toán hoặc không tồn tại).";
        }
        public HoaDonPreviewDTO LayPreviewGoiY(Guid nhapVienId)
        {
            return _repo.LayGoiYVienPhi(nhapVienId);
        }
    }
}
