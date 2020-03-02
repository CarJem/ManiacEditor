using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor.Controls.Global.Controls.PropertyGrid
{
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
        private Xceed.Wpf.Toolkit.ColorPicker ColorPickerControl { get; set; }
        #endregion

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

        public PropertyEntryControlAttribute(ManiacEditor.Controls.Global.Controls.PropertyGrid.PropertyControl.PropertyGridObject.PropertyObject Property) : base()
        {
            PropertyObject = Property;
            ValueControl = new PropertyEntryValue();
            ValueControl.Value1Label.Text = Property.ValueName;
            GenerateHostControl(Property.Value);
            base.Children.Add(ValueControl);
        }
        private void GenerateHostControl(object DefaultValue)
        {
            HostType = DefaultValue.GetType();
            if (HostType == typeof(byte))
            {
                ByteNumericControl = new Xceed.Wpf.Toolkit.ByteUpDown();
                ByteNumericControl.Value = (byte)DefaultValue;
                ValueControl.Value1Host.Children.Add(ByteNumericControl);
            }
            else if (HostType == typeof(ushort))
            {
                UShortNumericControl = new Xceed.Wpf.Toolkit.UShortUpDown();
                UShortNumericControl.Value = (ushort)DefaultValue;
                ValueControl.Value1Host.Children.Add(UShortNumericControl);
            }
            else if (HostType == typeof(uint))
            {
                UIntNumericControl = new Xceed.Wpf.Toolkit.UIntegerUpDown();
                UIntNumericControl.Value = (uint)DefaultValue;
                ValueControl.Value1Host.Children.Add(UIntNumericControl);
            }
            else if (HostType == typeof(sbyte))
            {
                SByteNumericControl = new Xceed.Wpf.Toolkit.SByteUpDown();
                SByteNumericControl.Value = (sbyte)DefaultValue;
                ValueControl.Value1Host.Children.Add(SByteNumericControl);
            }
            else if (HostType == typeof(short))
            {
                ShortNumericControl = new Xceed.Wpf.Toolkit.ShortUpDown();
                ShortNumericControl.Value = (short)DefaultValue;
                ValueControl.Value1Host.Children.Add(ShortNumericControl);
            }
            else if (HostType == typeof(int))
            {
                IntNumericControl = new Xceed.Wpf.Toolkit.IntegerUpDown();
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
                StringTextboxControl = new System.Windows.Controls.TextBox();
                StringTextboxControl.Text = (string)DefaultValue;
                ValueControl.Value1Host.Children.Add(StringTextboxControl);
            }
            else if (HostType == typeof(float))
            {
                FloatNumericControl = new Xceed.Wpf.Toolkit.SingleUpDown();
                FloatNumericControl.Value = (float)DefaultValue;
                ValueControl.Value1Host.Children.Add(FloatNumericControl);
            }
            else if (HostType == typeof(System.Drawing.Color)) 
            {
                ColorPickerControl = new Xceed.Wpf.Toolkit.ColorPicker();
                System.Drawing.Color initalColor = (System.Drawing.Color)DefaultValue;
                ColorPickerControl.SelectedColor = new System.Windows.Media.Color() { A = initalColor.A, R = initalColor.R, G = initalColor.G, B = initalColor.B };
                ValueControl.Value1Host.Children.Add(ColorPickerControl);
            }

            EnableEvents();
        }

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

        public void DisableEvents()
        {
            if (HostType == typeof(byte))
            {
                ByteNumericControl.ValueChanged -= Host_ValueChanged;
            }
            else if (HostType == typeof(ushort))
            {
                UShortNumericControl.ValueChanged -= Host_ValueChanged;
            }
            else if (HostType == typeof(uint))
            {
                UIntNumericControl.ValueChanged -= Host_ValueChanged;
            }
            else if (HostType == typeof(sbyte))
            {
                SByteNumericControl.ValueChanged -= Host_ValueChanged;
            }
            else if (HostType == typeof(short))
            {
                ShortNumericControl.ValueChanged -= Host_ValueChanged;
            }
            else if (HostType == typeof(int))
            {
                IntNumericControl.ValueChanged -= Host_ValueChanged;
            }
            else if (HostType == typeof(bool))
            {
                BoolCheckboxControl.Checked -= Host_Checked;
                BoolCheckboxControl.Unchecked -= Host_Unchecked;
            }
            else if (HostType == typeof(string))
            {
                StringTextboxControl.GotFocus -= StringTextboxControl_GotFocus;
                StringTextboxControl.LostFocus -= StringTextboxControl_LostFocus;
            }
            else if (HostType == typeof(float))
            {
                FloatNumericControl.ValueChanged -= Host_ValueChanged;
            }
            else if (HostType == typeof(System.Drawing.Color))
            {
                ColorPickerControl.SelectedColorChanged -= Host_SelectedColorChanged;
            }
        }
        public void EnableEvents()
        {
            if (HostType == typeof(byte))
            {
                ByteNumericControl.ValueChanged += Host_ValueChanged;
            }
            else if (HostType == typeof(ushort))
            {
                UShortNumericControl.ValueChanged += Host_ValueChanged;
            }
            else if (HostType == typeof(uint))
            {
                UIntNumericControl.ValueChanged += Host_ValueChanged;
            }
            else if (HostType == typeof(sbyte))
            {
                SByteNumericControl.ValueChanged += Host_ValueChanged;
            }
            else if (HostType == typeof(short))
            {
                ShortNumericControl.ValueChanged += Host_ValueChanged;
            }
            else if (HostType == typeof(int))
            {
                IntNumericControl.ValueChanged += Host_ValueChanged;
            }
            else if (HostType == typeof(bool))
            {
                BoolCheckboxControl.Checked += Host_Checked;
                BoolCheckboxControl.Unchecked += Host_Unchecked;
            }
            else if (HostType == typeof(string))
            {
                StringTextboxControl.GotFocus += StringTextboxControl_GotFocus;
                StringTextboxControl.LostFocus += StringTextboxControl_LostFocus;
            }
            else if (HostType == typeof(float))
            {
                FloatNumericControl.ValueChanged += Host_ValueChanged;
            }
            else if (HostType == typeof(System.Drawing.Color))
            {
                ColorPickerControl.SelectedColorChanged += Host_SelectedColorChanged;
            }
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

        private void Host_SelectedColorChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {

            System.Drawing.Color OldColor = (e.OldValue.HasValue ? System.Drawing.Color.FromArgb(e.OldValue.Value.A, e.OldValue.Value.R, e.OldValue.Value.G, e.OldValue.Value.B) : System.Drawing.Color.Black);
            System.Drawing.Color NewColor = (e.NewValue.HasValue ? System.Drawing.Color.FromArgb(e.NewValue.Value.A, e.NewValue.Value.R, e.NewValue.Value.G, e.NewValue.Value.B) : System.Drawing.Color.Black);

            base.PropertyValueChanged_Invoke(sender, new PropertyControl.PropertyChangedEventArgs(PropertyObject.Category, PropertyObject.ValueName, OldColor, NewColor));
        }

        private void StringTextboxControl_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            base.PropertyValueChanged_Invoke(sender, new PropertyControl.PropertyChangedEventArgs(PropertyObject.Category, PropertyObject.ValueName, TextBoxLastString, StringTextboxControl.Text));
        }

        private void StringTextboxControl_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            TextBoxLastString = StringTextboxControl.Text;
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
}
