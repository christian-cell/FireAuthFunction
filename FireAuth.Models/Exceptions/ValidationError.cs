namespace FireAuth.Models.Exceptions
{
    public class ValidationError
    {
        public string Error { get; set; }
        public string PropertyName { get; set; }
        public string Code { get; set; } 
    }
};