// Trong AppDbContextFactory.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using baitapweb2.Data; // Thay thế bằng namespace/đường dẫn của DbContext của bạn

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // HARDCODE CHUỖI KẾT NỐI ĐỂ BỎ QUA APPSETTINGS.JSON
        var connectionString = "Server=(localdb)\\mssqllocaldb;Database=baitapweb2DB;Trusted_Connection=True;MultipleActiveResultSets=true";
        
        var builder = new DbContextOptionsBuilder<AppDbContext>();
        builder.UseSqlServer(connectionString);

        return new AppDbContext(builder.Options);
    }
}