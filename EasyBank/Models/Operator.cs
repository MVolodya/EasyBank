using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EasyBank.Models
{
    public class Operator
    {
        [Key]
        public int OperatorID { get; set; }

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

        [Display(Name = "Email", ResourceType = typeof(Resources.Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "EmailRequired")]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessage = "EmailInvalid")]
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }

        public string DepartmentInfo { get; set; }

        [Index("UniqueOperatorPId", IsUnique = true)]
        [Display(Name = "PIdNum", ResourceType = typeof(Resources.Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "PIdNumberRequired")]
        [StringLength(10, MinimumLength = 10, ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "Len10")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "OnlyDigits")]
        public string PId { get; set; }

        public string Password { get; set; }
        
        public virtual UserProfile UserProfile { get; set; }
        public virtual ICollection<Operation> OperationsHistory { get; set; }

        public Operator()
        {
            RegistrationDate = DateTime.Now;
        }
    }
}