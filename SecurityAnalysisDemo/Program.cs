using Microsoft.Data.SqlClient;

class Program
{
    static readonly string ConnectionString = "Server=localhost;Database=SecurityAnalysisDemo;Trusted_Connection=True;TrustServerCertificate=True";

    static void Main(string[] args)
    {
        Console.WriteLine("Security Analysis Demo");
        Console.WriteLine("---------------------");

        while (true)
        {
            Console.Write("\nEnter username to search or 'q' to quit: ");
            string? input = Console.ReadLine();

            if (string.IsNullOrEmpty(input) || input.ToLower() == "q")
                break;

            Console.WriteLine("\n1. Search using vulnerable method");
            Console.WriteLine("2. Search using secure method");
            Console.Write("Select option: ");

            string? choice = Console.ReadLine();

            if (choice == "1")
            {
                SearchUserVulnerable(input);
            }
            else if (choice == "2")
            {
                SearchUserSecure(input);
            }
        }
    }

    static void SearchUserVulnerable(string username)
    {
        try
        {
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();
            using var command = new SqlCommand($"SELECT Username, Email FROM Users WHERE Username = '{username}'", connection);
            using var reader = command.ExecuteReader();
            DisplayResults(reader);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void SearchUserSecure(string username)
    {
        try
        {
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();
            using var command = new SqlCommand("SELECT Username, Email FROM Users WHERE Username = @username", connection);
            command.Parameters.AddWithValue("@username", username);
            using var reader = command.ExecuteReader();
            DisplayResults(reader);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void DisplayResults(SqlDataReader reader)
    {
        if (!reader.HasRows)
        {
            Console.WriteLine("No users found.");
            return;
        }

        while (reader.Read())
        {
            Console.WriteLine($"\nUsername: {reader["Username"]}");
            Console.WriteLine($"Email: {reader["Email"]}");
        }
    }
}