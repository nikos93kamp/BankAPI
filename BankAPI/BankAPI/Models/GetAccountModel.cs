﻿using System.ComponentModel.DataAnnotations;

namespace BankAPI.Models
{
    public class GetAccountModel
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
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }
    }
}
