using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YtaService.BLL.Interfaces;
using YtaService.DAL.Interfaces;
using YtaService.DTO;

namespace YtaService.BLL
{
    public class HoaDonReportBusiness : IHoaDonReportBusiness
    {
        private readonly IHoaDonRepository _repo;

        public HoaDonReportBusiness(IHoaDonRepository repo)
        {
            _repo = repo;
        }

        public byte[] ExportHoaDonPdf(Guid hoaDonId)
        {
            var hoaDon = _repo.GetById(hoaDonId);
            if (hoaDon == null) return null;

            // Sử dụng QuestPDF để tạo file PDF
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Text("HÓA ĐƠN VIỆN PHÍ")
                        .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium).AlignCenter();

                    page.Content().PaddingVertical(10).Column(column =>
                    {
                        column.Spacing(5);
                        column.Item().Text($"Mã hóa đơn: {hoaDon.Id}");
                        column.Item().Text($"Bệnh nhân: {hoaDon.TenBenhNhan}");
                        column.Item().Text($"Ngày nhập viện: {hoaDon.NgayNhapVien?.ToString("dd/MM/yyyy HH:mm") ?? "N/A"}");
                        column.Item().Text($"Ngày xuất viện: {hoaDon.NgayXuatVien?.ToString("dd/MM/yyyy HH:mm") ?? "N/A"}");
                        column.Item().Text($"Ngày xuất hóa đơn: {hoaDon.Ngay?.ToString("dd/MM/yyyy HH:mm")}");
                        
                        column.Item().LineHorizontal(1);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.ConstantColumn(100);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Nội dung");
                                header.Cell().AlignRight().Text("Số tiền (VNĐ)");
                            });

                            table.Cell().Text("Tổng chi phí dịch vụ & giường bệnh");
                            table.Cell().AlignRight().Text(hoaDon.TongTien.ToString("N0"));

                            table.Cell().Text("Bảo hiểm chi trả");
                            table.Cell().AlignRight().Text($"- {hoaDon.BaoHiemChiTra.ToString("N0")}");

