﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyBank.Models
{
    public class Operator
    {
        public int OperatorID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public virtual UserProfile UserProfile { get; set; }
        
    }
}