using AdminService.DTOs;

namespace AdminService.BLL;

public interface IExportService
{
    byte[] ExportBedCapacityToExcel(BedCapacityReportDTO data);
    byte[] ExportTreatmentCostToExcel(TreatmentCostReportDTO data);
    byte[] ExportBedCapacityToPdf(BedCapacityReportDTO data);
    byte[] ExportTreatmentCostToPdf(TreatmentCostReportDTO data);
}
