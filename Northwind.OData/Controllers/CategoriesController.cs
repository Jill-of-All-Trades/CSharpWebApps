using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Northwind.EFCore.Models;

namespace Northwind.OData.Controllers
{
	public class CategoriesController : ODataController
	{
		private readonly NorthwindContext db;

		public CategoriesController(NorthwindContext db)
		{
			this.db = db;
		}

		[EnableQuery]
		public IActionResult Get()
		{
			return Ok(db.Categories);
		}

		[EnableQuery]
		public IActionResult Get(int key) {
			return Ok(db.Categories.Find(key));
		}
	}
}
