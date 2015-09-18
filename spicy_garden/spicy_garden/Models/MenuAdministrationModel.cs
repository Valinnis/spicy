using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace spicy_garden.Models
{
	public enum HasSpice { Yes, No };
	public class MenuAdministrationView
	{
		[Required]
		[Display(Name = "Item Name")]
		public string ItemName { get; set; }

		[Display(Name = "Item Description")]
		public string ItemDesc { get; set; }

		[Required]
		[Display(Name = "Menu Category")]
		public MenuCategory Category { get; set; }

		[Display(Name = "Options")]
		public IEnumerable<MenuOptions> Options { get; set; }

		[Display(Name = "Base Price")]
		public decimal BasePrice { get; set; }
		public HasSpice HasSpice { get; set; }

		public decimal HalfOrderPrice { get; set; }
		public string Url { get; set; }
	}

	public class CurrentItemView
	{
		public CurrentItemView(SpicyGardenDbContext context)
		{
			this.MenuItems = context.Menu.ToList();
		}
		public IEnumerable<MenuItems> MenuItems { get; set; }
	}

	public class ItemOptionsView
	{
		public ItemOptionsView(SpicyGardenDbContext context, String ItemId)
		{
			this.Options = context.Options.Where(x => x.MenuItemId == ItemId).ToList();
		}
		public IEnumerable<MenuOptions> Options { get; set; }
	}
}