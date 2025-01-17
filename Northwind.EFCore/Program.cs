// Test Only
using Microsoft.EntityFrameworkCore;
using Northwind.EFCore.Shared;

static void QueryingCategories()
{
	using (NorthwindContext db = new())
	{
		Console.WriteLine("Categories and how many product they have:");

		IQueryable<Category>? categories = db.Categories?.Include(c => c.Products);

		if(categories is null)
		{
            Console.WriteLine("No categories found.");
			return;
        }

		foreach(Category c in categories)
		{
            Console.WriteLine($"{c.CategoryName} has {c.Products.Count} products.");
        }
    }
}

// MAIN
Console.WriteLine($"Using {Northwind.EFCore.Shared.NorthwindContext.provider} database provider.");
QueryingCategories();