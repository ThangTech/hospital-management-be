using AdminService.DTOs;

namespace AdminService.DAL;

public interface IAuditRepository
{
    PagedResult<NhatKyHeThongDTO> GetNhatKyHeThong(AuditSearchDTO search);
    PagedResult<AuditHoSoBenhAnDTO> GetAuditHoSoBenhAn(AuditSearchDTO search);
}
