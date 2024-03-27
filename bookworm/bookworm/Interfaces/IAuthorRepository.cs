using bookworm.Models;

namespace bookworm.Interfaces
{
    public interface IAuthorRepository
    {
        IEnumerable<ApplicationUser> GetFeaturedAuthors();
    }
}
