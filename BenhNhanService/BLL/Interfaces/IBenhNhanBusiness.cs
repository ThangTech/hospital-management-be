using QuanLyBenhNhan.Models;



namespace BenhNhanService.BLL.Interfaces
{
    public interface IBenhNhanBusiness
    {
        List<BenhNhan> GetListBenhNhan();
        bool AddBenhNhan(BenhNhan model);
    }
}
