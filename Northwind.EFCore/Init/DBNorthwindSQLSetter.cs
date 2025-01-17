using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Northwind.EFCore.Init
{
	public static class DBNorthwindSQLSetter
	{
		public static void Init()
		{
			const string connection = @"Server=(localdb)\mssqllocaldb;Initial Catalog=master;Integrated Security=True;";
			SqlConnection conn = new SqlConnection(connection);

			try
			{
                // Load sql file
                //const string sqlFilePath = "..\\Northwind.WebApi\\Init\\Northwind4SQLServer.sql";
                const string sqlFilePath = "..\\Init\\Northwind4SQLServer.sql";
                Console.WriteLine($"[Northwind.Init] Trying load sql script: {sqlFilePath} ...");

				FileInfo file = new FileInfo(sqlFilePath);
				StreamReader fileReader = file.OpenText();
				string script = fileReader.ReadToEnd();
				fileReader.Close();


				SqlCommand cmd = new SqlCommand(script, conn);

				Console.WriteLine("[Northwind.Init] Trying to open connection and execute script...");

				conn.Open();
				ExecuteScript(conn, script);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			finally { 
				

				if(conn.State == System.Data.ConnectionState.Open)
				{
					conn.Close();
				}
			}
			Console.WriteLine("[Northwind.Init] Done");
		}

		private static void ExecuteScript(SqlConnection connection, string script)
		{
			string[] commandTextArray = Regex.Split(script, "\r\n[\t ]*GO", RegexOptions.IgnoreCase);

			SqlCommand _cmd = new SqlCommand(String.Empty, connection);
			foreach (string commandText in commandTextArray)
			{
				if (commandText.Trim() == string.Empty) continue;
				if((commandText.Length >= 3) && (commandText.Substring(0, 3).ToUpper() == "USE"))
				{
					throw new Exception("Create-script contains USE-statement. Please provide non-database specific create-scripts!");
				}

				_cmd.CommandText = commandText;
				_cmd.ExecuteNonQuery();
			}
		}
	}
}
