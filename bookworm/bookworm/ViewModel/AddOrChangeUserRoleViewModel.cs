using Microsoft.AspNetCore.Identity;

namespace bookworm.ViewModel
{
    public class AddOrChangeUserRoleViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string CurrentRole { get; set; }
        public List<IdentityRole> Roles { get; set; }
        public string SelectedRoleId { get; set; }
    }
}
