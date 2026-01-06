using YtaService.DTO;

namespace YtaService.DAL.Interfaces
{
    public interface IHoSoBenhAnRepository
    {
        bool TaoHoSo(HoSoBenhAnCreateDTO hoso);
        List<HoSoBenhAnViewDTO> LayTheoNhapVien(Guid nhapVienId);
        List<HoSoBenhAnViewDTO> LayTatCa();
    }
}