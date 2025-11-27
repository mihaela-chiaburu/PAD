using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace MovieAPI.Repositories
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }

    }
}
