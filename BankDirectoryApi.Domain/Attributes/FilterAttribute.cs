using BankDirectoryApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FilterAttribute : Attribute
    {
        public FilterType Type { get; }
        public string MappedProperty { get; }

        public FilterAttribute(FilterType type, Type entityType, string entityProperty)
        {
            Type = type;
            MappedProperty = entityProperty;
        }
    }
}
