using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using spicy_garden.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace spicy_garden.Controllers
{
	public class OrderController : Controller
	{
		protected SpicyGardenDbContext SpicyGardenDbContext { get; set; }
		protected OrderHandler orderHandler { get; set; }
		public OrderController()
		{
			this.SpicyGardenDbContext = new SpicyGardenDbContext();
			this.orderHandler = new OrderHandler();
		}
		private void setDisplayParams()
		{
			if (Request.IsAuthenticated)
			{
				AccountUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<AccountManager>().FindById(User.Identity.GetUserId());
				var cust = this.SpicyGardenDbContext.Customers.Where(c => c.AccountId == user.Id).FirstOrDefault();
				ViewBag.Greeting = cust == null ? "Welcome!" : "Welcome " + cust.FirstName + "!";
				ViewBag.LoginText = "LOGOUT";
				ViewBag.LoginLink = "Logout";
				ViewBag.DisplayReg = "False";
			}
			else
			{
				ViewBag.Greeting = "Welcome everybody!";
				ViewBag.LoginText = "LOGIN";
				ViewBag.LoginLink = "Login";
				ViewBag.LoginHref = "/Account/Login";
				ViewBag.DisplayReg = "True";
			}
		}
		private async Task<HttpCookie> setOrderCookie(HttpCookie c, string Id)
		{
			if (c == null)
			{
				// generate the start of the order
				Id = Id == "" ? (new Customer()).Id : Id;
				Orders o = await new OrderHandler().StartOrder(Id);
				// create the new cookie
				c = new HttpCookie("blowme");
				c.Values.Add("cid", Id);
				c.Values.Add("oid", o.Id);
				Response.Cookies.Add(c);
			}
			// the only way cust is null here is if you started an order while logged in and then logged out. We don't want to save the state in that case
			// setting c[cid] to nothing triggers failure of AssertNonTampered and resets the cookie for a new order.
			c["cid"] = Id == "" ? c["cid"] : Id;
			c.Expires = DateTime.Now.AddHours(4);  // 4 hour expiry make sure it doesn't go away before 4 hours
			c.Domain = null;
			Response.Cookies.Set(c);
			return c;
		}
		private bool AssertNonTampered(string cid, string oid, HttpCookie cookie)
		{
			var foo = this.SpicyGardenDbContext.Orders.Where(x => x.Id == oid && x.CustomerId == cid && x.OrderStatus == OrderStatus.Shopping).FirstOrDefault();
			if (foo == null)
			{
				// the order has been closed or cancelled or the user has altered the data in which case we cancel the cookie and refresh the page
				cookie.Expires = DateTime.Now.AddDays(-1);
				Response.Cookies.Add(cookie);
				return false;
			}
			return true;
		}

		// helper function to get list of options
		private IEnumerable<SelectListItem> ObtainOptions(List<MenuOptions> list)
		{
			var returnList = new List<SelectListItem>();
			foreach (var item in list)
			{
				returnList.Add(new SelectListItem { Value = item.Id, Text = item.OptionName + " - " + item.OptionPrice });
			}
			return returnList;
		}
		
		private async Task<bool> StartOrder()
		{
			HttpCookie cookie = Request.Cookies["blowme"];
			if (!Request.IsAuthenticated)
			{
				// entry anonymous orders
				cookie = await setOrderCookie(cookie, "");
				if (!AssertNonTampered(cookie["cid"], cookie["oid"], cookie))
				{
					return false;
				}
			}
			else
			{
				// user is logged in
				string uid = User.Identity.GetUserId();
				Customer cust = this.SpicyGardenDbContext.Customers.Where(c => c.AccountId == uid).FirstOrDefault();
				cookie = await setOrderCookie(cookie, cust.Id);
				string oid = cookie["oid"];
				// if there is an existing order on the computer we assume that they just forgot to log in and we add to the order. Don't use a public PC to make food orders?
				await orderHandler.UpdateOrderAccount(this.SpicyGardenDbContext.Orders.Where(k => k.Id == oid).FirstOrDefault(), cust);
				if (!AssertNonTampered(cookie["cid"], cookie["oid"], cookie))
				{
					return false;
				}
			}
			return true;
		}
		/* GetMenuView() returns an IEnumerable of everything in the Menu with additional information to display to the user */
		private IEnumerable<MenuItemView> GetMenuView()
		{
			var Items =  
			(from item in this.SpicyGardenDbContext.Menu
			 select new MenuItemView
			 {
				 IsHalfOrder = false,
				 SpiceLevel = SpicyScale.Hot,
				 Sauce = Sauce.Fish,
				 Id = item.Id,
				 Description = item.Description,
				 HasOptions = item.HasOptions,
				 HasSauce = item.HasSauce,
				 HasSpicy = item.HasSpicy,
				 Name = item.Name
			 }).ToList();

			foreach (var item in Items)
			{
				item.Options = ObtainOptions(this.SpicyGardenDbContext.Options.Where(o => o.MenuItemId == item.Id).ToList());
			}
			return Items;
		}
		/* GetCartView() returns an IEnumerable of everything in the shopping cart already with additional information to display to the user */
		private IEnumerable<CartItemView> GetCartView()
		{
			string oid = Request.Cookies["blowme"]["oid"];
			var CartItems = (from item in this.SpicyGardenDbContext.OrderItems
								  where item.OrderId == oid && item.Removed == false
								  select new CartItemView
								  {
									  IsHalfOrder        =  item.IsHalfOrder,
									  OptionName         =  this.SpicyGardenDbContext.Options.Where(o => o.Id == item.OptionId).FirstOrDefault() != null ?
																	this.SpicyGardenDbContext.Options.Where(o => o.Id == item.OptionId).FirstOrDefault().OptionName :
																	null,
									  SelectedOptionId   =  item.OptionId,
									  Quantity           =  item.Quantity,
									  SpiceLevel         =  item.SpiceLevel,
									  Sauce              =  item.Sauce,
									  Id                 =  item.MenuItemId,
									  Name               =  this.SpicyGardenDbContext.Menu.Where(m => m.Id == item.MenuItemId).FirstOrDefault().Name,
									  Category				=	this.SpicyGardenDbContext.Menu.Where(m => m.Id == item.MenuItemId).FirstOrDefault().Category
								  }).ToList();
			return CartItems;
		}
		// GET: Order
		public async Task<ActionResult> Index()
		{
			if (!(await StartOrder()))
			{
				return RedirectToAction("Index");
			}
			// everything should be good to go. user order has been set or reset. First we grab a representation of all the menu items to display to the user 
			var MenuItems = GetMenuView();
			var CartItems = GetCartView();
			
			//var CartItems = new List<CartItemView>();
			//Now grab the order items that already exist (or don't exist)
			setDisplayParams();
			return View(new MainOrderView(MenuItems, CartItems));
		}

		private string GenerateItemHtml(OrderItems o)
		{
			string s = "<table class='item " + o.MenuItemId + " tiny'>\n";
			if (o != null)
			{
				MenuItems item = this.SpicyGardenDbContext.Menu.Where(x => x.Id == o.MenuItemId).First();
            s += "<tr><td colspan='2'>" + o.Quantity + " " + item.Name + (o.IsHalfOrder ? " (Half-Order)</td></tr>\n" : "</td></tr>\n");
				if (o.OptionId != null)
				{
					MenuOptions opt = this.SpicyGardenDbContext.Options.Where(x => x.Id == o.OptionId).First();
					s += "<tr><td class='cart-det'>" + opt.OptionName + (o.Sauce != null ? " with " + o.Sauce.Value + " sauce</td>" : "</td>");
					s += "<td class='cart-s price'>" + (o.IsHalfOrder ? opt.HalfOrderPrice * o.Quantity : opt.OptionPrice * o.Quantity) + "</td></tr>\n";
				}
				else
				{
					s += "<tr><td class='cart-det'>" + (o.Sauce != null ? o.Sauce.Value + " sauce</td>" : "</td>");
					s += "<td class='cart-s price'>" + (o.IsHalfOrder ? item.HalfOrderPrice * o.Quantity : item.Price * o.Quantity) + "</td></tr>\n";
				}
			}
			s += "</table>";
			return s;
		} 
		//POST: Order
		[HttpPost]
		[AjaxOnly]
		public async Task<string> AddToCart(string itemId, bool halfOrder, string optionId, int quantity, SpicyScale ? spiceLevel, Sauce ? sauce)
		{
			try
			{
				MenuItemView newItem = new MenuItemView() { IsHalfOrder = halfOrder, OptionSelected = optionId, Quantity = quantity, SpiceLevel = spiceLevel, Sauce = sauce };
				var dbItem = await SpicyGardenDbContext.Menu.Where(m => m.Id == itemId).FirstAsync();
				newItem.Id = dbItem.Id;
				
				HttpCookie c = Request.Cookies["blowme"];
				if (!AssertNonTampered(c["cid"], c["oid"], c))
				{
					RedirectToAction("Index");
				}
				var oid = c["oid"];
				await new OrderHandler().AddItemToCart(oid, newItem);
				return GenerateItemHtml(await this.SpicyGardenDbContext.OrderItems.Where(x => x.OrderId == oid && x.MenuItemId == itemId && x.Removed == false).FirstOrDefaultAsync());
			}
			catch (Exception e)
			{
				return "error";
			}

		}

		[HttpPost]
		[AjaxOnly]
		public async Task<bool> RemoveFromCart(string itemId, string optionId)
		{
			try
			{
				HttpCookie c = Request.Cookies["blowme"];
				if (!AssertNonTampered(c["cid"], c["oid"], c))
				{
					RedirectToAction("Index");
				}
				await new OrderHandler().RemoveItemFromCart(c["oid"], itemId, optionId);
				return true;
			}
			catch (Exception e)
			{
				return false;
			}

		}

		[HttpGet]
		[AjaxOnly]
		public async Task<decimal> GetItemPrice(string Id, int quantity, string optionId, bool halfOrder)
		{
			var price = 0.00m;
			var menuitem = await this.SpicyGardenDbContext.Menu.Where(m => m.Id == Id).FirstOrDefaultAsync();
			if (null != menuitem)
			{
				if (optionId != null)
				{
					var t = await this.SpicyGardenDbContext.Options.Where(x => x.MenuItemId == Id && x.Id == optionId).FirstOrDefaultAsync();
					if (halfOrder)
					{
						price = t.HalfOrderPrice;
					}
					else
					{
						price = t.OptionPrice;
					}
				}
				else
				{
					if (halfOrder)
					{
						price = menuitem.HalfOrderPrice;
					}
					else
					{
						price = menuitem.Price;
					}
				}
			}
			return price * quantity;
		}
	}
}
