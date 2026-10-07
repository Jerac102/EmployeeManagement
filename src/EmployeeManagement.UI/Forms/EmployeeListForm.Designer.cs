namespace EmployeeManagement.UI.Forms
{
    partial class EmployeeListForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            DataGridViewCellStyle hireDateCellStyle = new DataGridViewCellStyle();
            DataGridViewCellStyle salaryCellStyle = new DataGridViewCellStyle();
            employeesGrid = new DataGridView();
            toolStrip = new ToolStrip();
            addButton = new ToolStripButton();
            saveButton = new ToolStripButton();
            deleteButton = new ToolStripButton();
            reloadButton = new ToolStripButton();
            statusLabel = new ToolStripLabel();
            idColumn = new DataGridViewTextBoxColumn();
            firstNameColumn = new DataGridViewTextBoxColumn();
            lastNameColumn = new DataGridViewTextBoxColumn();
            emailColumn = new DataGridViewTextBoxColumn();
            hireDateColumn = new DateOnlyPickerColumn();
            salaryColumn = new DataGridViewTextBoxColumn();
            departmentColumn = new DataGridViewComboBoxColumn();
            positionColumn = new DataGridViewComboBoxColumn();
            ((System.ComponentModel.ISupportInitialize)employeesGrid).BeginInit();
            toolStrip.SuspendLayout();
            SuspendLayout();
            //
            // toolStrip
            //
            toolStrip.Dock = DockStyle.Top;
            toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip.Items.AddRange(new ToolStripItem[] { addButton, saveButton, deleteButton, reloadButton, statusLabel });
            toolStrip.Name = "toolStrip";
            toolStrip.TabIndex = 1;
            //
            // addButton
            //
            addButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            addButton.Name = "addButton";
            addButton.Text = "Add";
            addButton.Click += AddButton_Click;
            //
            // saveButton
            //
            saveButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            saveButton.Name = "saveButton";
            saveButton.Text = "Save";
            saveButton.ToolTipText = "Save all changes (Ctrl+S)";
            saveButton.Click += SaveButton_Click;
            //
            // deleteButton
            //
            deleteButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            deleteButton.Name = "deleteButton";
            deleteButton.Text = "Delete";
            deleteButton.Click += DeleteButton_Click;
            //
            // reloadButton
            //
            reloadButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            reloadButton.Name = "reloadButton";
            reloadButton.Text = "Reload";
            reloadButton.ToolTipText = "Discard unsaved changes and reload from the database";
            reloadButton.Click += ReloadButton_Click;
            //
            // statusLabel
            //
            statusLabel.Alignment = ToolStripItemAlignment.Right;
            statusLabel.Name = "statusLabel";
            statusLabel.Text = "No unsaved changes";
            //
            // employeesGrid
            //
            employeesGrid.AllowUserToAddRows = true;
            employeesGrid.AllowUserToDeleteRows = true;
            employeesGrid.AutoGenerateColumns = false;
            employeesGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            employeesGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            employeesGrid.Columns.AddRange(new DataGridViewColumn[] { idColumn, firstNameColumn, lastNameColumn, emailColumn, hireDateColumn, salaryColumn, departmentColumn, positionColumn });
            employeesGrid.Dock = DockStyle.Fill;
            employeesGrid.EditMode = DataGridViewEditMode.EditOnEnter;
            employeesGrid.Location = new Point(0, 0);
            employeesGrid.MultiSelect = false;
            employeesGrid.Name = "employeesGrid";
            employeesGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            employeesGrid.TabIndex = 0;
            employeesGrid.CellFormatting += EmployeesGrid_CellFormatting;
            employeesGrid.CellValueChanged += EmployeesGrid_CellValueChanged;
            employeesGrid.CurrentCellDirtyStateChanged += EmployeesGrid_CurrentCellDirtyStateChanged;
            employeesGrid.DataError += EmployeesGrid_DataError;
            employeesGrid.DefaultValuesNeeded += EmployeesGrid_DefaultValuesNeeded;
            employeesGrid.RowsAdded += EmployeesGrid_RowsChanged;
            employeesGrid.RowsRemoved += EmployeesGrid_RowsChanged;
            employeesGrid.SelectionChanged += EmployeesGrid_SelectionChanged;
            employeesGrid.UserDeletingRow += EmployeesGrid_UserDeletingRow;
            //
            // idColumn
            //
            idColumn.DataPropertyName = "Id";
            idColumn.HeaderText = "Id";
            idColumn.Name = "idColumn";
            idColumn.ReadOnly = true;
            idColumn.FillWeight = 35;
            idColumn.MinimumWidth = 40;
            //
            // firstNameColumn
            //
            firstNameColumn.DataPropertyName = "FirstName";
            firstNameColumn.HeaderText = "First name";
            firstNameColumn.Name = "firstNameColumn";
            firstNameColumn.MinimumWidth = 168;
            //
            // lastNameColumn
            //
            lastNameColumn.DataPropertyName = "LastName";
            lastNameColumn.HeaderText = "Last name";
            lastNameColumn.Name = "lastNameColumn";
            lastNameColumn.MinimumWidth = 168;
            //
            // emailColumn
            //
            emailColumn.DataPropertyName = "Email";
            emailColumn.HeaderText = "Email";
            emailColumn.Name = "emailColumn";
            emailColumn.MinimumWidth = 280;
            //
            // hireDateColumn
            //
            hireDateCellStyle.Format = "d";
            hireDateColumn.DataPropertyName = "HireDate";
            hireDateColumn.DefaultCellStyle = hireDateCellStyle;
            hireDateColumn.HeaderText = "Hire date";
            hireDateColumn.Name = "hireDateColumn";
            hireDateColumn.MinimumWidth = 140;
            //
            // salaryColumn
            //
            salaryCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            salaryCellStyle.Format = "N2";
            salaryColumn.DataPropertyName = "Salary";
            salaryColumn.DefaultCellStyle = salaryCellStyle;
            salaryColumn.HeaderText = "Salary";
            salaryColumn.Name = "salaryColumn";
            salaryColumn.MinimumWidth = 140;
            //
            // departmentColumn
            //
            departmentColumn.DataPropertyName = "DepartmentId";
            departmentColumn.DisplayMember = "Name";
            departmentColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            departmentColumn.FlatStyle = FlatStyle.Flat;
            departmentColumn.HeaderText = "Department";
            departmentColumn.Name = "departmentColumn";
            departmentColumn.ValueMember = "Id";
            departmentColumn.MinimumWidth = 196;
            //
            // positionColumn
            //
            positionColumn.DataPropertyName = "PositionId";
            positionColumn.DisplayMember = "Name";
            positionColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            positionColumn.FlatStyle = FlatStyle.Flat;
            positionColumn.HeaderText = "Position";
            positionColumn.Name = "positionColumn";
            positionColumn.ValueMember = "Id";
            positionColumn.MinimumWidth = 196;
            //
            // EmployeeListForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1400, 600);
            Controls.Add(employeesGrid);
            Controls.Add(toolStrip);
            Name = "EmployeeListForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Employees";
            ((System.ComponentModel.ISupportInitialize)employeesGrid).EndInit();
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView employeesGrid;
        private ToolStrip toolStrip;
        private ToolStripButton addButton;
        private ToolStripButton saveButton;
        private ToolStripButton deleteButton;
        private ToolStripButton reloadButton;
        private ToolStripLabel statusLabel;
        private DataGridViewTextBoxColumn idColumn;
        private DataGridViewTextBoxColumn firstNameColumn;
        private DataGridViewTextBoxColumn lastNameColumn;
        private DataGridViewTextBoxColumn emailColumn;
        private DateOnlyPickerColumn hireDateColumn;
        private DataGridViewTextBoxColumn salaryColumn;
        private DataGridViewComboBoxColumn departmentColumn;
        private DataGridViewComboBoxColumn positionColumn;
    }
}
