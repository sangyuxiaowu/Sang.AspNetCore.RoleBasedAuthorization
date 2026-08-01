# Sang.AspNetCore.RoleBasedAuthorization

[![NuGet version (Sang.AspNetCore.RoleBasedAuthorization)](https://img.shields.io/nuget/v/Sang.AspNetCore.RoleBasedAuthorization.svg?style=flat-square)](https://www.nuget.org/packages/Sang.AspNetCore.RoleBasedAuthorization/)

Role-Based Authorization for ASP.NET.

> For the Chinese version, see [README.zh-CN.md](README.zh-CN.md).

## Instructions

##### Step 1

Add this package:

```bash
Install-Package Sang.AspNetCore.RoleBasedAuthorization
```

##### Step 2

Add RBAC services:

```csharp
builder.Services.AddSangRoleBasedAuthorization();
```

You can also configure the administrator role name. The default is `SangRBAC_Administrator`:

```csharp
builder.Services.AddSangRoleBasedAuthorization(options =>
{
    options.AdministratorRoleName = "Admin";
});
```

##### Step 3

Add the `ResourceAttribute` tag to the Controller or action that needs to be authorized:

```csharp
[Route("api/[controller]")]
[ApiController]
[ResourceModule("roles", "Role permissions")]
public class RolesController : ControllerBase
{
    [Resource("delete", "Delete role", "Allows deleting non-system roles")]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        return Ok();
    }
}
```

The resource and action keys are stable English identifiers and form the `roles.delete` permission code. Display names and descriptions stay next to the endpoint, making authorization intent self-documenting.

Use the complete form for an operation outside a controller's module:

```csharp
[Resource("weather", "read", "Weather", "View weather", "Allows viewing weather forecasts")]
```

##### Step 4

After completing the above operations, the authorization check will verify whether `User.Claims` contains the corresponding `Permission`.
The claims can be included directly in the JWT token, or the middleware described in the next section can load them by role or user identity before authorization. When using the middleware, the JWT does not need to contain `Permission` claims.

```csharp
var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, "uid"),
    new Claim(ClaimTypes.Name, "UserName"),
    new Claim(ClaimTypes.Email, "test@exp.com"),
    new Claim(ClaimTypes.Role, "user"),
};
var token = new JwtSecurityToken(
        "Issuer",
        "Audience",
        claims,
        expires: DateTime.UtcNow.AddSeconds(3600),
        signingCredentials: credentials
    );
```

> Note: If the role is named `SangRBAC_Administrator`, no authorization check will be done. This role name can be customized via `SangRoleBasedAuthorizationOptions.AdministratorRoleName`.

## Optional Features

Use the provided role-permission middleware. You can also use this component alone.

##### Step 1

Implement `IRolePermission` to get permissions by role and, optionally, permissions directly assigned to the current user:

```csharp
public class MyRolePermission : IRolePermission
{
    public Task<List<Claim>> GetRolePermissionClaimsByName(string roleName)
    {
        List<Claim> list = new();
        // your code
        return Task.FromResult(list);
    }

    public Task<List<Claim>> GetUserPermissionClaims(ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // Query permissions directly assigned to userId.
        return Task.FromResult(new List<Claim>());
    }
}
```

`GetUserPermissionClaims` has a default empty implementation, so existing role-only implementations do not need to change. Role and direct-user permissions are merged, deduplicated by Claim `Type` and `Value`, and added to the current request's `User` once.

Then add the service:

```csharp
builder.Services.AddRolePermission<MyRolePermission>();
```

##### Step 2

Enable this middleware before `app.UseAuthorization();` and after `app.UseAuthentication();`:

```csharp
app.UseAuthentication();
app.UseRolePermission();
app.UseAuthorization();
```

##### Options

`UseRolePermission` accepts the following options:

**1. `option.Always`**

Whether to always check and execute the addition. By default, the middleware only adds permissions when the current request has a `ResourceAttribute` to be verified.

## Wildcard Permission Matching

The authorization handler supports the following permission claim formats:

- `"roles"` — grants all actions under the resource.
- `"roles.delete"` — grants a specific action.
- `"roles.*"` — grants all actions under the resource (explicit wildcard).
- `"*"` — grants all resources and actions (global super-administrator permission).

## Resource Details

Use `ResourceData.GetResourceInfos()` to retrieve hierarchical metadata for permissions actually used by the application:

```csharp
var permissions = ResourceData.GetResourceInfos();
```

The result groups actions under their resource:

```json
[
    {
        "resourceKey": "values",
        "resourceName": "Values",
        "actions": [
            {
                "actionKey": "read",
                "actionName": "View values",
                "description": "Allows viewing value lists",
                "permission": "values.read"
            }
        ]
    }
]
```

### Frontend Localization

`ResourceName`, `ActionName`, and `Description` are default display text. Frontends can use `ResourceKey` and `Permission` as translation keys, then fall back to the backend text when a translation is absent. This avoids additional name-key fields.

```json
{
    "zh-CN": {
        "values": { "name": "数值" },
        "values.read": { "name": "查看数值", "description": "允许查看数值列表" }
    }
}
```

## Demo

- Simple Demo: https://github.com/sangyuxiaowu/Sang.AspNetCore.RoleBasedAuthorization/tree/main/TestDemo
- Used in Identity: https://github.com/sangyuxiaowu/IdentityRBAC
