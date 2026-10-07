using System.ComponentModel;
using System.Globalization;

namespace EmployeeManagement.UI.Forms
{
    public sealed class DateOnlyPickerColumn : DataGridViewColumn
    {
        public DateOnlyPickerColumn() : base(new DateOnlyPickerCell())
        {
        }

        public override DataGridViewCell? CellTemplate
        {
            get => base.CellTemplate;
            set
            {
                if (value is not null and not DateOnlyPickerCell)
                {
                    throw new InvalidCastException("Cell template must be a DateOnlyPickerCell.");
                }

                base.CellTemplate = value;
            }
        }
    }

    public sealed class DateOnlyPickerCell : DataGridViewTextBoxCell
    {
        public override Type EditType => typeof(DateOnlyPickerEditingControl);

        public override Type ValueType => typeof(DateOnly);

        public override Type FormattedValueType => typeof(string);

        public override object DefaultNewRowValue => DateOnly.FromDateTime(DateTime.Today);

        public override void InitializeEditingControl(int rowIndex, object? initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            if (DataGridView?.EditingControl is DateOnlyPickerEditingControl control)
            {
                control.Value = Value is DateOnly date
                    ? date.ToDateTime(TimeOnly.MinValue)
                    : DateTime.Today;
            }
        }

        public override object? ParseFormattedValue(object? formattedValue, DataGridViewCellStyle cellStyle, TypeConverter? formattedValueTypeConverter, TypeConverter? valueTypeConverter)
        {
            return formattedValue is string text && DateOnly.TryParse(text, CultureInfo.CurrentCulture, out var date)
                ? date
                : throw new FormatException("Invalid date.");
        }
    }

    public sealed class DateOnlyPickerEditingControl : DateTimePicker, IDataGridViewEditingControl
    {
        private bool _valueChanged;

        public DateOnlyPickerEditingControl()
        {
            Format = DateTimePickerFormat.Short;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataGridView? EditingControlDataGridView { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object EditingControlFormattedValue
        {
            get => DateOnly.FromDateTime(Value).ToString("d", CultureInfo.CurrentCulture);
            set
            {
                if (value is string text && DateOnly.TryParse(text, CultureInfo.CurrentCulture, out var date))
                {
                    Value = date.ToDateTime(TimeOnly.MinValue);
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int EditingControlRowIndex { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EditingControlValueChanged
        {
            get => _valueChanged;
            set => _valueChanged = value;
        }

        public Cursor EditingPanelCursor => base.Cursor;

        public bool RepositionEditingControlOnValueChange => false;

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            Font = dataGridViewCellStyle.Font;
            CalendarForeColor = dataGridViewCellStyle.ForeColor;
        }

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            return keyData is Keys.Left or Keys.Right or Keys.Up or Keys.Down
                or Keys.Home or Keys.End or Keys.PageDown or Keys.PageUp;
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context) => EditingControlFormattedValue;

        public void PrepareEditingControlForEdit(bool selectAll)
        {
        }

        protected override void OnValueChanged(EventArgs eventargs)
        {
            _valueChanged = true;
            EditingControlDataGridView?.NotifyCurrentCellDirty(true);
            base.OnValueChanged(eventargs);
        }
    }
}
