# Sang.AspNetCore.RoleBasedAuthorization

[![NuGet version (Sang.AspNetCore.RoleBasedAuthorization)](https://img.shields.io/nuget/v/Sang.AspNetCore.RoleBasedAuthorization.svg?style=flat-square)](https://www.nuget.org/packages/Sang.AspNetCore.RoleBasedAuthorization/)

Role-Based Authorization for ASP.NET

ASP.NET RBAC 权限管理

## Instructions:

##### Step 1 

Add this package.

```bash
Install-Package Sang.AspNetCore.RoleBasedAuthorization
```

##### Step 2 

Add RBAC Services.

```
builder.Services.AddSangRoleBasedAuthorization();
```

##### Step 3

Add the ResourceAttribute tag to the interface or Controller that needs to be checked for authorization.

在需要进行授权检查的接口或 Controller 处添加 ResourceAttribute 标记。

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
[Resource("删除-数值")] //[Resource("删除", Action = "数值")]
[HttpDelete("{id}")]
public IActionResult Delete(int id)
{
    return Ok("删除-数值");
}
```

##### Step 4

After completing the above operations, the authorization check will check whether `User.Claims` has the corresponding `Permission`.
You need to add the corresponding `Claims` for the user, which can be included directly when generating the jwt token.
You can also use middleware to read the corresponding role and add it before the authorization check.
You can implement it yourself or use the provided functions described in the next section.

完成以上操作后，授权检查，将检查`User.Claims`是否存在对应的`Permission`。
需要为用户添加对应的 `Claims` ，可以在生成 jwt token 时直接包含。
也可以使用中间件读取对应的角色，在授权检查前添加，可以自己实现也可以使用提供的下一节介绍的功能。

```csharp
var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, "uid"),
    new Claim(ClaimTypes.Name,"用户名"),
    new Claim(ClaimTypes.Email,"test@exp.com"),
    new Claim(ClaimTypes.Role, "user"),
    new Claim(ResourceClaimTypes.Permission,"查询"),
};
var token = new JwtSecurityToken(
        "Issuer",
        "Audience",
        claims,
        expires: DateTime.UtcNow.AddSeconds(3600),
        signingCredentials: credentials
    );
```

> Note: If the role is named `SangRBAC_Administrator`, no authorization check will be done.

> 注意：如果角色名为`SangRBAC_Administrator`，将不进行授权检查。

## Optional Features

Use the provided add role permission middleware, You can also use this component alone.

使用提供的添加角色权限中间件，你也可以单独使用该组件。

##### Step 1 

Implement `IRolePermission`, get the role permission list by role name.

实现`IRolePermission`，通过角色名获取该角色权限列表

```csharp
public class MyRolePermission : IRolePermission
{
    public Task<List<Claim>> GetRolePermissionClaimsByName(string roleName)
    {
        List<Claim> list = new();
        // you code
        return Task.FromResult(list);
    }
}
```

Then add service;

然后添加服务。

```csharp
builder.Services.AddRolePermission<MyRolePermission>();
```

##### Step 2

Enable this middleware before `app.UseAuthorization();` and after `app.UseAuthentication();`.

在`app.UseAuthorization();`前`app.UseAuthentication()`后启用这个中间件。

```csharp
app.UseAuthentication();
app.UseRolePermission();
app.UseAuthorization();
```
##### Option

UseRolePermission 

**1. option.UserAdministratorRoleName：**

Set a custom role to have the same built-in super administrator privileges as `SangRBAC_Administrator`.

设置一个自定义角色，使其拥有 `SangRBAC_Administrator` 一样的系统内置超级管理员权限。

**2. option.Always：**

Whether to check and execute the addition all the time. By default, only when the `ResourceAttribute` is included for permission verification, the access middleware will start the adding permission function.

是否一直检查并执行添加，默认只有在含有 `ResourceAttribute` 要进行权限验证时，此次访问中间件才启动添加权限功能。

## Demo

- Simple Demo https://github.com/sangyuxiaowu/Sang.AspNetCore.RoleBasedAuthorization/tree/main/TestDemo
- Used in the Identity https://github.com/sangyuxiaowu/IdentityRBAC