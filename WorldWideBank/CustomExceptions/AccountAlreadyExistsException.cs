using System;

namespace WorldWideBank.CustomExceptions
{
    public class AccountAlreadyExistsException: Exception
    {
        public AccountAlreadyExistsException(int accountNumber): base($"Account with Account Id of {accountNumber} already exists.") { }
    }
}
