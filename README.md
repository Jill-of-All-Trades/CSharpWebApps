# C# ASP Core 6.0 web apps
This solution includes:

| Project name  | Description  |
| ------------- | ------------ |
| Cryptography  | AES encrypt/decrypt, Users Auth, Digital Signature |
| Northwind.**EFCore** | [Northwind](https://docs.yugabyte.com/preview/sample-data/northwind/) database migration via **Entity Framework**|
| Northwind.**GraphQL** | Query examples for _{Categories}_ and _{Products}_ with GraphQL  |
| Northwind.**gRPC** | Simple gRPC service for getting _{Shippers}_ from Database |
| Northwind.**OData** | Basic example of using **Open Data Protocol** with Database |
| Northwind.**WebApi** | Example of RESTful API (CRUD for _{Customers}_ in Database) |
| WebChat | Chat via SignalR (WebSocket), supports user registration & groups |
| RestClientTest | Testes for WebApi and OData projects (Rest Client) |

## Dependencies
* C# .NET 6.0 (recommended)
* C# ASP.NET CORE 6.0
* MS SQL Server (2019 LocalDB or newer)

Some project may require extra packages. See .csproj files. If you are using Visual Studio,
it will probably download them automatically.
## Installation

`git clone https://github.com/Jill-of-All-Trades/CSharpWebApps`

### - Northwind database init 
Any **Nortwind.{SomeName}** project requires connection to Northwind database.
To initialize database and fill tables with test data, open **Northwind.EFCore** project,
and then add these lines at the beggining of **Program.cs**:<br/>
```
using Northwind.EFCore.Init;

DBNorthwindSQLSetter.Init();
// ... other code
```
Build the project and run it once. It will create a database at localDB MS SQL Server.
All classes have already been migrated (**Northwind.EFCore.Models**).

### - Northwind database connection (Visual Studio)
At toolbar (at top by default): Tools -> Connect to database...<br/>
In the new window (Add connection):
* __Source data__: Microsoft SQL Server (SqlClient)
* __Server name__: (localdb)\mssqllocaldb <br/> Note: some sql server versions use `.` for local server
* __Database name__: Northwind

<br/>Then press __OK__ button.
<br/>Check server and DB connections: Toolbar -> View -> Server Explorer (Ctrl + Alt + S)
