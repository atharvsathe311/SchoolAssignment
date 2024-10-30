using SchoolApi.Core.Models;
using SchoolApi.Core.Data;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Core.Constants;
using SchoolApi.Core.Service;

namespace SchoolApi.Core.Repository
{
    public class StudentRepository : IStudentRepository
    {
        private readonly SchoolDbContext _schoolDbContext;
        private readonly IStudentService _studentService;

        public StudentRepository(SchoolDbContext schoolDbContext, IStudentService studentService)
        {
            _studentService = studentService;
            _schoolDbContext = schoolDbContext;
        }

        public async Task<Student> CreateStudentAsync(Student student)
        {
            await _schoolDbContext.Students.AddAsync(student);
            await _schoolDbContext.SaveChangesAsync();
            return student;
        }

        public async Task<(IEnumerable<Student>, int count)> GetAllStudentAsync(int page, int pageSize, string searchTerm)
        {
            IQueryable<Student> query = _schoolDbContext.Students.AsQueryable();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(s => s.IsActive == true);
            }
            else
            {
                query = query.Where(s => (s.FirstName.Contains(searchTerm) ||
                                           s.LastName.Contains(searchTerm) ||
                                           s.Email.Contains(searchTerm) ||
                                           s.Age.ToString().Contains(searchTerm)) 
                                           && s.IsActive == true);
            }

            var totalCount = await query.CountAsync();
            var students = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (students, totalCount);
        }

        public async Task<Student?> GetStudentByIdAsync(int studentId)
        {
            var student = await _schoolDbContext.Students
                .FirstOrDefaultAsync(s => s.IsActive == true && s.StudentId == studentId) ?? throw new Exception(ErrorMessages.STUDENT_NOT_FOUND);

            return student ;
        }

        public async Task<Student> UpdateStudentAsync(Student student)
        {
            var oldStudent = await GetStudentByIdAsync(student.StudentId) ?? throw new Exception(ErrorMessages.STUDENT_NOT_FOUND);
            bool isUpdated = false;
            
            if (!string.IsNullOrEmpty(student.FirstName))
            {
                oldStudent.FirstName = student.FirstName;
                isUpdated = true;
            }

            if (!string.IsNullOrEmpty(student.LastName))
            {
                oldStudent.LastName = student.LastName;
                isUpdated = true;
            }

            if (student.BirthDate.HasValue)
            {
                oldStudent.BirthDate = (DateTime)student.BirthDate;
                oldStudent.Age = _studentService.GetAge((DateTime)student.BirthDate);
                isUpdated = true;
            }

            if (!string.IsNullOrEmpty(student.Email))
            {
                oldStudent.Email = student.Email;
                isUpdated = true;
            }

            if (!string.IsNullOrEmpty(student.Phone))
            {
                oldStudent.Phone = student.Phone;
                isUpdated = true;
            }

            if (isUpdated)
            {
                oldStudent.Updated = DateTime.Now;
                await _schoolDbContext.SaveChangesAsync();
                return oldStudent;
            }
            throw new Exception(ErrorMessages.NOTHING_TO_UPDATE);
        }

        public async Task DeleteStudentAsync(int studentId)
        {
            var student = await GetStudentByIdAsync(studentId);
            if(student != null)
                student.IsActive = false;
            await _schoolDbContext.SaveChangesAsync();
        }
    }
}
