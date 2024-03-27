using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace bookworm.ViewModel
{
    public class AddBookWithAuthorViewModel
    {
        public IEnumerable<SelectListItem> Authors { get; set; }
        [Required(ErrorMessage = "Please select an author.")]
        public string SelectedAuthorId { get; set; }
    }
}
