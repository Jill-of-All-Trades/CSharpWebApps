using GraphQL.Types;
using Northwind.EFCore.Models;

namespace Northwind.GraphQL
{
	public class CategoryType : ObjectGraphType<Category>
	{
		public CategoryType() 
		{
			Name = "Category";
			Field(c => c.CategoryId).Description("Id of the category.");
			Field(c => c.CategoryName).Description("Name of the category.");
			Field(c => c.Description).Description("Description of the category.");
			Field(c => c.Products, type: typeof(ListGraphType<ProductType>))
				.Description("Products in the category.");
		}
	}
}
