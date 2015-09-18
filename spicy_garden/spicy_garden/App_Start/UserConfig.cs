using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using spicy_garden.Models;

namespace spicy_garden
{
	public class EmailService : IIdentityMessageService
	{
		public Task SendAsync(IdentityMessage message)
		{
			// Plug in your email service here to send an email.
			return Task.FromResult(0);
		}
	}

	public class SmsService : IIdentityMessageService
	{
		public Task SendAsync(IdentityMessage message)
		{
			// Plug in your SMS service here to send a text message.
			return Task.FromResult(0);
		}
	}

	// Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
	public class AccountManager : UserManager<AccountUser>
	{
		protected AccountUserStore<AccountUser> _users;
		public AccountManager(AccountUserStore<AccountUser> store)
			 : base(store)
		{
		}

		public static AccountManager Create(IdentityFactoryOptions<AccountManager> options, IOwinContext context)
		{
			var manager = new AccountManager(new AccountUserStore<AccountUser>(context.Get<SpicyGardenDbContext>()));
			return manager;
		}
		public override Task<IdentityResult> CreateAsync(AccountUser user, string password)
		{
			return base.CreateAsync(user, password);
		}
	}

	// Configure the application sign-in manager which is used in this application.
	public class ApplicationSignInManager : SignInManager<AccountUser, string>
	{
		public ApplicationSignInManager(AccountManager userManager, IAuthenticationManager authenticationManager)
			 : base(userManager, authenticationManager)
		{
		}

		public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
		{
			return new ApplicationSignInManager(context.GetUserManager<AccountManager>(), context.Authentication);
		}
	}
}
