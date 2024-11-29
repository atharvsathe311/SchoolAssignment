namespace SchoolApi.Core.Extensions
{
    public class CreateStudentEventDTO
    {
        public StudentGetDTO? Student { get; set; }
        public List<int> StudentIds { get; set;} = new List<int>();
        
    }
}