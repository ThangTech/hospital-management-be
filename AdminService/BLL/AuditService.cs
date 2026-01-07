using AdminService.DAL;
using AdminService.DTOs;

namespace AdminService.BLL;

public class AuditService : IAuditService
{
    private readonly IAuditRepository _repo;

    public AuditService(IAuditRepository repo)
    {
        _repo = repo;
    }

    public PagedResult<NhatKyHeThongDTO> GetNhatKyHeThong(AuditSearchDTO search)
    {
        // Validate pagination
        if (search.PageNumber < 1) search.PageNumber = 1;
        if (search.PageSize < 1) search.PageSize = 20;
        if (search.PageSize > 100) search.PageSize = 100; // Limit max page size

        return _repo.GetNhatKyHeThong(search);
    }

    public PagedResult<AuditHoSoBenhAnDTO> GetAuditHoSoBenhAn(AuditSearchDTO search)
    {
        if (search.PageNumber < 1) search.PageNumber = 1;
        if (search.PageSize < 1) search.PageSize = 20;
        if (search.PageSize > 100) search.PageSize = 100;

        return _repo.GetAuditHoSoBenhAn(search);
    }
}
