using Microsoft.EntityFrameworkCore;
using OrderSystem.Infrastructure.Models;

namespace OrderSystem.Infrastructure.Context;

public class AppDbContext : DbContext
{
	public DbSet<Order> Orders { get; set; }
	public DbSet<OutboxMessage> OutboxMessages { get; set; }

	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
		
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Order>()
			.Property(x => x.TotalAmount)
			.HasPrecision(18, 2);
	}
}
