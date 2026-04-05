using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Mesage> Mesages { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.Migrate();
        }
    }
}
