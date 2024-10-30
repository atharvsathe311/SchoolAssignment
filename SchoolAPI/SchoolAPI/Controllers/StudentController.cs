using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SchoolApi.Core.Constants;
using SchoolAPI.DTO;
using SchoolApi.Core.Models;
using SchoolApi.Core.Repository;
using SchoolApi.Core.Service;

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

        [HttpPost("/newstudent")]
        public async Task<IActionResult> Post([FromBody] StudentPostDTO studentPostDTO)
        {
            var student = _mapper.Map<Student>(studentPostDTO);
            student.Age = _studentService.GetAge(studentPostDTO.BirthDate);
            student.Created = DateTime.Now;
            student.Updated = DateTime.Now;
            student.IsActive = true;

            var createdStudent = await _studentRepository.CreateStudentAsync(student) ?? throw new Exception(ErrorMessages.STUDENT_CREATE_FAILED);
            var newStudentDTO = _mapper.Map<StudentGetDTO>(createdStudent);
            return Ok(newStudentDTO);
        }

        [HttpGet("/getallstudents")]
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

        [HttpGet("/getstudentbyid/{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var student = await _studentRepository.GetStudentByIdAsync(id);
            var studentDTO = _mapper.Map<StudentGetDTO>(student);
            return Ok(studentDTO);
        }

        [HttpPut("/updatestudent/")]
        public async Task<IActionResult> Put([FromBody] StudentUpdateDTO studentUpdateDTO)
        {
            var student = _mapper.Map<Student>(studentUpdateDTO);
            var updatedStudent = await _studentRepository.UpdateStudentAsync(student) ?? throw new Exception(ErrorMessages.STUDENT_UPDATE_FAILED);
            var studentDTO = _mapper.Map<StudentGetDTO>(updatedStudent);
            return Ok(studentDTO);
        }

        [HttpDelete("DeleteStudent/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _studentRepository.DeleteStudentAsync(id);
            return Ok();
        }
    }
}
