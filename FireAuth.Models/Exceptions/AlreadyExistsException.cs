namespace FireAuth.Models.Exceptions
{
    public class AlreadyExistsException : Exception
    {
        public AlreadyExistsException(string entity , string value): base($"{entity} with Id '{value}' already exists"){}
        public AlreadyExistsException(string entity , string property , string value): base($"{entity} with {property} '{value}' already exists"){}
        public AlreadyExistsException(string entity , string property , string value , string secondProperty , string secondValue ): base($"{entity} with {property} '{value}' and {secondProperty} '{secondValue}' already exists"){}
       
        public AlreadyExistsException(string entity , Guid value): base($"{entity} with Id '{value}' already exists"){}
        public AlreadyExistsException(string entity , string property , Guid value): base($"{entity} with {property} '{value}' already exists"){}
        public AlreadyExistsException(string entity , string property , Guid value , string secondProperty , Guid secondValue ): base($"{entity} with {property} '{value}' and {secondProperty} '{secondValue}' already exists"){}
    }
};