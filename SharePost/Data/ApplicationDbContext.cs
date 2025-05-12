using Microsoft.EntityFrameworkCore;
using SharePost.Models;

namespace SharePost.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Post> Posts { get; set; }
    }
}
