using System;

namespace WorldWideBank.CustomExceptions
{
    public class NoDestinationAccountForTransferException: Exception
    {
        public NoDestinationAccountForTransferException(): base("No destination account to perform Transfer transaction.") { }
    }
}
