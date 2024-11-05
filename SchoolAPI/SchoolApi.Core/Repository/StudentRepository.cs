using SchoolApi.Core.Models;
using SchoolApi.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace SchoolApi.Core.Repository
{
    public class StudentRepository : IStudentRepository
    {
        private readonly SchoolDbContext _schoolDbContext;

        public StudentRepository(SchoolDbContext schoolDbContext)
        {
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
                .FirstOrDefaultAsync(s => s.IsActive == true && s.StudentId == studentId);

            return student ;
        }

        public async Task SaveChangesAsync()
        {
            await _schoolDbContext.SaveChangesAsync();
        }

        public async Task<bool> DuplicateEntriesChecker(Student student)
        {
            var studentList = await _schoolDbContext.Students.Where(s => s.Email == student.Email || s.Phone == student.Phone).ToListAsync();
            if (studentList.Count > 0)
                return true;
            return false;
        }
    }
}
