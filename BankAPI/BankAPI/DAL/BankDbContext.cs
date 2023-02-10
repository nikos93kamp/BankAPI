using BankAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.DAL
{
    public class BankDbContext : DbContext
    {
        public BankDbContext(DbContextOptions<BankDbContext> options) : base(options)
        {

        }

        //DbSet
        //property for the entity set
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
