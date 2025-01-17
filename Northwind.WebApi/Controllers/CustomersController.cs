using Microsoft.AspNetCore.Mvc;

using Northwind.EFCore.Models;
using Northwind.WebApi.Repositories;
using System.Collections.Specialized;

namespace Northwind.WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CustomersController : ControllerBase
	{
		private readonly ICustomerRepository repo;

		// Constructor that injects repo
		public CustomersController(ICustomerRepository repo)
		{
			this.repo = repo;
		}

		// GET: api/customers
		// GET: api/customers/?country=[country]
		// Always retruns a list of customers (may be empty)
		[HttpGet]
		[ProducesResponseType(200, Type = typeof(IEnumerable<Customer>))]
		public async Task<IEnumerable<Customer>> GetCustomers(string? country)
		{
			if (string.IsNullOrWhiteSpace(country))
			{
				return await repo.RetrieveAllAsync();
			}
			else
			{
				return (await repo.RetrieveAllAsync())
					.Where(customer => customer.Country == country);
			}
		}

		// GET: api/customers/[id]
		[HttpGet("{id}", Name = nameof(GetCustomer))]
		[ProducesResponseType(200, Type = typeof(Customer))]
		[ProducesResponseType(404)]
		public async Task<IActionResult> GetCustomer(string id)
		{
			Customer? c = await repo.RetrieveAsync(id);
			if (c is null)
			{
				return NotFound(); // 404 err
			}
			return Ok(c); // 200 - OK
		}

		// POST: api/customers
		// BODY: Customer (JSON, XML)
		[HttpPost]
		[ProducesResponseType(201, Type = typeof(Customer))]
		[ProducesResponseType(400)]
		public async Task<IActionResult> Create([FromBody] Customer c)
		{
			if (c is null)
			{
				return BadRequest(); // 400 - bad req
			}

			Customer? addedCustomer = await repo.CreateAsync(c);

			if (addedCustomer is null)
			{
				return BadRequest("Repository failed to create customer.");
			}
			else
			{
				return CreatedAtRoute(  // 201 - resource created
					routeName: nameof(GetCustomer),
					routeValues: new { id = addedCustomer.CustomerId.ToLower() },
					value: addedCustomer
					);
			}
		}

		// PUT: api/customers/[id]
		// BODY: Customer (JSON, XML)
		[HttpPut("{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> Update(string id, [FromBody] Customer c)
		{
			id = id.ToUpper();
			c.CustomerId = c.CustomerId.ToUpper();

			if(c is null || c.CustomerId != id)
			{
				return BadRequest(); // 400
			}

			Customer? existing = await repo.RetrieveAsync(id);
			if(existing == null)
			{
				return NotFound(); // 404
			}

			await repo.UpdateAsync(id, c);

			return new NoContentResult(); // 204 - no content
		}

		// DELETE: api/customers/[id]
		[HttpDelete("{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> Delete(string id)
		{
			// TEST
			if(id == "bad")
			{
				ProblemDetails problemDetails = new()
				{
					Status = StatusCodes.Status400BadRequest,
					Type = "https://localhost:5001/customers/failed-to-delete",
					Title = $"Customer ID {id} found but failed to delete.",
					Detail = "More details like Company Name, Country and so on.",
					Instance = HttpContext.Request.Path
				};
				return BadRequest(problemDetails); // 400
			}

			Customer? existing = await repo.RetrieveAsync(id);
			if(existing == null)
			{
				return NotFound(); // 404
			}

			bool? deleted =  await repo.DeleteAsync(id);
			if(deleted.HasValue && deleted.Value)
			{
				return new NoContentResult(); // 204
			}
			else
			{
				return BadRequest($"Customer {id} was found but failed to delete."); // 400
			}
		}
	}
}
