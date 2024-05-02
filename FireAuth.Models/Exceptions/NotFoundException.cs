namespace FireAuth.Models.Exceptions
{
    public class NotFoundException : Exception
    {
       
        public NotFoundException(string entity , string value): base($"{entity} with Id '{value}' not found"){}
        public NotFoundException(string entity , string property , string value): base($"{entity} with {property} '{value}' not found"){}
       
        public NotFoundException(string entity , Guid value): base($"{entity} with Id '{value}' not found"){}
        public NotFoundException(string entity , string property , Guid value): base($"{entity} with {property} '{value}' not found"){}
        public NotFoundException(string entity , string property , Guid value , string propertyTwo , Guid valueTwo): base($"{entity} with {property} '{value}' and {propertyTwo} '{valueTwo}' not found"){}

    }
};