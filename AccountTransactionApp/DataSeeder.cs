using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountTransactionApp
{
    public static class DataSeeder
    {
        public static void SeedDatabase(AccountDbContext context)
        {
            // Path to your JSON file
            var jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data.json");

            // Check if data already exists to avoid duplicate seeding
            if (!context.Accounts.Any() && !context.Transactions.Any())
            {
                Console.WriteLine("Seeding database...");
                // Read JSON file
                var jsonData = File.ReadAllText(jsonFilePath);
                var data = JsonConvert.DeserializeObject<DataModel>(jsonData);

                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Accounts ON;");
                        context.Accounts.AddRange(data.Accounts);
                        context.SaveChanges();
                        context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Accounts OFF;");

                        context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Transactions ON;");
                        context.Transactions.AddRange(data.Transactions);
                        context.SaveChanges();
                        context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Transactions OFF;");

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Error seeding database: {ex.Message}");
                    }
                }

            }
            else
            {
                Console.WriteLine("Database already seeded.");
            }
        }
    }

    public class DataModel
    {
        public List<Account> Accounts { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}
