using Bogus;
using SchoolApi.Core.Models;

namespace SchoolApi.Core.Tests.Helper
{
    public class FakeDataCreator
    {
        public Faker<Student> StudentFaker { get; set; }
        public FakeDataCreator()
        {
            StudentFaker = new Faker<Student>()
                .RuleFor(s => s.FirstName, f => f.Name.FirstName())
                .RuleFor(s => s.LastName, f => f.Name.LastName())
                .RuleFor(s => s.Email, f => f.Internet.Email())
                .RuleFor(s => s.Phone, f => f.Phone.PhoneNumber("9#########"))
                .RuleFor(s => s.BirthDate, f => f.Date.Past(20))
                .RuleFor(s => s.Age, (f, s) => DateTime.Now.Year - s.BirthDate.Value.Year)
                .RuleFor(s => s.Created, f => f.Date.Past())
                .RuleFor(s => s.Updated, f => f.Date.Recent())
                .RuleFor(s => s.IsActive, f => f.Random.Bool(0.75f));
        }
        

    }
}