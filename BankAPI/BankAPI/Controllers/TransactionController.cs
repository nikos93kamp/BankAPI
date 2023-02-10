using AutoMapper;
using BankAPI.Models;
using BankAPI.Services.Implementations;
using BankAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace BankAPI.Controllers
{
    [ApiController]
    [Route("api/v3/[controller]")]
    public class TransactionController : ControllerBase
    {

        private ITransactionService _transactionService;
        IMapper _mapper;

        public TransactionController(ITransactionService transactionService, IMapper mapper)
        {
            _transactionService = transactionService;
            _mapper = mapper;
        }

        //create new transaction
        [HttpPost]
        [Route("create_new_transaction")]
        public IActionResult CreateNewTransaction([FromBody] TransactionRequestDto transactionRequest)
        {
            //but we cannot attach a Transaction model because it has stuff that the user doesn't have to fill
            //so let's create a transactionRequestDto and map to Transaction, now let's create the mapping first in our AutomapperProfiles
            if (!ModelState.IsValid) return BadRequest(transactionRequest);

            var transaction = _mapper.Map<Transaction>(transactionRequest);
            return Ok(_transactionService.CreateNewTransaction(transaction));
        }

        [HttpGet]
        [Route("get_all_transactions")]
        public IActionResult GetAllTransactions()
        {
            //we want to map to GetTransactionModel as defined in our Models
            var transactions = _transactionService.GetAllTransactions();
            var cleanedTransactions = _mapper.Map<IList<GetTransactionModel>>(transactions);
            return Ok(cleanedTransactions);
        }

        //FindTransactionByDate
        [HttpGet]
        [Route("get_transaction_by_date")]
        public IActionResult FindTransactionByDate(DateTime date)
        {
            var transaction = _transactionService.FindTransactionByDate(date);
            var cleanedTransaction = _mapper.Map<IList<GetTransactionModel>>(transaction);
            return Ok(cleanedTransaction);
        }

        [HttpPost]
        [Route("make_deposit")]
        public IActionResult MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin)
        {
            if (!Regex.IsMatch(AccountNumber, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$"))
                return BadRequest("AccountNumber must be 10-digit");

            return Ok(_transactionService.MakeDeposit(AccountNumber, Amount, TransactionPin));
        }

        [HttpPost]
        [Route("make_withdraw")]
        public IActionResult MakeWithdraw(string AccountNumber, decimal Amount, string TransactionPin)
        {
            if (!Regex.IsMatch(AccountNumber, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$"))
                return BadRequest("AccountNumber must be 10-digit");

            return Ok(_transactionService.MakeWithdraw(AccountNumber, Amount, TransactionPin));
        }
    }
}
