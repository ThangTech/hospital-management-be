using AdminService.BLL;
using AdminService.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;
    private readonly IExportService _exportService;

    public ReportController(IReportService reportService, IExportService exportService)
    {
        _reportService = reportService;
        _exportService = exportService;
    }

    #region Báo cáo JSON

    /// <summary>
    /// Báo cáo công suất giường bệnh theo khoa
    /// </summary>
    [HttpPost("bed-capacity")]
    public ActionResult<BedCapacityReportDTO> GetBedCapacityReport([FromBody] ReportFilterDTO? filter)
    {
        var result = _reportService.GetBedCapacityReport(filter);
        return Ok(result);
    }

    /// <summary>
    /// Báo cáo chi phí điều trị
    /// </summary>
    [HttpPost("treatment-cost")]
    public ActionResult<TreatmentCostReportDTO> GetTreatmentCostReport([FromBody] ReportFilterDTO? filter)
    {
        var result = _reportService.GetTreatmentCostReport(filter);
        return Ok(result);
    }

    #endregion

    #region Export Excel

    /// <summary>
    /// Xuất báo cáo công suất giường ra file Excel
    /// </summary>
    [HttpPost("bed-capacity/export-excel")]
    public IActionResult ExportBedCapacityToExcel([FromBody] ReportFilterDTO? filter)
    {
        var data = _reportService.GetBedCapacityReport(filter);
        var bytes = _exportService.ExportBedCapacityToExcel(data);
        var fileName = $"BaoCao_CongSuatGiuong_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    /// <summary>
    /// Xuất báo cáo chi phí điều trị ra file Excel
    /// </summary>
    [HttpPost("treatment-cost/export-excel")]
    public IActionResult ExportTreatmentCostToExcel([FromBody] ReportFilterDTO? filter)
    {
        var data = _reportService.GetTreatmentCostReport(filter);
        var bytes = _exportService.ExportTreatmentCostToExcel(data);
        var fileName = $"BaoCao_ChiPhiDieuTri_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    #endregion

    #region Export PDF

    /// <summary>
    /// Xuất báo cáo công suất giường ra file PDF
    /// </summary>
    [HttpPost("bed-capacity/export-pdf")]
    public IActionResult ExportBedCapacityToPdf([FromBody] ReportFilterDTO? filter)
    {
        var data = _reportService.GetBedCapacityReport(filter);
        var bytes = _exportService.ExportBedCapacityToPdf(data);
        var fileName = $"BaoCao_CongSuatGiuong_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
        return File(bytes, "application/pdf", fileName);
    }

    /// <summary>
    /// Xuất báo cáo chi phí điều trị ra file PDF
    /// </summary>
    [HttpPost("treatment-cost/export-pdf")]
    public IActionResult ExportTreatmentCostToPdf([FromBody] ReportFilterDTO? filter)
    {
        var data = _reportService.GetTreatmentCostReport(filter);
        var bytes = _exportService.ExportTreatmentCostToPdf(data);
        var fileName = $"BaoCao_ChiPhiDieuTri_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
        return File(bytes, "application/pdf", fileName);
    }

    #endregion
}
