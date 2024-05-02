using FireAuth.Models.Exceptions;
using FValidationResult = FluentValidation.Results.ValidationResult;

namespace FireAuth.Services.Extensions
{
    public static class ValidationResultExtension
    {
        public static Models.Exceptions.ValidationError[] ToValidationErrors(this FValidationResult validationResult)
        {
            return validationResult.Errors.Select(failure => new Models.Exceptions.ValidationError()
            {
                Code = failure.ErrorCode,
                PropertyName = failure.PropertyName,
                Error = failure.ErrorMessage
            }).ToArray();
        }
    }
};

