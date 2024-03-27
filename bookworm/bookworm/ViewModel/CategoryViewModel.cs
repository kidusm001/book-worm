using System.ComponentModel.DataAnnotations;

namespace bookworm.ViewModel
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        [Required,
        Display(Name = "Category Name")]
        public string Name { get; set; }
    }
}
