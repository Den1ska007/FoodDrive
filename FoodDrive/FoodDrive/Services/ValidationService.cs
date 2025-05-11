
using System.ComponentModel.DataAnnotations;

public static class ValidationService
{
    public static List<ValidationResult> Validate<T>(T entity)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(entity);
        Validator.TryValidateObject(entity, context, results, true);
        return results;
    }

    public static bool IsValid<T>(T entity) => !Validate(entity).Any();
}