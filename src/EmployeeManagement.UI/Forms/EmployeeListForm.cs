using EmployeeManagement.Shared.Models;
using EmployeeManagement.Shared.Services;

namespace EmployeeManagement.UI.Forms
{
    public partial class EmployeeListForm : Form
    {
        private static readonly Color NewRowColor = Color.FromArgb(222, 245, 222);
        private static readonly Color DeletedRowColor = Color.FromArgb(250, 220, 220);
        private static readonly Color ModifiedRowColor = Color.FromArgb(255, 248, 196);

        private readonly IEmployeeEditor _viewModel;
        private bool _isBusy;
        private bool _closeConfirmed;

        public EmployeeListForm(IEmployeeEditor viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
            UpdateToolbar();
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);
            await LoadEmployeesAsync();
        }

        protected override async void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (_closeConfirmed || e.Cancel)
            {
                return;
            }

            CommitPendingEdit();

            if (!_viewModel.HasChanges)
            {
                return;
            }

            e.Cancel = true;

            if (_isBusy)
            {
                return;
            }

            var answer = MessageBox.Show(
                this,
                "There are unsaved changes. Do you want to save them before closing?",
                "Unsaved changes",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning);

            if (answer == DialogResult.Cancel)
            {
                return;
            }

            if (answer == DialogResult.Yes && !await SaveAllAsync())
            {
                return;
            }

