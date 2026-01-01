using KhoaPhongService.Models;

namespace KhoaPhongService.BLL.Interfaces
{
    public interface IKhoaPhongBusiness
    {
        List<KhoaPhong> GetAll();
        KhoaPhong GetById(string id);
        bool Create(KhoaPhong model);
    }
}