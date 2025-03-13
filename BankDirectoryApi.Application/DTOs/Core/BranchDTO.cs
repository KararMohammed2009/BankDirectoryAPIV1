using BankDirectoryApi.Domain.Entities;
using BankDirectoryApi.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.DTOs.Core
{
    public class BranchDTO : IValidatableObject
    {
        public int Id { get; set; }
        public int BankId { get; set; } // Foreign Key
        public string FullName { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(FullName) || FullName.Length > 1000)
            {
                yield return new ValidationResult("FullName length must be < 1000.", new[] { "FullName" });
            }
        }
    }
}
