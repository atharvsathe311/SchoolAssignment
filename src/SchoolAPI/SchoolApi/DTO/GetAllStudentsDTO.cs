using SchoolApi.Core.Extensions;

namespace SchoolAPI.DTO
{
    public class GetAllStudentsDTO
    {
        public required IEnumerable<StudentGetDTO> StudentList  { get; set; }
        public int TotalCount { get; set;}
    }
}