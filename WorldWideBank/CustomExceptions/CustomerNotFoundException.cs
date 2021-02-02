using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorldWideBankSample.CustomExceptions
{
    public class CustomerNotFoundException: Exception
    {
        public CustomerNotFoundException(int customerId): base($"Customer with Id {customerId} was not found") { }
    }
}
