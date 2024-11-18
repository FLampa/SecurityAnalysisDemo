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
            Console.WriteLine("3. Export results (vulnerable)");
            Console.WriteLine("4. Export results (secure)");
            Console.Write("Select option: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    SearchUserVulnerable(input);
                    break;
                case "2":
                    SearchUserSecure(input);
                    break;
                case "3":
                    ExportResultsVulnerable(input);
                    break;
                case "4":
                    ExportResultsSecure(input);
                    break;
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

    // Vulnerable to path traversal
    static void ExportResultsVulnerable(string username)
    {
        try
        {
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();
            using var command = new SqlCommand("SELECT Username, Email FROM Users WHERE Username = @username", connection);
            command.Parameters.AddWithValue("@username", username);
            using var reader = command.ExecuteReader();

            // Vulnerable to path traversal - directly using user input in file path
            string filePath = $"exports/{username}_results.txt";
            File.WriteAllText(filePath, FormatResults(reader));
            Console.WriteLine($"Results exported to {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    // Secure version with path sanitization
    static void ExportResultsSecure(string username)
    {
        try
        {
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();
            using var command = new SqlCommand("SELECT Username, Email FROM Users WHERE Username = @username", connection);
            command.Parameters.AddWithValue("@username", username);
            using var reader = command.ExecuteReader();

            // Secure - sanitize filename and use Path.Combine
            string sanitizedUsername = string.Join("_", username.Split(Path.GetInvalidFileNameChars()));
            string fileName = $"{sanitizedUsername}_results.txt";
            string filePath = Path.Combine("exports", fileName);
            
            // Ensure the exports directory exists and is within the intended directory
            string fullPath = Path.GetFullPath(filePath);
            string exportsDirPath = Path.GetFullPath("exports");
            
            if (!fullPath.StartsWith(exportsDirPath))
            {
                throw new InvalidOperationException("Invalid export path detected");
            }

            Directory.CreateDirectory("exports");
            File.WriteAllText(filePath, FormatResults(reader));
            Console.WriteLine($"Results exported to {filePath}");
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

    static string FormatResults(SqlDataReader reader)
    {
        var results = new System.Text.StringBuilder();
        
        if (!reader.HasRows)
        {
            return "No users found.";
        }

        while (reader.Read())
        {
            results.AppendLine($"Username: {reader["Username"]}");
            results.AppendLine($"Email: {reader["Email"]}");
            results.AppendLine();
        }

        return results.ToString();
    }
}