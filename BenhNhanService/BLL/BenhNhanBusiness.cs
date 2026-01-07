using System;
using System.Collections.Generic;
using System.Linq;
using BenhNhanService.BLL.Interfaces;
using BenhNhanService.DAL.Interfaces;
using QuanLyBenhNhan.Models;
using static BenhNhanService.DTO.BenhNhanSearchDTO;

namespace BenhNhanService.BLL
{
    public class BenhNhanBusiness : IBenhNhanBusiness
    {
        private readonly IBenhNhanRepository _res;
        private readonly HospitalManageContext _context;

        public BenhNhanBusiness(IBenhNhanRepository res, HospitalManageContext context)
        {
            _res = res;
            _context = context;
        }

        // Sửa tên hàm GetListBenhNhan -> GetAll để khớp với Interface IBenhNhanBusiness
        public List<BenhNhan> GetAll()
        {
            return _res.GetAll();
        }

        public bool Create(BenhNhan model)
        {
            if (model.Id == Guid.Empty) model.Id = Guid.NewGuid();
            return _res.Create(model);
        }

        // Bây giờ Interface Repository đã có các hàm này rồi nên sẽ hết lỗi đỏ
        public bool Update(BenhNhan model) => _res.Update(model);
        
        public bool Delete(string id)
        {
            if (!Guid.TryParse(id, out Guid guidId)) return false;

            // 1. Kiểm tra xem bệnh nhân có đang trong quá trình điều trị không
            var dangDieuTri = _context.NhapViens.Any(x => x.BenhNhanId == guidId && x.TrangThai == "Đang điều trị");
            if (dangDieuTri)
            {
                throw new Exception("Không thể xóa bệnh nhân đang trong quá trình điều trị.");
            }

            // 2. Kiểm tra xem bệnh nhân còn hóa đơn chưa thanh toán không
            var chuaThanhToan = _context.HoaDons.Any(x => x.BenhNhanId == guidId && x.TrangThai == "Chưa thanh toán");
            if (chuaThanhToan)
            {
                throw new Exception("Không thể xóa bệnh nhân còn hóa đơn chưa thanh toán.");
            }

            // Nếu vượt qua các kiểm tra, thực hiện xóa
            return _res.Delete(id);
        }
        
        public BenhNhan GetDatabyID(string id) => _res.GetDatabyID(id);
        public List<BenhNhan> Search(BenhNhanSearchModel model, out long total)
        {
            // Gọi sang Repository
            return _res.Search(model, out total);
        }
    }
}