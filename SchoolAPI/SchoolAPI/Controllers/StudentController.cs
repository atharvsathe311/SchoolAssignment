using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SchoolApi.Core.DTO;
using SchoolAPI.Extensions;
using SchoolApi.Core.Service;


namespace SchoolAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {

        private readonly IStudentService _studentService;
        private readonly IValidator<StudentPostDTO> _validator;
        public StudentController(IStudentService studentService, IValidator<StudentPostDTO> validator)
        {
            _studentService = studentService;
            _validator = validator;
        }

        [HttpPost("NewStudent")]
        public async Task<IActionResult> Post([FromBody] StudentPostDTO studentPostDTO)
        {
            var studentValidator = await _validator.ValidateAsync(studentPostDTO);
            if (!studentValidator.IsValid)
            {
                studentValidator.AddToModelState(ModelState);
                return UnprocessableEntity(ModelState);
            }

            try
            {
                var student = await _studentService.CreateStudentAsync(studentPostDTO);
                return Ok(student);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpGet("GetAllStudent")]
        public async Task<IActionResult> GetAllStudentAsync(int page = 1, int pageSize = 10, string searchTerm = "")
        {
            try
            {
                var result = await _studentService.GetAllStudentAsync(page, pageSize, searchTerm);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        [HttpGet("GetStudentById/{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            try
            {
                StudentGetDTO student = await _studentService.GetStudentByIdAsync(id);
                return Ok(student);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("UpdateStudent/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] StudentPostDTO studentPostDTO)
        {
            var studentValidator = await _validator.ValidateAsync(studentPostDTO);
            if (!studentValidator.IsValid)
            {
                studentValidator.AddToModelState(ModelState);
                return UnprocessableEntity(ModelState);
            }

            try
            {
                var student = await _studentService.UpdateStudentAsync(id, studentPostDTO);
                return Ok(student);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("DeleteStudent/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _studentService.DeleteStudentAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
