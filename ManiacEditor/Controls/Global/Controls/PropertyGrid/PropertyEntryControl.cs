using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Integration;

namespace ManiacEditor.Controls.Global.Controls.PropertyGrid
{

    public class PropertyEntryControlAttribute : PropertyEntryControlBase
    {
        #region Attribute Controls
        private Xceed.Wpf.Toolkit.ByteUpDown ByteNumericControl { get; set; }
        private Xceed.Wpf.Toolkit.UShortUpDown UShortNumericControl { get; set; }
        private Xceed.Wpf.Toolkit.UIntegerUpDown UIntNumericControl { get; set; }
        private Xceed.Wpf.Toolkit.SByteUpDown SByteNumericControl { get; set; }
        private Xceed.Wpf.Toolkit.ShortUpDown ShortNumericControl { get; set; }
        private Xceed.Wpf.Toolkit.IntegerUpDown IntNumericControl { get; set; }
        private Xceed.Wpf.Toolkit.SingleUpDown FloatNumericControl { get; set; }
        private System.Windows.Controls.CheckBox BoolCheckboxControl { get; set; }
        private System.Windows.Controls.TextBox StringTextboxControl { get; set; }
        private string TextBoxLastString { get; set; }
        private object LastValue { get; set; }
        private object LastValueChanged { get; set; }
        private Xceed.Wpf.Toolkit.ColorPicker ColorPickerControl { get; set; }
        private System.Windows.Media.Color? ColorPickerLastColor { get; set; }
        #endregion

        #region Definitions

        private Type HostType { get; set; }

        public PropertyEntryValue ValueControl { get; set; }

        public string ValueName
        {
            get { return PropertyObject.ValueName; }
        }
        public string CategoryName
        {
            get { return PropertyObject.Category; }
        }
        private ManiacEditor.Controls.Global.Controls.PropertyGrid.PropertyControl.PropertyGridObject.PropertyObject PropertyObject { get; set; }

        #endregion

        #region Init

        public PropertyEntryControlAttribute(ManiacEditor.Controls.Global.Controls.PropertyGrid.PropertyControl.PropertyGridObject.PropertyObject Property) : base()
        {
            PropertyObject = Property;
            ValueControl = new PropertyEntryValue();
            ValueControl.Value1Label.Text = Property.ValueName;
            GenerateHostControl(Property.Value, Property.Type);
            base.MouseEnter += PropertyEntryControlAttribute_MouseEnter;
            base.Children.Add(ValueControl);
        }

        #endregion

