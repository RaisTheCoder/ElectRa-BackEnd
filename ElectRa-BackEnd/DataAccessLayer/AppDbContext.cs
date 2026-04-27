using System;
using System.Collections.Generic;
using ElectRa_BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace ElectRa_BackEnd.DataAccessLayer;

public partial class AppDbContext : DbContext
{
	public AppDbContext()
	{ }

	public AppDbContext(DbContextOptions<AppDbContext> options)
		: base(options)
	{ }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

	// DbSets
	public DbSet<User> Users { get; set; }
	public DbSet<Product> Products { get; set; }
	public DbSet<Review> Reviews { get; set; }
	public DbSet<Category> Categories { get; set; }
	public DbSet<Brand> Brands { get; set; }
	public DbSet<SubCategory> SubCategories { get; set; }
}