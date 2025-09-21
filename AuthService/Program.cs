using AuthService.Models;
using AuthService.Repositories;
using AuthService.Services;
using AuthService.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Options;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// 0️⃣ Load .env file
// ----------------------
Env.Load(); // by default looks for .env in project root (AuthService/)

// ----------------------
// 1️⃣ Register DbContext
// ----------------------
builder.Services.AddDbContext<BookNowDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnectionLocal")));

// ----------------------
// 2️⃣ Configure JwtSettings
// ----------------------
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<IOptions<JwtSettings>>().Value;
    return new TokenService(settings);
});

// ----------------------
// 3️⃣ Register Repositories & Services
// ----------------------
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService.Services.AuthService>();

// ----------------------
// 4️⃣ Add Controllers
// ----------------------
builder.Services.AddControllers();

// ----------------------
// 5️⃣ Configure CORS (optional)
// ----------------------
//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(policy =>
//    {
//        policy
//            .WithOrigins("http://localhost:3000")
//            .AllowAnyHeader()
//            .AllowAnyMethod()
//            .AllowCredentials();
//    });
//});

// ----------------------
// 6️⃣ Add Authentication (Google + Cookie)
// ----------------------
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    // Load secrets from .env
    var clientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
    var clientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");

    if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
    {
        throw new Exception("Google ClientId or ClientSecret not set. Check your .env file.");
    }

    options.ClientId = clientId;
    options.ClientSecret = clientSecret;

    options.CallbackPath = "/signin-google"; // must match Google Console redirect URI

    options.Events.OnRemoteFailure = context =>
    {
        // Redirect or return JSON response
        context.Response.Redirect("/api/auth/google-failure");
        context.HandleResponse(); // IMPORTANT: stops the exception
        return Task.CompletedTask;
    };
});

// ----------------------
// 7️⃣ Build App
// ----------------------
var app = builder.Build();

app.UseHttpsRedirection();

// ----------------------
// 8️⃣ Use Middleware
// ----------------------
//app.UseCors(); // must come before Authentication/Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
