using AdminService.DTOs;

namespace AdminService.DAL;

public interface IReportRepository
{
    BedCapacityReportDTO GetBedCapacityReport(ReportFilterDTO? filter);
    TreatmentCostReportDTO GetTreatmentCostReport(ReportFilterDTO? filter);
}
