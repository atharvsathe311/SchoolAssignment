namespace SchoolApi.Core.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IEnumerable<Student>? Students { get; set; } 
    }
}