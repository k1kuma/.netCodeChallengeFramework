using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WorldWideBankSample.Dtos
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
