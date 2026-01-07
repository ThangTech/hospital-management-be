using AdminService.DTOs;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AdminService.BLL;

public class ExportService : IExportService
{
    public ExportService()
    {
        // Thiết lập license cho QuestPDF (Community - miễn phí)
        QuestPDF.Settings.License = LicenseType.Community;
    }

    #region Excel Export

    public byte[] ExportBedCapacityToExcel(BedCapacityReportDTO data)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Công suất giường");

        // Tiêu đề
        worksheet.Cell(1, 1).Value = "BÁO CÁO CÔNG SUẤT GIƯỜNG BỆNH";
        worksheet.Range("A1:F1").Merge().Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontSize = 16;
        worksheet.Cell(2, 1).Value = $"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}";

        // Header bảng
        var headerRow = 4;
        worksheet.Cell(headerRow, 1).Value = "STT";
        worksheet.Cell(headerRow, 2).Value = "Tên Khoa";
        worksheet.Cell(headerRow, 3).Value = "Tổng Giường";
        worksheet.Cell(headerRow, 4).Value = "Đang Sử Dụng";
        worksheet.Cell(headerRow, 5).Value = "Giường Trống";
        worksheet.Cell(headerRow, 6).Value = "Tỷ Lệ Sử Dụng (%)";

        var headerRange = worksheet.Range(headerRow, 1, headerRow, 6);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        // Dữ liệu
        var row = headerRow + 1;
        var stt = 1;
        foreach (var dept in data.DepartmentStats)
        {
            worksheet.Cell(row, 1).Value = stt++;
            worksheet.Cell(row, 2).Value = dept.TenKhoa;
            worksheet.Cell(row, 3).Value = dept.TongGiuong;
            worksheet.Cell(row, 4).Value = dept.GiuongDangSuDung;
            worksheet.Cell(row, 5).Value = dept.GiuongTrong;
            worksheet.Cell(row, 6).Value = dept.TyLeSuDung;
            row++;
        }

