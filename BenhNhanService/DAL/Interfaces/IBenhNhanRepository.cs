using QuanLyBenhNhan.Models;
namespace BenhNhanService.DAL.Interfaces
{
    public interface IBenhNhanRepository
    {
        // Bạn thiếu các dòng này nên Business báo lỗi CS1061
        List<BenhNhan> GetAll();
        bool Create(BenhNhan model);
        bool Update(BenhNhan model);
        bool Delete(string id);
        BenhNhan GetDatabyID(string id);
    }
}
