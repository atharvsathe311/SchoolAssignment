using Microsoft.EntityFrameworkCore;
using SchoolApi.Core.Models;

namespace SchoolApi.Core.Data
{
    public class SchoolDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public SchoolDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }
    }
}
