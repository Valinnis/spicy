using System;
using System.ComponentModel.DataAnnotations;

/* These represent the data models for items that will eventually go into the database */
namespace spicy_garden.Models
{
	public class MenuOptions
	{
		public MenuOptions()
		{
			this.Id = Guid.NewGuid().ToString();
		}
		[Key]
		public string Id { get; set; }
		public string OptionName { get; set; }
		public decimal OptionPrice { get; set; }
		public decimal HalfOrderPrice { get; set; }
		public string MenuItemId { get; set; }
	}

	public class Orders
	{
		public Orders()
		{
			this.Id = Guid.NewGuid().ToString();
		}
		[Key]
		public string Id { get; set; }
		public string CustomerId { get; set; }
		public string AccountId { get; set; }
		public string OrderNotes { get; set; }
		public OrderStatus OrderStatus { get; set; }
		public DateTime Created { get; set; }
	}
	public abstract class Item
	{
		[Key]
		public string Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal Price { get; set; }
		public decimal HalfOrderPrice { get; set; }
		public MenuCategory Category { get; set; }
	}
	public class OrderItems
	{
		public OrderItems()
		{
			this.Id = Guid.NewGuid().ToString();
		}
		[Key]
		public string Id { get; set; }
		public string OrderId { get; set; }
		public string MenuItemId { get; set; }
		public string OptionId { get; set; }
		public bool IsHalfOrder { get; set; }
		public int Quantity { get; set; }
		public bool Removed { get; set; }
		public Sauce ? Sauce { get; set; }
		public SpicyScale ? SpiceLevel { get; set; }
		public DateTime Created { get; set; }
	}

	public class MenuItems : Item
	{
		public MenuItems()
		{
			this.Id = Guid.NewGuid().ToString();
		}
		public bool HasOptions { get; set; }
		public bool HasSpicy { get; set; }
		public bool HasSauce { get; set; }
		public string ImgURL { get; set; }
		public DateTime Created { get; set; }
	}
}