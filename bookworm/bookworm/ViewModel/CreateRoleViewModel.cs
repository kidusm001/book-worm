using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace bookworm.ViewModel
{
    public class CreateRoleViewModel
    {
        [Required,
        Display(Name ="Role Name")] 
        public string name { get; set;}
        public IEnumerable<IdentityRole>? roles { get; set;}
    }
}
