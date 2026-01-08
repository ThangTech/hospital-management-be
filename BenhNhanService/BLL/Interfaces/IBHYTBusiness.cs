using BenhNhanService.DTO;

namespace BenhNhanService.BLL.Interfaces
{
    public interface IBHYTBusiness
    {
        KetQuaKiemTraBHYT CheckValidity(string soThe);
        KetQuaTinhPhiBHYT CalculatePayout(YeuCauTinhPhiBHYT request);
    }
}
