using SchoolAPI.DTO;

namespace SchoolApi.DTO
{
    public class NewSagaStudent
    {
        public StudentPostDTO? Student { get; set;} 
        public List<int> CourseIds { get; set;} = [];
    }
}