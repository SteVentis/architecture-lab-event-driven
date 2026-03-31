using Microsoft.EntityFrameworkCore;
using OrderSystem.Infrastructure.Models;

namespace OrderSystem.Infrastructure.Context;

public class AppDbContext : DbContext
{
	public DbSet<Order> Orders { get; set; }
	public DbSet<OutboxMessage> OutboxMessage { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseSqlServer("connectionstring");
	}
}
