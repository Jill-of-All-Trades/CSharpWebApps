﻿using Grpc.Core;
using Northwind.EFCore.Models;

namespace Northwind.gRPC.Services
{
	public class ShipperService : Shipr.ShiprBase
	{
		private readonly ILogger<ShipperService> _logger;
		private readonly NorthwindContext db;

		public ShipperService(ILogger<ShipperService> logger, NorthwindContext db)
		{
			_logger = logger;
			this.db = db;
		}

		public override async Task<ShipperReply> GetShipper(ShipperRequest request, ServerCallContext context)
		{
			return ToShipperReply(
				await db.Shippers.FindAsync(request.ShipperId)
			);
		}

		private ShipperReply ToShipperReply(Shipper? shipper)
		{
			return new ShipperReply
			{
				ShipperId = shipper?.ShipperId ?? 0,
				CompanyName = shipper?.CompanyName ?? String.Empty,
				Phone = shipper?.Phone ?? String.Empty,
			};
		}
	}
}
