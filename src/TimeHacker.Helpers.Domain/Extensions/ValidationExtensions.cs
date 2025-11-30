using System.ComponentModel.DataAnnotations;

namespace TimeHacker.Helpers.Domain.Extensions;

public static class ValidationExtensions
{
    public static bool IsObjectValid<TObject>(this TObject obj)
    {
        if (obj == null)
            return false;

        var validationContext = new ValidationContext(obj);
        return Validator.TryValidateObject(obj, validationContext, null, true);
    }
    public static bool IsObjectValid<TObject>(this TObject obj, out List<ValidationResult> validationResults)
    {
        validationResults = [];
        if (obj == null)
            return false;

        var validationContext = new ValidationContext(obj);
        return Validator.TryValidateObject(obj, validationContext, validationResults, true);
    }
}
