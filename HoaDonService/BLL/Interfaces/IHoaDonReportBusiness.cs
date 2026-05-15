using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HoaDonService.BLL.Interfaces
{
    public interface IHoaDonReportBusiness
    {
        byte[] ExportHoaDonPdf(Guid hoaDonId);
        byte[] ExportDanhSachHoaDonExcel(Guid? benhNhanId, Guid? nhapVienId);
        byte[] ExportHoaDonExcel(Guid hoaDonId);
        Task<string> ImportHoaDonFromExcel(byte[] fileContent);
    }
}
