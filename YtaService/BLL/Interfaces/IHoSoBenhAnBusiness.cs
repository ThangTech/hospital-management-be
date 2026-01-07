using YtaService.DTO;
using System.Collections.Generic; // Cần dòng này cho List<>

namespace YtaService.BLL.Interfaces
{
    public interface IHoSoBenhAnBusiness
    {
        // Các hàm cũ của bạn
        bool TaoMoi(HoSoBenhAnCreateDTO dto);
        List<HoSoBenhAnViewDTO> LayTheoNhapVien(Guid nhapVienId);

        // --- BỔ SUNG DÒNG NÀY ĐỂ FIX LỖI ---
        List<HoSoBenhAnViewDTO> LayTatCaHoSo();
    }
}