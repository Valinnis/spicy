using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using spicy_garden.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace spicy_garden.Controllers
{
	public class ManageController : Controller
	{
		protected SpicyGardenDbContext SpicyGardenDbContext = new SpicyGardenDbContext();
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
		// GET: MenuAdmin()
		public  ActionResult MenuAdmin()
		{
			setDisplayParams();
			return View();
		}

		// POST: MenuAdmin()
		[HttpPost]
		public async Task<ActionResult> MenuAdmin(MenuAdministrationView model)
		{
			setDisplayParams();
			// checks just in case the model fails.
			if (ModelState.IsValid)
			{
				// create the menu item first
				MenuItems item = new MenuItems() { HasSpicy = model.HasSpice == HasSpice.Yes ? true : false, HalfOrderPrice = model.HalfOrderPrice, Description = model.ItemDesc, Name = model.ItemName, Price = model.BasePrice, Category = model.Category, ImgURL = model.Url, Created = DateTime.Now };
				// are we dealing with options or just a menu item
				if (model.Options != null)
				{
					item.HasOptions = true;
					foreach (var option in model.Options)
					{
						option.MenuItemId = item.Id;
						await new OrderHandler().AddMenuOptionAsync(option);
					}
				}
				await new OrderHandler().AddMenuItemAsync(item);
				return RedirectToAction("MenuAdmin");
			}
			return View();
		}
		// GET: Manage/Details/5
		public ActionResult Details(int id)
		{
			return View();
		}

		// GET: Manage/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: Manage/Create
		[HttpPost]
		public ActionResult Create(FormCollection collection)
		{
			try
			{
				// TODO: Add insert logic here

				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}

		// GET: Manage/Edit/5
		public ActionResult Edit(int id)
		{
			return View();
		}

		// POST: Manage/Edit/5
		[HttpPost]
		public ActionResult Edit(int id, FormCollection collection)
		{
			try
			{
				// TODO: Add update logic here

				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}

		// GET: Manage/Delete/5
		public ActionResult Delete(int id)
		{
			return View();
		}

		// POST: Manage/Delete/5
		[HttpPost]
		public ActionResult Delete(int id, FormCollection collection)
		{
			try
			{
				// TODO: Add delete logic here

				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}
	}
}
