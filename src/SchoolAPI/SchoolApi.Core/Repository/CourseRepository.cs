using Microsoft.EntityFrameworkCore;
using SchoolApi.Core.Data;
using SchoolApi.Core.Models;

namespace SchoolApi.Core.Repository
{
    public class CourseRepository : ICourseRepository
    {
        private readonly SchoolDbContext _schoolDbContext;

        public CourseRepository(SchoolDbContext schoolDbContext)
        {
            _schoolDbContext = schoolDbContext;
        }

        public async Task<Course> CreateCourseAsync(Course course)
        {
            await _schoolDbContext.Courses.AddAsync(course);
            await _schoolDbContext.SaveChangesAsync();
            return course;
        }

        public async Task<Course?> GetCourseByIdAsync(int courseId)
        {
            return await _schoolDbContext.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);
        }

    }
}