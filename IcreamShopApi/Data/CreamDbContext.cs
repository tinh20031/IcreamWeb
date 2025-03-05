using IcreamShopApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IcreamShopApi.Data
{
	public class CreamDbContext : DbContext 

	{


		public CreamDbContext() { }
		public CreamDbContext(DbContextOptions<CreamDbContext> options) : base(options) { }
		public DbSet<User> Users { get; set; }
		public DbSet<IceCream> IceCreams { get; set; }
		public DbSet<Cart> Carts { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderDetail> OrderDetails { get; set; }
		public DbSet<Review> Reviews { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>()
				.HasIndex(u => u.Email)
				.IsUnique();

			modelBuilder.Entity<User>()
				.Property(u => u.Role)
				.HasDefaultValue("Customer");

			modelBuilder.Entity<Order>()
				.Property(o => o.Status)
				.HasDefaultValue("Pending");

			modelBuilder.Entity<Cart>()
				.HasOne(c => c.User)
				.WithMany()
				.HasForeignKey(c => c.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<OrderDetail>()
				.HasOne(od => od.Order)
				.WithMany(o => o.OrderDetails)
				.HasForeignKey(od => od.OrderId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Review>()
				.HasOne(r => r.User)
				.WithMany(u => u.Reviews)
				.HasForeignKey(r => r.UserId)
				.OnDelete(DeleteBehavior.Restrict);
			
			


			modelBuilder.Entity<Category>().HasData(
	  new Category { CategoryId = 1, Name = "Chocolate", image = "https://upload.wikimedia.org/wikipedia/commons/thumb/f/f2/Chocolate.jpg/1155px-Chocolate.jpg" }

  );
			modelBuilder.Entity<IceCream>().HasData(
		new IceCream { IceCreamId = 1, Name = "Chocolate Ice Cream", Price = 10.99M, Stock = 100, CategoryId = 1, ImageUrl = "choco.jpg", Description = "kem xin" }
	);
		}

	}

}


