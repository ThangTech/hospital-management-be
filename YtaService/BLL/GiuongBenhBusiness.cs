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
                TrangThai = "Available"
            };

            _repo.Create(giuong);
        }
    }
}