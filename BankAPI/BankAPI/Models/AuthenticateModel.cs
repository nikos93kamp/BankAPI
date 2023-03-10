using System.ComponentModel.DataAnnotations;

namespace BankAPI.Models
{
    public class AuthenticateModel
    {
        [Required] //let's validate the account is 10-digit using RegExp attribute
        [RegularExpression(@"^[0][1-9]\d{9}$|^[1-9]\d{9}$")]
        public string AccountNumber { get; set; }
        [Required]
        public string Pin { get; set; }
    }
}
