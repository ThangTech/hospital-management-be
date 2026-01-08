using BenhNhanService.BLL.Interfaces;
using BenhNhanService.DAL.Interfaces;
using BenhNhanService.DTO;
using System;

namespace BenhNhanService.BLL
{
    public class BHYTBusiness : IBHYTBusiness
    {
        private readonly IBenhNhanRepository _repo;
        private readonly string _maBVHienTai;

        public BHYTBusiness(IBenhNhanRepository repo, IConfiguration config)
        {
            _repo = repo;
            _maBVHienTai = config["HospitalConfig:MaBenhVien"] ?? "79001";
        }

        public KetQuaKiemTraBHYT CheckValidity(string soThe)
        {
            // GIẢ ĐỊNH: Nếu soThe có dạng "SốThẻ|MãNơiĐK" thì bóc tách, nếu chỉ có SốThẻ thì lấy mặc định
            string maThe = soThe;
            string maNoiDK = "Unknown";
            
            if (soThe.Contains("|"))
            {
                var parts = soThe.Split('|');
                maThe = parts[0];
                maNoiDK = parts[1];
            }

            if (string.IsNullOrEmpty(maThe) || maThe.Length < 15) 
                return new KetQuaKiemTraBHYT { HopLe = false, ThongBao = "Mã thẻ BHYT phải đủ 15 ký tự" };

            char kyTuMucHuong = maThe[2]; 
            decimal mucHuong = 0.8m; 

            switch (kyTuMucHuong)
            {
                case '1':
                case '2': mucHuong = 1.0m; break; 
                case '3': mucHuong = 0.95m; break; 
                default: mucHuong = 0.8m; break; 
            }

            bool isDungTuyen = (maNoiDK == _maBVHienTai);

            return new KetQuaKiemTraBHYT
            {
                HopLe = true,
                MaDoiTuong = maThe.Substring(0, 2).ToUpper(),
                MucHuong = mucHuong,
                HanThe = new DateTime(DateTime.Now.Year, 12, 31),
                MaNoiDK = maNoiDK,
                GoiYTuyen = isDungTuyen ? "Đúng tuyến" : "Trái tuyến (Cần kiểm tra Giấy chuyển viện/Cấp cứu)",
                ThongBao = $"Nhận diện mã {maThe.Substring(0, 2).ToUpper()}: Hưởng {mucHuong * 100}%"
            };
        }

        public KetQuaTinhPhiBHYT CalculatePayout(YeuCauTinhPhiBHYT request)
        {
            var benhNhan = _repo.GetDatabyID(request.IdBenhNhan.ToString());
            if (benhNhan == null) return null;

            decimal tyLeGoc = benhNhan.MucHuong ?? 0.8m; 
            
            // QUY TRÌNH BỆNH VIỆN:
            // Đúng tuyến nếu: 
            // 1. Tích chọn đúng tuyến
            // 2. HOẶC là ca Cấp cứu
            // 3. HOẶC có Giấy chuyển viện hợp lệ
            bool tinhDungTuyen = request.DungTuyen || request.LaCapCuu || request.CoGiayChuyenVien;

            decimal tyLeThucTe = tinhDungTuyen ? tyLeGoc : (tyLeGoc * 0.4m);
            
            decimal bhTra = request.TongTien * tyLeThucTe;
            
            string lyDo = tinhDungTuyen ? "Đúng tuyến" : "Trái tuyến (40% nội trú)";
            if (request.LaCapCuu) lyDo = "Đúng tuyến (Cấp cứu)";
            else if (request.CoGiayChuyenVien) lyDo = "Đúng tuyến (Có giấy chuyển viện)";

            return new KetQuaTinhPhiBHYT
            {
                TongTien = request.TongTien,
                BaoHiemChiTra = bhTra,
                BenhNhanPhaiTra = request.TongTien - bhTra,
                TyLeHuong = tyLeThucTe,
                DienGiai = $"{lyDo} - Tỷ lệ thực: {tyLeThucTe * 100}%"
            };
        }
    }
}
