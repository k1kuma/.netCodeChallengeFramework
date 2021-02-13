using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorldWideBank.Dtos
{
    public class AccountDto
    {
        [Required]
        public int AccountNumber { get; set; }
        public decimal Balance { get; set; }
        [Required]
        public string CurrencyCode { get; set; }
        [Required]
        public ICollection<CustomerDto> Owners { get; set; }
    }
}
