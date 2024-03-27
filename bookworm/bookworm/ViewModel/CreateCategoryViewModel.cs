using bookworm.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace bookworm.ViewModel
{
    public class CreateCategoryViewModel
    {
        [Required,
        Display(Name = "Category Name")]
        public string name { get; set; }
        public IEnumerable<Category>? categories { get; set; }
    }
}