            _closeConfirmed = true;
            Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                if (saveButton.Enabled)
                {
                    SaveButton_Click(this, EventArgs.Empty);
                }

                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private async Task LoadEmployeesAsync()
        {
            SetBusy(true);
            try
            {
                await _viewModel.LoadAsync();

                departmentColumn.DataSource = _viewModel.Departments;
                positionColumn.DataSource = _viewModel.Positions;
                employeesGrid.DataSource = _viewModel.Rows;
            }
            catch (Exception ex)
            {
                ShowError("Employees could not be loaded.", ex);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task<bool> SaveAllAsync()
        {
            CommitPendingEdit();

            var allSaved = true;
            SetBusy(true);
            try
            {
                var deleteResult = await _viewModel.CommitDeletesAsync();
                if (!deleteResult.Success)
                {
                    allSaved = false;
                    MessageBox.Show(this, deleteResult.Error, "Delete employee", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                var changedRows = employeesGrid.Rows
                    .Cast<DataGridViewRow>()
                    .Where(r => r.DataBoundItem is EmployeeRow { HasChanges: true, IsMarkedForDelete: false })
                    .ToList();

                foreach (var gridRow in changedRows)
                {
                    var employee = (EmployeeRow)gridRow.DataBoundItem!;
                    var result = await _viewModel.SaveAsync(employee);
                    gridRow.ErrorText = result.Success ? string.Empty : result.Error ?? "An unknown error occurred.";
                    allSaved &= result.Success;
                }
            }
            catch (Exception ex)
            {
                allSaved = false;
                ShowError("The changes could not be saved.", ex);
            }
            finally
            {
                SetBusy(false);
                employeesGrid.Invalidate();
            }

            if (!allSaved)
            {
                MessageBox.Show(
                    this,
                    "Some rows could not be saved. Check the rows marked with an error icon.",
                    "Save changes",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            return allSaved;
        }

        private void DeleteRow(EmployeeRow employee)
        {
            _viewModel.MarkForDelete(employee);
            employeesGrid.Invalidate();
            UpdateToolbar();
        }

        private void CommitPendingEdit()
        {
            employeesGrid.EndEdit();
            if (employeesGrid.DataSource is not null)
            {
                BindingContext?[_viewModel.Rows].EndCurrentEdit();
            }
        }

        private void SetBusy(bool isBusy)
        {
            _isBusy = isBusy;
            employeesGrid.Enabled = !isBusy;
            UpdateToolbar();
        }

        private void UpdateToolbar()
        {
            var changedCount = _viewModel.ChangedRowCount;

            addButton.Enabled = !_isBusy;
            saveButton.Enabled = !_isBusy && changedCount > 0;
            deleteButton.Enabled = !_isBusy && employeesGrid.CurrentRow?.DataBoundItem is EmployeeRow;
            reloadButton.Enabled = !_isBusy;
            statusLabel.Text = changedCount == 0
                ? "No unsaved changes"
                : $"{changedCount} unsaved change(s)";
        }

        private void ShowError(string message, Exception exception)
        {
            MessageBox.Show(this, $"{message}{Environment.NewLine}{exception.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void AddButton_Click(object? sender, EventArgs e)
        {
            if (employeesGrid.NewRowIndex < 0)
            {
                return;
            }

            employeesGrid.CurrentCell = employeesGrid.Rows[employeesGrid.NewRowIndex].Cells[firstNameColumn.Index];
            employeesGrid.Focus();
            employeesGrid.BeginEdit(true);
        }

        private async void SaveButton_Click(object? sender, EventArgs e)
        {
            await SaveAllAsync();
        }

        private void DeleteButton_Click(object? sender, EventArgs e)
        {
            if (employeesGrid.CurrentRow?.DataBoundItem is not EmployeeRow employee)
            {
                return;
            }

            DeleteRow(employee);
        }

        private async void ReloadButton_Click(object? sender, EventArgs e)
        {
            CommitPendingEdit();

            if (_viewModel.HasChanges)
            {
                var confirmed = MessageBox.Show(
                    this,
                    "Discard unsaved changes and reload from the database?",
                    "Reload",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) == DialogResult.Yes;

                if (!confirmed)
                {
                    return;
                }
            }

            await LoadEmployeesAsync();
        }

        private void EmployeesGrid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _viewModel.Rows.Count)
            {
                return;
            }

            var row = _viewModel.Rows[e.RowIndex];
            if (!row.HasChanges)
            {
                return;
            }

            e.CellStyle!.BackColor = row.IsMarkedForDelete
                ? DeletedRowColor
                : row.IsNew ? NewRowColor : ModifiedRowColor;
            e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;

            if (row.IsMarkedForDelete)
            {
                e.CellStyle.ForeColor = Color.DimGray;
                e.CellStyle.Font = new Font(employeesGrid.Font, FontStyle.Strikeout);
            }
        }

        private void EmployeesGrid_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            employeesGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = string.Empty;
            employeesGrid.InvalidateRow(e.RowIndex);
            UpdateToolbar();
        }

        private void EmployeesGrid_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
        {
            if (employeesGrid.IsCurrentCellDirty && employeesGrid.CurrentCell is DataGridViewComboBoxCell)
            {
                employeesGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void EmployeesGrid_RowsChanged(object? sender, EventArgs e)
        {
            UpdateToolbar();
        }

        private void EmployeesGrid_SelectionChanged(object? sender, EventArgs e)
        {
            UpdateToolbar();
        }

        private void EmployeesGrid_DefaultValuesNeeded(object? sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[departmentColumn.Index].Value = _viewModel.Departments.FirstOrDefault()?.Id;
            e.Row.Cells[positionColumn.Index].Value = _viewModel.Positions.FirstOrDefault()?.Id;
            e.Row.Cells[hireDateColumn.Index].Value = DateOnly.FromDateTime(DateTime.Today);
        }

        private void EmployeesGrid_UserDeletingRow(object? sender, DataGridViewRowCancelEventArgs e)
        {
            e.Cancel = true;

            if (e.Row.DataBoundItem is not EmployeeRow employee)
            {
                return;
            }

            DeleteRow(employee);
        }

        private void EmployeesGrid_DataError(object? sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;

            if (e.Context.HasFlag(DataGridViewDataErrorContexts.Parsing) ||
                e.Context.HasFlag(DataGridViewDataErrorContexts.Commit))
            {
                employeesGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "Invalid value.";
            }
        }
    }
}
