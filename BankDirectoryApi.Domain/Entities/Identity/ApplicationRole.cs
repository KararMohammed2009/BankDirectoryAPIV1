﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.Entities.Identity
{
    public class ApplicationRole :IdentityRole
    {

        public ApplicationRole()
        {
          
        }
        public ApplicationRole(string roleName) : this()
        {
            Name = roleName;
        }
    }
}
