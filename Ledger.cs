using System;
using System.IO;
using System.Globalization;

namespace WGBL {
    class Ledger {
        private static void Greet() {
            Console.WriteLine("Welcome to World's Greatest Banking Ledger.");
        }

        private static void LogOut() {
            Console.WriteLine(
                "Thank you for using the World's Greatest Banking Ledger.");
            Environment.Exit(0);
        }
        

        private static Account CreateNewAccount() {
            Account account = new Account();
            Console.WriteLine("Choose new Username: ");
            var name = Console.ReadLine();
            Console.WriteLine("Choose new PIN: ");
            var pin = Console.ReadLine();
            Console.WriteLine("Re-enter your PIN: ");
            var pinAgain = Console.ReadLine();
            if (pin != pinAgain) {
                Console.WriteLine("PINs did not match, please try again...");
                CreateNewAccount();
            } else {
                account = new Account(name, pin); 
                account.SaveAsNewAccount();
            }
            return account;
        }

        private static Account LogIn() {
            Console.WriteLine("Please enter your Username:");
            var userName = Console.ReadLine();
            Console.WriteLine("Please enter your PIN:");
            var pin = Console.ReadLine();
            var account = Account.GetAccount(userName, pin);
            if (account == null) {
                Console.WriteLine("Invalid Username or PIN; Try again...");
                LogIn();
            }
            Console.WriteLine($"Logged in as: {userName}");
            return account;
        }

        private static void AnotherAction(Account account) {
            Console.WriteLine("Would you like to perform another action? (Y) or (N):");
            var choice = Console.ReadLine();
            switch (choice) {
                case "Y":
                case "y":
                case "yes":
                case "YES":
                case "Yes":
                    ActionPrompt(account);
                    break;
                case "N":
                case "n":
                case "No":
                case "NO":
                default:
                    LogOut();
                    break;
            }
        }

        public static void ActionPrompt(Account account) {
            Console.WriteLine(
                "Select from the following actions: \n" + 
                "(1) ---- Record a deposit \n" +
                "(2) ---- Record a withdrawl \n" +
                "(3) ---- Check balance \n" +
                "(4) ---- See transaction history \n" +
                "(5) ---- Log out");
            var workflowOption = Console.ReadLine();
            String transAmt = "";
            switch (workflowOption) {
                case "1":
                    Console.WriteLine("Enter deposit amount: ");
                    transAmt = Console.ReadLine();
                    Console.WriteLine("Enter transaction description: ");
                    var depDesc = Console.ReadLine();
                    account.Deposit(transAmt, depDesc);
                    Console.WriteLine("Desposit successful\n");
                    break;
                case "2":
                    Console.WriteLine("Enter withdrawl amount: ");
                    transAmt = Console.ReadLine();
                    Console.WriteLine("Enter transaction description: ");
                    var withDesc = Console.ReadLine();
                    account.Withdrawl(transAmt, withDesc);
                    Console.WriteLine("Withdrawl successful\n");
                    break;
                case "3":
                    account.CheckBalance();
                    break;
                case "4":
                    account.ViewTransactions();
                    break;
                case "5":
                    LogOut();
                    break;
                default:
                    Console.WriteLine("Invalid choice, please try again\n");
                    AccountPrompt();
                    break;
            }
            AnotherAction(account);
        }

        public static Account AccountPrompt() {
            Console.WriteLine("Select (1) to login or (2) to create a new account");
            var workflowOption = Console.ReadLine();
            Account account = null;
            switch (workflowOption) {
                case "1":
                    account = LogIn();
                    break;
                case "2":
                    account = CreateNewAccount();
                    break;
                default:
                    Console.WriteLine("Invalid choice, please try again\n");
                    AccountPrompt();
                    break;
            }
            return account;
        }

        static void Main() {
            Greet();
            var account = AccountPrompt();
            ActionPrompt(account);
        }
    }
}
