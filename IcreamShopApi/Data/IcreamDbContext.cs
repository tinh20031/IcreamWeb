namespace IcreamShopApi.Data
{
	using IcreamShopApi.Models;
	using Microsoft.EntityFrameworkCore;

	public class AppDbContext : DbContext
	{
		public AppDbContext() { }
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<User> Users { get; set; }
		public DbSet<IceCream> IceCreams { get; set; }
		public DbSet<Cart> Carts { get; set; }
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
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<OrderDetail>()
				.HasOne(od => od.Order)
				.WithMany(o => o.OrderDetails)
				.HasForeignKey(od => od.OrderId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Review>()
				.HasOne(r => r.User)
				.WithMany(u => u.Reviews)
				.HasForeignKey(r => r.UserId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}

}
