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

            // S? d?ng QuestPDF d? t?o file PDF
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Text("HÓA ÐON VI?N PHÍ")
                        .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium).AlignCenter();

                    page.Content().PaddingVertical(10).Column(column =>
                    {
                        column.Spacing(5);
                        column.Item().Text($"Mã hóa don: {hoaDon.Id}");
                        column.Item().Text($"B?nh nhân: {hoaDon.TenBenhNhan}");
                        column.Item().Text($"Ngày nh?p vi?n: {hoaDon.NgayNhapVien?.ToString("dd/MM/yyyy HH:mm") ?? "N/A"}");
                        column.Item().Text($"Ngày xu?t vi?n: {hoaDon.NgayXuatVien?.ToString("dd/MM/yyyy HH:mm") ?? "N/A"}");
                        column.Item().Text($"Ngày xu?t hóa don: {hoaDon.Ngay?.ToString("dd/MM/yyyy HH:mm")}");
                        
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
                                header.Cell().Text("N?i dung");
                                header.Cell().AlignRight().Text("S? ti?n (VNÐ)");
                            });

                            table.Cell().Text("T?ng chi phí d?ch v? & giu?ng b?nh");
                            table.Cell().AlignRight().Text(hoaDon.TongTien.ToString("N0"));

                            table.Cell().Text("B?o hi?m chi tr?");
                            table.Cell().AlignRight().Text($"- {hoaDon.BaoHiemChiTra.ToString("N0")}");

                            table.Cell().Text("B?nh nhân th?c tr?").SemiBold();
                            table.Cell().AlignRight().Text(hoaDon.BenhNhanThanhToan.ToString("N0")).SemiBold();
                        });

                        column.Item().PaddingTop(20).Text("Tr?ng thái: " + hoaDon.TrangThai)
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
                
                // Tiêu d? c?t
                worksheet.Cell(1, 1).Value = "Mã Hóa Ðon";
                worksheet.Cell(1, 2).Value = "Tên B?nh Nhân";
                worksheet.Cell(1, 3).Value = "Ngày Nh?p";
                worksheet.Cell(1, 4).Value = "Ngày Xu?t";
                worksheet.Cell(1, 5).Value = "T?ng Ti?n";
                worksheet.Cell(1, 6).Value = "B?o Hi?m";
                worksheet.Cell(1, 7).Value = "Th?c Tr?";
                worksheet.Cell(1, 8).Value = "Ngày L?p HÐ";
                worksheet.Cell(1, 9).Value = "Tr?ng Thái";

                // Ð?nh d?ng header
                var headerRange = worksheet.Range(1, 1, 1, 9);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // D? li?u
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

                    // Ð?nh d?ng s?
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

                // Tiêu d?
                worksheet.Cell(1, 1).Value = "Mã Hóa Ðon";
                worksheet.Cell(1, 2).Value = item.Id.ToString();
                
                worksheet.Cell(2, 1).Value = "Tên B?nh Nhân";
                worksheet.Cell(2, 2).Value = item.TenBenhNhan;

                worksheet.Cell(3, 1).Value = "Ngày Nh?p Vi?n";
                worksheet.Cell(3, 2).Value = item.NgayNhapVien?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";

                worksheet.Cell(4, 1).Value = "Ngày Xu?t Vi?n";
                worksheet.Cell(4, 2).Value = item.NgayXuatVien?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";

                worksheet.Cell(5, 1).Value = "T?ng Chi Phí";
                worksheet.Cell(5, 2).Value = item.TongTien;
                worksheet.Cell(5, 2).Style.NumberFormat.Format = "#,##0";

                worksheet.Cell(6, 1).Value = "B?o Hi?m Chi Tr?";
                worksheet.Cell(6, 2).Value = item.BaoHiemChiTra;
                worksheet.Cell(6, 2).Style.NumberFormat.Format = "#,##0";

                worksheet.Cell(7, 1).Value = "B?nh Nhân Th?c Tr?";
                worksheet.Cell(7, 2).Value = item.BenhNhanThanhToan;
                worksheet.Cell(7, 2).Style.NumberFormat.Format = "#,##0";
                worksheet.Cell(7, 2).Style.Font.Bold = true;

                worksheet.Cell(8, 1).Value = "Ngày L?p HÐ";
                worksheet.Cell(8, 2).Value = item.Ngay?.ToString("dd/MM/yyyy HH:mm");

                worksheet.Cell(9, 1).Value = "Tr?ng Thái";
                worksheet.Cell(9, 2).Value = item.TrangThai;

                // Format c?t tiêu d? bên trái
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
                        var rows = worksheet.RowsUsed().Skip(1); // B? qua tiêu d?
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
                                // B? qua dòng l?i ho?c log l?i
                            }
                        }
                        return $"Ðã nh?p thành công {count} hóa don t? file Excel.";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"L?i khi nh?p Excel: {ex.Message}";
            }
        }
    }
}
