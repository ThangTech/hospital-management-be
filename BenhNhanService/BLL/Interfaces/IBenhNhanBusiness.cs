using QuanLyBenhNhan.Models;



namespace BenhNhanService.BLL.Interfaces
{
    public interface IBenhNhanBusiness
    {
        List<BenhNhan> GetAll();
        bool Create(BenhNhan model);
        bool Update(BenhNhan model);
        bool Delete(string id);
        BenhNhan GetDatabyID(string id);
    }
}
