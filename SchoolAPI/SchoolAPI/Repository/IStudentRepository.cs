using SchoolAPI.DTO;
using SchoolAPI.Models;

namespace SchoolAPI.Repository
{
    public interface IStudentRepository
    {
        Task<Student> CreateStudentAsync(Student student);
        Task<(IEnumerable<Student>,int count)> GetAllStudentAsync(int page, int pageSize, string searchTerm);
        Task<Student?> GetStudentByIdAsync(int studentId);
        Task SaveChangesAsync();

    }
}
