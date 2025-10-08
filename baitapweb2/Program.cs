using Microsoft.EntityFrameworkCore;
using baitapweb2.Data;
using baitapweb2.Repositories; // Thêm namespace cho Repository

var builder = WebApplication.CreateBuilder(args);

// =====================================================================
// 1. Đăng ký DBContext (Chỉ một lần)
// =====================================================================
builder.Services.AddDbContext<AppDbContext>(options =>
{
    // Lấy chuỗi kết nối từ appsettings.json
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// =====================================================================
// 2. ĐĂNG KÝ REPOSITORY (BẮT BUỘC cho Dependency Injection)
// Sử dụng AddScoped để đảm bảo mỗi HTTP Request có một instance Repository mới.
// =====================================================================
builder.Services.AddScoped<IBookRepository, BookRepository>();


// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run(); 