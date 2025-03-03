using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.DTOs
{
    public class BankDTO
    {
      
        public int Id { get; set; }
        [Required(ErrorMessage = "Bank name is required.")]
        [StringLength(100, ErrorMessage = "Bank name can't exceed 100 characters.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public string Website { get; set; }
        [Range(1, 1000, ErrorMessage = "The number of branches must be between 1 and 1000.")]
        public int NumberOfBranches { get; set; }
    }
}
