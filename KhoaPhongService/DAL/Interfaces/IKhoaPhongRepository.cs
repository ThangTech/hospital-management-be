using KhoaPhongService.Models;

namespace KhoaPhongService.DAL.Interfaces
{
    public interface IKhoaPhongRepository
    {
        List<KhoaPhong> GetAll();
        KhoaPhong GetById(string id);
        bool Create(KhoaPhong model);
    }
}
