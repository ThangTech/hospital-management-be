using System;
using System.Collections.Generic;
using YtaService.BLL.Interfaces;
using YtaService.DAL.Interfaces;
using YtaService.DTO;
using YtaService.Models;

namespace YtaService.BLL
{
    public class GiuongBenhBusiness : IGiuongBenhBusiness
    {
        // Bạn đang đặt tên là _repo
        private readonly IGiuongBenhRepository _repo;

        public GiuongBenhBusiness(IGiuongBenhRepository repo)
        {
            _repo = repo;
        }

        public List<GiuongBenh> GetAllGiuong()
        {
            // Gọi xuống DAL để lấy danh sách thô
            return _repo.GetAll();
        }

        public GiuongBenhDetailDTO GetById(Guid id)
        {
            return _repo.GetById(id);
        }
        public void CreateGiuong(GiuongBenhCreateDTO dto)
        {
            var giuong = new GiuongBenh
            {
                Id = Guid.NewGuid(),
                KhoaId = dto.KhoaId,
                TenGiuong = dto.TenGiuong,
                LoaiGiuong = dto.LoaiGiuong,
                GiaTien = dto.GiaTien,
                TrangThai = "Trống"
            };

            _repo.Create(giuong);
        }
        public string UpdateGiuong(GiuongUpdateDTO giuong)
        {
            // 1. Validate dữ liệu (Logic nghiệp vụ)
            if (giuong.GiaTien < 0)
            {
                return "Giá tiền không được nhỏ hơn 0.";
            }

            if (string.IsNullOrEmpty(giuong.TenGiuong))
            {
                return "Tên giường không được để trống.";
            }

            // 2. Gọi xuống DAL
            // SỬA TẠI ĐÂY: đổi _repository thành _repo
            bool isSuccess = _repo.UpdateGiuong(giuong);

            if (isSuccess)
            {
                return "Cập nhật thành công.";
            }
            else
            {
                return "Cập nhật thất bại (Không tìm thấy ID).";
            }
        }
        public string DeleteGiuong(Guid id)
        {
            // Gọi xuống DAL
            int ketQua = _repo.DeleteGiuong(id); // Chú ý: _repo đã khai báo ở bài trước

            switch (ketQua)
            {
                case 1:
                    return "Xóa thành công.";
                case -1:
                    return "Lỗi: Không tìm thấy giường này trong hệ thống.";
                case -2:
                    return "Lỗi: Giường ĐANG CÓ NGƯỜI nằm (Trạng thái không phải Trống).";
                case -3:
                    return "Lỗi: Giường này đã có lịch sử nhập viện. Không thể xóa (Hãy dùng chức năng ẩn/ngưng hoạt động).";
                default:
                    return "Lỗi không xác định.";
            }
        }
    }
}