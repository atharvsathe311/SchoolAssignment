namespace UserAPI.GeneralModels
{
    public class ValidationErrors:ErrorDetails
    {
        public Dictionary<string, List<string>>? Errors { get; set;}
    }
}