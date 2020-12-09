﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hairdresser.Models
{
    public class UserRole {     
        public IdentityUser User { get; set; }
        public string? Role { get; set; }
    }
}
