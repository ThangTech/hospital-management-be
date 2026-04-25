using AdminService.DTOs;

namespace AdminService.BLL;

public interface IReportService
{
    BedCapacityReportDTO GetBedCapacityReport(ReportFilterDTO? filter);
    TreatmentCostReportDTO GetTreatmentCostReport(ReportFilterDTO? filter);
}
