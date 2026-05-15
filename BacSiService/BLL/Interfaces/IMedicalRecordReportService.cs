using System;

namespace BacSiService.BLL.Interfaces
{
    public interface IMedicalRecordReportService
    {
        byte[]? ExportMedicalRecordPdf(Guid id);
    }
}
