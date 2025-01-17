using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.EFCore.Shared
{
	public class Product
	{
		public int ProductID { get; set; } // key

		[Required]
		[StringLength(40)]
		public string ProductName { get; set; } = null!;

		[Column("UnitPrice", TypeName = "money")]
		public decimal? Cost { get; set; }

		[Column("UnitsInStock")]
		public short? Stock {  get; set; }

		public bool Discontinued { get; set; }

		public int CategoryId { get; set; }
		public virtual Category Category { get; set; } = null!;
	}
}
