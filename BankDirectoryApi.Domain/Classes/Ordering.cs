﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.Classes
{
    public class Ordering<T>
    {
        public Expression<Func<T, object>> OrderBy { get; set; }
        public bool IsDescending { get; set; }
    }
}
