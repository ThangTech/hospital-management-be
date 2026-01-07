namespace AdminService.DTOs;

// ==================== FILTER DTO ====================
public class ReportFilterDTO
{
    public DateTime? TuNgay { get; set; }
    public DateTime? DenNgay { get; set; }
    public Guid? KhoaId { get; set; }
}

// ==================== BED CAPACITY REPORT ====================
public class BedCapacityReportDTO
{
    public List<DepartmentBedStatsDTO> DepartmentStats { get; set; } = new();
    public BedSummaryDTO Summary { get; set; } = new();
}

public class DepartmentBedStatsDTO
{
    public Guid KhoaId { get; set; }
    public string? TenKhoa { get; set; }
    public int TongGiuong { get; set; }
    public int GiuongDangSuDung { get; set; }
    public int GiuongTrong { get; set; }
    public decimal TyLeSuDung { get; set; } // Percentage
}

public class BedSummaryDTO
{
    public int TongGiuongToanVien { get; set; }
    public int TongGiuongDangSuDung { get; set; }
    public int TongGiuongTrong { get; set; }
    public decimal TyLeSuDungTrungBinh { get; set; }
}

// ==================== TREATMENT COST REPORT ====================
public class TreatmentCostReportDTO
{
    public List<DepartmentCostDTO> ChiPhiTheoKhoa { get; set; } = new();
    public List<ServiceTypeCostDTO> ChiPhiTheoLoaiDichVu { get; set; } = new();
    public CostSummaryDTO Summary { get; set; } = new();
}

public class DepartmentCostDTO
{
    public Guid KhoaId { get; set; }
    public string? TenKhoa { get; set; }
    public decimal TongChiPhiDichVu { get; set; }
    public decimal TongChiPhiPhauThuat { get; set; }
    public decimal TongChiPhiXetNghiem { get; set; }
    public decimal TongCong { get; set; }
    public int SoLuotDieuTri { get; set; }
}

public class ServiceTypeCostDTO
{
    public string LoaiDichVu { get; set; } = string.Empty;
    public decimal TongChiPhi { get; set; }
    public int SoLuong { get; set; }
}

public class CostSummaryDTO
{
    public decimal TongChiPhiDichVuDieuTri { get; set; }
    public decimal TongChiPhiPhauThuat { get; set; }
    public decimal TongChiPhiXetNghiem { get; set; }
    public decimal TongChiPhiToanBo { get; set; }
    public decimal ChiPhiTrungBinhMoiLuot { get; set; }
    public int TongSoLuotDieuTri { get; set; }
}
