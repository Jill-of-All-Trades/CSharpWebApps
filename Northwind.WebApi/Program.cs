//using Northwind.WebApi.Init;
//DBNorthwindSQLSetter.Init();

using Microsoft.AspNetCore.Mvc.Formatters;
using Northwind.EFCore.Models;
using Northwind.WebApi.Repositories;

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

// HTTP Logging
using Microsoft.AspNetCore.HttpLogging;
using Northwind.WebApi;


// MAIN
var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("https://localhost:5002/");

// Service - logging
builder.Services.AddHttpLogging(options =>
{
	options.LoggingFields = HttpLoggingFields.All;
	options.RequestBodyLogLimit = 4096; // 32 KB
	options.ResponseBodyLogLimit = 4096; // 32 KB
});

// Service - CORS (cross-origin resource sharing)
builder.Services.AddCors();

// HealthCare
builder.Services.AddHealthChecks()
	.AddDbContextCheck<NorthwindContext>();

// Add services to the container.
builder.Services.AddDbContext<NorthwindContext>();

builder.Services.AddControllers(options =>
{
	Console.WriteLine("Default output formatters:");
	foreach (IOutputFormatter formatter in options.OutputFormatters)
	{
		OutputFormatter? mediaFormatter = formatter as OutputFormatter;
		if (mediaFormatter == null)
		{
			Console.WriteLine($" {formatter.GetType().Name}");
		}
		else 
		{
            Console.WriteLine(" {0}, Media types: {1}",
				arg0: mediaFormatter.GetType().Name,
				arg1: string.Join(", ", mediaFormatter.SupportedMediaTypes));
        }
		
	}
})
	.AddXmlDataContractSerializerFormatters()
	.AddXmlSerializerFormatters();
	
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new() { Title = "Northwind Service API", Version = "v1" });
});

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

// build
var app = builder.Build();

app.UseCors(configurePolicy: options => {
	options.WithMethods("GET", "POST", "PUT", "DELETE");
	options.WithOrigins(
		"https://localhost:5001" // allow request from this uri
	);
});

app.UseHealthChecks(path: "/howdoyoufeel");

app.UseMiddleware<SecurityHeaders>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json",
			"Northwind Service API Version 1");

		c.SupportedSubmitMethods(new[]
		{
			SubmitMethod.Get, SubmitMethod.Post,
			SubmitMethod.Put, SubmitMethod.Delete
		});
	});

	app.UseHttpLogging();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
