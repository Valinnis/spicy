using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace spicy_garden.Models
{
	public class OrderHandler
	{
		private SpicyGardenDbContext database = new SpicyGardenDbContext();
		public async Task AddMenuItemAsync(MenuItems item)
		{
			try {
				this.database.Menu.Add(item);
				await database.SaveChangesAsync();
				return;
			} catch (DbEntityValidationException ex)
			{
				var errorMessages = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
				var fullErrorMessage = string.Join("; ", errorMessages);
				var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
				throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
			}
		}
		public async Task AddMenuOptionAsync(MenuOptions option)
		{
			try
			{
				this.database.Options.Add(option);
				await database.SaveChangesAsync();
				return;
			}
			catch (DbEntityValidationException ex)
			{
				var errorMessages = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
				var fullErrorMessage = string.Join("; ", errorMessages);
				var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
				throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
			}
		}

		public Task RemoveMenuItem(MenuItems item)
		{
			if (item != null)
			{
				if (database.Entry(item).State == EntityState.Detached)
				{
					database.Menu.Attach(item);
				}
				database.Menu.Remove(item);
			}
			return database.SaveChangesAsync();
		}

		public async Task<bool> AddItemToCart(string orderId,  MenuItemView item)//string optionId, Sauce sauce, string itemId, int quantity)
		{
			if (orderId != null && orderId != "" && item != null)
			{
				// see if there is already a menuitem with that comb
				OrderItems exists = await this.database.OrderItems.Where(x => x.OrderId == orderId && x.MenuItemId == item.Item.Id).FirstOrDefaultAsync();
				if (exists != null)
				{
					// in case we get some smart ass...
					if (item.Quantity == 0)
					{
						exists.Quantity = 0;
						exists.Removed = true;
					}
					else
					{
						// we just update the changes
						exists.Quantity = item.Quantity;
						exists.Removed = false;
						//exists.Option = await this.database.Options.Where(i => i.Id == item.OptionSelected).FirstOrDefaultAsync();
						exists.OptionId = item.OptionSelected;
						exists.IsHalfOrder = item.HalfOrder;
						exists.Sauces = item.Sauce;
						exists.SpiceLevel = item.SpiceLevel;
						exists.Created = DateTime.Now;
					}
					if (this.database.Entry(exists).State == EntityState.Detached)
					{
						this.database.OrderItems.Attach(exists);
					}
					this.database.Entry(exists).State = EntityState.Modified;
				}
				else
				{
					// we create the new item
					exists = new OrderItems() { MenuItemId = item.Item.Id, OrderId = orderId, Quantity = item.Quantity, IsHalfOrder = item.HalfOrder, OptionId = item.OptionSelected, SpiceLevel = item.SpiceLevel, Created = DateTime.Now };
					this.database.OrderItems.Add(exists);
				}
			}
			await this.database.SaveChangesAsync();
			return true;
		}

		public Task RemoveItemFromCart(string orderId, string itemId)
		{
			if (orderId != "" && itemId != "")
			{
				OrderItems e = this.database.OrderItems.Where(x => x.OrderId == orderId && x.MenuItemId == itemId).FirstOrDefault();
				if (e != null)
				{
					e.Quantity = 0;
					e.Removed = true;
				}
				if (this.database.Entry(e).State == EntityState.Detached)
				{
					this.database.OrderItems.Attach(e);
				}
				this.database.Entry(e).State = EntityState.Modified;
			}
			return this.database.SaveChangesAsync();
		}

		public async Task<Orders> StartOrder(String custId)
		{
			System.Diagnostics.Debug.WriteLine("Starting Order");
			// start the order
			Orders o = new Orders();

			// try to get corresponding account if it exists
			Customer c = await this.database.Customers.Where(x => x.Id == custId).FirstOrDefaultAsync();
			AccountUser user = null;
			if (c != null)
			{
				user = await this.database.Users.Where(u => u.Id == c.AccountId).FirstOrDefaultAsync();
			}
			if (user != null)
			{
				// person is signed in supposedly
				o.AccountId = user.Id;
			}
			o.CustomerId = custId;
			o.Created = DateTime.Now;
			o.OrderStatus = OrderStatus.Shopping;
			this.database.Orders.Add(o);
			await this.database.SaveChangesAsync();
			return o;
		}

		public async Task UpdateOrderAccount(Orders o, Customer c)
		{
			System.Diagnostics.Debug.WriteLine("Update");
			o.CustomerId = c.Id;
			o.AccountId = (await this.database.Users.Where(u => u.Id == c.AccountId).FirstOrDefaultAsync()).Id;
			if (this.database.Entry(o).State == EntityState.Detached){
				this.database.Orders.Attach(o);
			}
			this.database.Entry(o).State = EntityState.Modified;
			await this.database.SaveChangesAsync();
		}
		public Task CancelOrder(Orders o)
		{
			System.Diagnostics.Debug.WriteLine("Cancel");
			if (o != null)
			{
				var query = (from item in this.database.OrderItems
								 where item.OrderId == o.Id
								 select item).ToList();
				foreach (var i in query)
				{
					i.Removed = true;
					if (database.Entry(i).State == EntityState.Detached)
					{
						this.database.OrderItems.Attach(i);
					}
					this.database.Entry(i).State = EntityState.Modified;
				}
				o.OrderStatus = OrderStatus.Cancelled;
				if (database.Entry(o).State == EntityState.Detached)
				{
					this.database.Orders.Attach(o);
				}
				this.database.Entry(o).State = EntityState.Modified;
			}
			return this.database.SaveChangesAsync();
		}
	}
}