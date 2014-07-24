using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyBank.Models
{
    public class RegisterCompositeModel
    {
        public int ClientId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PIdNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }


        public virtual ICollection<Image> Images { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
    }
}