        #region Event Generation
        private void GenerateHostControl(object DefaultValue, Type CurrentType)
        {
            if (DefaultValue != null) HostType = DefaultValue.GetType();
            else HostType = CurrentType;

            LastValue = DefaultValue;

            if (HostType == typeof(byte))
            {
                ByteNumericControl = new Xceed.Wpf.Toolkit.ByteUpDown();
                ByteNumericControl.UpdateValueOnEnterKey = true;
                ByteNumericControl.Value = (byte)DefaultValue;
                ValueControl.Value1Host.Children.Add(ByteNumericControl);
            }
            else if (HostType == typeof(ushort))
            {
                UShortNumericControl = new Xceed.Wpf.Toolkit.UShortUpDown();
                UShortNumericControl.UpdateValueOnEnterKey = true;
                UShortNumericControl.Value = (ushort)DefaultValue;
                ValueControl.Value1Host.Children.Add(UShortNumericControl);
            }
            else if (HostType == typeof(uint))
            {
                UIntNumericControl = new Xceed.Wpf.Toolkit.UIntegerUpDown();
                UIntNumericControl.UpdateValueOnEnterKey = true;
                UIntNumericControl.Value = (uint)DefaultValue;
                ValueControl.Value1Host.Children.Add(UIntNumericControl);
            }
            else if (HostType == typeof(sbyte))
            {
                SByteNumericControl = new Xceed.Wpf.Toolkit.SByteUpDown();
                SByteNumericControl.UpdateValueOnEnterKey = true;
                SByteNumericControl.Value = (sbyte)DefaultValue;
                ValueControl.Value1Host.Children.Add(SByteNumericControl);
            }
            else if (HostType == typeof(short))
            {
                ShortNumericControl = new Xceed.Wpf.Toolkit.ShortUpDown();
                ShortNumericControl.UpdateValueOnEnterKey = true;
                ShortNumericControl.Value = (short)DefaultValue;
                ValueControl.Value1Host.Children.Add(ShortNumericControl);
            }
            else if (HostType == typeof(int))
            {
                IntNumericControl = new Xceed.Wpf.Toolkit.IntegerUpDown();
                IntNumericControl.UpdateValueOnEnterKey = true;
                IntNumericControl.Value = (int)DefaultValue;
                ValueControl.Value1Host.Children.Add(IntNumericControl);
            }
            else if (HostType == typeof(bool))
            {
                BoolCheckboxControl = new System.Windows.Controls.CheckBox();
                BoolCheckboxControl.IsChecked = (bool)DefaultValue;
                BoolCheckboxControl.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                BoolCheckboxControl.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                BoolCheckboxControl.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                BoolCheckboxControl.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                ValueControl.Value1Host.Children.Add(BoolCheckboxControl);
            }
            else if (HostType == typeof(string))
            {
                StringTextboxControl = new System.Windows.Controls.TextBox() { IsInactiveSelectionHighlightEnabled = true };
                StringTextboxControl.Text = (string)DefaultValue;
                TextBoxLastString = (string)DefaultValue;
                ValueControl.Value1Host.Children.Add(StringTextboxControl);
            }
            else if (HostType == typeof(float))
            {
                FloatNumericControl = new Xceed.Wpf.Toolkit.SingleUpDown();
                FloatNumericControl.UpdateValueOnEnterKey = true;
                FloatNumericControl.Value = (float)DefaultValue;
                ValueControl.Value1Host.Children.Add(FloatNumericControl);
            }
            else if (HostType == typeof(System.Drawing.Color))
            {
                ColorPickerControl = new Xceed.Wpf.Toolkit.ColorPicker();
                System.Drawing.Color initalColor = (System.Drawing.Color)DefaultValue;
                ColorPickerLastColor = new System.Windows.Media.Color() { A = initalColor.A, R = initalColor.R, G = initalColor.G, B = initalColor.B };
                ColorPickerControl.SelectedColor = ColorPickerLastColor;
                ValueControl.Value1Host.Children.Add(ColorPickerControl);
            }

            EnableEvents();
        }
        public void EnableEvents()
        {
            if (HostType == typeof(byte))
            {
                ByteNumericControl.ValueChanged += Host_ValueChanged;
                ByteNumericControl.KeyDown += Numeric_KeyDown;
                ByteNumericControl.LostFocus += Numeric_LostFocus;
            }
            else if (HostType == typeof(ushort))
            {
                UShortNumericControl.ValueChanged += Host_ValueChanged;
                UShortNumericControl.KeyDown += Numeric_KeyDown;
                UShortNumericControl.LostFocus += Numeric_LostFocus;
            }
            else if (HostType == typeof(uint))
            {
                UIntNumericControl.ValueChanged += Host_ValueChanged;
                UIntNumericControl.KeyDown += Numeric_KeyDown;
                UIntNumericControl.LostFocus += Numeric_LostFocus;
            }
            else if (HostType == typeof(sbyte))
            {
                SByteNumericControl.ValueChanged += Host_ValueChanged;
                SByteNumericControl.KeyDown += Numeric_KeyDown;
                SByteNumericControl.LostFocus += Numeric_LostFocus;
            }
            else if (HostType == typeof(short))
            {
                ShortNumericControl.ValueChanged += Host_ValueChanged;
                ShortNumericControl.KeyDown += Numeric_KeyDown;
                ShortNumericControl.LostFocus += Numeric_LostFocus;
            }
            else if (HostType == typeof(int))
            {
                IntNumericControl.ValueChanged += Host_ValueChanged;
                IntNumericControl.KeyDown += Numeric_KeyDown;
                IntNumericControl.LostFocus += Numeric_LostFocus;
            }
            else if (HostType == typeof(bool))
            {
                BoolCheckboxControl.Checked += Host_Checked;
                BoolCheckboxControl.Unchecked += Host_Unchecked;
            }
            else if (HostType == typeof(string))
            {
                StringTextboxControl.PreviewKeyDown += StringTextboxControl_KeyDown;
                StringTextboxControl.TextChanged += StringTextboxControl_TextChanged;
                StringTextboxControl.GotFocus += StringTextboxControl_GotFocus;
                StringTextboxControl.LostFocus += StringTextboxControl_LostFocus;
            }
            else if (HostType == typeof(float))
            {
                FloatNumericControl.ValueChanged += Host_ValueChanged;
                FloatNumericControl.KeyDown += Numeric_KeyDown;
                FloatNumericControl.LostFocus += Numeric_LostFocus;
            }
            else if (HostType == typeof(System.Drawing.Color))
            {
                ColorPickerControl.Closed += ColorPickerControl_Closed;
            }
        }

