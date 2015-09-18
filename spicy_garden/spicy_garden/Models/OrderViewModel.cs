using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace spicy_garden.Models
{
	public class MainOrderView
	{
		public MainOrderView(List<MenuItemView> listOfItems)
		{
			this.MenuItems = listOfItems;
		}
		public IEnumerable<MenuItemView> MenuItems { get; set; }
	}
	public class MenuItemView
	{
		public MenuItemView()
		{
			this.Quantity = 1;
		}
		public MenuItems Item { get; set; }
		public bool Selected { get; set; }
		public int Quantity { get; set; }
		[Display(Name = "Half Order?")]
		public bool HalfOrder { get; set; }
		public bool HasHalfOrder { get; set; }
		public string OptionSelected { get; set; }
		public IEnumerable<SelectListItem> Options { get; set; }
		public SpicyScale ? SpiceLevel { get; set; }
		public Sauce ? Sauce { get; set; }
	}
}