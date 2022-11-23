using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace AcdemyDashbord.ViewModels
{
    public class AdministrationCreateRoleViewModel
    {
        [Required]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }
}
