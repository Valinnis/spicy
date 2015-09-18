using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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
}