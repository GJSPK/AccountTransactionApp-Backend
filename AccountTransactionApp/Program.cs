using Microsoft.EntityFrameworkCore;

namespace AccountTransactionApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Account Transaction App");

            using (var context = new AccountDbContext())
            {
                DataSeeder.SeedDatabase(context);

                var transactionService = new TransactionService(context);

                bool exit = false;

                while (!exit)
                {
                    Console.WriteLine("\nPlease select an option:");
                    Console.WriteLine("1. Create a Transaction");
                    Console.WriteLine("2. View Transactions for an Account");
                    Console.WriteLine("3. Update a Transaction");
                    Console.WriteLine("4. Delete a Transaction");
                    Console.WriteLine("5. Exit");
                    Console.Write("\nEnter your choice: ");

                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            CreateTransaction(transactionService);
                            break;
                        case "2":
                            ViewTransactions(transactionService);
                            break;
                        case "3":
                            UpdateTransaction(transactionService);
                            break;
                        case "4":
                            DeleteTransaction(transactionService);
                            break;
                        case "5":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Invalid choice, please try again.");
                            break;
                    }
                }
            }

        }

        static void CreateTransaction(TransactionService transactionService)
        {
            Console.Write("Enter Account ID: ");
            int accountId = int.Parse(Console.ReadLine());

            Console.Write("Enter Description: ");
            string description = Console.ReadLine();

            Console.Write("Enter Amount: ");
            decimal amount = decimal.Parse(Console.ReadLine());

            Console.Write("Is this a Debit? (yes/no): ");
            bool isDebit = Console.ReadLine().ToLower() == "yes";

            try
            {
                var transaction = new Transaction
                {
                    AccountId = accountId,
                    Description = description,
                    Amount = amount,
                    IsDebit = isDebit
                };

                transactionService.CreateTransaction(transaction);
                Console.WriteLine("Transaction created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating transaction: {ex.Message}");
            }
        }

        static void ViewTransactions(TransactionService transactionService)
        {
            Console.Write("Enter Account ID: ");
            int accountId = int.Parse(Console.ReadLine());

            try
            {
                var account = transactionService.GetAccountById(accountId);

                if (account == null)
                {
                    Console.WriteLine("Account not found.");
                    return;
                }

                Console.WriteLine($"Account ID: {account.Id}, Name: {account.Name}, Number: {account.Number}, Current Balance: {account.CurrentBalance}, Overdraft Limit: {account.OverdraftLimit}");

                var transactions = transactionService.GetTransactionsByAccount(accountId);
                if (transactions.Count > 0)
                {
                    Console.WriteLine($"Transactions for Account ID {accountId}:");
                    foreach (var transaction in transactions)
                    {
                        Console.WriteLine($"ID: {transaction.Id}, Description: {transaction.Description}, Amount: {transaction.Amount}, Type: {(transaction.IsDebit ? "Debit" : "Credit")}");
                    }
                }
                else
                {
                    Console.WriteLine("No transactions found for this account.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving transactions: {ex.Message}");
            }
        }

        static void UpdateTransaction(TransactionService transactionService)
        {
            Console.Write("Enter Transaction ID to Update: ");
            int transactionId = int.Parse(Console.ReadLine());

            Console.Write("Enter New Description: ");
            string description = Console.ReadLine();

            Console.Write("Enter New Amount: ");
            decimal amount = decimal.Parse(Console.ReadLine());

            Console.Write("Is this a Debit? (yes/no): ");
            bool isDebit = Console.ReadLine().ToLower() == "yes";

            try
            {
                var transaction = new Transaction
                {
                    Id = transactionId,
                    Description = description,
                    Amount = amount,
                    IsDebit = isDebit
                };

                transactionService.UpdateTransaction(transaction);
                Console.WriteLine("Transaction updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating transaction: {ex.Message}");
            }
        }

        static void DeleteTransaction(TransactionService transactionService)
        {
            Console.Write("Enter Transaction ID to Delete: ");
            int transactionId = int.Parse(Console.ReadLine());

            try
            {
                transactionService.DeleteTransaction(transactionId);
                Console.WriteLine("Transaction deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting transaction: {ex.Message}");
            }
        }
    }
}
