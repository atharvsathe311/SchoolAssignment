using Microsoft.Extensions.DependencyInjection;
using SchoolApi.Core.Models;
using SchoolApi.Core.Repository;

namespace SchoolApi.Core.Service
{
    public class CommonSagaService : ICommonSagaService
    {
        private readonly IServiceProvider _serviceProvider;

        public CommonSagaService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public async Task<bool> EnrolCourses(int id, List<int> courseIds)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _studentRepository = scope.ServiceProvider.GetRequiredService<IStudentRepository>();
                var _courseRepository = scope.ServiceProvider.GetRequiredService<ICourseRepository>();
                var oldStudent = await _studentRepository.GetStudentByIdAsync(id);

                List<Course> courses = [];
                foreach (int courseId in courseIds)
                {
                    var course = await _courseRepository.GetCourseByIdAsync(courseId);

                    if (course == null)
                    {
                        return false;
                    }
                    courses.Add(course);
                }
                oldStudent.Courses = courses;
                await _studentRepository.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> UpdatePaymentStatus(int id)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _studentRepository = scope.ServiceProvider.GetRequiredService<IStudentRepository>();
                var oldStudent = await _studentRepository.GetStudentByIdAsync(id);

                if (oldStudent.StudentId % 2 == 0)
                    return false;
                oldStudent.PaymentStatus = true;
                await _studentRepository.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> DeleteStudent(int id)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _studentRepository = scope.ServiceProvider.GetRequiredService<IStudentRepository>();
                var oldStudent = await _studentRepository.GetStudentByIdAsync(id);
                await _studentRepository.DeleteStudent(oldStudent);
                return true;
            }
        }
    }
}