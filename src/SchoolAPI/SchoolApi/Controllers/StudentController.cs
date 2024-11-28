using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SchoolAPI.DTO;
using SchoolApi.Core.Models;
using SchoolApi.Core.Repository;
using SchoolApi.Core.Service;
using Microsoft.AspNetCore.Authorization;
using CommonLibrary.GeneralModels;
using CommonLibrary.Constants;
using CommonLibrary.Exceptions;
using SchoolApi.Helper;
using SchoolApi.Core.Extensions;
using Serilog;
using Newtonsoft.Json;

namespace SchoolAPI.Controllers
{
    /// <summary>
    /// Handles operations related to students such as adding, updating, deleting, and retrieving student records.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    // [Authorize]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;
        private readonly RabbitMQProducer _producer;

        public StudentController(IStudentService studentService, IStudentRepository studentRepository, IMapper mapper, RabbitMQProducer producer, ICourseRepository courseRepository)
        {
            _studentService = studentService;
            _studentRepository = studentRepository;
            _mapper = mapper;
            _producer = producer;
            _courseRepository = courseRepository;
        }

        /// <summary>
        /// Adds a new student to the system.
        /// </summary>
        /// <param name="studentPostDTO">The student data to be added.</param>
        /// <returns>An <see cref="ActionResult"/> representing the created student data.</returns>
        /// <response code="200">The student was successfully created.</response>
        /// <response code="400">The student already exists in the system.</response>
        /// <response code="401">Unauthorized - Invalid or missing authentication token.</response>
        /// <response code="403">Forbidden - User does not have the required role.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpPost]
        // [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(typeof(StudentGetDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

            var createdStudent = await _studentRepository.CreateStudentAsync(student);
            var newStudentDTO = _mapper.Map<StudentGetDTO>(createdStudent);

            Event<StudentGetDTO> message = new()
            {
                EventType = EventType.StudentCreated,
                Content = newStudentDTO
            };

            _producer.SendMessage(message);
            return Ok(newStudentDTO);
        }

        /// <summary>
        /// Retrieves a paginated list of all students, with optional search functionality.
        /// </summary>
        /// <param name="page">The page number to retrieve (default is 1).</param>
        /// <param name="pageSize">The number of records per page (default is 10).</param>
        /// <param name="searchTerm">A search term to filter students by (optional).</param>
        /// <returns>A paginated list of students.</returns>
        /// <response code="200">Successfully retrieved the list of students.</response>
        /// <response code="401">Unauthorized - Invalid or missing authentication token.</response>
        /// <response code="403">Forbidden - User does not have the required role.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpGet]
        [ProducesResponseType(typeof(GetAllStudentsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllStudentAsync(int page = 1, int pageSize = 10, string searchTerm = "")
        {
            var (studentList, count) = await _studentRepository.GetAllStudentAsync(page, pageSize, searchTerm);
            IEnumerable<StudentGetDTO> newStudentList = _mapper.Map<IEnumerable<StudentGetDTO>>(studentList);

            GetAllStudentsDTO getAllStudents = new()
            {
                StudentList = newStudentList,
                TotalCount = count
            };

            return Ok(getAllStudents);
        }

        /// <summary>
        /// Retrieves a student by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the student.</param>
        /// <returns>The student data if found, or a NotFound result if the student does not exist.</returns>
        /// <response code="200">Successfully retrieved the student data.</response>
        /// <response code="401">Unauthorized - Invalid or missing authentication token.</response>
        /// <response code="403">Forbidden - User does not have the required role.</response>
        /// <response code="404">The student with the given ID was not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StudentGetDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Updates the details of an existing student.
        /// </summary>
        /// <param name="student">The updated student data.</param>
        /// <returns>The updated student data if successful, or a BadRequest result if no fields were updated.</returns>
        /// <response code="200">Successfully updated the student data.</response>
        /// <response code="400">No fields were updated.</response>
        /// <response code="401">Unauthorized - Invalid or missing authentication token.</response>
        /// <response code="403">Forbidden - User does not have the required role.</response>
        /// <response code="404">The student with the given ID was not found.</response>
        [HttpPut]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(typeof(StudentGetDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(StudentUpdateDTO student)
        {
            var oldStudent = await _studentRepository.GetStudentByIdAsync(student.StudentId);
            if (oldStudent == null)
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

                Event<StudentGetDTO> message = new()
                {
                    EventType = EventType.StudentUpdated,
                    Content = _mapper.Map<StudentGetDTO>(oldStudent)
                };
                _producer.SendMessage(message);
                return Ok(_mapper.Map<StudentGetDTO>(oldStudent));
            }
            return BadRequest(ErrorMessages.NothingToUpdate);
        }

        /// <summary>
        /// Deactivates a student record (marks the student as inactive).
        /// </summary>
        /// <param name="id">The unique identifier of the student.</param>
        /// <returns>A success result if the student was deactivated, or a NotFound result if the student does not exist.</returns>
        /// <response code="200">Successfully deactivated the student.</response>
        /// <response code="401">Unauthorized - Invalid or missing authentication token.</response>
        /// <response code="403">Forbidden - User does not have the required role.</response>
        /// <response code="404">The student with the given ID was not found.</response>
        [HttpDelete("{id}")]
        // [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var oldStudent = await _studentRepository.GetStudentByIdAsync(id);
            if (oldStudent == null)
            {
                return NotFound(ErrorMessages.StudentNotFoundExceptionDetails);
            }
            oldStudent.IsActive = false;
            await _studentRepository.SaveChangesAsync();
            Event<object> message = new()
            {
                EventType = EventType.StudentDeleted,
                Content = _mapper.Map<StudentGetDTO>(oldStudent)
            };
            _producer.SendMessage(message);
            return Ok();
        }

        [HttpPut("enrol-courses/{id}")]
        public async Task<IActionResult> EnrolCourses(int id, List<int> courseIds)
        {
            var oldStudent = await _studentRepository.GetStudentByIdAsync(id);
            if (oldStudent == null)
            {
                return NotFound(ErrorMessages.StudentNotFoundExceptionDetails);
            }

            List<Course> courses = [];
            foreach (int courseId in courseIds)
            {
                var course = await _courseRepository.GetCourseByIdAsync(courseId);

                if (course == null)
                {
                    BadRequest($"Course with ID {courseId} does not exist.");
                }
                courses.Add(course);
                }
            Log.Logger.Information(JsonConvert.SerializeObject(courses));
            oldStudent.Courses = courses ;
            await _studentRepository.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("update-payment-status/{id}")]
        public async Task<IActionResult> UpdatePaymentStatus(int id)
        {
            var oldStudent = await _studentRepository.GetStudentByIdAsync(id);
            if (oldStudent == null)
            {
                return NotFound(ErrorMessages.StudentNotFoundExceptionDetails);
            }

            oldStudent.PaymentStatus = true;
            await _studentRepository.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("create-course")]
        public async Task<IActionResult> CreateCourse(Course course)
        {
            var createdCourse = await _courseRepository.CreateCourseAsync(course);
            return Ok(createdCourse);
        }

        [HttpDelete("delete-student/{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var oldStudent = await _studentRepository.GetStudentByIdAsync(id);
            if (oldStudent == null)
            {
                return NotFound(ErrorMessages.StudentNotFoundExceptionDetails);
            }
            
            await _studentRepository.DeleteStudent(oldStudent);
            return Ok();
        }
    }
}
