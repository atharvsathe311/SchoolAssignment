namespace UserAPI.DTO
{
    public class UserGetDTO
    {
        public string? Email { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}