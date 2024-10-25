using AutoMapper;
using SchoolApi.Core.DTO;
using SchoolApi.Core.Models;
using SchoolApi.Core.Repository;

namespace SchoolApi.Core.Service
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IMapper _mapper;
        public StudentService(IStudentRepository studentRepository, IMapper mapper)
        {
            _studentRepository = studentRepository;
            _mapper = mapper;
        }

        public async Task<StudentGetDTO> CreateStudentAsync(StudentPostDTO studentPostDTO)
        {
            var student = _mapper.Map<Student>(studentPostDTO);
            student.Age = GetAge(student.BirthDate);
            student.Created = DateTime.Now;
            student.Updated = DateTime.Now;
            student.IsActive = true;
            var createdStudent = await _studentRepository.CreateStudentAsync(student);
            var newStudentDTO = _mapper.Map<StudentGetDTO>(createdStudent);
            return newStudentDTO;
        }

        public async Task<object> GetAllStudentAsync(int page, int pageSize, string searchTerm)
        {
            var (students, totalCount) = await _studentRepository.GetAllStudentAsync(page, pageSize, searchTerm);
            var studentList = _mapper.Map<IEnumerable<StudentGetDTO>>(students);

            var result = new
            {
                StudentsList = studentList,
                TotalCount = totalCount
            };

            return result;
        }

        public async Task<StudentGetDTO> GetStudentByIdAsync(int studentId)
        {
            var student = await _studentRepository.GetStudentByIdAsync(studentId) ?? throw new Exception("Student Not Found");
            StudentGetDTO result = _mapper.Map<StudentGetDTO>(student);
            return result;
        }

        public async Task<StudentGetDTO> UpdateStudentAsync(int id, StudentPostDTO studentPostDTO)
        {
            var student = await _studentRepository.GetStudentByIdAsync(id) ?? throw new Exception("Student Not Found");
            bool isUpdated = false;

            if (student.FirstName != studentPostDTO.FirstName)
            {
                student.FirstName = studentPostDTO.FirstName;
                isUpdated = true;
            }

            if (student.LastName != studentPostDTO.LastName)
            {
                student.LastName = studentPostDTO.LastName;
                isUpdated = true;
            }

            if (student.BirthDate != studentPostDTO.BirthDate)
            {
                student.BirthDate = studentPostDTO.BirthDate;
                student.Age = GetAge(studentPostDTO.BirthDate);
                isUpdated = true;
            }

            if (student.Email != studentPostDTO.Email)
            {
                student.Email = studentPostDTO.Email;
                isUpdated = true;
            }

            if (student.Phone != studentPostDTO.Phone)
            {
                student.Phone = studentPostDTO.Phone;
                isUpdated = true;
            }

            if (isUpdated)
            {
                student.Updated = DateTime.Now;
                await _studentRepository.SaveChangesAsync();
                var studentToReturnUpdated = _mapper.Map<StudentGetDTO>(student);
                return studentToReturnUpdated;
            }

            await _studentRepository.SaveChangesAsync();
            var studentToReturn = _mapper.Map<StudentGetDTO>(student);
            return studentToReturn;
        }

        public async Task DeleteStudentAsync(int studentId)
        {
            var student = await _studentRepository.GetStudentByIdAsync(studentId) ?? throw new Exception("Student Not Found");

            student.IsActive = false;
            student.Updated = DateTime.Now;
            await _studentRepository.SaveChangesAsync();
        }

        private static int GetAge(DateTime birthDate)
        {
            DateTime currentDate = DateTime.Now;
            int age = currentDate.Year - birthDate.Year;

            if (currentDate < birthDate.AddYears(age))
            {
                age--;
            }
            return age;
        }


    }
}
