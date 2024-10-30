namespace SchoolAPI.DTO
{
    public class StudentPostDTO
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
