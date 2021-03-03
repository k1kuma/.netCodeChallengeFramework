using System;

namespace WorldWideBank.CustomExceptions
{
    public class InvalidTransactionException: Exception
    {
        public InvalidTransactionException(string transaction): base($"Transaction {transaction} was not found") { }
    }
}
