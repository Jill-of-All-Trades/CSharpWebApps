using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;


namespace Northwind.EFCore.Shared
{
	public class NorthwindContext : DbContext
	{
		static public readonly string provider = "SQL";

		public DbSet<Category>? Categories {get; set;}
		public DbSet<Product>? Products { get; set;}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			//base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<Category>()
				.Property(category => category.CategoryName)
				.IsRequired() // NOT NULL
				.HasMaxLength(15);

			if(provider == "SQLite")
			{
				modelBuilder.Entity<Product>()
					.Property(product => product.Cost)
					.HasConversion<double>(); // SQLite doesn't support decimal types
			}
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			//base.OnConfiguring(optionsBuilder);

			switch(provider)
			{
				case "SQLite":
					string path = Path.Combine(Environment.CurrentDirectory, "Northwind.db");
					optionsBuilder.UseSqlServer($"Filename={path}");
					break;
				default:
					string connection = @"Data Source=(localdb)\mssqllocaldb;Initial Catalog=Northwind;Integrated Security=True;MultipleActiveResultSets=True;";
					optionsBuilder.UseSqlServer(connection);
					break;
			}
		}
	}
}
