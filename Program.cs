using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using banbet.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using banbet.Services;
using banbet.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();

// AUTORYZACJA
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

builder.Services.AddDbContext<ApplicationDbContext>
(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseLocal"))
);


var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

builder.Services.AddScoped<OddsService>();
builder.Services.AddScoped<CreateAdminService>();
builder.Services.AddScoped<EventsService>();

var app = builder.Build();

// użycie metody która tworzy konto admina dla nowych użytkowników aplikacji
// by mieli dostęp do wszystkich funkcjonalności 
using (var scope = app.Services.CreateScope())
{
    var adminService = scope.ServiceProvider.GetRequiredService<CreateAdminService>();
    await adminService.CreateAdmin();
}



app.UseCors("AllowReactApp");

// wykonanie migracji
// if (app.Environment.IsDevelopment()){
//     app.ApplyMigrations();
// }

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


