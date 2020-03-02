using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ManiacEditor.Controls.Global.Controls.PropertyGrid;

namespace ManiacEditor.Controls.Global.Controls.PropertyGrid
{
    /// <summary>
    /// Interaction logic for PropertyControl.xaml
    /// </summary>
    public partial class PropertyControl : UserControl
    {
        #region Definitions

        public event EventHandler<PropertyChangedEventArgs> PropertyValueChanged;
        public PropertyGridObject SelectedObject
        {
            get
            {
                return _SelectedObject;
            }
            set
            {
                if (_SelectedObject != null) _SelectedObject.PropertyValueChanged -= Value_PropertyValueChanged;
                _SelectedObject = value;
                if (_SelectedObject != null) _SelectedObject.PropertyValueChanged += Value_PropertyValueChanged;
                Update(_SelectedObject);
            }
        }
        public PropertyGridObject _SelectedObject { get; set; }


        #endregion

        #region Classes
        public class PropertyGridObject
        {
            public event EventHandler<PropertyChangedEventArgs> PropertyValueChanged;
            public List<PropertyCategory> Categories { get; set; } = new List<PropertyCategory>();
            public List<PropertyEntryControlBase> PropertyList { get; set; } = new List<PropertyEntryControlBase>();

            public void Dispose()
            {
                Categories.Clear();
            }

            public void Clear()
            {
                foreach (var entry in PropertyList)
                {
                    if (entry is PropertyEntryControlAttribute)
                    {
                        entry.PropertyValueChanged -= Controller_PropertyValueChanged;
                    }
                    entry.Dispose();
                }
                PropertyList.Clear();
            }

            public void AddProperty(string CategoryName, string ValueName, string ValueTypeName, Type ValueType, object Value)
            {
                if (!Categories.Exists(x => x.Name == CategoryName))
                {
                    Categories.Add(new PropertyCategory(CategoryName));
                    PropertyList.Add(new PropertyEntryControlHeader(CategoryName));
                }

                var NewObject = new PropertyObject(ValueName, CategoryName, ValueTypeName, ValueType, Value);

                Categories.Where(x => x.Name == CategoryName).FirstOrDefault().Values.Add(NewObject);


                PropertyEntryControlAttribute ValueController = new PropertyEntryControlAttribute(NewObject);
                ValueController.PropertyValueChanged += Controller_PropertyValueChanged;
                PropertyList.Add(ValueController);
            }

            private void Controller_PropertyValueChanged(object sender, PropertyChangedEventArgs e)
            {
                PropertyValueChanged?.Invoke(sender, e);
            }

            public void UpdateProperty(string CategoryName, string ValueName, object Value)
            {
                if (Categories.Exists(x => x.Name == CategoryName))
                {
                    if (Categories.Where(x => x.Name == CategoryName).FirstOrDefault().Values.Exists(x => x.ValueName == ValueName))
                    {
                        Categories.Where(x => x.Name == CategoryName).FirstOrDefault().Values.Where(x => x.ValueName == ValueName).FirstOrDefault().Value = Value;
                        (PropertyList.Where(x => x is PropertyEntryControlAttribute && (x as PropertyEntryControlAttribute).ValueName == ValueName && (x as PropertyEntryControlAttribute).CategoryName == CategoryName).FirstOrDefault() as PropertyEntryControlAttribute).UpdateValue(Value);
                    }
                }
            }

            public class PropertyCategory
            {
                public PropertyCategory(string name)
                {
                    Name = name;
                }
                public string Name { get; set; }
                public List<PropertyObject> Values { get; set; } = new List<PropertyObject>();
            }

            public class PropertyObject
            {
                public PropertyObject(string name, string category, string typeName, Type type, object value)
                {
                    ValueName = name;
                    TypeName = typeName;
                    Category = category;
                    Type = type;
                    Value = value;
                }
                public object Value { get; set; }
                public string TypeName { get; set; }
                public string Category { get; set; }
                public Type Type { get; set; }
                public string ValueName { get; set; }

            }
        }
        public class PropertyChangedEventArgs : EventArgs
        {
            public PropertyChangedEventArgs(string category, string name, object oldValue, object newValue)
            {
                Category = category;
                Name = name;
                NewValue = newValue;
                OldValue = oldValue;
            }

            public object NewValue { get; }
            public object OldValue { get; }
            public string Name { get; }
            public string Category { get; }
            public string Property
            {
                get
                {
                    return string.Format("{0},{1}", Category, Name);
                }
            }
        }

        #endregion

        #region Init
        public PropertyControl()
        {
            InitializeComponent();
        }
        #endregion

        #region Properties Pane

        private void UpdatePropertiesPaneEntries(PropertyGridObject Obj)
        {
            this.PropertyPane.ItemsSource = Obj.PropertyList;
        }

        private void Value_PropertyValueChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyValueChanged?.Invoke(sender, e);
        }

        private void CleanPropertiesPaneEntries(PropertyGridObject Obj = null)
        {
            if (Obj != null) Obj.Clear();
            else if (_SelectedObject != null) _SelectedObject.Clear();
            this.PropertyPane.ItemsSource = null;
        }

        #endregion

        #region UI
        public void Refresh()
        {
            if (_SelectedObject != null) UpdatePropertiesPaneEntries(_SelectedObject);
            else CleanPropertiesPaneEntries();
        }

        public void Update(PropertyGridObject Obj = null)
        {
            if (Obj != null) UpdatePropertiesPaneEntries(Obj);
            else if (_SelectedObject != null) UpdatePropertiesPaneEntries(_SelectedObject);
            else CleanPropertiesPaneEntries();
        }

        public void Dispose()
        {
            CleanPropertiesPaneEntries();
        }

        #endregion
    }
}
