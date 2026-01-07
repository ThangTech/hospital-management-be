using System;
using System.Collections.Generic;
using YtaService.BLL.Interfaces;
using YtaService.DAL.Interfaces;
using YtaService.DTO;

namespace YtaService.BLL
{
    public class XuatVienBusiness : IXuatVienBusiness
    {
        private readonly IXuatVienRepository _repo;

        public XuatVienBusiness(IXuatVienRepository repo)
        {
            _repo = repo;
        }

        public string XuatVien(XuatVienDTO dto)
        {
            if (dto.Id == Guid.Empty)
            {
                return "Mã nhập viện không hợp lệ.";
            }

            int result = _repo.XuatVien(dto);

            switch (result)
            {
                case 1:
                    return "Xuất viện thành công.";
                case -1:
                    return "Lỗi: Không tìm thấy phiếu nhập viện.";
                case -2:
                    return "Lỗi: Bệnh nhân còn hóa đơn CHƯA THANH TOÁN. Vui lòng thanh toán trước khi xuất viện.";
                case -3:
                    return "Lỗi: Bệnh nhân đã xuất viện rồi.";
                case -99:
                    return "Lỗi hệ thống.";
                default:
                    return "Lỗi không xác định.";
            }
        }

        // 2. LẤY DANH SÁCH SẴN SÀNG XUẤT VIỆN
        public List<SanSangXuatVienDTO> LayDanhSachSanSangXuatVien()
        {
            return _repo.LayDanhSachSanSangXuatVien();
        }

        // 3. LẤY LỊCH SỬ XUẤT VIỆN
        public List<LichSuXuatVienDTO> LayLichSuXuatVien()
        {
            return _repo.LayLichSuXuatVien();
        }

        // 4. XEM TRƯỚC XUẤT VIỆN
        public XuatVienPreviewDTO XemTruocXuatVien(Guid nhapVienId)
        {
            return _repo.XemTruocXuatVien(nhapVienId);
        }
    }
}