        public void DisableEvents()
        {
            if (HostType == typeof(byte))
            {
                ByteNumericControl.ValueChanged -= Host_ValueChanged;
                ByteNumericControl.KeyDown -= Numeric_KeyDown;
                ByteNumericControl.LostFocus -= Numeric_LostFocus;
            }
            else if (HostType == typeof(ushort))
            {
                UShortNumericControl.ValueChanged -= Host_ValueChanged;
                UShortNumericControl.KeyDown -= Numeric_KeyDown;
                UShortNumericControl.LostFocus -= Numeric_LostFocus;
            }
            else if (HostType == typeof(uint))
            {
                UIntNumericControl.ValueChanged -= Host_ValueChanged;
                UIntNumericControl.KeyDown -= Numeric_KeyDown;
                UIntNumericControl.LostFocus -= Numeric_LostFocus;
            }
            else if (HostType == typeof(sbyte))
            {
                SByteNumericControl.ValueChanged -= Host_ValueChanged;
                SByteNumericControl.KeyDown -= Numeric_KeyDown;
                SByteNumericControl.LostFocus -= Numeric_LostFocus;
            }
            else if (HostType == typeof(short))
            {
                ShortNumericControl.ValueChanged -= Host_ValueChanged;
                ShortNumericControl.KeyDown -= Numeric_KeyDown;
                ShortNumericControl.LostFocus -= Numeric_LostFocus;
            }
            else if (HostType == typeof(int))
            {
                IntNumericControl.ValueChanged -= Host_ValueChanged;
                IntNumericControl.KeyDown -= Numeric_KeyDown;
                IntNumericControl.LostFocus -= Numeric_LostFocus;
            }
            else if (HostType == typeof(bool))
            {
                BoolCheckboxControl.Checked -= Host_Checked;
                BoolCheckboxControl.Unchecked -= Host_Unchecked;
            }
            else if (HostType == typeof(string))
            {
                StringTextboxControl.PreviewKeyDown -= StringTextboxControl_KeyDown;
                StringTextboxControl.TextChanged -= StringTextboxControl_TextChanged;
                StringTextboxControl.GotFocus -= StringTextboxControl_GotFocus;
                StringTextboxControl.LostFocus -= StringTextboxControl_LostFocus;
            }
            else if (HostType == typeof(float))
            {
                FloatNumericControl.ValueChanged -= Host_ValueChanged;
                FloatNumericControl.KeyDown -= Numeric_KeyDown;
                FloatNumericControl.LostFocus -= Numeric_LostFocus;
            }
            else if (HostType == typeof(System.Drawing.Color))
            {
                ColorPickerControl.Closed -= ColorPickerControl_Closed;
            }
        }

