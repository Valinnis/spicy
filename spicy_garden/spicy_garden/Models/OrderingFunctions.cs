using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;

namespace spicy_garden.Models
{
	/*
	 * This class handles anything to do with CRUD orders. 
	 * USAGE: var handler = new OrderHandler();
	 */
	public class OrderHandler : IDisposable
	{
		private SpicyGardenDbContext database = new SpicyGardenDbContext();

		// AddMenuItemAsync(item) => void
		// Takes an item (MenuItems) and adds it to the Menu table in the database
		// PRODUCTION: comment out for production code. This will not be available.
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

		// AddMenuOptionAsync(option) => void
		// Takes an option (MenuOptions) and adds it to the Option table in the database
		// PRODUCTION: comment out for production code. This will not be available.
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

		// RemoveMenuItem(item) => void
		// Removes an item (MenuItems)
		// PRODUCTION: comment out for production code. This will not be available.
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

		// AddItemToCart(string, MenuItemView) => async void
		// Associates a given item with an order id and sets it in the database
		// PRODUCTION: This should be called from an AJAX POST/GET script. 
		public async Task AddItemToCart(string orderId,  MenuItemView item)//string optionId, Sauce sauce, string itemId, int quantity)
		{
			if (orderId != null && orderId != "" && item != null)
			{
				// see if there is already a menuitem with that comb
				OrderItems existingItem = await this.database.OrderItems.Where(x => x.OrderId == orderId && x.MenuItemId == item.Id && x.OptionId == item.OptionSelected).FirstOrDefaultAsync();
				
				// Entry point: Item does exist already in the cart
				if (existingItem != null)
				{
					// just in case - this shouldn't be possible since there is javascript verification and currently this is AJAX only.
					// However, this is here in case we have users who don't use JS or somehow validation doesn't catch it properly
					if (item.Quantity < 0)
					{
						existingItem.Quantity = 0;
						existingItem.Removed = true;
					}
					else
					{
						// we just update the changes
						existingItem.Quantity = item.Quantity;
						existingItem.Removed = false;
						existingItem.OptionId = item.OptionSelected;
						existingItem.IsHalfOrder = item.IsHalfOrder;
						existingItem.Sauce = item.Sauce;
						existingItem.SpiceLevel = item.SpiceLevel;
						existingItem.Created = DateTime.Now;
					}
					if (this.database.Entry(existingItem).State == EntityState.Detached)
					{
						this.database.OrderItems.Attach(existingItem);
					}
					this.database.Entry(existingItem).State = EntityState.Modified;
				} // end item exists and entry item doesn't exist
				else
				{
					// we create the new item
					existingItem = new OrderItems() { MenuItemId = item.Id, OrderId = orderId, Quantity = item.Quantity, IsHalfOrder = item.IsHalfOrder, OptionId = item.OptionSelected, SpiceLevel = item.SpiceLevel, Created = DateTime.Now };
					this.database.OrderItems.Add(existingItem);
				}
			}
			await this.database.SaveChangesAsync();
		}

		// RemoveItemFromCart(string, string, string) => async void
		// tries to find a matching item in the database and removes that item from the cart.
		// PRODUCTION: This should be called from an AJAX POST/GET script. 
		public async Task RemoveItemFromCart(string orderId, string itemId, string optionId)
		{
			if (orderId != "" && itemId != "")
			{
				// FIX: we want to allow users to add more than one option element to the cart. Therefore we also need to ensure that the options are the same for the element we remove 
				OrderItems e = await this.database.OrderItems.Where(x => x.OrderId == orderId && x.MenuItemId == itemId && x.OptionId == optionId).FirstOrDefaultAsync();
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
			await this.database.SaveChangesAsync();
		}

		// StartOrder(string) => async Orders
		// initializes an order for a user and sets the state to Shopping
		// PRODUCTION: This is called any time someone reaches the Orders page without a valid cookie set
		public async Task<Orders> StartOrder(String custId)
		{
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
			// we set the remaining parameters or the Orders object and save.
			o.CustomerId = custId;
			o.Created = DateTime.Now;
			o.OrderStatus = OrderStatus.Shopping;
			this.database.Orders.Add(o);
			await this.database.SaveChangesAsync();
			return o;
		}

		// UpdateOrderAccount(Orders, Customer) => async void
		// Associates a previously anonymous order with a user account
		// PRODUCTION: This is called if someone starts an order while logged out and then logs back in afterwards.
		public async Task UpdateOrderAccount(Orders o, Customer c)
		{
			o.CustomerId = c.Id;
			o.AccountId = (await this.database.Users.Where(u => u.Id == c.AccountId).FirstOrDefaultAsync()).Id;
			if (this.database.Entry(o).State == EntityState.Detached){
				this.database.Orders.Attach(o);
			}
			this.database.Entry(o).State = EntityState.Modified;
			await this.database.SaveChangesAsync();
		}

		// CancelOrder(Orders) => async void
		// Cancels all items in an order and closes the order status
		// PRODUCTION: This is called if someone logged in starts an order and then logs out without finalizing the order.
		public Task CancelOrder(Orders o)
		{
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

		public void Dispose()
		{
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.database.Dispose();
				this.database = null;
			}
		}
	}
}