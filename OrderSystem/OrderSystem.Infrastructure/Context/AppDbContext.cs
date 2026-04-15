using Microsoft.EntityFrameworkCore;
using OrderSystem.Infrastructure.Models;

namespace OrderSystem.Infrastructure.Context;

public class AppDbContext : DbContext
{
	public DbSet<Order> Orders { get; set; }
	public DbSet<OutboxMessage> OutboxMessages { get; set; }
	public DbSet<InboxMessage> InboxMessages { get; set; }

	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
		
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Order>()
			.Property(x => x.TotalAmount)
			.HasPrecision(18, 2);

		modelBuilder.Entity<OutboxMessage>(entity =>
		{
			entity.HasKey(e => e.Id);

			entity.HasIndex(e => e.EventId)
				.IsUnique();
		});

		modelBuilder.Entity<InboxMessage>(entity =>
		{
			entity.HasKey(e => e.Id);

			entity.HasIndex(e => e.EventId)
				.IsUnique();
		});

	}
}
