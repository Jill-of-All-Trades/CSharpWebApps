/*
 * IMPORTANT
 * How to fix: Grpc namespace couldn't be found ect
 * Cause: cyrillic path to folder contains .nuget packages (probably Username)
 * Fix:
 * 1) Go to %appdata%\NuGet\NuGet.config
 * 2) Open file and add these lines:
 *  <configuration>
        <config>
            <add key="globalPackagesFolder" value="D:\.nuget\packages" />
        </config>
    </configuration>
 * 3) Rebuild project
 * Note: default nuget packet location: %UserProfile%\.nuget\packages
 */

using Northwind.gRPC.Services;
using Northwind.EFCore.Models;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("https://localhost:5006/");
// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddDbContext<NorthwindContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<ShipperService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
