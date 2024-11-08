namespace SchoolApi.Core.Service
{
    public class StudentService : IStudentService
    {
        public int GetAge(DateTime birthDate)
        {
            DateTime currentDate = DateTime.Now;
            int age = currentDate.Year - birthDate.Year;

            if (currentDate < birthDate.AddYears(age))
            {
                age--;
            }
            return age;
        }
    }
}
