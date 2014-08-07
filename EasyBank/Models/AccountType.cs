using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EasyBank.Models
{
    public class AccountType
    {
        [Key]
        public int TypeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "AccountTypeRequired")]
        public string TypeName { get; set; }

        public virtual ICollection<Account> Account { get; set; }
    }
}