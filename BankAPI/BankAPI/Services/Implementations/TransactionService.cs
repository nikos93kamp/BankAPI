using BankAPI.DAL;
using BankAPI.Models;
using BankAPI.Services.Interfaces;
using BankAPI.Utils;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;


namespace BankAPI.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private BankDbContext _dbContext;
        ILogger<TransactionService> _logger;
        private AppSettings _settings;
        private static string _bankSettlementAccount;
        private readonly IAccountService _accountService;
        public TransactionService(BankDbContext dbContext, ILogger<TransactionService> logger, IOptions<AppSettings> settings, IAccountService accountService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _settings = settings.Value;
            _bankSettlementAccount = _settings.BankSettlementAccount;
            _accountService = accountService;
        }
        public Response CreateNewTransaction(Transaction transaction)
        {
            //create a new transaction
            Response response = new Response();
            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();
            response.ResponseCode = "00";
            response.ResponseMessage = "Transaction created successfully!";
            response.Data = null;

            return response;
        }

        public IEnumerable<Transaction> FindTransactionByDate(DateTime date)
        {
            Response response = new Response();
            var transaction = _dbContext.Transactions.Where(x => x.TransactionDate == date).ToList(); //because there are many transactions in a day
            //var transaction = (from c in _dbContext.Transactions
            //                   where c.TransactionDate == date
            //                   select c).ToList();
            response.ResponseCode = "00";
            response.ResponseMessage = "Transaction created successfully!";
            response.Data = transaction;

            return transaction;
        }

        public IEnumerable<Transaction> GetAllTransactions()
        {
            return _dbContext.Transactions.ToList();
        }

        public Response MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin)
        {
            //make deposit
            Response response = new Response();
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();

            //first check that user - account is valid,
            //we'll need authenticate in UserService, so let's inject IUserService here
            var authUser = _accountService.Authenticate(AccountNumber, TransactionPin);
            if (authUser == null)
                throw new ApplicationException("Invalid credentials");

            //validation passes
            try
            {
                //for deposit, our bankSettlementAccount is the source giving money to the user's account
                sourceAccount = _accountService.GetByAccountNumber(_bankSettlementAccount);
                destinationAccount = _accountService.GetByAccountNumber(AccountNumber);

                //let's update their account balances
                sourceAccount.CurrentAccountBalance -= Amount;
                destinationAccount.CurrentAccountBalance += Amount;

                //check if there's updates
                if ((_dbContext.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) && (_dbContext.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                {
                    //so transaction is successful
                    transaction.TransactionStatus = TranStatus.Success;
                    transaction.DescriptionTransactionStatus = TranStatus.Success.ToString();
                    response.ResponseCode = "00";
                    response.ResponseMessage = "Transaction successful!";
                    response.Data = null;
                }
                else
                {
                    //so transaction is unsuccessful
                    transaction.TransactionStatus = TranStatus.Failed;
                    transaction.DescriptionTransactionStatus = TranStatus.Failed.ToString();
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Transaction failed!";
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {

                _logger.LogError($"An Error Occured => {ex.Message}");
            }

            //set other props of transaction here
            transaction.TransactionType = TranType.Deposit;
            transaction.DescriptionTransactionType = TranType.Deposit.ToString();
            transaction.TransactionSourceAmount = _bankSettlementAccount;
            transaction.TransactionDestinationAmount = AccountNumber;
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionDescription = $"New Transaction From Source => {JsonConvert.SerializeObject(transaction.TransactionSourceAmount)} To Destination Account => {JsonConvert.SerializeObject(transaction.TransactionDestinationAmount)} On Date => {transaction.TransactionDate} For Amount => {JsonConvert.SerializeObject(transaction.TransactionAmount)} Transaction Type => {nameof(transaction.TransactionType)} Transaction Status => {nameof(transaction.TransactionStatus)}";

            //let's commit to db
            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();

            return response;

        }


        public Response MakeWithdraw(string AccountNumber, decimal Amount, string TransactionPin)
        {
            //make withdraw
            Response response = new Response();
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();

            //first check that user - account is valid,
            //we'll need authenticate in UserService, so let's inject IUserService here
            var authUser = _accountService.Authenticate(AccountNumber, TransactionPin);
            if (authUser == null)
                throw new ApplicationException("Invalid credentials");

            //validation passes
            try
            {
                //for withdraw, our bankSettlementAccount is the destination getting money from the user's account
                sourceAccount = _accountService.GetByAccountNumber(AccountNumber);
                destinationAccount = _accountService.GetByAccountNumber(_bankSettlementAccount);

                //let's update their account balances
                sourceAccount.CurrentAccountBalance -= Amount;
                destinationAccount.CurrentAccountBalance += Amount;

                //check if there's updates
                if ((_dbContext.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) && (_dbContext.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                {
                    //so transaction is successful
                    transaction.TransactionStatus = TranStatus.Success;
                    transaction.DescriptionTransactionStatus = TranStatus.Success.ToString();
                    response.ResponseCode = "00";
                    response.ResponseMessage = "Transaction successful!";
                    response.Data = null;
                }
                else
                {
                    //so transaction is unsuccessful
                    transaction.TransactionStatus = TranStatus.Failed;
                    transaction.DescriptionTransactionStatus = TranStatus.Failed.ToString();
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Transaction failed!";
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {

                _logger.LogError($"An Error Occured => {ex.Message}");
            }

            //set other props of transaction here
            transaction.TransactionType = TranType.Withdraw;
            transaction.DescriptionTransactionType = TranType.Withdraw.ToString();
            transaction.TransactionSourceAmount = AccountNumber;
            transaction.TransactionDestinationAmount = _bankSettlementAccount;
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionDescription = $"New Transaction From Source => {JsonConvert.SerializeObject(transaction.TransactionSourceAmount)} To Destination Account => {JsonConvert.SerializeObject(transaction.TransactionDestinationAmount)} On Date => {transaction.TransactionDate} For Amount => {JsonConvert.SerializeObject(transaction.TransactionAmount)} Transaction Type => {nameof(transaction.TransactionType)} Transaction Status => {nameof(transaction.TransactionStatus)}";

            //let's commit to db
            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();

            return response;
        }
    }
}
