using System;
using System.Collections.Generic;
using BenhNhanService.BLL.Interfaces;
using BenhNhanService.DAL.Interfaces;
using QuanLyBenhNhan.Models;
using static BenhNhanService.DTO.BenhNhanSearchDTO;

namespace BenhNhanService.BLL
{
    public class BenhNhanBusiness : IBenhNhanBusiness
    {
        private readonly IBenhNhanRepository _res;

        public BenhNhanBusiness(IBenhNhanRepository res)
        {
            _res = res;
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
        public bool Delete(string id) => _res.Delete(id);
        public BenhNhan GetDatabyID(string id) => _res.GetDatabyID(id);
        public List<BenhNhan> Search(BenhNhanSearchModel model, out long total)
        {
            // Gọi sang Repository
            return _res.Search(model, out total);
        }
    }
}