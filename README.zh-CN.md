# Sang.AspNetCore.RoleBasedAuthorization

[![NuGet version (Sang.AspNetCore.RoleBasedAuthorization)](https://img.shields.io/nuget/v/Sang.AspNetCore.RoleBasedAuthorization.svg?style=flat-square)](https://www.nuget.org/packages/Sang.AspNetCore.RoleBasedAuthorization/)

ASP.NET RBAC 权限管理

> 英文版本见 [README.md](README.md)。

## 使用说明

##### 步骤 1

安装 NuGet 包：

```bash
Install-Package Sang.AspNetCore.RoleBasedAuthorization
```

##### 步骤 2

添加 RBAC 服务：

```csharp
builder.Services.AddSangRoleBasedAuthorization();
```

也可以配置超级管理员角色名，默认值为 `SangRBAC_Administrator`：

```csharp
builder.Services.AddSangRoleBasedAuthorization(options =>
{
    options.AdministratorRoleName = "Admin";
});
```

##### 步骤 3

在需要进行授权检查的 Controller 或 Action 处添加 `ResourceAttribute` 标记：

```csharp
[Route("api/[controller]")]
[ApiController]
[ResourceModule("roles", "角色权限")]
public class RolesController : ControllerBase
{
    [Resource("delete", "删除角色", "允许删除非系统内置角色")]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        return Ok();
    }
}
```

模块键和操作键使用稳定英文标识，组合为权限码 `roles.delete`；中文名称和介绍随接口定义，既可直接供前端展示，也便于在代码中理解授权意图。

少数跨模块操作可使用完整形式：

```csharp
[Resource("weather", "read", "天气", "查看天气", "允许查看天气预报")]
```

##### 步骤 4

完成以上操作后，授权检查将验证 `User.Claims` 是否存在对应的 `Permission`。
可以在生成 JWT Token 时直接包含，也可以使用下一节的中间件在授权检查前按角色或用户标识读取并添加；使用中间件时 JWT 无需包含 `Permission`。

```csharp
var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, "uid"),
    new Claim(ClaimTypes.Name, "用户名"),
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

> 注意：如果角色名为 `SangRBAC_Administrator`，将不进行授权检查。该角色名可通过 `SangRoleBasedAuthorizationOptions.AdministratorRoleName` 自定义。

## 可选功能

使用提供的添加角色权限中间件，你也可以单独使用该组件。

##### 步骤 1

实现 `IRolePermission`，通过角色名获取角色权限；也可按当前用户读取直接授予的权限：

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
        // 按 userId 查询直接授予的权限
        return Task.FromResult(new List<Claim>());
    }
}
```

`GetUserPermissionClaims` 具有默认空实现；仅使用角色权限时不需要实现它。角色权限与用户直授权限会合并、按 Claim 的 `Type` 和 `Value` 去重后一次性加入当前请求的 `User`。

然后添加服务：

```csharp
builder.Services.AddRolePermission<MyRolePermission>();
```

##### 步骤 2

在 `app.UseAuthorization();` 前、`app.UseAuthentication();` 后启用这个中间件：

```csharp
app.UseAuthentication();
app.UseRolePermission();
app.UseAuthorization();
```

##### 配置项

`UseRolePermission` 支持以下配置：

**1. `option.Always`**

是否一直检查并执行添加。默认只有在含有 `ResourceAttribute` 要进行权限验证时，此次访问中间件才启动添加权限功能。

## 通配符权限匹配

授权处理支持以下权限 Claim 格式：

- `"roles"` — 授予模块下的所有操作。
- `"roles.delete"` — 授予指定操作。
- `"roles.*"` — 显式通配，授予模块下的所有操作。
- `"*"` — 全局通配，授予所有资源和操作（全局超级管理员权限）。

## 资源详情

通过 `ResourceData.GetResourceInfos()` 获取当前应用实际使用的层级权限详情：

```csharp
var permissions = ResourceData.GetResourceInfos();
```

返回结构按模块分组，每个模块包含操作列表：

```json
[
    {
        "resourceKey": "values",
        "resourceName": "数值",
        "actions": [
            {
                "actionKey": "read",
                "actionName": "查看数值",
                "description": "允许查看数值列表",
                "permission": "values.read"
            }
        ]
    }
]
```

### 前端本地化建议

后端返回的 `ResourceName`、`ActionName` 和 `Description` 是默认展示文本。前端可直接使用 `ResourceKey` 与 `Permission` 作为翻译键覆盖当前语言；找不到翻译时回退到后端默认文本，无需额外维护 `NameKey`。

```json
{
    "en-US": {
        "values": { "name": "Values" },
        "values.read": { "name": "View values", "description": "Allows viewing value lists" }
    }
}
```

## Demo

- 简单示例：https://github.com/sangyuxiaowu/Sang.AspNetCore.RoleBasedAuthorization/tree/main/TestDemo
- 在 Identity 中使用：https://github.com/sangyuxiaowu/IdentityRBAC