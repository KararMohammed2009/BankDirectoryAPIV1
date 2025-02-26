using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.DTOs
{
    public class BankWithBranchesDTO
    {
        public int Id { get; set; }  // Maps to BankId
        public string Name { get; set; }  // Maps to BankName
        public string Address { get; set; }
        public string ContactNumber { get; set; }  // Maps to Phone
        public string Website { get; set; }  // Maps to WebsiteUrl

        public List<BranchDTO> Branches { get; set; } = new();

    }
}
