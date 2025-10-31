using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using ShootingAcademy.Filters;
using ShootingAcademy.Hubs;
using ShootingAcademy.Middleware;
using ShootingAcademy.Models;
using ShootingAcademy.Services;
using ShootingAcademy.Services.Media;
using Microsoft.AspNetCore.Mvc;

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

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.EnableDynamicJson();
var dataSource = dataSourceBuilder.Build();

builder.Services.AddSingleton(dataSource);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(dataSource);
});

builder.Services.AddSingleton<IAmazonS3>(_ =>
{
    var credentials = new BasicAWSCredentials(
        Environment.GetEnvironmentVariable("STORAGE_ACCESS"),
        Environment.GetEnvironmentVariable("STORAGE_SECRET")
    );

    var awsConfig = new AmazonS3Config
    {
        RegionEndpoint = Amazon.RegionEndpoint.EUCentral1,
        ServiceURL = Environment.GetEnvironmentVariable("STORAGE_URI")
    };

    return new AmazonS3Client(credentials, awsConfig);
});

builder.Services.Configure<StorjOptions>(options =>
{
    options.BucketName = Environment.GetEnvironmentVariable("STORAGE_BUCKET_NAME");
    options.Endpoint = Environment.GetEnvironmentVariable("STORAGE_URI");
    options.PresignedExpiryMinutes = int.Parse(Environment.GetEnvironmentVariable("STORAGE_PRESIGN_EXPIRY_MINUTES") ?? "30");
});

builder.Services.AddSingleton<IMediaPolicy, DefaultMediaPolicy>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IVideoService, VideoService>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var salt = Environment.GetEnvironmentVariable("SALT") ?? "default";
builder.Services.AddSingleton(new PasswordHasher(salt));

var access = new JwtSettings
{
    Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? configuration["JwtSettings:Audience"],
    Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? configuration["JwtSettings:Issuer"],
    SecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? configuration["JwtSettings:SecretKey"],
    ExpiryMinutes = Convert.ToInt32(Environment.GetEnvironmentVariable("JWT_ACCESS_EXPIRY_MINUTES"))
};

var refresh = new JwtSettings
{
    Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? configuration["JwtSettings:Audience"],
    Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? configuration["JwtSettings:Issuer"],
    SecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? configuration["JwtSettings:SecretKey"],
    ExpiryMinutes = Convert.ToInt32(Environment.GetEnvironmentVariable("JWT_REFRESH_EXPIRY_MINUTES"))
};

builder.Services.AddSingleton(new JwtManager(access, refresh));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICompetitionService, CompetitionService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IRangeService, RangeService>();
builder.Services.AddScoped<ITrainingSessionService, TrainingSessionService>();

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("Coors", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .SetIsOriginAllowed(_ => true);
    });
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.MaximumReceiveMessageSize = 102400; // 100 KB
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
    options.KeepAliveInterval = TimeSpan.FromSeconds(10);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseGetToken();
app.UseCors("Coors");
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseSession();

app.MapControllers();
app.MapFallbackToFile("index.html");
app.MapHub<NotificationHub>("/hubs/notifications");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    DbInitializer.Initialize(services);
}

app.Run();