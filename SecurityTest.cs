using Microsoft.EntityFrameworkCore;
using MyDotNetProject.Data;
using MyDotNetProject.Services;

namespace MyDotNetProject.Tests
{
    public class SecurityTest
    {
        public static void TestEnvironmentVariables()
        {
            // Load environment variables from .env file
            if (File.Exists(".env"))
            {
                foreach (var line in File.ReadAllLines(".env"))
                {
                    var parts = line.Split('=', 2);
                    if (parts.Length == 2 && !parts[0].StartsWith("#"))
                    {
                        Environment.SetEnvironmentVariable(parts[0], parts[1]);
                    }
                }
            }

            // Test that API key is loaded from environment
            var apiKey = Environment.GetEnvironmentVariable("MMA_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("❌ ERROR: MMA_API_KEY not found in environment variables");
                return;
            }

            Console.WriteLine("✅ SUCCESS: MMA_API_KEY loaded from environment variables");
            Console.WriteLine($"   Key starts with: {apiKey.Substring(0, Math.Min(8, apiKey.Length))}...");

            // Verify it's not hardcoded in config
            var configFiles = new[] { "appsettings.json", "appsettings.Development.json" };
            foreach (var file in configFiles)
            {
                if (File.Exists(file))
                {
                    var content = File.ReadAllText(file);
                    if (content.Contains(apiKey))
                    {
                        Console.WriteLine($"❌ ERROR: API key found hardcoded in {file}");
                        return;
                    }
                }
            }

            Console.WriteLine("✅ SUCCESS: No API keys hardcoded in configuration files");
            Console.WriteLine("🔒 Security setup is correct!");
        }
    }
}
