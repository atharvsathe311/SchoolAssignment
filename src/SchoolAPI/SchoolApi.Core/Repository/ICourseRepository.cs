using SchoolApi.Core.Models;

namespace SchoolApi.Core.Repository
{
    public interface ICourseRepository
    {
        Task<Course> CreateCourseAsync(Course course);
        Task<Course?> GetCourseByIdAsync(int courseId);

    }
}