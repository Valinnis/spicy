using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using spicy_garden.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace spicy_garden.Controllers
{
	public class HomeController : Controller
	{
		protected SpicyGardenDbContext SpicyGardenDbContext { get; set; }
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
		public HomeController()
		{
			this.SpicyGardenDbContext = new SpicyGardenDbContext();
		}
		public ActionResult Index()
		{
			setDisplayParams();
			return View();
		}

		public ActionResult Contact()
		{
			setDisplayParams();
			ViewBag.Message = "Your contact page.";
			return View();
		}
		public ActionResult Order()
		{
			setDisplayParams();
			ViewBag.Title = "Order Online";
			return View();
		}
	}
}