using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyBank.Models
{
    public class Client
    {
        public int ClientId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName="NameRequired")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$", ErrorMessage = "First name must start with Capital letter and consists of latin characters only")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "First name must be longer then 3 letters and smaller then 20")]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "SurnameRequired")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Surname must start with Capital letter and consists of latin characters only")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$", ErrorMessage = "Surname must be longer then 3 letters and smaller then 20")]
        public string Surname { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "PIdNumberRequired")]
        [StringLength(10, MinimumLength = 10, ErrorMessage="PId number must have 10 numbers")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage="Only digits are allowed")]
        public string PIdNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "BirthDateRequired")]
        [Display(Name = "Birth date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }


        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "EmailRequired")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Registration date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RegistrationDate { get; set; }


        public virtual ICollection<Account> Accounts { get; set; }
    }
}