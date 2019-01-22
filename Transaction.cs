using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WGBL {
    public enum TransactionTypes { Withdrawl, Deposit };
    public class Transaction {
        public String Id { get; set; }
        public TransactionTypes TransactionType { get; set; }
        public float Amount;
        public DateTime TransactionDate { get; set; }
        public String FromAccountId { get; set; }
        public String ToAccountId { get; set; }
        public String Description { get; set; }
        private static String DataBasePath = 
            "/Users/i869673/Code/WGBL/Database/Transactions.txt";
        public Transaction(TransactionTypes type, float amt) {
            this.Id = Transaction.NewTransactionId(amt, type);
            this.TransactionType = type;
            this.Amount = amt;
        }
        public void Display() {
            Console.WriteLine(
                $"---- Action: {this.TransactionType}\n" +
                $"---- Amount: ${this.Amount}\n" +
                $"---- Date: {this.TransactionDate}\n" +
                $"---- Description: {this.Description}\n");
        }

        public static void SaveTransaction(Transaction trans) {
            string json = JsonConvert.SerializeObject(trans);    
            using (StreamWriter accountDbFile = File.AppendText(Transaction.DataBasePath)) {
                accountDbFile.WriteLine(json);
            }
        }

        private delegate String del(float amt, TransactionTypes type);  
        private static String NewTransactionId(float amt, TransactionTypes type) {
            del getHash = (amount, transType) => 
                String.Format(
                    "{0:X}", Math.Abs(
                        transType.GetHashCode() 
                        + amount.GetHashCode() 
                        + DateTime.Now.GetHashCode()));
            var id = getHash(amt, type);
            return id;
        }

        public static List<Transaction> GetTransactions(String accountId) {
            String record;
            Transaction gotTransaction = null;
            var transactions = new List<Transaction>();
            var dbPath = Transaction.DataBasePath;
            using (var sr = new StreamReader(dbPath)) {
                while ((record = sr.ReadLine()) != null) {
                    gotTransaction = JsonConvert
                        .DeserializeObject<Transaction>(record);
                    if (
                        (gotTransaction.FromAccountId == accountId) 
                        || (gotTransaction.ToAccountId == accountId)) {
                        transactions.Add(gotTransaction);
                    }
                }
                if (gotTransaction == null) {
                   return null;
                }
            }
            return transactions;
        }
    }
}