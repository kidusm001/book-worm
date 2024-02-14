using System.ComponentModel.DataAnnotations;

namespace bookworm.ViewModel
{
    public class EditRoleViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Role name is required")]
        [Display(Name = "Role Name")]
        public string Name { get; set; }

        public List<string>? Users { get; set; }
    }
}
