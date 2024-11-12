namespace UserAPI.DTO
{
    public class UserGetDTO
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}