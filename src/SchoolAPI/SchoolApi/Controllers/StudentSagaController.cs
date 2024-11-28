using System.Net.Http.Headers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolApi.Core.Extensions;
using SchoolApi.Core.Models;
using SchoolApi.DTO;
using SchoolAPI.DTO;

namespace SchoolApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StudentSagaController : ControllerBase
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMapper _mapper;
        private readonly AuthenticationHeaderValue _credentials = new("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJFbWFpbCI6ImF0aGFydnNhdGhlMDMwMkBnbWFpbC5jb20iLCJVc2VySWQiOiIxIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJleHAiOjE3MzI3MDQ4NDUsImlzcyI6IkpXVEF1dGhlbnRpY2F0aW9uU2VydmVyIiwiYXVkIjoiSldUU2VydmljZVBvc3RtYW5DbGllbnQifQ.2-mX0qrhWQ095zWS3upVltrIW0rJuSx95bE0uCo6NS8");
        public StudentSagaController(IHttpClientFactory httpClientFactory, IMapper mapper)
        {
            _httpClientFactory = httpClientFactory;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<IActionResult> ProcessStudent(NewSagaStudent newSagaStudent)
        {
            var student = newSagaStudent.Student;

            var createResponse = await CreateStudent(student);
            var createResponseContent = await createResponse.Content.ReadAsStringAsync();
            StudentGetDTO? responseContent = JsonConvert.DeserializeObject<StudentGetDTO>(createResponseContent);

            if (!createResponse.IsSuccessStatusCode)
                return BadRequest("Student Creation Failed.");

            var courseEnrollmentResponse = await EnrollCourse(responseContent.StudentId,newSagaStudent.CourseIds);

            if (!courseEnrollmentResponse.IsSuccessStatusCode)
            {
                await DeleteStudent(responseContent.StudentId);
                return BadRequest("Course Enrollment Failed.");
            }

            var paymentResponse = await ProcessPayment(responseContent.StudentId);

            if(!paymentResponse.IsSuccessStatusCode)
            {
                await DeleteStudent(responseContent.StudentId);
                return BadRequest("Payment Failed");
            }

            return Ok(responseContent);
        }
        private async Task<HttpResponseMessage> CreateStudent(StudentPostDTO studentPostDTO)
        {
            var client = _httpClientFactory.CreateClient();
            return await client.PostAsJsonAsync("http://ocelotgateway:5273/infinity/service1", studentPostDTO);
        }  
        private async Task<HttpResponseMessage> DeleteStudent(int id)
        {
            var client = _httpClientFactory.CreateClient();
            return await client.DeleteAsync($"http://ocelotgateway:5273/infinity/service1/delete-student/{id}");
        }
        private async Task<HttpResponseMessage> EnrollCourse(int studentId ,List<int> courseIDs)
        {
            var client = _httpClientFactory.CreateClient();
            return await client.PutAsJsonAsync($"http://ocelotgateway:5273/infinity/service1/enrol-courses/{studentId}",courseIDs);
        }
        private async Task<HttpResponseMessage> ProcessPayment(int studentId)
        {
            var client = _httpClientFactory.CreateClient();
            return await client.PutAsync($"http://ocelotgateway:5273/infinity/service1/update-payfdment-status/{studentId}",new StringContent(""));
        }
    }
}