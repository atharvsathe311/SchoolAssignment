using MySqlX.XDevAPI;
using SchoolApi.Core.DTO;
using SchoolApi.Core.Models;

namespace SchoolApi.Core.Service
{
    public interface IStudentService
    {
        Task<StudentGetDTO> CreateStudentAsync(StudentPostDTO studentPostDTO);
        Task<object> GetAllStudentAsync(int page, int pageSize, string searchTerm);
        Task<StudentGetDTO> GetStudentByIdAsync(int studentId);
        Task<StudentGetDTO> UpdateStudentAsync(int id, StudentUpdateDTO studentPostDTO);
        Task DeleteStudentAsync(int studentId);

    }
}
