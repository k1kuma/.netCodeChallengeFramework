# .netCodeChallengeFramework

If you choose to use Alcumus’s starter app, here are the instructions:
Visit this repository on Github:  https://github.com/Alcumus/.netCodeChallengeFramework 
Create a PRIVATE Repository on your Github account either using "Use this template" or "Fork" option
Work on solution for the coding challenge. Commit as you go to save your work incrementally.
When done with the solution, push the code to github repository.

Go to Github Repository > Settings > Manage Access.
Grant access to following users on your private repository: 
@dhruv-m-patel
@sj-dixon
@nitishnmathur87

## Scenario:
The World Wide Bank is a growing bank based in Canada that accepts money in North American
currencies. The Bank’s customers can create 1 or more checking accounts, including joint accounts. The
checking account balance is always tracked in Canadian currency. Deposits and withdraws can be made
in Canadian dollars, US dollars or Mexican Pesos. The applicable exchange rate is applied when
depositing or withdrawing a foreign currency. Funds can be transferred between two different accounts.

## Instructions:
Create an application that demonstrates your ability to build an API and object model that reflects the
scenario. You are free to implement any mechanism for feeding input into your solution (for example,
using hardcoded data within a unit test). You should provide enough evidence that your solution is
complete by, as a minimum, indicating that it works correctly against the supplied test data. The
solution should be created in C#. Persistent storage is not required for this test.

The focus of our evaluation will be:
1. To test your ability to create an API and object model based on a problem statement
2. To test your ability to use a unit testing framework of your choice
3. To test that the expected output is correct

**Optional:**
Fully demonstrate your full stack skills by creating a UI for the C# application in modern React. If you
choose to do this, we will evaluate it as a demonstration of your ability to create a UI and manage state
in a react front end.

**Exchange Rates:**
$1.00 CAD = $0.50 USD $1.00 CAD = $10.00 MXN


### Test Cases:

**Case 1:** Customer: Stewie Griffin Customer ID: 777 Account Number: 1234 Initial Balance for account
number 1234: $100.00 CAD
Stewie Griffin deposits $300.00 USD to account number 1234.

**Case 2:** Customer: Glenn Quagmire Customer ID: 504 Account Number: 2001 Initial balance for
account number 2001: $35,000.00 CAD
Glenn Quagmire withdraws $5,000.00 MXN from account number 2001. Glenn Quagmire withdraws
$12,500.00 USD from account number 2001. Glenn Quagmire deposits $300.00 CAD to account number
2001.

**Case 3:** Customer: Joe Swanson Customer ID: 002
Account Number: 1010 Initial balance for account number 1010: $7,425.00 CAD
Customer: Joe Swanson Customer ID: 002 Account Number: 5500 Initial balance for account number
5500: $15,000.00 CAD
Joe Swanson withdraws $5,000.00 CAD from account number 5500. Joe Swanson transfers $7,300.00
CAD from account number 1010 to account number 5500. Joe Swanson deposits $13,726.00 MXN to
account number 1010.

**Case 4:** Customer: Peter Griffin Customer ID: 123 Account Number: 0123 Initial balance for account
number 0123: $150.00 CAD
Customer: Lois Griffin Customer ID: 456 Account Number: 0456 Initial balance for account number 0456:
$65,000.00 CAD
Peter Griffin withdraws $70.00 USD from account number 0123. Lois Griffin deposits $23,789.00 USD to
account number 0456. Lois Griffin transfers $23.75 CAD from account number 0456 to Peter Griffin
(account number 0123).

**Case 5:** Customer: Joe Swanson Customer ID: 002 Account Number: 1010 Initial balance for account
number 1010: $7,425.00 CAD
Famous social engineer and thief John Shark (Customer ID 219) attempts to withdraw $100 USD from
account 1010. The bank determines that the account is not John’s and refuses to give him the money.
Optional: The bank notifies Joe Swanson that an unauthorized user attempted to withdraw money.

**Output 1:** Account Number: 1234 Balance: $700.00 CAD
**Output 2:** Account Number: 2001 Balance: $9,800 CAD
**Output 3:** Account Number: 1010 Balance: $1,497.60 CAD Account Number: 5500 Balance: $17,300.00
CAD
**Output 4:** Account Number: 0123 Balance: $33.75 CAD Account Number: 0456 Balance: $112,554.25
CAD
**Output 5:** Account Number: 1010 Balance: $7,425.00 CAD