using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace AccountTransactionApp.Tests
{
    public class TransactionServiceTests
    {
        private TransactionService _service;
        private AccountDbContext _context;
        private string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data.json");

        public TransactionServiceTests()
        {
            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new AccountDbContext(options);
            _service = new TransactionService(_context);

            SeedDatabaseFromJson();


        }

        private void SeedDatabaseFromJson()
        {
            // Clear existing data before seeding new data
            _context.Transactions.RemoveRange(_context.Transactions);
            _context.Accounts.RemoveRange(_context.Accounts);
            _context.SaveChanges();

            // Read JSON file
            var jsonData = File.ReadAllText(jsonFilePath);
            var data = JsonConvert.DeserializeObject<DataModel>(jsonData);

            // Seed Accounts
            _context.Accounts.AddRange(data.Accounts);

            // Seed Transactions
            _context.Transactions.AddRange(data.Transactions);

            _context.SaveChanges();
        }

        [Fact]
        public void CreateTransaction_ShouldUpdateAccountBalance()
        {
            var account = _context.Accounts.Find(1);
            var prevBalance = account.CurrentBalance;
            // Arrange
            var transaction = new Transaction
            {
                AccountId = 1,
                Amount = 100,
                IsDebit = true,
                Description = "Test Transaction"
            };

            // Act
            _service.CreateTransaction(transaction);

            // Assert
            account = _context.Accounts.Find(1);
            Assert.Equal(prevBalance - transaction.Amount, account.CurrentBalance);
        }

        [Fact]
        public void CreateTransaction_ShouldThrowExceptionWhenExceedingOverdraftLimit()
        {
            // Arrange
            var transaction = new Transaction
            {
                AccountId = 1,
                Amount = 2000,
                IsDebit = true,
                Description = "Test Overdraft"
            };

            // Act & Assert
            Assert.Throws<Exception>(() => _service.CreateTransaction(transaction));
        }


        [Fact]
        public void UpdateTransaction_ShouldModifyTransactionAndUpdateBalance()
        {
            var account = _context.Accounts.Find(1);
            var preBalance = account.CurrentBalance;
            // Arrange
            var transaction = new Transaction
            {
                AccountId = 1,
                Amount = 100,
                IsDebit = true,
                Description = "Original Transaction"
            };
            _service.CreateTransaction(transaction);

            // Act
            var updatedTransaction = new Transaction
            {
                Id = transaction.Id,
                AccountId = 1,
                Amount = 200,  // Change amount
                IsDebit = true,
                Description = "Updated Transaction"
            };

            _service.UpdateTransaction(updatedTransaction);

            // Assert
            account = _context.Accounts.Find(1);
            Assert.Equal(preBalance - updatedTransaction.Amount, account.CurrentBalance);  // Balance should be reduced further
            var updated = _context.Transactions.Find(transaction.Id);
            Assert.Equal("Updated Transaction", updated.Description);
            Assert.Equal(200, updated.Amount);
        }

        [Fact]
        public void DeleteTransaction_ShouldRemoveTransactionAndRevertBalance()
        {
            var account = _context.Accounts.Find(1);
            var preBalance = account.CurrentBalance;

            // Arrange
            var transaction = new Transaction
            {
                AccountId = 1,
                Amount = 100,
                IsDebit = true,
                Description = "Test Transaction"
            };
            _service.CreateTransaction(transaction);

            // Act
            _service.DeleteTransaction(transaction.Id);

            // Assert
            account = _context.Accounts.Find(1);
            Assert.Equal(preBalance, account.CurrentBalance);  // Balance should be restored
            var deleted = _context.Transactions.Find(transaction.Id);
            Assert.Null(deleted);  // Transaction should be deleted
        }

        [Fact]
        public void DeleteTransaction_ShouldThrowExceptionForNonExistentTransaction()
        {
            // Act & Assert
            Assert.Throws<Exception>(() => _service.DeleteTransaction(999));  // Transaction 999 doesn't exist
        }
    }
}
