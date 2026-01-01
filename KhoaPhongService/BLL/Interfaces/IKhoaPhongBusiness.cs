using KhoaPhongService.Models;

namespace KhoaPhongService.BLL.Interfaces
{
    public interface IKhoaPhongBusiness
    {
        List<KhoaPhong> GetAll();
        KhoaPhong GetById(string id);
        bool Create(KhoaPhong model);
        bool Update(KhoaPhong model);
        bool Delete(string id);
        List<KhoaPhong> Search(string keyword);

    }
}