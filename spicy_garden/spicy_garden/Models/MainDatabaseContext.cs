using System.Data.Entity;

/* The database context for the application */
namespace spicy_garden.Models
{
	public partial class SpicyGardenDbContext : DbContext
	{
		public SpicyGardenDbContext()
			 : base("DefaultConnection")
		{ }
		public virtual IDbSet<AccountUser> Users { get; set; }
		public virtual DbSet<Customer> Customers { get; set; }
		public virtual DbSet<Address> Addresses { get; set; }
		public virtual DbSet<Orders> Orders { get; set; }
		public virtual DbSet<OrderItems> OrderItems { get; set; }
		public virtual DbSet<MenuItems> Menu { get; set; }
		public virtual DbSet<MenuOptions> Options { get; set; }
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<AccountUser>().ToTable("Accounts");
			modelBuilder.Entity<Customer>().ToTable("Customers");
			modelBuilder.Entity<Address>().ToTable("Addresses");
		}
	}
}