using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using ShootingAcademy.Middleware;
using ShootingAcademy.Models;
using ShootingAcademy.Services;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var connectionString = configuration.GetConnectionString("DefaultConnection")?
    .Replace("$DB_HOST", Environment.GetEnvironmentVariable("DB_HOST"))
    .Replace("$DB_PORT", Environment.GetEnvironmentVariable("DB_PORT"))
    .Replace("$DB_NAME", Environment.GetEnvironmentVariable("DB_NAME"))
    .Replace("$DB_USER", Environment.GetEnvironmentVariable("DB_USER"))
    .Replace("$DB_PASSWORD", Environment.GetEnvironmentVariable("DB_PASSWORD"));

var salt = Environment.GetEnvironmentVariable("SALT") ?? "default";

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.EnableDynamicJson();

var dataSource = dataSourceBuilder.Build();
builder.Services.AddSingleton(dataSource);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(dataSource);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(new PasswordHasher(salt));

JwtSettings access = new JwtSettings()
{
    Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? configuration["JwtSettings:Audience"],
    Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? configuration["JwtSettings:Issuer"],
    SecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? configuration["JwtSettings:SecretKey"],
    ExpiryMinutes = Convert.ToInt32(Environment.GetEnvironmentVariable("JWT_ACCESS_EXPIRY_MINUTES")),
};

JwtSettings refresh = new JwtSettings()
{
    Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? configuration["JwtSettings:Audience"],
    Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? configuration["JwtSettings:Issuer"],
    SecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? configuration["JwtSettings:SecretKey"],
    ExpiryMinutes = Convert.ToInt32(Environment.GetEnvironmentVariable("JWT_REFRESH_EXPIRY_MINUTES")),
};

builder.Services.AddCors(options =>
{
    options.AddPolicy("Coors",
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5173")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                      });
});

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = JwtManager.GetParameters(access);
    });

builder.Services.AddSingleton(new JwtManager(access, refresh));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseGetToken();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseStaticFiles();
app.UseCors("Coors");

app.UseSession();


app.MapFallbackToFile("index.html");

app.Run();
