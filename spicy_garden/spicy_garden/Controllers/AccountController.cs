using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using spicy_garden.Models;
using System.Linq;
using System;
using System.Data.Entity.Validation;
using System.Data.Entity;

namespace spicy_garden.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		private ApplicationSignInManager _signInManager;
		private AccountManager _userManager;
		protected SpicyGardenDbContext SpicyGardenDbContext;
		protected OrderHandler orderHandler;
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
		public AccountController()
		 : this(new AccountManager(new AccountUserStore<AccountUser>()))
		{
			this.SpicyGardenDbContext = new SpicyGardenDbContext();
			this.orderHandler = new OrderHandler();
		}

		public AccountController(AccountManager userManager)
		{
			this.UserManager = userManager;
		}


		public ApplicationSignInManager SignInManager
		{
			get
			{
				return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			}
			private set
			{
				_signInManager = value;
			}
		}

		public AccountManager UserManager
		{
			get
			{
				return _userManager ?? HttpContext.GetOwinContext().GetUserManager<AccountManager>();
			}
			private set
			{
				_userManager = value;
			}
		}
		public ActionResult Manage()
		{
			setDisplayParams();
			AccountUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<AccountManager>().FindById(User.Identity.GetUserId());
			Customer cust = this.SpicyGardenDbContext.Customers.Where(c => c.AccountId == user.Id).FirstOrDefault();
			Address address = this.SpicyGardenDbContext.Addresses.Where(a => a.AccountId == user.Id).FirstOrDefault();
			ViewBag.Email = user.Email;
			if (cust != null)
			{
				ViewBag.FirstName = cust.FirstName;
				ViewBag.LastName = cust.LastName;
				ViewBag.Telephone = cust.Telephone;
			}
			if (address != null)
			{
				ViewBag.AddrLine1 = address.AddrLine1;
				ViewBag.AddrLine2 = address.AddrLine2;
				ViewBag.PostalCode = address.PostalCode;
			}
			return View();
		}
		private void setAccountInfo()
		{
			AccountUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<AccountManager>().FindById(User.Identity.GetUserId());
			Customer cust = this.SpicyGardenDbContext.Customers.Where(c => c.AccountId == user.Id).FirstOrDefault();
			Address address = this.SpicyGardenDbContext.Addresses.Where(a => a.AccountId == user.Id).FirstOrDefault();
			ViewBag.Email = user.Email;
			if (cust != null)
			{
				ViewBag.FirstName = cust.FirstName;
				ViewBag.LastName = cust.LastName;
				ViewBag.Telephone = cust.Telephone;
			}
			if (address != null)
			{
				ViewBag.AddrLine1 = address.AddrLine1;
				ViewBag.AddrLine2 = address.AddrLine2;
				ViewBag.PostalCode = address.PostalCode;
			}
		}
		// GET: /Account/EditAccountInformation
		public ActionResult EditAccountInformation()
		{
			setDisplayParams();
			setAccountInfo();
			return View();
		}
		// POST: /Account/EditAccountInformation
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult EditAccountInformation(AccountManagementModel model)
		{
			setDisplayParams();
			setAccountInfo();
			SpicyGardenDbContext database = new SpicyGardenDbContext();
			if (!ModelState.IsValid)
			{
				return View();
			}
			// everything is good so now check if password entered is correct
			var oldUser = UserManager.Find(User.Identity.Name, model.Password);
			var cust = this.SpicyGardenDbContext.Customers.Where(c => c.AccountId == oldUser.Id).FirstOrDefault();
			var address = this.SpicyGardenDbContext.Addresses.Where(a => a.AccountId == oldUser.Id).FirstOrDefault();
			if (oldUser == null)
			{
				// wrong password
				return View();
			}
			oldUser.Email = model.Email;
			oldUser.ModifiedDate = DateTime.Now;
			cust.FirstName = model.FirstName;
			cust.LastName = model.LastName;
			cust.Telephone = model.Telephone;
			cust.Email = model.Email;
			cust.ModifiedDate = DateTime.Now;
			address.AddrLine1 = model.AddrLine1;
			address.AddrLine2 = model.AddrLine2;
			address.PostalCode = model.PostalCode;
			address.ModifiedDate = DateTime.Now;
			try
			{
				var result = UserManager.Update(oldUser);
				if (result.Succeeded)
				{
					database.Customers.Attach(cust);
					database.Entry(cust).State = EntityState.Modified;
					database.Addresses.Attach(address);
					database.Entry(address).State = EntityState.Modified;
					database.SaveChanges();
					return Redirect("/Account/Manage");
				}
			}
			catch (DbEntityValidationException ex)
			{
				var errorMessages = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
				var fullErrorMessage = string.Join("; ", errorMessages);
				var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
				throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
			}
			return View();
		}
		//
		// GET: /Account/Login
		[AllowAnonymous]
		public ActionResult Login()
		{
			setDisplayParams();
			if (Request.IsAuthenticated)
			{
				return Redirect("/");
			}
			if (Request.UrlReferrer != null)
			{
				Session["returnUrl"] = (Request.UrlReferrer.AbsolutePath == "/Account/Login" || Request.UrlReferrer.AbsolutePath == "/Account/Register") ? Session["returnUrl"] : Request.UrlReferrer.AbsolutePath;
			}
			else
			{
				Session["returnUrl"] = "/";
			}
			ViewBag.sourceUrl = Session["returnUrl"].ToString() == "" ? "/" : Session["returnUrl"].ToString();
			return View();
		}

		//
		// POST: /Account/Login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
		{
			setDisplayParams();
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			// This doesn't count login failures towards account lockout
			// To enable password failures to trigger account lockout, change to shouldLockout: true
			var result = await SignInManager.PasswordSignInAsync(model.UserId, model.Password, model.RememberMe, shouldLockout: false);
			switch (result)
			{
				case SignInStatus.Success:
					string return_url = Server.HtmlEncode(Request.Form["r"]);
					if (return_url == null)
					{
						return Redirect("/");
					}
					else
					{
						return Redirect(return_url);
					}
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.RequiresVerification:
					return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
				case SignInStatus.Failure:
				default:
					ModelState.AddModelError("", "Invalid login attempt.");
					return View(model);
			}
		}

		//
		// GET: /Account/VerifyCode
		[AllowAnonymous]
		public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
		{
			// Require that the user has already logged in via username/password or external login
			if (!await SignInManager.HasBeenVerifiedAsync())
			{
				return View("Error");
			}
			return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
		}

		//
		// POST: /Account/VerifyCode
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// The following code protects for brute force attacks against the two factor codes. 
			// If a user enters incorrect codes for a specified amount of time then the user account 
			// will be locked out for a specified amount of time. 
			// You can configure the account lockout settings in IdentityConfig
			var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
			switch (result)
			{
				case SignInStatus.Success:
					return RedirectToLocal(model.ReturnUrl);
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.Failure:
				default:
					ModelState.AddModelError("", "Invalid code.");
					return View(model);
			}
		}

		//
		// GET: /Account/Register
		[AllowAnonymous]
		public ActionResult Register()
		{
			setDisplayParams();
			if (Request.IsAuthenticated)
			{
				ViewBag.displayForm = "False";
			}
			else
			{
				ViewBag.displayForm = "True";
			}
			if (Request.UrlReferrer != null)
			{
				Session["returnUrl"] = (Request.UrlReferrer.AbsolutePath == "/Account/Login" || Request.UrlReferrer.AbsolutePath == "/Account/Register") ? Session["returnUrl"] : Request.UrlReferrer.AbsolutePath;
			}
			else
			{
				Session["returnUrl"] = "/";
         }
         ViewBag.sourceUrl = Session["returnUrl"].ToString() == "" ? "/" : Session["returnUrl"].ToString();
			return View();
		}


		//
		// POST: /Account/Register
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(RegisterViewModel model)
		{
			setDisplayParams();
			if (ModelState.IsValid)
			{
				SpicyGardenDbContext data = new SpicyGardenDbContext();
				var user = new AccountUser {	UserName = model.UserName,
														Email = model.Email,
														Validated = false,
														CreatedDate = DateTime.Now
				};
				var customer = new Customer { FirstName = model.FirstName,
														LastName = model.LastName,
														Email = model.Email,
														Telephone = model.Telephone,
														CreatedDate = DateTime.Now,
														AccountId = user.Id,
														Validated = false
				};
				var address = new Address {	AddrLine1 = model.AddrLine1,
														AddrLine2 = model.AddrLine2,
														CustomerId = customer.Id,
														AccountId = user.Id,
														CreatedDate = DateTime.Now,
														PostalCode = model.PostalCode
				};
				customer.AddressId = address.Id;
				var result = await UserManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					data.Customers.Add(customer);
					data.Addresses.Add(address);
					try
					{
						data.SaveChanges();
					}
					catch (DbEntityValidationException ex)
					{
						var errorMessages = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
						var fullErrorMessage = string.Join("; ", errorMessages);
						var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
						throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
					}
					await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

					// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
					// Send an email with this link
					// string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
					// var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
					// await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

					// first check html entity encode the return url just in case
					string return_url = Server.HtmlEncode(Request.Form["r"]);
					if (return_url == null)
					{
						return Redirect("/");
					}
					else
					{
						return Redirect(return_url);
					}
				}
				AddErrors(result);
			}
			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Account/ConfirmEmail
		[AllowAnonymous]
		public async Task<ActionResult> ConfirmEmail(string userId, string code)
		{
			if (userId == null || code == null)
			{
				return View("Error");
			}
			var result = await UserManager.ConfirmEmailAsync(userId, code);
			return View(result.Succeeded ? "ConfirmEmail" : "Error");
		}

		//
		// GET: /Account/ForgotPassword
		[AllowAnonymous]
		public ActionResult ForgotPassword()
		{
			setDisplayParams();
			return View();
		}

		//
		// POST: /Account/ForgotPassword
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await UserManager.FindByNameAsync(model.Email);
				if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
				{
					// Don't reveal that the user does not exist or is not confirmed
					return View("ForgotPasswordConfirmation");
				}

				// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
				// Send an email with this link
				// string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
				// var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
				// await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
				// return RedirectToAction("ForgotPasswordConfirmation", "Account");
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Account/ForgotPasswordConfirmation
		[AllowAnonymous]
		public ActionResult ForgotPasswordConfirmation()
		{
			return View();
		}

		//
		// GET: /Account/ResetPassword
		[AllowAnonymous]
		public ActionResult ResetPassword(string code)
		{
			setDisplayParams();
			return code == null ? View("Error") : View();
		}

		//
		// POST: /Account/ResetPassword
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			var user = await UserManager.FindByNameAsync(model.Email);
			if (user == null)
			{
				// Don't reveal that the user does not exist
				return RedirectToAction("ResetPasswordConfirmation", "Account");
			}
			var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
			if (result.Succeeded)
			{
				return RedirectToAction("ResetPasswordConfirmation", "Account");
			}
			AddErrors(result);
			return View();
		}

		//
		// GET: /Account/ResetPasswordConfirmation
		[AllowAnonymous]
		public ActionResult ResetPasswordConfirmation()
		{
			setDisplayParams();
			return View();
		}

		//
		// POST: /Account/LogOff
		public ActionResult Logout()
		{
			System.Diagnostics.Debug.WriteLine("You are being logged out!");
			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
			string oid = Request.Cookies["blowme"].Values["oid"];
			if (Request.Cookies["blowme"] != null)
			{
				HttpCookie myCookie = new HttpCookie("blowme");
				myCookie.Expires = DateTime.Now.AddDays(-1d);
				Response.Cookies.Add(myCookie);
			}
			//clear all open orders for this account
			Orders toCancel = this.SpicyGardenDbContext.Orders.Where(o => o.Id == oid && o.OrderStatus == OrderStatus.Open).FirstOrDefault();
			orderHandler.CancelOrder(toCancel);
			return RedirectToAction("Index", "Home");
		}

		//
		// GET: /Account/ExternalLoginFailure
		[AllowAnonymous]
		public ActionResult ExternalLoginFailure()
		{
			return View();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_userManager != null)
				{
					_userManager.Dispose();
					_userManager = null;
				}

				if (_signInManager != null)
				{
					_signInManager.Dispose();
					_signInManager = null;
				}
			}

			base.Dispose(disposing);
		}

		#region Helpers
		// Used for XSRF protection when adding external logins
		private const string XsrfKey = "XsrfId";

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return RedirectToAction("Index", "Home");
		}

		internal class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string redirectUri)
				 : this(provider, redirectUri, null)
			{
			}

			public ChallengeResult(string provider, string redirectUri, string userId)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				UserId = userId;
			}

			public string LoginProvider { get; set; }
			public string RedirectUri { get; set; }
			public string UserId { get; set; }

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
				if (UserId != null)
				{
					properties.Dictionary[XsrfKey] = UserId;
				}
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
		}
		#endregion
	}
}