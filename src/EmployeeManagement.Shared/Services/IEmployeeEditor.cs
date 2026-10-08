using System.ComponentModel;
using EmployeeManagement.Shared.Models;

namespace EmployeeManagement.Shared.Services;

public interface IEmployeeEditor
{
    BindingList<EmployeeRow> Rows { get; }

    IReadOnlyList<LookupItem> Departments { get; }

    IReadOnlyList<LookupItem> Positions { get; }

    bool HasChanges { get; }

    int ChangedRowCount { get; }

    int PendingDeleteCount { get; }

    Task LoadAsync(CancellationToken cancellationToken = default);

    Task<SaveResult> SaveAsync(EmployeeRow row, CancellationToken cancellationToken = default);

    Task<SaveResult> DeleteAsync(EmployeeRow row, CancellationToken cancellationToken = default);

    void MarkForDelete(EmployeeRow row);

    Task<SaveResult> CommitDeletesAsync(CancellationToken cancellationToken = default);
}
