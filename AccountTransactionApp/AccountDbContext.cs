using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace AccountTransactionApp
{

    public class AccountDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        // Default constructor for the main application
        public AccountDbContext()
        {
        }

        // Constructor with options for testing
        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // If no options were configured (e.g., in tests), use SQL Server by default
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=10.211.55.2;Initial Catalog=AccountTransactionsDb;uid=sa;password=Docker@1;TrustServerCertificate=true;");
            }
        }

    }


    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal OverdraftLimit { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
    }

    public class Transaction
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsDebit { get; set; }
        public decimal Amount { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}
