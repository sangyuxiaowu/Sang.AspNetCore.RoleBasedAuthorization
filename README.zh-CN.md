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
[Resource("资源")]
[Route("api/[controller]")]
[ApiController]
public class RolesController : ControllerBase
{
}
```

```csharp
/// <summary>
/// 删除-数值
/// </summary>
/// <param name="id"></param>
[Resource("删除", "数值")]
[HttpDelete("{id}")]
public IActionResult Delete(int id)
{
    return Ok("删除-数值");
}
```

##### 步骤 4

完成以上操作后，授权检查将验证 `User.Claims` 是否存在对应的 `Permission`。
需要为用户添加对应的 `Claims`，可以在生成 JWT Token 时直接包含。
也可以使用中间件读取对应的角色，在授权检查前添加，可以自己实现也可以使用下一节介绍的功能。

```csharp
var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, "uid"),
    new Claim(ClaimTypes.Name, "用户名"),
    new Claim(ClaimTypes.Email, "test@exp.com"),
    new Claim(ClaimTypes.Role, "user"),
    new Claim(ResourceClaimTypes.Permission, "查询"),
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

实现 `IRolePermission`，通过角色名获取该角色权限列表：

```csharp
public class MyRolePermission : IRolePermission
{
    public Task<List<Claim>> GetRolePermissionClaimsByName(string roleName)
    {
        List<Claim> list = new();
        // your code
        return Task.FromResult(list);
    }
}
```

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

- `"资源"` — 授予该资源下的所有操作。
- `"资源-操作"` — 授予指定操作。
- `"资源-*"` — 显式通配，授予该资源下的所有操作。
- `"*"` — 全局通配，授予所有资源和操作（全局超级管理员权限）。

## Demo

- 简单示例：https://github.com/sangyuxiaowu/Sang.AspNetCore.RoleBasedAuthorization/tree/main/TestDemo
- 在 Identity 中使用：https://github.com/sangyuxiaowu/IdentityRBAC