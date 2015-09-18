using System;
using System.ComponentModel.DataAnnotations;

namespace spicy_garden.Models
{
	public enum OrderStatus { Shopping, Open, Filled, Cancelled };
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
		public Sauce ? Sauces { get; set; }
		public SpicyScale ? SpiceLevel { get; set; }
		public DateTime Created { get; set; }
	}
	public enum MenuCategory { Appetizers, Entrees, Dessert, Soup };
	public enum Sauce { Fish, Plum, Curry, Satay, Teriyaki };
	public class MenuItems
	{
		public MenuItems()
		{
			this.Id = Guid.NewGuid().ToString();
		}
		[Key]
		public string Id { get; set; }
		public string ItemName { get; set; }
		public decimal ItemPrice { get; set; }
		public decimal HalfOrderPrice { get; set; }
		public MenuCategory Category { get; set; }
		public bool HasOptions { get; set; }
		public Sauce ? Sauces { get; set; }
		public bool HasSpicy { get; set; }
		public string ItemDesc { get; set; }
		public string ImgURL { get; set; }
		public bool HasSauce { get; set; }
	}
}