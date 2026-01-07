using AdminService.DAL;
using AdminService.DTOs;

namespace AdminService.BLL;

public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;

    public ReportService(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public BedCapacityReportDTO GetBedCapacityReport(ReportFilterDTO? filter)
    {
        return _reportRepository.GetBedCapacityReport(filter);
    }

    public TreatmentCostReportDTO GetTreatmentCostReport(ReportFilterDTO? filter)
    {
        return _reportRepository.GetTreatmentCostReport(filter);
    }
}
