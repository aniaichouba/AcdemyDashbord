using AcdemyDashbord.Tools;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace AcdemyDashbord.ViewModels
{
    public class AccountEditUserViewModel
    {
        public AccountEditUserViewModel()
        {
            Roles = new List<string>();
            Claims = new List<string>();
        }

        public string Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [ValidEmailDomainAttribute(Domain: "gmail.com,outlook.com")]
        public string Email { get; set; }

        public IList<string> Roles { get; set; }
        public IList<string> Claims { get; set; }
    }
}
