using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountTransactionApp
{
    public class TransactionService
    {
        private readonly AccountDbContext _context;

        public TransactionService(AccountDbContext context)
        {
            _context = context;
        }

        // get account by id
        public Account GetAccountById(int accountId)
        {
            return _context.Accounts.Find(accountId);
        }

        // Create a new transaction
        public void CreateTransaction(Transaction transaction)
        {
            var account = _context.Accounts.Find(transaction.AccountId);

            if (account == null) throw new Exception("Account not found");

            // Check overdraft limit
            if (transaction.IsDebit && account.CurrentBalance - transaction.Amount < -account.OverdraftLimit)
            {
                throw new Exception("Transaction exceeds overdraft limit");
            }

            // Update account balance based on the transaction type
            if (transaction.IsDebit)
            {
                account.CurrentBalance -= transaction.Amount;
            }
            else
            {
                account.CurrentBalance += transaction.Amount;
            }

            // Add transaction and save changes
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }

        // Read transactions for a specific account
        public List<Transaction> GetTransactionsByAccount(int accountId)
        {
            return _context.Transactions.Where(t => t.AccountId == accountId).ToList();
        }

        // Update an existing transaction
        public void UpdateTransaction(Transaction updatedTransaction)
        {
            var existingTransaction = _context.Transactions.Find(updatedTransaction.Id);

            if (existingTransaction == null) throw new Exception("Transaction not found");

            var account = _context.Accounts.Find(existingTransaction.AccountId);

            // Revert the original transaction impact on balance
            if (existingTransaction.IsDebit)
            {
                account.CurrentBalance += existingTransaction.Amount;
            }
            else
            {
                account.CurrentBalance -= existingTransaction.Amount;
            }

            // Apply new transaction impact on balance
            if (updatedTransaction.IsDebit && account.CurrentBalance - updatedTransaction.Amount < -account.OverdraftLimit)
            {
                throw new Exception("Transaction exceeds overdraft limit");
            }

            if (updatedTransaction.IsDebit)
            {
                account.CurrentBalance -= updatedTransaction.Amount;
            }
            else
            {
                account.CurrentBalance += updatedTransaction.Amount;
            }

            // Update the transaction
            existingTransaction.Description = updatedTransaction.Description;
            existingTransaction.Amount = updatedTransaction.Amount;
            existingTransaction.IsDebit = updatedTransaction.IsDebit;

            _context.SaveChanges();
        }

        // Delete a transaction
        public void DeleteTransaction(int transactionId)
        {
            var transaction = _context.Transactions.Find(transactionId);

            if (transaction == null) throw new Exception("Transaction not found");

            var account = _context.Accounts.Find(transaction.AccountId);

            // Reverse the impact of the transaction on the account balance
            if (transaction.IsDebit)
            {
                account.CurrentBalance += transaction.Amount;
            }
            else
            {
                account.CurrentBalance -= transaction.Amount;
            }

            // Remove the transaction and save changes
            _context.Transactions.Remove(transaction);
            _context.SaveChanges();
        }
    }

}
