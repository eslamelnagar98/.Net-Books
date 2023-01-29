using FluentValidationDemo.Entities;
using Microsoft.EntityFrameworkCore;

namespace FluentValidationDemo.persistence.Data;
public class FluentValidationContext : DbContext
{
	public DbSet<Customer> MyProperty { get; set; }
	public FluentValidationContext(DbContextOptions<FluentValidationContext> options)
		:base(options)
	{

	}
}
