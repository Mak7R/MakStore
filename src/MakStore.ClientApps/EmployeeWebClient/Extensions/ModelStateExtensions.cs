using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EmployeeWebClient.Extensions;

public static class ModelStateExtensions
{
    public static void AddErrors(this ModelStateDictionary modelState, IDictionary<string, string[]> errors)
    {
        foreach (var error in errors)
            foreach (var errorMessage in error.Value)
                modelState.AddModelError(error.Key, errorMessage);
    }
}