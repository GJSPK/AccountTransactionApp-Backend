using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountTransactionApp
{
    public class ExportService
    {
        public void ExportDataToExcel(List<Account> accounts, List<Transaction> transactions, string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Accounts");

                // Add header
                worksheet.Cell(1, 1).Value = "Account Name";
                worksheet.Cell(1, 2).Value = "Account Number";
                worksheet.Cell(1, 3).Value = "Current Balance";

                // Add accounts data
                for (int i = 0; i < accounts.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = accounts[i].Name;
                    worksheet.Cell(i + 2, 2).Value = accounts[i].Number;
                    worksheet.Cell(i + 2, 3).Value = accounts[i].CurrentBalance;
                }

                // Add transactions data
                var transactionWorksheet = workbook.Worksheets.Add("Transactions");
                transactionWorksheet.Cell(1, 1).Value = "Account ID";
                transactionWorksheet.Cell(1, 2).Value = "Description";
                transactionWorksheet.Cell(1, 3).Value = "Amount";
                transactionWorksheet.Cell(1, 4).Value = "Debit/Credit";

                for (int i = 0; i < transactions.Count; i++)
                {
                    transactionWorksheet.Cell(i + 2, 1).Value = transactions[i].AccountId;
                    transactionWorksheet.Cell(i + 2, 2).Value = transactions[i].Description;
                    transactionWorksheet.Cell(i + 2, 3).Value = transactions[i].Amount;
                    transactionWorksheet.Cell(i + 2, 4).Value = transactions[i].IsDebit ? "Debit" : "Credit";
                }

                workbook.SaveAs(filePath);
            }
        }
    }

}
