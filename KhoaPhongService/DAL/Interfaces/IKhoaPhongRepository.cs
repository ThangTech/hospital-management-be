using KhoaPhongService.Models;

namespace KhoaPhongService.DAL.Interfaces
{
    public interface IKhoaPhongRepository
    {
        List<KhoaPhong> GetAll();
        KhoaPhong GetById(string id);
        bool Create(KhoaPhong model);
        bool Update(KhoaPhong model);
        bool Delete(string id);
        int CheckDependencies(string id, out string message);
    }
}
