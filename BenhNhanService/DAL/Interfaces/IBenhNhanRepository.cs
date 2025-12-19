using QuanLyBenhNhan.Models;
namespace BenhNhanService.DAL.Interfaces
{
    public interface IBenhNhanRepository
    {
        List<BenhNhan> GetAll();
        bool Create(BenhNhan model);
    }
}
