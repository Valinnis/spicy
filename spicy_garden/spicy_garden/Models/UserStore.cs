using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace spicy_garden.Models
{
	public class AccountUserStore<TUser> :
		IUserStore<AccountUser>,
		IUserPasswordStore<AccountUser>,
		IUserLockoutStore<AccountUser, string>,
		IUserTwoFactorStore<AccountUser, string>
		where TUser : IUser
	{
		private SpicyGardenDbContext database = new SpicyGardenDbContext();
		public AccountUserStore()
		{
			// implement this
		}
		public AccountUserStore(SpicyGardenDbContext database)
		{

		}
		public Task SetLockoutEndDateAsync(AccountUser user, DateTimeOffset lockoutEnd)
		{
			return null;
		}
		public Task<DateTimeOffset> GetLockoutEndDateAsync(AccountUser user)
		{
			return null;
		}

		public Task SetPasswordHashAsync(AccountUser user, string hash)
		{
			System.Diagnostics.Debug.WriteLine("Setting Password hash to " + hash);
			user.Hash = hash;
			return Task.FromResult(0);
		}
		public async Task<string> GetPasswordHashAsync(AccountUser user)
		{
			System.Diagnostics.Debug.WriteLine("Getting the Password Hash");
			AccountUser t = await this.database.Users.Where(u => u.Id == user.Id).FirstOrDefaultAsync();
			return t.Hash;
		}
		public Task<bool> HasPasswordAsync(AccountUser user)
		{
			System.Diagnostics.Debug.WriteLine("Checking if user has password");
			AccountUser t = this.database.Users.Where(u => u.Id == user.Id).FirstOrDefault();
			return Task.FromResult(t != null);
		}
		public Task CreateAsync(AccountUser user)
		{
			SpicyGardenDbContext database = new SpicyGardenDbContext();
			database.Users.Add(user);
			return database.SaveChangesAsync();
		}
		public Task UpdateAsync(AccountUser user)
		{
			SpicyGardenDbContext database = new SpicyGardenDbContext();
			System.Diagnostics.Debug.WriteLine("Updating the user: "+user.UserName);
			database.Users.Attach(user);
			database.Entry(user).State = EntityState.Modified;
			return database.SaveChangesAsync();
		}
		public Task DeleteAsync(AccountUser user)
		{
			if (user != null)
			{
				System.Diagnostics.Debug.WriteLine("Deleting the user: " + user.ToString());
				SpicyGardenDbContext database = new SpicyGardenDbContext();
				if (database.Entry(user).State == EntityState.Detached)
				{
					database.Users.Attach(user);

				}
				database.Users.Remove(user);
				return database.SaveChangesAsync();
			}
			return null;
		}
		public async Task<AccountUser> FindByIdAsync(string id)
		{
			System.Diagnostics.Debug.WriteLine("Finding the user by ID: " + id);
			AccountUser user = await this.database.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
			return user;
		}
		public async Task<AccountUser> FindByNameAsync(string name)
		{
			System.Diagnostics.Debug.WriteLine("Finding the user by name");
			AccountUser user = await this.database.Users.Where(u => u.UserName.ToLower() == name.ToLower()).FirstOrDefaultAsync();
			return user;
		}
		public void Dispose()
		{
		}

		public Task<int> IncrementAccessFailedCountAsync(AccountUser user)
		{
			throw new NotImplementedException();
		}

		public Task ResetAccessFailedCountAsync(AccountUser user)
		{
			throw new NotImplementedException();
		}

		public Task<int> GetAccessFailedCountAsync(AccountUser user)
		{
			return Task.FromResult(0);
		}

		public Task<bool> GetLockoutEnabledAsync(AccountUser user)
		{
			return Task.FromResult(false);
		}

		public Task SetLockoutEnabledAsync(AccountUser user, bool enabled)
		{
			return Task.FromResult(0);
		}

		public Task SetTwoFactorEnabledAsync(AccountUser user, bool enabled)
		{
			return Task.FromResult(0);
		}

		public Task<bool> GetTwoFactorEnabledAsync(AccountUser user)
		{
			return Task.FromResult(false);
		}
	}
}