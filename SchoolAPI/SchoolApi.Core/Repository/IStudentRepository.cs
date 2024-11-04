﻿// using SchoolApi.Core.DTO;
using SchoolApi.Core.Models;

namespace SchoolApi.Core.Repository
{
    public interface IStudentRepository
    {
        Task<Student> CreateStudentAsync(Student student);
        Task<(IEnumerable<Student>, int count)> GetAllStudentAsync(int page, int pageSize, string searchTerm);
        Task<Student?> GetStudentByIdAsync(int studentId);
        Task SaveChangesAsync();
        Task<bool> DuplicateEntriesChecker(Student student);

    }
}