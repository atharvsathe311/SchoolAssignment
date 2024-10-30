namespace SchoolApi.Core.DTO
{
    public class StudentUpdateDTO
    {
        public int StudentId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? BirthDate { get; set; } = null;
    }
}