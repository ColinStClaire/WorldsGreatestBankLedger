# Worlds Greatest Bank Ledger

## Overview
A simple C# console application fufilling basic banking ledger actions:

1. Create a new account
2. Login to existing account
3. Record a deposit
4. Record a withdrawl
5. Check balance
6. See transaction history
7. Logout

## Database
The program uses JSON files for local persistance of account and transaction data. Be sure to set file path variables in Account.cs and Transaction.cs.

## Use
Set `Account.DataBasePath` and `Transaction.DataBasePath` member variables to desired text data storage files.
```
> cd WGBL
> dotnet run
```

## Dependencies
[JSON.NET](https://www.newtonsoft.com/json)

Install with NuGet:
```
PM> Install-Package Newtonsoft.Json
```

