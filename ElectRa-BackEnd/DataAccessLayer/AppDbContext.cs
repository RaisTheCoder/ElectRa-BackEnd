using System;
using System.Collections.Generic;
using ElectRa_BackEnd.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ElectRa_BackEnd.DataAccessLayer;

public partial class AppDbContext : IdentityDbContext<User, IdentityRole<long>, long>
{
	public AppDbContext(DbContextOptions<AppDbContext> options)
		: base(options)
	{ }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<ReviewHelpful>()
			.HasKey(x => new { x.ReviewId, x.UserId });

		modelBuilder.Entity<Favorite>()
			.HasKey(f => new { f.UserId, f.ProductId });
		
		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

	// DbSets
	public DbSet<Product> Products { get; set; }
	public DbSet<Review> Reviews { get; set; }
	public DbSet<Category> Categories { get; set; }
	public DbSet<Brand> Brands { get; set; }
	public DbSet<SubCategory> SubCategories { get; set; }
	public DbSet<ReviewHelpful> ReviewHelpfuls { get; set; }
	public DbSet<Favorite> ProductFavorites { get; set; }
	public DbSet<Order> Orders { get; set; }
	public DbSet<OrderItem> OrderItems { get; set; }
	public DbSet<VisitHistory> VisitHistories { get; set; }
}