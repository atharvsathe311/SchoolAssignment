using Microsoft.AspNetCore.Mvc;
using SchoolApi.Core.DTO;
using SchoolApi.Core.Service;


namespace SchoolAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {

        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpPost("NewStudent")]
        public async Task<IActionResult> Post([FromBody] StudentPostDTO studentPostDTO)
        {
            var student = await _studentService.CreateStudentAsync(studentPostDTO);
            return Ok(student);
        }

        [HttpGet("GetAllStudent")]
        public async Task<IActionResult> GetAllStudentAsync(int page = 1, int pageSize = 10, string searchTerm = "")
        {
            var result = await _studentService.GetAllStudentAsync(page, pageSize, searchTerm);
            return Ok(result);
        }

        [HttpGet("GetStudentById/{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            StudentGetDTO student = await _studentService.GetStudentByIdAsync(id);
            return Ok(student);
        }

        [HttpPut("UpdateStudent/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] StudentUpdateDTO studentPostDTO)
        {
            var student = await _studentService.UpdateStudentAsync(id, studentPostDTO);
            return Ok(student);
        }

        [HttpDelete("DeleteStudent/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _studentService.DeleteStudentAsync(id);
            return Ok();
        }

    }
}
