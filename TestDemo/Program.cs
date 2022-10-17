using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Sang.AspNetCore.RoleBasedAuthorization;
using Sang.AspNetCore.RoleBasedAuthorization.RolePermission;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using TestDemo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    // ���� Swagger ��֤��Ϣ
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
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var filePath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    options.IncludeXmlComments(filePath);
});

// 
// json ʱ���ʽ�����⴦��
builder.Services.Configure<JsonOptions>(options => {
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});


// ����jwt
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
            //��֤ǩ��
            ValidateIssuerSigningKey = true,
            //����ǩ����֤
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            //��֤�䷢��
            ValidateIssuer = false,
            ValidIssuer = jwtSettings.Issuer,
            //֤����Ⱥ��
            ValidateAudience = false,
            ValidAudience = jwtSettings.Audience,

        };
    });

// ��� Sang RBAC ����
builder.Services.AddSangRoleBasedAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        //����swagger��json����λ��
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Test Sang RBAC v1");
    });
}



app.UseAuthentication();
// UseAuthentication ֮�� UseAuthorization ֮ǰ
app.UseRolePermission(opt =>
{
    opt.rolePermission = new MyRolePermission();
});
app.UseAuthorization();

app.MapControllers();

app.Run();
