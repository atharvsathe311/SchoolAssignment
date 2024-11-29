namespace SchoolApi.Core.Service
{
    public interface IStudentService
    {
        int GetAge(DateTime birthDate);
        Task<bool> GetStudentStatusAsync();
    }
}
