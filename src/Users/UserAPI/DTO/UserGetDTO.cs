namespace UserAPI.DTO
{
    public class UserGetDTO
    {
        public int UserId {get; set;}
        public string? Email { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}