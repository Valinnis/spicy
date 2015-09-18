using System;
using System.Linq;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Validation;
using System.ComponentModel.DataAnnotations;

namespace spicy_garden.Models
{
	public class AccountUser : IUser<string>, IUser
	{
		// Yuli Chen:
		// This is the default constructor for our user class
		public AccountUser()
		{
			this.Id = Guid.NewGuid().ToString();
		}
		public string Id { get; set; }
		public string UserName { get; set; }
		public string Hash { get; set; }
		public string Email { get; set; }
		public bool Validated { get; set; }
		public string LastOrderId { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime ModifiedDate { get; set; }
	}
	public class Customer
	{
		public Customer()
		{
			this.Id = Guid.NewGuid().ToString();
		}
		[Key]
		public string Id { get; set; }
		public string AccountId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Telephone { get; set; }
		public string AddressId { get; set; }
		public bool Validated { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime ModifiedDate { get; set; }
	}

	public class Address
	{
		public Address()
		{
			this.Id = Guid.NewGuid().ToString();
		}
		[Key]
		public string Id { get; set; }
		public string AccountId { get; set; }
		public string CustomerId { get; set; }
		public string AddrLine1 { get; set; }
		public string AddrLine2 { get; set; }
		public string PostalCode { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime ModifiedDate { get; set; }
	}
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

		public override int SaveChanges()
		{
			try
			{
				return base.SaveChanges();
			}
			catch (DbEntityValidationException ex)
			{
				var errorMessages = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
				var fullErrorMessage = string.Join("; ", errorMessages);
				var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
				throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
			}
		}
	}
}
 