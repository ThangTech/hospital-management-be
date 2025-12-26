using QuanLyBenhNhan.Models;
using static BenhNhanService.DTO.BenhNhanSearchDTO;



namespace BenhNhanService.BLL.Interfaces
{
    public interface IBenhNhanBusiness
    {
        List<BenhNhan> GetAll();
        bool Create(BenhNhan model);
        bool Update(BenhNhan model);
        bool Delete(string id);
        BenhNhan GetDatabyID(string id);
        List<BenhNhan> Search(BenhNhanSearchModel model, out long total);
    }
}
