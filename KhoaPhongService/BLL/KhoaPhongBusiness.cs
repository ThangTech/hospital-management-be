using KhoaPhongService.BLL.Interfaces;
using KhoaPhongService.DAL.Interfaces;
using KhoaPhongService.Models;

namespace KhoaPhongService.BLL
{
    public class KhoaPhongBusiness : IKhoaPhongBusiness
    {
        private readonly IKhoaPhongRepository _repo;
        public KhoaPhongBusiness(IKhoaPhongRepository repo)
        {
            _repo = repo;
        }

        public List<KhoaPhong> GetAll() => _repo.GetAll();
        public KhoaPhong GetById(string id) => _repo.GetById(id);
        public bool Create(KhoaPhong model)
        {
            return _repo.Create(model);
        }
        public bool Update(KhoaPhong model)
        {
            return _repo.Update(model);
        }
        public List<KhoaPhong> Search(string keyword)
        {
            return _repo.Search(keyword);
        }
        public bool Delete(string id)
        {
            var khoa = _repo.GetById(id);
            if (khoa == null) throw new Exception("Khoa không tồn tại");

            string msgDetail = "";
            int count = _repo.CheckDependencies(id, out msgDetail);

            if (count > 0)
            {
                throw new Exception($"Không thể xóa! {msgDetail}");
            }

            return _repo.Delete(id);
        }
    }
}