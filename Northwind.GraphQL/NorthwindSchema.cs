using GraphQL.Types;

using Northwind.EFCore.Models;
using Microsoft.Extensions.DependencyInjection; // GetRequiredService

namespace Northwind.GraphQL
{
	public class NorthwindSchema : Schema
	{
		public NorthwindSchema(IServiceProvider provider) : base(provider) 
		{
			// Query = new GreetQuery();
			Query = new NorthwindQuery(provider.GetRequiredService<NorthwindContext>());
		}
	}
}
