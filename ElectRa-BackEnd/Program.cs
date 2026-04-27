using ElectRa_BackEnd.DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add services to the container.
services.AddControllers();
services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("Default")));

services.AddCors(options =>
{
	options.AddPolicy("AllowReact",
		policy =>
		{
			policy.WithOrigins("http://localhost:5173")
				.AllowAnyHeader()
				.AllowAnyMethod();
		});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseRouting();

app.UseCors("AllowReact");

app.UseHttpsRedirection();
// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllers();

app.Run();