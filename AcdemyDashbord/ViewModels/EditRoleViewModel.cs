using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace AcdemyDashbord.ViewModels
{
    public class EditRoleViewModel
    {
        public string Id { get; set; }
        [Required]
        [Display(Name = "Role Name")]
        [Remote(action: "CheckingRoleName", controller: "Administration", AdditionalFields = "Id")]
        public string RoleName { get; set; }
        public List<string> Users { get; set; }
    }
}
