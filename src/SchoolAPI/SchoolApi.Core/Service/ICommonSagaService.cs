namespace SchoolApi.Core.Service
{
    public interface ICommonSagaService
    {
        Task<bool> EnrolCourses(int id, List<int> courseIds);
        Task<bool> UpdatePaymentStatus(int id);
        Task<bool> DeleteStudent(int id); 
    }
}