using Microsoft.EntityFrameworkCore;
using MobileController;
using MobileController.Models.Data;
using MobileController.Repositories;
using MobileController.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MyDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDBContext"));
});//DB Connection Config.(MSSQL)

// ###Register your repository and service
builder.Services.AddScoped<IShiftRepository, ShiftRepository>();
builder.Services.AddScoped<IShiftService, ShiftService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthorization();

app.MapControllers();

app.Run();
