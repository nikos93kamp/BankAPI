using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BankAPI.Models
{
    public class GetTransactionModel
    {
        [Key]
        public int Id { get; set; }
        //θα δημιουργειται σε καθε instance of this class
        public string TransactionUniqueReference { get; set; }
        public decimal TransactionAmount { get; set; }
        public string DescriptionTransactionStatus { get; set; }
        public string TransactionSourceAmount { get; set; }
        public string TransactionDestinationAmount { get; set; }
        public string TransactionDescription { get; set; }
        public string DescriptionTransactionType { get; set; }
        public string Date { get; set; }
        [DataType(DataType.Date)]
        public DateTime? TransactionDate { get; set; } = DateTime.Now;
    }
}

