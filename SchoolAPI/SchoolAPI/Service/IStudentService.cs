using MySqlX.XDevAPI;
using SchoolAPI.DTO;
using SchoolAPI.Models;

namespace SchoolAPI.Service
{
    public interface IStudentService
    {
        Task<StudentGetDTO> CreateStudentAsync(StudentPostDTO studentPostDTO);
        Task<object> GetAllStudentAsync(int page, int pageSize, string searchTerm);
        Task<StudentGetDTO> GetStudentByIdAsync(int studentId);
        Task<StudentGetDTO> UpdateStudentAsync(int id, StudentPostDTO studentPostDTO);
        Task DeleteStudentAsync(int studentId);

    }
}
