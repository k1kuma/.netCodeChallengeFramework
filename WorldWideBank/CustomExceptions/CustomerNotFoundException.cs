using System;

namespace WorldWideBank.CustomExceptions
{
    public class CustomerNotFoundException: Exception
    {
        public CustomerNotFoundException(int customerId): base($"Customer with Id {customerId} was not found") { }
    }
}
