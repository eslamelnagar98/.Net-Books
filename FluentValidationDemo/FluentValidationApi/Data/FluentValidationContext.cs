namespace FluentValidationApi.Data;
public class FluentValidationContext : DbContext
{
	public DbSet<Customer> Customers { get; set; }
	public DbSet<Address> Addresses { get; set; }
	public FluentValidationContext(DbContextOptions options)
		: base(options)
	{

	}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
		modelBuilder.Entity<Customer>(customer =>
		{
			customer.Ignore(customer => customer.Basket);
		});
    }
}
