using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Reflection.Metadata;
using System.Text.Json.Serialization;

namespace BankAPI.Models
{
    [Table("Transactions")]
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        //θα δημιουργειται σε καθε instance of this class
        public string TransactionUniqueReference { get; set; }
        public decimal TransactionAmount { get; set; }
        public TranStatus TransactionStatus { get; set; }
        public string DescriptionTransactionStatus { get; set; }
        public bool IsSuccessful => TransactionStatus.Equals(TranStatus.Success);
        public string TransactionSourceAmount { get; set; }
        public string TransactionDestinationAmount { get; set; }
        public string TransactionDescription { get; set; }
        public TranType TransactionType { get; set; }
        public string DescriptionTransactionType { get; set; }
        public string Date { get; set; }
        [DataType(DataType.Date)]
        public DateTime? TransactionDate { get; set; } = DateTime.Now;

        //we will use guid to generate it
        public Transaction()
        {
            TransactionUniqueReference = $"{Guid.NewGuid().ToString().Replace("-","").Substring(1,27)}";
        }
    }
    public enum TranStatus
    {
        Failed,
        Success,
        Error
    }

    public enum TranType
    {
        Deposit,
        Withdraw,
    }
}