                            table.Cell().Text("Bệnh nhân thực trả").SemiBold();
                            table.Cell().AlignRight().Text(hoaDon.BenhNhanThanhToan.ToString("N0")).SemiBold();
                        });

                        column.Item().PaddingTop(20).Text("Trạng thái: " + hoaDon.TrangThai)
                            .Italic().AlignCenter();
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Trang ");
                        x.CurrentPageNumber();
                    });
                });
            });

            return document.GeneratePdf();
        }

        public byte[] ExportDanhSachHoaDonExcel(Guid? benhNhanId, Guid? nhapVienId)
        {
            var list = _repo.LayDanhSach(benhNhanId, nhapVienId);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("DanhSachHoaDon");
                
                // Tiêu đề cột
                worksheet.Cell(1, 1).Value = "Mã Hóa Đơn";
                worksheet.Cell(1, 2).Value = "Tên Bệnh Nhân";
                worksheet.Cell(1, 3).Value = "Ngày Nhập";
                worksheet.Cell(1, 4).Value = "Ngày Xuất";
                worksheet.Cell(1, 5).Value = "Tổng Tiền";
                worksheet.Cell(1, 6).Value = "Bảo Hiểm";
                worksheet.Cell(1, 7).Value = "Thực Trả";
                worksheet.Cell(1, 8).Value = "Ngày Lập HĐ";
                worksheet.Cell(1, 9).Value = "Trạng Thái";

                // Định dạng header
                var headerRange = worksheet.Range(1, 1, 1, 9);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // Dữ liệu
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    int row = i + 2;
                    worksheet.Cell(row, 1).Value = item.Id.ToString();
                    worksheet.Cell(row, 2).Value = item.TenBenhNhan;
                    worksheet.Cell(row, 3).Value = item.NgayNhapVien?.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 4).Value = item.NgayXuatVien?.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 5).Value = item.TongTien;
                    worksheet.Cell(row, 6).Value = item.BaoHiemChiTra;
                    worksheet.Cell(row, 7).Value = item.BenhNhanThanhToan;
                    worksheet.Cell(row, 8).Value = item.Ngay?.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 9).Value = item.TrangThai;

                    // Định dạng số
                    worksheet.Cell(row, 5).Style.NumberFormat.Format = "#,##0";
                    worksheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0";
                    worksheet.Cell(row, 7).Style.NumberFormat.Format = "#,##0";
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }


        public byte[] ExportHoaDonExcel(Guid hoaDonId)
        {
            var item = _repo.GetById(hoaDonId);
            if (item == null) return null;

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("HoaDonChiTiet");

                // Tiêu đề
                worksheet.Cell(1, 1).Value = "Mã Hóa Đơn";
                worksheet.Cell(1, 2).Value = item.Id.ToString();
                
                worksheet.Cell(2, 1).Value = "Tên Bệnh Nhân";
                worksheet.Cell(2, 2).Value = item.TenBenhNhan;

                worksheet.Cell(3, 1).Value = "Ngày Nhập Viện";
                worksheet.Cell(3, 2).Value = item.NgayNhapVien?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";

                worksheet.Cell(4, 1).Value = "Ngày Xuất Viện";
                worksheet.Cell(4, 2).Value = item.NgayXuatVien?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";

                worksheet.Cell(5, 1).Value = "Tổng Chi Phí";
                worksheet.Cell(5, 2).Value = item.TongTien;
                worksheet.Cell(5, 2).Style.NumberFormat.Format = "#,##0";

                worksheet.Cell(6, 1).Value = "Bảo Hiểm Chi Trả";
                worksheet.Cell(6, 2).Value = item.BaoHiemChiTra;
                worksheet.Cell(6, 2).Style.NumberFormat.Format = "#,##0";

                worksheet.Cell(7, 1).Value = "Bệnh Nhân Thực Trả";
                worksheet.Cell(7, 2).Value = item.BenhNhanThanhToan;
                worksheet.Cell(7, 2).Style.NumberFormat.Format = "#,##0";
                worksheet.Cell(7, 2).Style.Font.Bold = true;

                worksheet.Cell(8, 1).Value = "Ngày Lập HĐ";
                worksheet.Cell(8, 2).Value = item.Ngay?.ToString("dd/MM/yyyy HH:mm");

                worksheet.Cell(9, 1).Value = "Trạng Thái";
                worksheet.Cell(9, 2).Value = item.TrangThai;

                // Format cột tiêu đề bên trái
                var titleRange = worksheet.Range(1, 1, 9, 1);
                titleRange.Style.Font.Bold = true;
                titleRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public async Task<string> ImportHoaDonFromExcel(byte[] fileContent)
        {
            try
            {
                using (var stream = new MemoryStream(fileContent))
                {
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1);
                        var rows = worksheet.RowsUsed().Skip(1); // Bỏ qua tiêu đề
                        int count = 0;

                        foreach (var row in rows)
                        {
                            try
                            {
                                var dto = new HoaDonCreateDTO
                                {
                                    BenhNhanId = Guid.Parse(row.Cell(1).GetString()),
                                    NhapVienId = Guid.Parse(row.Cell(2).GetString()),
                                    TongTien = row.Cell(3).GetValue<decimal>(),
                                    BaoHiemChiTra = row.Cell(4).GetValue<decimal?>(),
                                    GhiChu = "Imported from Excel"
                                };

                                _repo.TaoHoaDon(dto);
                                count++;
                            }
                            catch (Exception)
                            {
                                // Bỏ qua dòng lỗi hoặc log lại
                            }
                        }
                        return $"Đã nhập thành công {count} hóa đơn từ file Excel.";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Lỗi khi nhập Excel: {ex.Message}";
            }
        }
    }
}
