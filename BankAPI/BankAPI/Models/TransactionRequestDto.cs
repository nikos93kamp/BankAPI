using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BankAPI.Models
{
    public class TransactionRequestDto
    {
        public decimal TransactionAmount { get; set; }
        public string TransactionSourceAmount { get; set; }
        public string TransactionDestinationAmount { get; set; }
        public TranType TransactionType { get; set; }
        public string Date { get; set; }
        [DataType(DataType.Date)]
        public DateTime? TransactionDate { get; set; } = DateTime.Now;
    }
}
