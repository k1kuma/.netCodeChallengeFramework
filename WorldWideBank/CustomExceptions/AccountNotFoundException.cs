using System;

namespace WorldWideBank.CustomExceptions
{
    public class AccountNotFoundException: Exception
    {
        public AccountNotFoundException(int accountId): base($"Account with Id {accountId} was not found") { }
    }
}
