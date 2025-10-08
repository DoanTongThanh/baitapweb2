// File: Program.cs (Phiên bản đã sửa lỗi và hoàn thiện theo đề bài)

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using baitapweb2.Data;
using Serilog;
using Serilog.Events;
using baitapweb2.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models; // Cần thêm

// =========================================================================
// ************ BỔ SUNG SERILOG: Cấu hình Logger Tĩnh ************
// =========================================================================
var logger = new LoggerConfiguration()
// Ghi ra Console
    .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
// Ghi ra file log_book.txt, quay vòng hàng phút
    .WriteTo.File("Logs/book_log.txt",
    rollingInterval: RollingInterval.Minute,
    restrictedToMinimumLevel: LogEventLevel.Information)
  .MinimumLevel.Information() // Thiết lập mức Log tối thiểu
    .CreateLogger();

// =========================================================================
// 1. KHAI BÁO BUILDER (PHẢI LÀ DÒNG CODE ĐẦU TIÊN CÓ THỂ CHẠY)
// =========================================================================
var builder = WebApplication.CreateBuilder(args);


// =========================================================================
// 2. Cấu hình Services (DbContext, Swagger, Controllers, Identity, JWT, Serilog, Image)
// =========================================================================

// Thêm DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Thêm Controllers và Swagger/OpenAPI
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ************ BỔ SUNG SERILOG: Sử dụng Serilog thay thế Logger mặc định ************
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// BỔ SUNG: Cấu hình Authorization cho Swagger UI
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "baitapweb2", Version = "v1" });
    // Thêm cấu hình bảo mật (Security Definition)
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Nhập JWT Token theo định dạng: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    // Thêm yêu cầu bảo mật (Security Requirement)
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
 {
  {
   new OpenApiSecurityScheme
   {
    Reference = new OpenApiReference
    {
     Type = ReferenceType.SecurityScheme,
     Id = JwtBearerDefaults.AuthenticationScheme
    },
    Scheme = "Oauth2",
    Name = JwtBearerDefaults.AuthenticationScheme,
    In = ParameterLocation.Header
   },
   new List<string>()
  }
 });
});


// Cấu hình Identity Core và EntityFramework Stores
builder.Services.AddIdentityCore<IdentityUser>()
.AddRoles<IdentityRole>()
.AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("baitapweb2")
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Cấu hình JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
     Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

// Đăng ký Repository & Service (Các Repository liên quan đến Auth và Data)
builder.Services.AddScoped<IAuthRepository, SQLAuthRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();

// ************ BỔ SUNG: Đăng ký Image Repository và HttpContextAccessor ************
builder.Services.AddHttpContextAccessor(); // Cần thiết cho LocalImageRepository
builder.Services.AddScoped<IImageRepository, LocalImageRepository>();


// Mở comment các Repository cần thiết cho CRUD
// builder.Services.AddScoped<IBookRepository, SQLBookRepository>();
// builder.Services.AddScoped<IPublisherRepository, SQLPublisherRepository>();
// builder.Services.AddScoped<IAuthorRepository, SQLAuthorRepository>();


// =========================================================================
// 3. BUILD ỨNG DỤNG
// =========================================================================
var app = builder.Build();


// =========================================================================
// 4. Cấu hình Middleware (Environment, Https, Static Files, v.v.)
// =========================================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// app.UseHttpsRedirection(); 

// ************ BỔ SUNG: Middleware phục vụ Static Files (để đọc ảnh) ************
app.UseStaticFiles();

// Seed Database Roles
using (var scope = app.Services.CreateScope())
{
    Task.Run(async () =>
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        // Thêm Role "Read" và "Write" để phù hợp với phân quyền trong BookController
        var roles = new[] { "User", "Admin", "Read", "Write" };


        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }).Wait();
}

app.UseAuthentication();
app.UseAuthorization();


// =========================================================================
// 5. Cấu hình Endpoint và Chạy Ứng dụng
// =========================================================================
app.MapControllers();
app.Run();