using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WorldWideBankSample.Dtos
{
    public class CustomerDto
    {
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
