using BenhNhanService.BLL.Interfaces;
using BenhNhanService.DAL.Interfaces;
using ClosedXML.Excel;

namespace BenhNhanService.BLL
{
    public class ExportBenhNhanService : IExportBenhNhanService
    {
        private readonly IBenhNhanRepository _benhNhanRepository;

        public ExportBenhNhanService(IBenhNhanRepository benhNhanRepository)
        {
            _benhNhanRepository = benhNhanRepository;
        }

        public byte[] ExportBenhNhanToExcel()
        {
            var danhSach = _benhNhanRepository.GetAll();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Danh sách bệnh nhân");

            // ===== TIÊU ĐỀ BÁO CÁO =====
            worksheet.Cell(1, 1).Value = "DANH SÁCH BỆNH NHÂN";
            worksheet.Range("A1:J1").Merge();
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(2, 1).Value = $"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}";
            worksheet.Range("A2:J2").Merge();
            worksheet.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // ===== HEADER BẢNG =====
            int headerRow = 4;
            worksheet.Cell(headerRow, 1).Value = "STT";
            worksheet.Cell(headerRow, 2).Value = "Họ Tên";
            worksheet.Cell(headerRow, 3).Value = "Ngày Sinh";
            worksheet.Cell(headerRow, 4).Value = "Giới Tính";
            worksheet.Cell(headerRow, 5).Value = "Địa Chỉ";
            worksheet.Cell(headerRow, 6).Value = "Số Điện Thoại";
            worksheet.Cell(headerRow, 7).Value = "Số Thẻ BHYT";
            worksheet.Cell(headerRow, 8).Value = "Mức Hưởng BHYT";
            worksheet.Cell(headerRow, 9).Value = "Hạn Thẻ BHYT";
            worksheet.Cell(headerRow, 10).Value = "Trạng Thái";

            // Style header
            var headerRange = worksheet.Range(headerRow, 1, headerRow, 10);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1976D2");
            headerRange.Style.Font.FontColor = XLColor.White;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // ===== DỮ LIỆU =====
            int row = headerRow + 1;
            int stt = 1;

            foreach (var bn in danhSach)
            {
                worksheet.Cell(row, 1).Value = stt++;
                worksheet.Cell(row, 2).Value = bn.HoTen ?? "";
                worksheet.Cell(row, 3).Value = bn.NgaySinh.ToString("dd/MM/yyyy");
                worksheet.Cell(row, 4).Value = bn.GioiTinh ?? "";
                worksheet.Cell(row, 5).Value = bn.DiaChi ?? "";
                worksheet.Cell(row, 6).Value = bn.SoDienThoai ?? "";
                worksheet.Cell(row, 7).Value = bn.SoTheBaoHiem ?? "";

                // Định dạng mức hưởng ra %
                if (bn.MucHuong.HasValue)
                {
                    decimal pct = bn.MucHuong.Value <= 1
                        ? bn.MucHuong.Value * 100
                        : bn.MucHuong.Value;
                    worksheet.Cell(row, 8).Value = $"{pct:0}%";
                }
                else
                {
                    worksheet.Cell(row, 8).Value = "";
                }

                worksheet.Cell(row, 9).Value = bn.HanTheBHYT.HasValue
                    ? bn.HanTheBHYT.Value.ToString("dd/MM/yyyy")
                    : "";

                worksheet.Cell(row, 10).Value = bn.TrangThai ?? "";

                // Màu xen kẽ cho dễ đọc
                if (row % 2 == 0)
                {
                    worksheet.Range(row, 1, row, 10).Style.Fill.BackgroundColor = XLColor.FromHtml("#F5F5F5");
                }

                // Border từng dòng
                worksheet.Range(row, 1, row, 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(row, 1, row, 10).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                row++;
            }

            // ===== DÒNG TỔNG CỘNG =====
            row++;
            worksheet.Cell(row, 1).Value = $"Tổng số bệnh nhân: {danhSach.Count}";
            worksheet.Range(row, 1, row, 10).Merge();
            worksheet.Cell(row, 1).Style.Font.Bold = true;
            worksheet.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#E3F2FD");

            // ===== TỰ ĐIỀU CHỈNH ĐỘ RỘNG CỘT =====
            worksheet.Columns().AdjustToContents();

            // Đặt độ rộng tối thiểu cho các cột quan trọng
            worksheet.Column(2).Width = Math.Max(worksheet.Column(2).Width, 25); // Họ Tên
            worksheet.Column(5).Width = Math.Max(worksheet.Column(5).Width, 30); // Địa Chỉ

            // Cố định dòng header khi scroll
            worksheet.SheetView.FreezeRows(headerRow);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
