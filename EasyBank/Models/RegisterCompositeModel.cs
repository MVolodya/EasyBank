using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyBank.Models
{
    public class RegisterCompositeModel
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public Client Client { get; set; } 
    }
}