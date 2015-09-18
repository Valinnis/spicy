using System.Collections.Generic;
using System.Web.Mvc;

namespace spicy_garden.Models
{
	public class MainOrderView
	{
		public MainOrderView(IEnumerable<MenuItemView> allMenuItems, IEnumerable<CartItemView> itemsInCart)
		{
			this.MenuItems = allMenuItems;
			this.Cart = itemsInCart;
		}
		public IEnumerable<MenuItemView> MenuItems { get; }
		public IEnumerable<CartItemView> Cart { get; }
	}

	public abstract class ItemView : Item
	{
		public SpicyScale ? SpiceLevel { get; set; }
		public Sauce ? Sauce { get; set; }
		public int Quantity { get; set; }
		public bool IsHalfOrder { get; set; }
	}
	/* What the cart needs to render */
	public class CartItemView : ItemView
	{
		public string SelectedOptionId { get; set; }
		public string OptionName { get; set; }

	}

	/* This represents what the page should require in order to render a menu item */
	public class MenuItemView : ItemView
	{
		public MenuItemView()
		{
			// we want all quantities to be at least 1. < 1 is invalid (0 is equivalent to not being in the cart)
			this.Quantity = 1;
		}
		public string OptionSelected { get; set; }
		public IEnumerable<SelectListItem> Options { get; set; }
		public bool HasOptions { get; set; }
		public bool HasSpicy { get; set; }
		public bool HasSauce { get; set; }
	}
}