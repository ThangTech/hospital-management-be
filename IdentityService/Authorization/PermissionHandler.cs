using Microsoft.AspNetCore.Authorization;

namespace IdentityService.Authorization;

/// <summary>
/// Attribute để yêu cầu quyền cụ thể
/// Sử dụng: [HasPermission(Permissions.BenhNhan_Xem)]
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission) : base(policy: permission)
    {
    }
}

/// <summary>
/// Requirement cho permission-based authorization
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}

/// <summary>
/// Handler xử lý kiểm tra quyền
/// </summary>
public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        PermissionRequirement requirement)
    {
        // Lấy danh sách permissions từ claims
        var permissionClaims = context.User.Claims
            .Where(c => c.Type == "permissions")
            .Select(c => c.Value)
            .ToList();

        // Kiểm tra user có quyền yêu cầu không
        if (permissionClaims.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