        #endregion

        #region Events
        private void PropertyEntryControlAttribute_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (HostType == typeof(string))
            {
                //StringTextboxControl.Focus();
            }
        }
        private void ColorPickerControl_Closed(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Media.Color? ColorPickerNewColor = ColorPickerControl.SelectedColor;
            System.Drawing.Color OldColor = (ColorPickerLastColor.HasValue ? System.Drawing.Color.FromArgb(ColorPickerLastColor.Value.A, ColorPickerLastColor.Value.R, ColorPickerLastColor.Value.G, ColorPickerLastColor.Value.B) : System.Drawing.Color.Black);
            System.Drawing.Color NewColor = (ColorPickerNewColor.HasValue ? System.Drawing.Color.FromArgb(ColorPickerNewColor.Value.A, ColorPickerNewColor.Value.R, ColorPickerNewColor.Value.G, ColorPickerNewColor.Value.B) : System.Drawing.Color.Black);

            base.PropertyValueChanged_Invoke(sender, new PropertyControl.PropertyChangedEventArgs(PropertyObject.Category, PropertyObject.ValueName, OldColor, NewColor));
        }
        private void StringTextboxControl_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                base.PropertyValueChanged_Invoke(sender, new PropertyControl.PropertyChangedEventArgs(PropertyObject.Category, PropertyObject.ValueName, TextBoxLastString, StringTextboxControl.Text));
                TextBoxLastString = StringTextboxControl.Text;
            }
            else e.Handled = false;
        }
        private void StringTextboxControl_LostFocus(object sender, EventArgs e)
        {
            base.PropertyValueChanged_Invoke(sender, new PropertyControl.PropertyChangedEventArgs(PropertyObject.Category, PropertyObject.ValueName, TextBoxLastString, StringTextboxControl.Text));
            TextBoxLastString = StringTextboxControl.Text;
        }
        private void StringTextboxControl_GotFocus(object sender, EventArgs e)
        {

        }
        private void StringTextboxControl_TextChanged(object sender, EventArgs e)
        {

        }
        private void Host_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            base.PropertyValueChanged_Invoke(sender, new PropertyControl.PropertyChangedEventArgs(PropertyObject.Category, PropertyObject.ValueName, false, true));

        }
        private void Host_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            base.PropertyValueChanged_Invoke(sender, new PropertyControl.PropertyChangedEventArgs(PropertyObject.Category, PropertyObject.ValueName, true, false));

        }
        private void Host_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            base.PropertyValueChanged_Invoke(sender, new PropertyControl.PropertyChangedEventArgs(PropertyObject.Category, PropertyObject.ValueName, e.OldValue, e.NewValue));
        }

        private void Numeric_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            /*
            base.PropertyValueChanged_Invoke(sender, new PropertyControl.PropertyChangedEventArgs(PropertyObject.Category, PropertyObject.ValueName, LastValue, LastValueChanged));
            LastValue = LastValueChanged;
            */
        }

        private void Numeric_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            /*
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                base.PropertyValueChanged_Invoke(sender, new PropertyControl.PropertyChangedEventArgs(PropertyObject.Category, PropertyObject.ValueName, LastValue, LastValueChanged));
                LastValue = LastValueChanged;
            }
            else e.Handled = false;
            */
        }

        #endregion

        #region Other 

        public void UpdateValue(object CurrentValue)
        {
            DisableEvents();

            if (HostType == typeof(byte))
            {
                ByteNumericControl.Value = (byte)CurrentValue;
            }
            else if (HostType == typeof(ushort))
            {
                UShortNumericControl.Value = (ushort)CurrentValue;
            }
            else if (HostType == typeof(uint))
            {
                UIntNumericControl.Value = (uint)CurrentValue;
            }
            else if (HostType == typeof(sbyte))
            {
                SByteNumericControl.Value = (sbyte)CurrentValue;
            }
            else if (HostType == typeof(short))
            {
                ShortNumericControl.Value = (short)CurrentValue;
            }
            else if (HostType == typeof(int))
            {
                IntNumericControl.Value = (int)CurrentValue;
            }
            else if (HostType == typeof(bool))
            {
                BoolCheckboxControl.IsChecked = (bool)CurrentValue;
            }
            else if (HostType == typeof(string))
            {
                StringTextboxControl.Text = (string)CurrentValue;
            }
            else if (HostType == typeof(float))
            {
                FloatNumericControl.Value = (float)CurrentValue;
            }
            else if (HostType == typeof(System.Drawing.Color))
            {
                System.Drawing.Color initalColor = (System.Drawing.Color)CurrentValue;
                ColorPickerControl.SelectedColor = new System.Windows.Media.Color() { A = initalColor.A, R = initalColor.R, G = initalColor.G, B = initalColor.B };
            }

            EnableEvents();
        }
        public override void Dispose()
        {
            DisableEvents();

            if (HostType == typeof(byte))
            {
                ValueControl.Value1Host.Children.Remove(ByteNumericControl);
                ByteNumericControl = null;
            }
            else if (HostType == typeof(ushort))
            {
                ValueControl.Value1Host.Children.Remove(UShortNumericControl);
                UShortNumericControl = null;
            }
            else if (HostType == typeof(uint))
            {
                ValueControl.Value1Host.Children.Remove(UIntNumericControl);
                UIntNumericControl = null;
            }
            else if (HostType == typeof(sbyte))
            {
                ValueControl.Value1Host.Children.Remove(SByteNumericControl);
                SByteNumericControl = null;
            }
            else if (HostType == typeof(short))
            {
                ValueControl.Value1Host.Children.Remove(ShortNumericControl);
                ShortNumericControl = null;
            }
            else if (HostType == typeof(int))
            {
                ValueControl.Value1Host.Children.Remove(IntNumericControl);
                IntNumericControl = null;
            }
            else if (HostType == typeof(bool))
            {
                ValueControl.Value1Host.Children.Remove(BoolCheckboxControl);
                BoolCheckboxControl = null;
            }
            else if (HostType == typeof(string))
            {
                ValueControl.Value1Host.Children.Remove(StringTextboxControl);
                StringTextboxControl = null;
            }
            else if (HostType == typeof(float))
            {
                ValueControl.Value1Host.Children.Remove(FloatNumericControl);
                FloatNumericControl = null;
            }
            else if (HostType == typeof(System.Drawing.Color))
            {
                ValueControl.Value1Host.Children.Remove(ColorPickerControl);
                ColorPickerControl = null;
            }
            base.Children.Remove(ValueControl);
            ValueControl = null;
        }

        #endregion
    }
    public class PropertyEntryControlHeader : PropertyEntryControlBase
    {
        private PropertyEntryCategory CategoryControl { get; set; }

        public PropertyEntryControlHeader(string HeaderText) : base()
        {
            CategoryControl = new PropertyEntryCategory();
            CategoryControl.NameLabel.Text = HeaderText;
            base.Children.Add(CategoryControl);
        }
        public override void Dispose()
        {
            base.Children.Remove(CategoryControl);
            CategoryControl = null;
        }

    }
    public class PropertyEntryControlBase : System.Windows.Controls.Grid
    {
        public event EventHandler<ManiacEditor.Controls.Global.Controls.PropertyGrid.PropertyControl.PropertyChangedEventArgs> PropertyValueChanged;
        public PropertyEntryControlBase() : base()
        {
            base.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
        }

        public void PropertyValueChanged_Invoke(object sender, ManiacEditor.Controls.Global.Controls.PropertyGrid.PropertyControl.PropertyChangedEventArgs e)
        {
            PropertyValueChanged?.Invoke(sender, e);
        }

        public virtual void Dispose()
        {

        }
    }
}
