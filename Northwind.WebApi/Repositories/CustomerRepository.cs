using Microsoft.EntityFrameworkCore.ChangeTracking;
using Northwind.EFCore.Models;
using System.Collections.Concurrent;

namespace Northwind.WebApi.Repositories
{
	public class CustomerRepository : ICustomerRepository
	{
		private static ConcurrentDictionary<string, Customer>? customersCache;

		private NorthwindContext db;

		public CustomerRepository(NorthwindContext injectedContext)
		{
			db = injectedContext;

			// Preload customers
			if(customersCache is null)
			{
				customersCache = new ConcurrentDictionary<string, Customer>(
					db.Customers.ToDictionary(c => c.CustomerId)
					);
			}
		}

		public async Task<Customer?> CreateAsync(Customer c)
		{
			c.CustomerId = c.CustomerId.ToUpper();

			// Add to db via EF
			EntityEntry<Customer> added = await db.Customers.AddAsync(c);
			int affected = await db.SaveChangesAsync();
			if(affected == 1)
			{
				if (customersCache is null) return c;
				// Add new customer in cache, or update it if exists
				return customersCache.AddOrUpdate(c.CustomerId, c, UpdateCache);
			}
            else
            {
				return null;
            }
        }

		public Task<IEnumerable<Customer>> RetrieveAllAsync()
		{
			// to optimize get from cache
			return Task.FromResult(customersCache is null
				? Enumerable.Empty<Customer>() : customersCache.Values);
		}

		public Task<Customer?> RetrieveAsync(string id)
		{
			id = id.ToUpper();
			if (customersCache is null) return null!;
			customersCache.TryGetValue(id, out Customer? c);
			return Task.FromResult(c);
		}

		private Customer UpdateCache(string id, Customer c)
		{
			Customer? old;
			if (customersCache is not null)
			{
				if(customersCache.TryGetValue(id, out old))
				{
					if(customersCache.TryUpdate(id, c, old))
					{
						return c;
					}
				}
			}

			return null!;
		}

		public async Task<Customer?> UpdateAsync(string id, Customer c)
		{
			id = id.ToUpper();
			c.CustomerId = c.CustomerId.ToUpper();

			// Update in db
			db.Customers.Update(c);
			int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                // Update in cache
				return UpdateCache(id, c);
            }
			return null;
        }

		public async Task<bool?> DeleteAsync(string id)
		{
			id = id.ToUpper();

			Customer? c = db.Customers.Find(id);
			if (c is null) return null;

			db.Customers.Remove(c);
			int affected = await db.SaveChangesAsync();
			if(affected == 1)
			{
				if (customersCache is null) return null;
				return customersCache.TryRemove(id, out c);
			}
			else
			{
				return null;
			}
		}
	}
}
