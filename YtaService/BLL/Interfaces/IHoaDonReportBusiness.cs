using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YtaService.BLL.Interfaces
{
    public interface IHoaDonReportBusiness
    {
        /// <summary>
        /// Xuất file PDF cho một hóa đơn cụ thể
        /// </summary>
        byte[] ExportHoaDonPdf(Guid hoaDonId);

        /// <summary>
        /// Xuất danh sách hóa đơn ra file Excel
        /// </summary>
        byte[] ExportDanhSachHoaDonExcel(Guid? benhNhanId, Guid? nhapVienId);


        /// <summary>
        /// Xuất một hóa đơn cụ thể ra file Excel
        /// </summary>
        byte[] ExportHoaDonExcel(Guid hoaDonId);

        /// <summary>
        /// Nhập dữ liệu từ file Excel (Ví dụ: danh sách hóa đơn)
        /// </summary>
        Task<string> ImportHoaDonFromExcel(byte[] fileContent);
    }
}
