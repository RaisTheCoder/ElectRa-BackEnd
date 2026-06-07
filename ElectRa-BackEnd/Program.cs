using ElectRa_BackEnd.DataAccessLayer;
using ElectRa_BackEnd.Models;
using ElectRa_BackEnd.Services.Implementations;
using ElectRa_BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add services to the container.
services.AddControllers();
services.AddSwaggerGen();
services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("Default")));

services.AddScoped<IPricingService, PricingService>();

services.AddIdentity<User, IdentityRole<long>>(options =>
{
	options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
	options.User.RequireUniqueEmail = true;
	
	options.Password.RequiredLength = 8;
	options.Password.RequiredUniqueChars = 3;
	options.Password.RequireUppercase = true;
	options.Password.RequireNonAlphanumeric = true;
	
	options.Lockout.MaxFailedAccessAttempts = 5;
	options.Lockout.AllowedForNewUsers = true;
	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

services.ConfigureApplicationCookie(options =>
{
	options.Cookie.HttpOnly = true;

	options.Cookie.SameSite = SameSiteMode.None;

	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

	options.LoginPath = "/api/account/login";

	options.ExpireTimeSpan = TimeSpan.FromDays(7);
});

services.AddAuthorization();
services.AddEndpointsApiExplorer();

services.AddCors(options =>
{
	options.AddPolicy("AllowReact",
		policy =>
		{
			policy.WithOrigins(
					"https://raiko-electra.vercel.app",
					"http://localhost:2027"
					)
				.AllowAnyHeader()
				.AllowCredentials()
				.AllowAnyMethod();
		});
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	using var scope = app.Services.CreateScope();
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	db.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
} else {
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("AllowReact");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles();

app.Run();
