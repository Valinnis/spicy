using System;
using Microsoft.AspNet.Identity;
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
	
}
 