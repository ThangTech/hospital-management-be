using System;
using System.Collections.Generic;
using YtaService.BLL.Interfaces;
using YtaService.DAL.Interfaces;
using YtaService.DTO;

namespace YtaService.BLL
{
    public class NhapVienBusiness : INhapVienBusiness
    {
        private readonly INhapVienRepository _repo;

        public NhapVienBusiness(INhapVienRepository repo)
        {
            _repo = repo;
        }

        public bool NhapVienMoi(NhapVienCreateDTO dto)
        {
            return _repo.TaoPhieuNhapVien(dto);
        }
        public List<NhapVienViewDTO> LayDanhSachNoiTru() 
        {
            return _repo.LayDanhSachDangDieuTri();
        }

        public NhapVienViewDTO LayChiTietNhapVien(Guid id)
        {
            return _repo.GetById(id);
        }

        // 3. CẬP NHẬT NHẬP VIỆN
        public string CapNhatNhapVien(NhapVienUpdateDTO dto)
        {
            // Validate
            if (dto.Id == Guid.Empty)
            {
                return "Mã nhập viện không hợp lệ.";
            }

            bool result = _repo.CapNhatNhapVien(dto);
            return result ? "Cập nhật thành công." : "Cập nhật thất bại (Không tìm thấy phiếu hoặc lỗi hệ thống).";
        }

        // 4. XÓA NHẬP VIỆN
        public string XoaNhapVien(Guid id)
        {
            int ketQua = _repo.XoaNhapVien(id);

            switch (ketQua)
            {
                case 1:
                    return "Xóa thành công.";
                case -1:
                    return "Lỗi: Không tìm thấy phiếu nhập viện này.";
                case -2:
                    return "Lỗi: Bệnh nhân đang nằm giường, không thể xóa.";
                case -3:
                    return "Lỗi: Phiếu nhập viện có hóa đơn liên quan, không thể xóa.";
                case -4:
                    return "Lỗi: Bệnh nhân chưa xuất viện, không thể xóa.";
                default:
                    return "Lỗi không xác định.";
            }
        }

        // 5. CHUYỂN GIƯỜNG (Có thể khác khoa)
        public string ChuyenGiuong(ChuyenGiuongDTO dto)
        {
            // Validate
            if (dto.NhapVienId == Guid.Empty)
            {
                return "Mã nhập viện không hợp lệ.";
            }
            if (dto.GiuongMoiId == Guid.Empty)
            {
                return "Mã giường mới không hợp lệ.";
            }

            bool result = _repo.ChuyenGiuong(dto);
            return result 
                ? "Chuyển giường thành công." 
                : "Chuyển giường thất bại (Giường mới không trống hoặc lỗi hệ thống).";
        }

        // 6. TÌM KIẾM NHẬP VIỆN
        public List<NhapVienViewDTO> TimKiem(NhapVienSearchDTO dto)
        {
            return _repo.TimKiem(dto);
        }
    }
}
