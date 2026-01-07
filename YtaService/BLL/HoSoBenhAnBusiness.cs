using YtaService.BLL.Interfaces; // <--- QUAN TRỌNG: Phải dùng dòng này để gọi Interface từ thư mục Interfaces
using YtaService.DAL.Interfaces;
using YtaService.DTO;
using System.Collections.Generic;

namespace YtaService.BLL
{
    // --- XÓA ĐOẠN "public interface..." Ở ĐÂY NẾU CÓ ---

    // Class chỉ thực thi (kế thừa) Interface, không định nghĩa lại nó
    public class HoSoBenhAnBusiness : IHoSoBenhAnBusiness
    {
        private readonly IHoSoBenhAnRepository _repo;

        public HoSoBenhAnBusiness(IHoSoBenhAnRepository repo)
        {
            _repo = repo;
        }

        public bool TaoMoi(HoSoBenhAnCreateDTO dto)
        {
            if (dto.BacSiPhuTrachId == null) return false;
            return _repo.TaoHoSo(dto);
        }

        public List<HoSoBenhAnViewDTO> LayTheoNhapVien(Guid nhapVienId)
        {
            return _repo.LayTheoNhapVien(nhapVienId);
        }

        public List<HoSoBenhAnViewDTO> LayTatCaHoSo()
        {
            return _repo.LayTatCa();
        }
    }
}