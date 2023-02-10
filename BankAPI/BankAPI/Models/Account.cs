using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BankAPI.Models
{
    [Table("Accounts")]
    public class Account
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccountName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public decimal CurrentAccountBalance { get; set; }
        public string AccountNumberGenerated { get; set; } //θα πρέπει να δημιουργήσουμε τον αριθμό λογαριασμού

        //θα αποθηκεύσουμε επίσης το hash και το salt του pin συναλλαγής λογαριασμού
        //we put JustIgnore data annotation, because we don't want to serialize these 2 properties
        [JsonIgnore]
        public byte[] PinHash { get; set; }
        [JsonIgnore]
        public byte[] PinSalt { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }

        //θα δημιουργήσουμε έναν αριθμό λογαριασμού, μέσω του constructor
        //αρχικά θα δημιουργήσουμε ένα random object
        Random random = new Random();

        public Account()
        {
            //θα μας επιστρέψει 10 ψήφιο random number
            //we want the floor of this generated number not the number itself
            AccountNumberGenerated = Convert.ToString((long) Math.Floor(random.NextDouble() * 9_000_000_000L + 1_000_000_000L));
            //επισης το property του AccountName = FirstName + LastName
            AccountName = $"{FirstName} {LastName}";
        }
    }
}
