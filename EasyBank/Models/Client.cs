using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EasyBank.Models
{
    public class Client
    {
        public int ClientId { get; set; }

        [Display(Name="IsOnlineUser", ResourceType=typeof(Resources.Resource))]
        public bool IsOnlineUser { get; set; }

        public bool IsAlreadyRegistered { get; set; }

        [Display(Name = "Name", ResourceType = typeof(Resources.Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "NameRequired")]
        [StringLength(20, MinimumLength = 3, ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "NameLen3To20")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$", ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "NameFromCap")]
        public string Name { get; set; }

        [Display(Name = "Surname", ResourceType = typeof(Resources.Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "SurnameRequired")]
        [StringLength(30, MinimumLength = 3, ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "SurnLen3To30")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$", ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "NameFromCap")]
        public string Surname { get; set; }

        [Index("UniquePId", IsUnique = true)]
        [Display(Name = "PIdNum", ResourceType = typeof(Resources.Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "PIdNumberRequired")]
        [StringLength(10, MinimumLength = 10, ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "Len10")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "OnlyDigits")]
        public string PIdNumber { get; set; }

        [Display(Name = "BirthDate", ResourceType = typeof(Resources.Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "BirthDateRequired")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Email", ResourceType = typeof(Resources.Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "EmailRequired")]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessage = "EmailInvalid")]
        public string Email { get; set; }

        [Display(Name = "RegDate", ResourceType = typeof(Resources.Resource))]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime RegistrationDate { get; set; }

        public virtual UserProfile UserProfile { get; set; }
        public virtual ICollection<ClientsImage> Images { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
    }
}