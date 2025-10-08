using Microsoft.EntityFrameworkCore;
using baitapweb2.Data;
using baitapweb2.Repositories; // Đảm bảo namespace này tồn tại

var builder = WebApplication.CreateBuilder(args);

// =====================================================================
// 1. Đăng ký DBContext
// =====================================================================
builder.Services.AddDbContext<AppDbContext>(options =>
{
    // Lấy chuỗi kết nối từ appsettings.json
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// =====================================================================
// 2. ĐĂNG KÝ CÁC REPOSITORY (Dependency Injection)
// Đăng ký tất cả các Repository đã tạo (Book, Author, Publisher)
// =====================================================================
builder.Services.AddScoped<IBookRepository, SQLBookRepository>();
builder.Services.AddScoped<IAuthorRepository, SQLAuthorRepository>();
builder.Services.AddScoped<IPublisherRepository, SQLPublisherRepository>();


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