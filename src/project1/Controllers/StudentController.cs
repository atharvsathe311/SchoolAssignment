using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SchoolAPI.Constants;
using SchoolAPI.DTO;
using SchoolApi.Core.Models;
using SchoolApi.Core.Repository;
using SchoolApi.Core.Service;
using SchoolAPI.Exceptions;

namespace SchoolAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IStudentRepository _studentRepository;
        private readonly IMapper _mapper;

        public StudentController(IStudentService studentService,IStudentRepository studentRepository, IMapper mapper)
        {
            _studentService = studentService;
            _studentRepository = studentRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Post(StudentPostDTO studentPostDTO)
        {
            
            var student = _mapper.Map<Student>(studentPostDTO);

            bool duplicateCheck = await _studentRepository.DuplicateEntriesChecker(student);

            if (duplicateCheck)
                throw new CustomException(ErrorMessages.StudentExistsExceptionDetails);

            student.Age = _studentService.GetAge(studentPostDTO.BirthDate);
            student.Created = DateTime.Now;
            student.Updated = DateTime.Now;
            student.IsActive = true;

            var createdStudent = await _studentRepository.CreateStudentAsync(student) ?? throw new Exception(ErrorMessages.STUDENT_CREATE_FAILED);
            var newStudentDTO = _mapper.Map<StudentGetDTO>(createdStudent);
            return Ok(newStudentDTO);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStudentAsync(int page = 1, int pageSize = 10, string searchTerm = "")
        {
            var (studentList , count) = await _studentRepository.GetAllStudentAsync(page, pageSize, searchTerm);
            IEnumerable<StudentGetDTO> newStudentList = _mapper.Map<IEnumerable<StudentGetDTO>>(studentList);
            GetAllStudentsDTO getAllStudents = new() {
                StudentList = newStudentList,
                TotalCount = count
            }; 
            return Ok(getAllStudents);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var student = await _studentRepository.GetStudentByIdAsync(id);
            if (student == null)
            {
                return NotFound(ErrorMessages.StudentNotFoundExceptionDetails);
            }
            var studentDTO = _mapper.Map<StudentGetDTO>(student);
            return Ok(studentDTO);
        }

        [HttpPut]
        public async Task<IActionResult> Put(StudentUpdateDTO student)
        {
            var oldStudent = await _studentRepository.GetStudentByIdAsync(student.StudentId);
            if(oldStudent == null)
            {
                return NotFound(ErrorMessages.StudentNotFoundExceptionDetails);
            }
            
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
                await _studentRepository.SaveChangesAsync();
                return Ok(_mapper.Map<StudentGetDTO>(oldStudent));
            }
            return BadRequest(ErrorMessages.NothingToUpdate);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var oldStudent = await _studentRepository.GetStudentByIdAsync(id);
            if (oldStudent == null)
            {
                return NotFound(ErrorMessages.StudentNotFoundExceptionDetails);
            }
            oldStudent.IsActive = false;
            await _studentRepository.SaveChangesAsync();
            return Ok();
        }
    }
}
