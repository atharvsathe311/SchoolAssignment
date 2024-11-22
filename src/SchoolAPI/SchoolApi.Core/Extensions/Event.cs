namespace SchoolApi.Core.Extensions
{
    public class Event<T>
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public string EventType { get; set; } = string.Empty ;
        public T? Content { get; set; }
    }
}
