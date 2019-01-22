using System;
using System.IO;
using System.Linq.Expressions;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Globalization;

namespace WGBL {
    public class _Account {
        public String Id { get; set; }
        public String OwnerName { get; set; }
        public String Pin { get; set; }
        public float Balance { get; set; }
        public DateTime CreatedOn { get; set; }
    }
    class Account: _Account {
        public List<Transaction> SessionTransactions { get; set; }
        private String DataBasePath = 
            "/Users/i869673/Code/WGBL/Database/Accounts.txt";

        public Account() {
            this.SessionTransactions = new List<Transaction>();
         }
        public Account(
            String _ownerName, String _pin) {
                this.OwnerName = _ownerName;
                this.Pin = _pin;
                this.Balance = 0.00F;
                this.CreatedOn = DateTime.Now;
                this.Id = NewAccountId(this.OwnerName, this.CreatedOn);
                this.SessionTransactions = new List<Transaction>();
        }

        private delegate String del(String name, DateTime createdOn);  
        private static String NewAccountId(String ownerName, DateTime createdOn) {
            del getHash = (name, time) => 
                String.Format(
                    "{0:X}", Math.Abs(name.GetHashCode() + time.GetHashCode()));
            var id = getHash(ownerName, createdOn);
            return id;
        }

        public void Withdrawl(String amount, String description = "") {
            float amt = float.Parse(
                        amount, CultureInfo.InvariantCulture.NumberFormat);
            var balance = this.Balance;
            if (balance - amt < 0) {
                throw new ApplicationException("Insufficient funds");
            }
            var type = TransactionTypes.Withdrawl;
            this.Balance -= amt;
            var newTrans = new Transaction(type, amt) {
                FromAccountId = this.Id,
                ToAccountId = this.Id,
                TransactionDate = DateTime.Now,
                Description = description,
            };
            this.SessionTransactions.Add(newTrans);
            UpdateBalance(newTrans.FromAccountId, this.Balance);
            Transaction.SaveTransaction(newTrans);
        }

        public void Deposit(String amount, String description = "") {
            float amt = float.Parse(
                        amount, CultureInfo.InvariantCulture.NumberFormat);
            var type = TransactionTypes.Deposit;
            this.Balance += amt;
            var transaction = new Transaction(type, amt) {
                TransactionDate = DateTime.Now,
                ToAccountId = this.Id,
                FromAccountId = this.Id,
                Description = description,
            };
            this.SessionTransactions.Add(transaction);
            UpdateBalance(transaction.ToAccountId, this.Balance);
            Transaction.SaveTransaction(transaction);
        }

        public void SaveAsNewAccount() {
            var newOwner = this.OwnerName;
            var newPin = this.Pin;
            var exists = Account.GetAccount(newOwner, newPin);
            if (exists != null) {
                throw new ApplicationException("Account already exists");
            }
            var toSave = new _Account() {
                Id = this.Id,
                Pin = this.Pin, // need to encrypt
                OwnerName = this.OwnerName,
                Balance = this.Balance,
                CreatedOn = DateTime.Now
            };
            string json = JsonConvert.SerializeObject(toSave);
            Console.WriteLine($"Saved new account for: {newOwner}\n");
            var dbPath = 
                "/Users/i869673/Code/WGBL/Database/Accounts.txt";
            //  save to file
            using (StreamWriter accountDbFile = File.AppendText(dbPath)) {
                accountDbFile.WriteLine(json);
            }
        }

        public static Account GetAccount(String ownerName, String PIN) {
            String record;
            Account gotAccount = null;
            _Account account = null;
            var dbPath = 
                "/Users/i869673/Code/WGBL/Database/Accounts.txt";
            using (var sr = new StreamReader(dbPath)) {
                while ((record = sr.ReadLine()) != null) {
                    account = JsonConvert.DeserializeObject<_Account>(record);
                    if (account.OwnerName == ownerName && account.Pin == PIN) {
                        gotAccount = new Account() {
                            Id = account.Id,
                            OwnerName = account.OwnerName,
                            Pin = account.Pin,
                            Balance = account.Balance,
                            CreatedOn = account.CreatedOn,
                        };
                    }
                }
            }
            return gotAccount;
        }

        private static void UpdateBalance(String accountId, float newBalance) {
            String record;
            _Account account = null;
            var dbPath = 
                "/Users/i869673/Code/WGBL/Database/Accounts.txt";
            using (var sr = new StreamReader(dbPath)) {
                while ((record = sr.ReadLine()) != null) {
                    account = JsonConvert.DeserializeObject<_Account>(record);
                    if (account.Id == accountId) {
                        break;
                    }
                }
            }
            account.Balance = newBalance;
            var newRecord = JsonConvert.SerializeObject(account);
            string text = File.ReadAllText(dbPath);
            text = text.Replace(record, newRecord);
            File.WriteAllText(dbPath, text);
        }

        public void ViewTransactions() {
            var id = this.Id;
            Console.WriteLine($"Viewing transaction history for account: {id}");
            var transactions = Transaction.GetTransactions(id);
            foreach (var t in transactions) {
                t.Display();
            }
        }

        public void CheckBalance() {
            // read from file
            Console.WriteLine(
                $"Balance for account {this.Id}: ${this.Balance}");
        }
    }
}
