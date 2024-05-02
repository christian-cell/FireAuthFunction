namespace FireAuth.Models.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationError[] Errors { get; set; }
       
        public ValidationException():base(){}
       
        public ValidationException(string message):base(message){}
       
        public ValidationException(string message , Exception innerException):base(message , innerException){}

        public ValidationException(string message, ValidationError[] errors) : base(message) => Errors = errors;
    }
};