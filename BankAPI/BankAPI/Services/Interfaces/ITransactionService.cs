using BankAPI.Models;

namespace BankAPI.Services.Interfaces
{
    public interface ITransactionService
    {
        Response CreateNewTransaction(Transaction transaction);
        IEnumerable<Transaction> GetAllTransactions();
        IEnumerable<Transaction> FindTransactionByDate(DateTime date);
        Response MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin);
        Response MakeWithdraw(string AccountNumber, decimal Amount, string TransactionPin);
    }
}
