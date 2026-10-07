namespace EmployeeManagement.UI.Models;

public sealed record SaveResult(bool Success, string? Error = null)
{
    public static SaveResult Ok() => new(true);

    public static SaveResult Failure(string error) => new(false, error);
}
