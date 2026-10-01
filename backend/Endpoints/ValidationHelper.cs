using System.ComponentModel.DataAnnotations;

namespace PhonebookApplication.Endpoints;

internal static class ValidationHelper
{
    public static bool TryValidateModel<T>(T model, out Dictionary<string, string[]> errors)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model);

        bool isValid = Validator.TryValidateObject(
            model,
            validationContext,
            validationResults,
            validateAllProperties: true);

        errors = validationResults
            .GroupBy(vr => vr.MemberNames.FirstOrDefault() ?? string.Empty)
            .ToDictionary(
                g => g.Key,
                g => g
                    .Select(vr => vr.ErrorMessage ?? "Validation failed.")
                    .ToArray()
            );

        return isValid;
    }
}
