namespace SchoolApi.Core.Extensions
{
    public class EventType
    {
        public const string StudentCreated = "student.created";
        public const string StudentCreateFailed = "rollback.createfailed";
        public const string StudentCreateSucess = "rollback.createsucess";
        public const string StudentCourseEnrolled = "student.courseenrolled";
        public const string StudentCourseEnrolledFailed = "student.courseenrolledfailed";
        public const string StudentPaymentSucess = "student.paymentsucess";
        public const string StudentPaymentFailed = "student.paymentfailed";
        public const string StudentUpdated = "student.updated";
        public const string StudentDeleted = "student.deleted";
    }
}