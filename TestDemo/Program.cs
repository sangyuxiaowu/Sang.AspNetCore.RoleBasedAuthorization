using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Sang.AspNetCore.RoleBasedAuthorization;
using Sang.AspNetCore.RoleBasedAuthorizationRolePermission;
using System.Text;
using TestDemo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    // 配置 Swagger 认证信息
    options.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" }
            },
            new string[] {}
        }
    });
});

// 配置jwt
JWTSettings jwtSettings = new()
{
    SecretKey = "You_JWT_Secret_Key",
    ExpireSeconds = 3600
};
builder.Services.Configure<JWTSettings>(opt => {
    opt.SecretKey = jwtSettings.SecretKey;
    opt.ExpireSeconds = jwtSettings.ExpireSeconds;
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt => {
        opt.TokenValidationParameters = new()
        {
            //验证签名
            ValidateIssuerSigningKey = true,
            //用于签名验证
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            //验证颁发者
            ValidateIssuer = false,
            ValidIssuer = jwtSettings.Issuer,
            //证访问群体
            ValidateAudience = false,
            ValidAudience = jwtSettings.Audience,

        };
    });

// 添加 Sang RBAC 服务
builder.Services.AddSangRoleBasedAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseAuthentication();
// UseAuthentication 之后 UseAuthorization 之前
app.UseRolePermission(opt =>
{
    opt.rolePermission = new MyRolePermission();
});
app.UseAuthorization();

app.MapControllers();

app.Run();
