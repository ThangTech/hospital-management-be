using AdminService.DTOs;

namespace AdminService.BLL;

public interface IAuditService
{
    PagedResult<NhatKyHeThongDTO> GetNhatKyHeThong(AuditSearchDTO search);
    PagedResult<AuditHoSoBenhAnDTO> GetAuditHoSoBenhAn(AuditSearchDTO search);
}
