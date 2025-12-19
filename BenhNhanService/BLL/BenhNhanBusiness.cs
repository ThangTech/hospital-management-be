using System; // Added for Exception
using System.Collections.Generic; // Added for List<>
using BenhNhanService.BLL.Interfaces;
using BenhNhanService.DAL.Interfaces;
using QuanLyBenhNhan.Models;

namespace BenhNhanService.BLL
{
    // FIX: You must declare the class here
    public class BenhNhanBusiness : IBenhNhanBusiness
    {
        private readonly IBenhNhanRepository _res; // Best practice: use readonly for dependency injection

        // Constructor
        public BenhNhanBusiness(IBenhNhanRepository res)
        {
            _res = res;
        }

        public List<BenhNhan> GetListBenhNhan()
        {
            return _res.GetAll();
        }

        public bool AddBenhNhan(BenhNhan model)
        {
            // Logic kiểm tra nghiệp vụ
            if (string.IsNullOrEmpty(model.HoTen))
                throw new Exception("Tên bệnh nhân không được để trống");

            return _res.Create(model);
        }
    }
}