using BankAPI.DAL;
using BankAPI.Models;
using BankAPI.Services.Interfaces;
using System;
using System.Text;

namespace BankAPI.Services.Implementations
{
    public class AccountService : IAccountService
    {
        //we will inject the database context through the constructor
        private BankDbContext _dbContext;
        public AccountService(BankDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Account Authenticate(string AccountNumber, string Pin)
        {
            //let's make authenticate
            //does account exist for that number
            var account = _dbContext.Accounts.Where(x => x.AccountNumberGenerated == AccountNumber).SingleOrDefault();
            if (account == null)
                return null;
            //so we have a match
            //verify pinHash
            if (!VerifyPinHash(Pin, account.PinHash, account.PinSalt))
                return null;

            //ok so authentication is passed
            return account;
        }

        private static bool VerifyPinHash(string Pin, byte[] pinHash, byte[] pinSalt)
        {
            if (string.IsNullOrWhiteSpace(Pin)) throw new ArgumentNullException("Pin");
            //now let's verify pin
            using(var hmac = new System.Security.Cryptography.HMACSHA512(pinSalt))
            {
                var computedPinHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Pin));
                for (int i = 0; i < computedPinHash.Length; i++)
                {
                    if (computedPinHash[i] != pinHash[i]) return false;
                }
            }
            return true;
        }

        public Account Create(Account account, string Pin, string ConfirmPin)
        {
            //this is to create a new account
            if (_dbContext.Accounts.Any(x => x.Email == account.Email)) throw new ApplicationException("An account already exists with this email");
            //validate pin
            if (!Pin.Equals(ConfirmPin)) throw new ArgumentException("Pins do not match", "Pin");

            //now all validation passes, let's create account
            //we are hashing/encrypting pin first
            byte[] pinHash, pinSalt;
            //let's create this crypto method
            CreatePinHash(Pin, out pinHash, out pinSalt);

            account.PinHash = pinHash;
            account.PinSalt = pinSalt;

            //all good add new account to db
            _dbContext.Accounts.Add(account);
            _dbContext.SaveChanges();

            return account;
        }

        private static void CreatePinHash(string pin, out byte[] pinHash, out byte[] pinSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                pinSalt = hmac.Key;
                pinHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(pin));
            }
        }

        public void Delete(int Id)
        {
            var account = _dbContext.Accounts.Find(Id);
            if (account != null)
            {
                _dbContext.Accounts.Remove(account);
                _dbContext.SaveChanges();
            }
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return _dbContext.Accounts.ToList();
        }

        public Account GetByAccountNumber(string AccountNumber)
        {
            var account = _dbContext.Accounts.Where(x => x.AccountNumberGenerated == AccountNumber).SingleOrDefault();
            if (account == null)
                return null;

            return account;
        }

        public Account GetById(int Id)
        {
            var account = _dbContext.Accounts.Where(x => x.Id == Id).FirstOrDefault();
            if (account == null)
                return null;

            return account;
        }

        public void Update(Account account, string Pin = null)
        {
            var accountToBeUpdated = _dbContext.Accounts.Where(x => x.Id == account.Id).SingleOrDefault();
            if (accountToBeUpdated == null)
                throw new ApplicationException("Account does not exist");
            //if it exists, let's listen for user wanting to change any of his properties
            if (!string.IsNullOrWhiteSpace(account.Email))
            {
                //this means the user wishes to change his email
                //check if the one he's changing to is not already taken
                if (_dbContext.Accounts.Any(x => x.Email == account.Email))
                    throw new ApplicationException("This Email " + account.Email + " already exists");
                //else change email for him
                accountToBeUpdated.Email = account.Email;
            }

            //actually we want to allow the user to be able to change only email and phonenumber and pin
            if (!string.IsNullOrWhiteSpace(account.PhoneNumber))
            {
                //this means the user wishes to change his phone
                //check if the one he's changing to is not already taken
                if (_dbContext.Accounts.Any(x => x.PhoneNumber == account.PhoneNumber))
                    throw new ApplicationException("This Email " + account.PhoneNumber + " already exists");
                //else change email for him
                accountToBeUpdated.PhoneNumber = account.PhoneNumber;
            }

            //actually we want to allow the user to be able to change only email and phonenumber and pin
            if (!string.IsNullOrWhiteSpace(Pin))
            {
                //this means the user wishes to change his pin
                byte[] pinHash, pinSalt;
                CreatePinHash(Pin, out pinHash, out pinSalt);

                accountToBeUpdated.PinHash = pinHash;
                accountToBeUpdated.PinSalt = pinSalt;
            }
            accountToBeUpdated.DateLastUpdated = DateTime.Now;

            //now persist this update to db
            _dbContext.Accounts.Update(accountToBeUpdated);
            _dbContext.SaveChanges();
        }
    }
}
