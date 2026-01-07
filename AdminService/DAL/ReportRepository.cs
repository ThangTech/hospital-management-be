using AdminService.DTOs;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AdminService.DAL;

public class ReportRepository : IReportRepository
{
    private readonly string? _connectionString;

    public ReportRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public BedCapacityReportDTO GetBedCapacityReport(ReportFilterDTO? filter)
    {
        var result = new BedCapacityReportDTO();

        if (string.IsNullOrEmpty(_connectionString)) return result;

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_GetBedCapacityReport", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        // Parameters
        cmd.Parameters.AddWithValue("@KhoaId", (object?)filter?.KhoaId ?? DBNull.Value);

        conn.Open();
        using var reader = cmd.ExecuteReader();

        int totalBeds = 0, totalOccupied = 0, totalAvailable = 0;

        while (reader.Read())
        {
            var tongGiuong = reader["TongGiuong"] != DBNull.Value ? Convert.ToInt32(reader["TongGiuong"]) : 0;
            var dangSuDung = reader["GiuongDangSuDung"] != DBNull.Value ? Convert.ToInt32(reader["GiuongDangSuDung"]) : 0;
            var giuongTrong = reader["GiuongTrong"] != DBNull.Value ? Convert.ToInt32(reader["GiuongTrong"]) : 0;

            result.DepartmentStats.Add(new DepartmentBedStatsDTO
            {
                KhoaId = (Guid)reader["KhoaId"],
                TenKhoa = reader["TenKhoa"] as string,
                TongGiuong = tongGiuong,
                GiuongDangSuDung = dangSuDung,
                GiuongTrong = giuongTrong,
                TyLeSuDung = tongGiuong > 0 ? Math.Round((decimal)dangSuDung / tongGiuong * 100, 2) : 0
            });

            totalBeds += tongGiuong;
            totalOccupied += dangSuDung;
            totalAvailable += giuongTrong;
        }

        result.Summary = new BedSummaryDTO
        {
            TongGiuongToanVien = totalBeds,
            TongGiuongDangSuDung = totalOccupied,
            TongGiuongTrong = totalAvailable,
            TyLeSuDungTrungBinh = totalBeds > 0 ? Math.Round((decimal)totalOccupied / totalBeds * 100, 2) : 0
        };

        return result;
    }

    public TreatmentCostReportDTO GetTreatmentCostReport(ReportFilterDTO? filter)
    {
        var result = new TreatmentCostReportDTO();

        if (string.IsNullOrEmpty(_connectionString)) return result;

        using var conn = new SqlConnection(_connectionString);
        conn.Open();

        // 1. Chi phí theo khoa - gọi stored procedure
        using (var cmd = new SqlCommand("sp_GetTreatmentCostByDepartment", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@KhoaId", (object?)filter?.KhoaId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TuNgay", (object?)filter?.TuNgay ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DenNgay", (object?)filter?.DenNgay ?? DBNull.Value);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var dichVu = reader["TongChiPhiDichVu"] != DBNull.Value ? Convert.ToDecimal(reader["TongChiPhiDichVu"]) : 0;
                var phauThuat = reader["TongChiPhiPhauThuat"] != DBNull.Value ? Convert.ToDecimal(reader["TongChiPhiPhauThuat"]) : 0;
                var xetNghiem = reader["TongChiPhiXetNghiem"] != DBNull.Value ? Convert.ToDecimal(reader["TongChiPhiXetNghiem"]) : 0;

                result.ChiPhiTheoKhoa.Add(new DepartmentCostDTO
                {
                    KhoaId = (Guid)reader["KhoaId"],
                    TenKhoa = reader["TenKhoa"] as string,
                    TongChiPhiDichVu = dichVu,
                    TongChiPhiPhauThuat = phauThuat,
                    TongChiPhiXetNghiem = xetNghiem,
                    TongCong = dichVu + phauThuat + xetNghiem,
                    SoLuotDieuTri = reader["SoLuotDieuTri"] != DBNull.Value ? Convert.ToInt32(reader["SoLuotDieuTri"]) : 0
                });
            }
        }

        // 2. Chi phí theo loại dịch vụ - gọi stored procedure
        using (var cmd = new SqlCommand("sp_GetTreatmentCostByServiceType", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@KhoaId", (object?)filter?.KhoaId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TuNgay", (object?)filter?.TuNgay ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DenNgay", (object?)filter?.DenNgay ?? DBNull.Value);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.ChiPhiTheoLoaiDichVu.Add(new ServiceTypeCostDTO
                {
                    LoaiDichVu = reader["LoaiDichVu"] as string ?? "Không xác định",
                    TongChiPhi = reader["TongChiPhi"] != DBNull.Value ? Convert.ToDecimal(reader["TongChiPhi"]) : 0,
                    SoLuong = reader["SoLuong"] != DBNull.Value ? Convert.ToInt32(reader["SoLuong"]) : 0
                });
            }
        }

        // 3. Tổng hợp
        var dichVuTotal = result.ChiPhiTheoKhoa.Sum(x => x.TongChiPhiDichVu);
        var phauThuatTotal = result.ChiPhiTheoKhoa.Sum(x => x.TongChiPhiPhauThuat);
        var xetNghiemTotal = result.ChiPhiTheoKhoa.Sum(x => x.TongChiPhiXetNghiem);
        var totalCost = dichVuTotal + phauThuatTotal + xetNghiemTotal;
        var totalAdmissions = result.ChiPhiTheoKhoa.Sum(x => x.SoLuotDieuTri);

        result.Summary = new CostSummaryDTO
        {
            TongChiPhiDichVuDieuTri = dichVuTotal,
            TongChiPhiPhauThuat = phauThuatTotal,
            TongChiPhiXetNghiem = xetNghiemTotal,
            TongChiPhiToanBo = totalCost,
            TongSoLuotDieuTri = totalAdmissions,
            ChiPhiTrungBinhMoiLuot = totalAdmissions > 0 ? Math.Round(totalCost / totalAdmissions, 2) : 0
        };

        return result;
    }
}
