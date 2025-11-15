using System.Collections.ObjectModel;
using System.Linq;
using FluentValidation.Results;

namespace Bil372Project.BusinessLayer;

public class ServiceResult
{
    private readonly IReadOnlyDictionary<string, string[]> _errors;

    private ServiceResult(bool succeeded, IDictionary<string, string[]> errors)
    {
        Succeeded = succeeded;
        _errors = new ReadOnlyDictionary<string, string[]>(errors);
    }

    public bool Succeeded { get; }
    public IReadOnlyDictionary<string, string[]> Errors => _errors;

    public static ServiceResult Success() => new(true, new Dictionary<string, string[]>());

    public static ServiceResult Failed(IEnumerable<ValidationFailure> failures)
    {
        var errorGroups = failures
            .GroupBy(f => string.IsNullOrWhiteSpace(f.PropertyName) ? string.Empty : f.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => f.ErrorMessage).Distinct().ToArray());

        return new ServiceResult(false, errorGroups);
    }

    public static ServiceResult Failed(string propertyName, string errorMessage)
    {
        var key = string.IsNullOrWhiteSpace(propertyName) ? string.Empty : propertyName;
        return new ServiceResult(false, new Dictionary<string, string[]>
        {
            [key] = new[] { errorMessage }
        });
    }
}
