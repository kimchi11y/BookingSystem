using System.Configuration;
using BookingSystem.Entities.DbContext;
using BookingSystem.Entities.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options => {
        // Configure identity options here
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
        
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<BookingSystemDbContext>()
    .AddDefaultTokenProviders();

if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<BookingSystemDbContext>(options =>
        options.UseMySQL(configuration.GetConnectionString("MySQLConnection"), b => b.MigrationsAssembly("BookingSystem.API")));
}
else
{
    builder.Services.AddDbContext<BookingSystemDbContext>(options =>
        options.UseSqlite("Data Source=BookingSystem.db;", b => b.MigrationsAssembly("BookingSystem.API")));
}
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
app.UseCors(options =>
{
    options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();

});
app.Run();