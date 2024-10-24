using Microsoft.EntityFrameworkCore;
using SchoolAPI.Models;

namespace SchoolAPI.Data
{
    public class SchoolDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public SchoolDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }
    }
}