        // Tổng hợp
        row++;
        worksheet.Cell(row, 1).Value = "TỔNG CỘNG";
        worksheet.Range(row, 1, row, 2).Merge().Style.Font.Bold = true;
        worksheet.Cell(row, 3).Value = data.Summary.TongGiuongToanVien;
        worksheet.Cell(row, 4).Value = data.Summary.TongGiuongDangSuDung;
        worksheet.Cell(row, 5).Value = data.Summary.TongGiuongTrong;
        worksheet.Cell(row, 6).Value = data.Summary.TyLeSuDungTrungBinh;
        worksheet.Range(row, 1, row, 6).Style.Font.Bold = true;

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] ExportTreatmentCostToExcel(TreatmentCostReportDTO data)
    {
        using var workbook = new XLWorkbook();
        
        // Sheet 1: Chi phí theo khoa
        var ws1 = workbook.Worksheets.Add("Chi phí theo Khoa");
        ws1.Cell(1, 1).Value = "BÁO CÁO CHI PHÍ ĐIỀU TRỊ THEO KHOA";
        ws1.Range("A1:G1").Merge().Style.Font.Bold = true;
        ws1.Cell(1, 1).Style.Font.FontSize = 16;
        ws1.Cell(2, 1).Value = $"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}";

        var headerRow = 4;
        ws1.Cell(headerRow, 1).Value = "STT";
        ws1.Cell(headerRow, 2).Value = "Tên Khoa";
        ws1.Cell(headerRow, 3).Value = "Chi phí Dịch vụ";
        ws1.Cell(headerRow, 4).Value = "Chi phí Phẫu thuật";
        ws1.Cell(headerRow, 5).Value = "Chi phí Xét nghiệm";
        ws1.Cell(headerRow, 6).Value = "Tổng Cộng";
        ws1.Cell(headerRow, 7).Value = "Số Lượt";

        var headerRange = ws1.Range(headerRow, 1, headerRow, 7);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGreen;

        var row = headerRow + 1;
        var stt = 1;
        foreach (var dept in data.ChiPhiTheoKhoa)
        {
            ws1.Cell(row, 1).Value = stt++;
            ws1.Cell(row, 2).Value = dept.TenKhoa;
            ws1.Cell(row, 3).Value = dept.TongChiPhiDichVu;
            ws1.Cell(row, 4).Value = dept.TongChiPhiPhauThuat;
            ws1.Cell(row, 5).Value = dept.TongChiPhiXetNghiem;
            ws1.Cell(row, 6).Value = dept.TongCong;
            ws1.Cell(row, 7).Value = dept.SoLuotDieuTri;
            
            // Format số tiền
            ws1.Cell(row, 3).Style.NumberFormat.Format = "#,##0";
            ws1.Cell(row, 4).Style.NumberFormat.Format = "#,##0";
            ws1.Cell(row, 5).Style.NumberFormat.Format = "#,##0";
            ws1.Cell(row, 6).Style.NumberFormat.Format = "#,##0";
            row++;
        }

        // Tổng cộng
        row++;
        ws1.Cell(row, 1).Value = "TỔNG CỘNG";
        ws1.Range(row, 1, row, 2).Merge().Style.Font.Bold = true;
        ws1.Cell(row, 3).Value = data.Summary.TongChiPhiDichVuDieuTri;
        ws1.Cell(row, 4).Value = data.Summary.TongChiPhiPhauThuat;
        ws1.Cell(row, 5).Value = data.Summary.TongChiPhiXetNghiem;
        ws1.Cell(row, 6).Value = data.Summary.TongChiPhiToanBo;
        ws1.Cell(row, 7).Value = data.Summary.TongSoLuotDieuTri;
        ws1.Range(row, 1, row, 7).Style.Font.Bold = true;

        ws1.Columns().AdjustToContents();

        // Sheet 2: Chi phí theo loại dịch vụ
        var ws2 = workbook.Worksheets.Add("Chi phí theo Loại DV");
        ws2.Cell(1, 1).Value = "CHI PHÍ THEO LOẠI DỊCH VỤ";
        ws2.Range("A1:C1").Merge().Style.Font.Bold = true;

        ws2.Cell(3, 1).Value = "Loại Dịch Vụ";
        ws2.Cell(3, 2).Value = "Tổng Chi Phí";
        ws2.Cell(3, 3).Value = "Số Lượng";
        ws2.Range("A3:C3").Style.Font.Bold = true;
        ws2.Range("A3:C3").Style.Fill.BackgroundColor = XLColor.LightYellow;

        row = 4;
        foreach (var svc in data.ChiPhiTheoLoaiDichVu)
        {
            ws2.Cell(row, 1).Value = svc.LoaiDichVu;
            ws2.Cell(row, 2).Value = svc.TongChiPhi;
            ws2.Cell(row, 3).Value = svc.SoLuong;
            ws2.Cell(row, 2).Style.NumberFormat.Format = "#,##0";
            row++;
        }

        ws2.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    #endregion

    #region PDF Export

    public byte[] ExportBedCapacityToPdf(BedCapacityReportDTO data)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Text("BÁO CÁO CÔNG SUẤT GIƯỜNG BỆNH")
                    .FontSize(18).Bold().AlignCenter();

                page.Content().PaddingVertical(10).Column(column =>
                {
                    column.Item().Text($"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}");
                    column.Item().PaddingTop(10);

                    // Bảng thống kê
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);  // STT
                            columns.RelativeColumn(3);   // Tên Khoa
                            columns.RelativeColumn(1);   // Tổng Giường
                            columns.RelativeColumn(1);   // Đang Sử Dụng
                            columns.RelativeColumn(1);   // Trống
                            columns.RelativeColumn(1);   // Tỷ Lệ
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Blue.Lighten3).Padding(5).Text("STT").Bold();
                            header.Cell().Background(Colors.Blue.Lighten3).Padding(5).Text("Tên Khoa").Bold();
                            header.Cell().Background(Colors.Blue.Lighten3).Padding(5).Text("Tổng").Bold();
                            header.Cell().Background(Colors.Blue.Lighten3).Padding(5).Text("Đang Dùng").Bold();
                            header.Cell().Background(Colors.Blue.Lighten3).Padding(5).Text("Trống").Bold();
                            header.Cell().Background(Colors.Blue.Lighten3).Padding(5).Text("Tỷ Lệ %").Bold();
                        });

                        // Data
                        var stt = 1;
                        foreach (var dept in data.DepartmentStats)
                        {
                            table.Cell().BorderBottom(1).Padding(5).Text(stt++.ToString());
                            table.Cell().BorderBottom(1).Padding(5).Text(dept.TenKhoa ?? "");
                            table.Cell().BorderBottom(1).Padding(5).Text(dept.TongGiuong.ToString());
                            table.Cell().BorderBottom(1).Padding(5).Text(dept.GiuongDangSuDung.ToString());
                            table.Cell().BorderBottom(1).Padding(5).Text(dept.GiuongTrong.ToString());
                            table.Cell().BorderBottom(1).Padding(5).Text($"{dept.TyLeSuDung}%");
                        }
                    });

                    // Tổng kết
                    column.Item().PaddingTop(20).Background(Colors.Grey.Lighten3).Padding(10).Column(summary =>
                    {
                        summary.Item().Text("TỔNG KẾT").Bold().FontSize(12);
                        summary.Item().Text($"• Tổng giường toàn viện: {data.Summary.TongGiuongToanVien}");
                        summary.Item().Text($"• Giường đang sử dụng: {data.Summary.TongGiuongDangSuDung}");
                        summary.Item().Text($"• Giường trống: {data.Summary.TongGiuongTrong}");
                        summary.Item().Text($"• Tỷ lệ sử dụng trung bình: {data.Summary.TyLeSuDungTrungBinh}%");
                    });
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Trang ");
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }

    public byte[] ExportTreatmentCostToPdf(TreatmentCostReportDTO data)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Text("BÁO CÁO CHI PHÍ ĐIỀU TRỊ")
                    .FontSize(18).Bold().AlignCenter();

                page.Content().PaddingVertical(10).Column(column =>
                {
                    column.Item().Text($"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}");
                    column.Item().PaddingTop(10);

                    // Bảng chi phí theo khoa
                    column.Item().Text("Chi phí theo Khoa").Bold().FontSize(12);
                    column.Item().PaddingTop(5).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);  // STT
                            columns.RelativeColumn(3);   // Tên Khoa
                            columns.RelativeColumn(2);   // Dịch vụ
                            columns.RelativeColumn(2);   // Phẫu thuật
                            columns.RelativeColumn(2);   // Xét nghiệm
                            columns.RelativeColumn(2);   // Tổng
                            columns.RelativeColumn(1);   // Số lượt
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Green.Lighten3).Padding(5).Text("STT").Bold();
                            header.Cell().Background(Colors.Green.Lighten3).Padding(5).Text("Tên Khoa").Bold();
                            header.Cell().Background(Colors.Green.Lighten3).Padding(5).Text("Dịch Vụ").Bold();
                            header.Cell().Background(Colors.Green.Lighten3).Padding(5).Text("Phẫu Thuật").Bold();
                            header.Cell().Background(Colors.Green.Lighten3).Padding(5).Text("Xét Nghiệm").Bold();
                            header.Cell().Background(Colors.Green.Lighten3).Padding(5).Text("Tổng Cộng").Bold();
                            header.Cell().Background(Colors.Green.Lighten3).Padding(5).Text("Số Lượt").Bold();
                        });

                        var stt = 1;
                        foreach (var dept in data.ChiPhiTheoKhoa)
                        {
                            table.Cell().BorderBottom(1).Padding(5).Text(stt++.ToString());
                            table.Cell().BorderBottom(1).Padding(5).Text(dept.TenKhoa ?? "");
                            table.Cell().BorderBottom(1).Padding(5).Text(dept.TongChiPhiDichVu.ToString("N0"));
                            table.Cell().BorderBottom(1).Padding(5).Text(dept.TongChiPhiPhauThuat.ToString("N0"));
                            table.Cell().BorderBottom(1).Padding(5).Text(dept.TongChiPhiXetNghiem.ToString("N0"));
                            table.Cell().BorderBottom(1).Padding(5).Text(dept.TongCong.ToString("N0"));
                            table.Cell().BorderBottom(1).Padding(5).Text(dept.SoLuotDieuTri.ToString());
                        }
                    });

                    // Tổng kết
                    column.Item().PaddingTop(20).Background(Colors.Grey.Lighten3).Padding(10).Column(summary =>
                    {
                        summary.Item().Text("TỔNG KẾT").Bold().FontSize(12);
                        summary.Item().Text($"• Tổng chi phí dịch vụ điều trị: {data.Summary.TongChiPhiDichVuDieuTri:N0} VNĐ");
                        summary.Item().Text($"• Tổng chi phí phẫu thuật: {data.Summary.TongChiPhiPhauThuat:N0} VNĐ");
                        summary.Item().Text($"• Tổng chi phí xét nghiệm: {data.Summary.TongChiPhiXetNghiem:N0} VNĐ");
                        summary.Item().Text($"• TỔNG CHI PHÍ: {data.Summary.TongChiPhiToanBo:N0} VNĐ").Bold();
                        summary.Item().Text($"• Chi phí trung bình mỗi lượt: {data.Summary.ChiPhiTrungBinhMoiLuot:N0} VNĐ");
                        summary.Item().Text($"• Tổng số lượt điều trị: {data.Summary.TongSoLuotDieuTri}");
                    });
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Trang ");
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }

    #endregion
}
