using BacSiService.BLL.Interfaces;
using BacSiService.DAL.Interfaces;
using BacSiService.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Linq;

namespace BacSiService.BLL.Services
{
    public class MedicalRecordReportService : IMedicalRecordReportService
    {
        private readonly IMedicalRecordRepository _repo;

        public MedicalRecordReportService(IMedicalRecordRepository repo)
        {
            _repo = repo;
        }

        public byte[]? ExportMedicalRecordPdf(Guid id)
        {
            var record = _repo.GetAll().FirstOrDefault(x => x.Id == id);
            if (record == null) return null;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.2f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Content().Column(column =>
                    {
                        column.Spacing(8);

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(left =>
                            {
                                left.Item().Text("Cơ quan chủ quản: ....................");
                                left.Item().Text("Cơ sở KCB: .............................");
                            });
                            row.RelativeItem().Column(mid =>
                            {
                                mid.Item().AlignCenter().Text("CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM").Bold();
                                mid.Item().AlignCenter().Text("Độc lập - Tự do - Hạnh phúc").Underline();
                            });
                            row.ConstantItem(80).AlignRight().Text("MS: 52/BV2");
                        });

                        column.Item().PaddingTop(12).AlignCenter().Text("BẢN TÓM TẮT HỒ SƠ BỆNH ÁN").Bold().FontSize(14);

                        Section(column, "I. HÀNH CHÍNH");
                        Line(column, "Họ và tên", record.TenBenhNhan, "Ngày sinh", Date(record.NgaySinhBenhNhan));
                        Line(column, "Giới tính", record.GioiTinh, "Số thẻ BHYT", record.SoTheBaoHiem);
                        Line(column, "Địa chỉ", record.DiaChi, "Khoa", record.TenKhoa);
                        Line(column, "Vào viện", Date(record.NgayNhap), "Ra viện", Date(record.NgayXuat));
                        Line(column, "Bác sĩ phụ trách", record.TenBacSi, "Giường", record.TenGiuong);

                        Section(column, "II. CHẨN ĐOÁN");
                        Field(column, "Lý do vào viện", record.LyDoNhap);
                        Field(column, "Chẩn đoán vào viện", record.ChanDoanBanDau);
                        Field(column, "Chẩn đoán ra viện", record.ChanDoanRaVien);

                        Section(column, "III. TÓM TẮT QUÁ TRÌNH ĐIỀU TRỊ");
                        Field(column, "Tiền sử bệnh", record.TienSuBenh);
                        Field(column, "Phương án điều trị", record.PhuongAnDieuTri);
                        Field(column, "Kết quả điều trị", record.KetQuaDieuTri);

                        column.Item().PaddingTop(24).Row(row =>
                        {
                            row.RelativeItem().Text("");
                            row.ConstantItem(220).AlignCenter().Column(sign =>
                            {
                                sign.Item().Text($"Ngày {DateTime.Now:dd} tháng {DateTime.Now:MM} năm {DateTime.Now:yyyy}");
                                sign.Item().Text("Bác sĩ điều trị").Bold();
                                sign.Item().PaddingTop(50).Text(record.TenBacSi ?? "");
                            });
                        });
                    });
                });
            }).GeneratePdf();
        }

        private static void Section(ColumnDescriptor column, string title)
        {
            column.Item().PaddingTop(6).Text(title).Bold();
        }

        private static void Field(ColumnDescriptor column, string label, string? value)
        {
            column.Item().Text(text =>
            {
                text.Span(label + ": ").Bold();
                text.Span(string.IsNullOrWhiteSpace(value) ? "........................................................" : value);
            });
        }

        private static void Line(ColumnDescriptor column, string label1, string? value1, string label2, string? value2)
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Text(text =>
                {
                    text.Span(label1 + ": ").Bold();
                    text.Span(string.IsNullOrWhiteSpace(value1) ? "...................." : value1);
                });
                row.RelativeItem().Text(text =>
                {
                    text.Span(label2 + ": ").Bold();
                    text.Span(string.IsNullOrWhiteSpace(value2) ? "...................." : value2);
                });
            });
        }

        private static string? Date(DateTime? value) => value?.ToString("dd/MM/yyyy");
    }
}
