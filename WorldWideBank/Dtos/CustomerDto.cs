using System.ComponentModel.DataAnnotations;

namespace WorldWideBank.Dtos
{
    public class CustomerDto
    {
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
