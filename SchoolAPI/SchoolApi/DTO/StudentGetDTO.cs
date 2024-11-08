namespace SchoolAPI.DTO
{
    public class StudentGetDTO
    {
        public int StudentId { get; set; }  
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }
    }
}
