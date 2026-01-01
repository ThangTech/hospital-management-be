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
        public bool Create(KhoaPhong model) => _repo.Create(model);
    }
}