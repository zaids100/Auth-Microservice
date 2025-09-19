using AuthService.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext
builder.Services.AddDbContext<BookNowDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnectionLocal")));

// Add controllers for Auth APIs
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers(); // map [ApiController]s like AuthController

app.Run();
