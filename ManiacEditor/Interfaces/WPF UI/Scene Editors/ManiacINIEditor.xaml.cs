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
using System.Windows.Shapes;

namespace ManiacEditor
{
    /// <summary>
    /// Interaction logic for ModPackEditor.xaml
    /// </summary>
    public partial class ManiacINIEditor : Window
    {
        private Editor Instance;
        private bool updatingKeys { get; set; } = false;
        private bool updatingValues { get; set; } = false;
        private List<Tuple<string, List<Tuple<string, string>>>> ManiacINISettingsUnedited;
        public ManiacINIEditor(Editor instance)
        {
            InitializeComponent();
            Instance = instance;
            if (Instance.EditorManiacINI.ManiacINISettings != null) ManiacINISettingsUnedited = Instance.EditorManiacINI.ManiacINISettings;
            RefreshKeyList();

            UpdateValueModButtons();
            UpdateKeyModButtons();
        }

        private void RefreshKeyList()
        {
            updatingKeys = true;
            if (ValueList.Items != null) ValueList.Items.Clear();
            if (KeyList.Items != null) KeyList.Items.Clear();
            foreach (var item in Instance.EditorManiacINI.ManiacINISettings)
            {
                KeyList.Items.Add(item.Item1);
            }
            updatingKeys = false;
        }

        private bool KeyIndexValid()
        {
            if (KeyList.SelectedItem == null) return false;
            if (Instance.EditorManiacINI.ManiacINISettings.Count > KeyList.SelectedIndex)
            {
                if (Instance.EditorManiacINI.ManiacINISettings[KeyList.SelectedIndex] != null)
                {
                    return true;
                }
                else return false;
            }
            else return false;            
        }

        private bool ValueIndexValid()
        {
            if (KeyIndexValid())
            {
                if (ValueList.SelectedItem == null) return false;
                if (Instance.EditorManiacINI.ManiacINISettings[KeyList.SelectedIndex].Item2.Count > ValueList.SelectedIndex)
                {
                    if (Instance.EditorManiacINI.ManiacINISettings[KeyList.SelectedIndex].Item2[ValueList.SelectedIndex] != null)
                    {
                        return true;
                    }
                    else return false;
                }
                else return false;
            }
            else return false;
        }

        private void RefreshValueList()
        {
            updatingValues = true;
            if (ValueList.Items != null) ValueList.Items.Clear();
            if (KeyIndexValid() == false) return;

            foreach (var item in Instance.EditorManiacINI.ManiacINISettings[KeyList.SelectedIndex].Item2)
            {
                string entry = item.Item1 + "=" + item.Item2;
                ValueList.Items.Add(entry);
            }
            updatingValues = false;
        }

        private void KeyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!updatingKeys)
            {
                RefreshValueList();
                RefreshKeyValues();
            }
            else
            {
                ClearValueList();
                ClearKeyValues();
            }

            UpdateValueModButtons();
            UpdateKeyModButtons();
        }

        public void UpdateKeyModButtons()
        {
            MoveUpKeyButton.IsEnabled = (KeyList.SelectedIndex != -1 && KeyList.SelectedItem != null && !(KeyList.SelectedIndex - 1 < 0));
            MoveDownKeyButton.IsEnabled = (KeyList.SelectedIndex != -1 && KeyList.SelectedItem != null && !(KeyList.SelectedIndex + 1 > KeyList.Items.Count - 1));
            //AddKeyButton.IsEnabled = (KeyList.SelectedIndex != -1 && KeyList.SelectedItem != null);
            RemoveKeyButton.IsEnabled = (KeyList.SelectedIndex != -1 && KeyList.SelectedItem != null);
        }

        public void UpdateValueModButtons()
        {
            MoveUpValueButton.IsEnabled = (ValueList.SelectedIndex != -1 && ValueList.SelectedItem != null && !(ValueList.SelectedIndex - 1 < 0));
            MoveDownValueButton.IsEnabled = (ValueList.SelectedIndex != -1 && ValueList.SelectedItem != null && !(ValueList.SelectedIndex + 1 > ValueList.Items.Count - 1));
            AddValueButton.IsEnabled = (ValueList.Items != null && ValueList.Items.Count >= 0);
            RemoveValueButton.IsEnabled = (ValueList.SelectedIndex != -1 && ValueList.SelectedItem != null);
        }

        private void RefreshKeyValues()
        {
            if (KeyIndexValid() == false) return;
            KeyNameTextBox.Text = Instance.EditorManiacINI.ManiacINISettings[KeyList.SelectedIndex].Item1;
        }

        private void RefreshValueValues()
        {
            if (ValueIndexValid() == false) return;
            ValueNameTextBox.Text = Instance.EditorManiacINI.ManiacINISettings[KeyList.SelectedIndex].Item2[ValueList.SelectedIndex].Item1;
            ValueTextBox.Text = Instance.EditorManiacINI.ManiacINISettings[KeyList.SelectedIndex].Item2[ValueList.SelectedIndex].Item2;
        }

        private void ClearKeyValues()
        {
            KeyNameTextBox.Text = "";
        }

        private void ClearValueValues()
        {
            ValueNameTextBox.Text = "";
            ValueTextBox.Text = "";
        }

        private void ClearValueList()
        {
            ValueList.Items.Clear();
        }

        private void AddKeyButton_Click(object sender, RoutedEventArgs e)
        {
            Instance.EditorManiacINI.ManiacINISettings.Add(new Tuple<string, List<Tuple<string, string>>>("New Entry", new List<Tuple<string, string>>()));
            RefreshKeyList();
        }

        private void RemoveKeyButton_Click(object sender, RoutedEventArgs e)
        {
            if (KeyIndexValid() == false) return;
            MessageBoxResult result = RSDKrU.MessageBox.Show("Are you sure you want to delete this entry?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            if (result == MessageBoxResult.Yes)
            {
                Instance.EditorManiacINI.ManiacINISettings.RemoveAt(KeyList.SelectedIndex);
                RefreshKeyList();
            }
        }

        private void MoveUpKeyButton_Click(object sender, RoutedEventArgs e)
        {
            if (KeyIndexValid() == false) return;
            if (KeyList.SelectedIndex - 1 < 0) return;
            MoveKey(KeyList.SelectedIndex, KeyList.SelectedIndex - 1);
            RefreshKeyList();
        }

        private void MoveDownKeyButton_Click(object sender, RoutedEventArgs e)
        {
            if (KeyIndexValid() == false) return;
            if (KeyList.SelectedIndex + 1 > Instance.EditorManiacINI.ManiacINISettings.Count) return;
            MoveKey(KeyList.SelectedIndex, KeyList.SelectedIndex + 1);
            RefreshKeyList();
        }


        public void MoveKey(int oldIndex, int newIndex)
        {
            Tuple<string, List<Tuple<string, string>>> item = Instance.EditorManiacINI.ManiacINISettings[oldIndex];
            Instance.EditorManiacINI.ManiacINISettings.RemoveAt(oldIndex);
            Instance.EditorManiacINI.ManiacINISettings.Insert(newIndex, item);
        }

        public void MoveValue(int oldIndex, int newIndex)
        {
            Tuple<string, string> item = Instance.EditorManiacINI.ManiacINISettings[KeyList.SelectedIndex].Item2[oldIndex];
            Instance.EditorManiacINI.ManiacINISettings[KeyList.SelectedIndex].Item2.RemoveAt(oldIndex);
            Instance.EditorManiacINI.ManiacINISettings[KeyList.SelectedIndex].Item2.Insert(newIndex, item);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = RSDKrU.MessageBox.Show("Are you sure you want to save?", "Confirm Save", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            if (result == MessageBoxResult.Yes)
            {
                Instance.EditorManiacINI.SaveFile();
            }

        }

        private void ChangeNameButton_Click(object sender, RoutedEventArgs e)
        {
            if (KeyIndexValid() == false) return;
            var itemToEdit = Instance.EditorManiacINI.ManiacINISettings[KeyList.SelectedIndex];
            Instance.EditorManiacINI.ManiacINISettings[KeyList.SelectedIndex] = new Tuple<string, List<Tuple<string, string>>>(KeyNameTextBox.Text, itemToEdit.Item2);
            RefreshKeyList();
        }

        private void ChangeValueButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeValueButton_Click(sender, e, true);
        }

        private void ChangeValueButton_Click(object sender, RoutedEventArgs e, bool refreshList = true)
        {
            if (ValueIndexValid() == false) return;
            var itemToEdit = Instance.EditorManiacINI.ManiacINISettings[KeyList.SelectedIndex];
            var subItemtoEdit = itemToEdit.Item2;
            var valueItemToEdit = subItemtoEdit[ValueList.SelectedIndex];
            var valueItemEdited = new Tuple<string, string>(valueItemToEdit.Item1, ValueTextBox.Text);
            subItemtoEdit[ValueList.SelectedIndex] = valueItemEdited;
            Instance.EditorManiacINI.ManiacINISettings[KeyList.SelectedIndex] = new Tuple<string, List<Tuple<string, string>>>(itemToEdit.Item1, subItemtoEdit);
            if (refreshList) RefreshValueList();
        }

        private void ChangeValueNameButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeValueNameButton_Click(sender, e, true);
        }

        private void ChangeValueNameButton_Click(object sender, RoutedEventArgs e, bool refreshList = true)
        {
            if (ValueIndexValid() == false) return;
            var itemToEdit = Instance.EditorManiacINI.ManiacINISettings[KeyList.SelectedIndex];
            var subItemtoEdit = itemToEdit.Item2;
            var valueItemToEdit = subItemtoEdit[ValueList.SelectedIndex];
            var valueItemEdited = new Tuple<string, string>(ValueNameTextBox.Text, valueItemToEdit.Item2);
            subItemtoEdit[ValueList.SelectedIndex] = valueItemEdited;
            Instance.EditorManiacINI.ManiacINISettings[KeyList.SelectedIndex] = new Tuple<string, List<Tuple<string, string>>>(itemToEdit.Item1, subItemtoEdit);
            if (refreshList) RefreshValueList();
        }

        private void ValueList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!updatingValues)
            {
                RefreshValueValues();
            }
            else
            {
                ClearValueValues();
            }

            UpdateValueModButtons();
        }


        private void AddValueButton_Click(object sender, RoutedEventArgs e)
        {
            //if (ValueIndexValid() == false) return;
            if (Instance.EditorManiacINI.ManiacINISettings[KeyList.SelectedIndex].Item2 == null) Instance.EditorManiacINI.ManiacINISettings[KeyList.SelectedIndex] = new Tuple<string, List<Tuple<string, string>>>(Instance.EditorManiacINI.ManiacINISettings[KeyList.SelectedIndex].Item1, new List<Tuple<string, string>>());
            Instance.EditorManiacINI.ManiacINISettings[KeyList.SelectedIndex].Item2.Add(new Tuple<string, string>("Mod", "n/a"));
            RefreshValueList();
        }

        private void RemoveValueButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValueIndexValid() == false) return;
            MessageBoxResult result = RSDKrU.MessageBox.Show("Are you sure you want to delete this entry?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            if (result == MessageBoxResult.Yes)
            {
                Instance.EditorManiacINI.ManiacINISettings[KeyList.SelectedIndex].Item2.RemoveAt(ValueList.SelectedIndex);
                RefreshValueList();
            }
        }

        private void MoveUpValueButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValueIndexValid() == false) return;
            if (ValueList.SelectedIndex - 1 < 0) return;
            MoveValue(ValueList.SelectedIndex, KeyList.SelectedIndex - 1);
            RefreshValueList();
        }

        private void MoveDownValueButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValueIndexValid() == false) return;
            if (ValueList.SelectedIndex + 1 > Instance.EditorManiacINI.ManiacINISettings[KeyList.SelectedIndex].Item2.Count) return;
            MoveValue(ValueList.SelectedIndex, ValueList.SelectedIndex + 1);
            RefreshValueList();
        }

        private void FindDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            FolderSelectDialog folderSelect = new FolderSelectDialog();   
            if (folderSelect.ShowDialog() == true)
            {
                ValueTextBox.Text = folderSelect.FileName;
            }
        }

        private void ChangeEntireValueButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeValueButton_Click(sender, e, false);
            ChangeValueNameButton_Click(sender, e, false);
            RefreshValueList();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Instance.EditorManiacINI.ManiacINISettings != ManiacINISettingsUnedited)
            {
                MessageBoxResult result = RSDKrU.MessageBox.ShowYesNoCancel("You haven't saved your changes yet! Would you like to save your changes?", "Unsaved Changes", "Save and Exit", "Exit without Saving", "Cancel", MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.Yes)
                {
                    Instance.EditorManiacINI.SaveFile();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
                else if (result == MessageBoxResult.No)
                {
                    Instance.EditorManiacINI.ManiacINISettings = ManiacINISettingsUnedited;
                }
                else
                {
                    e.Cancel = true;
                }
            }

        }

        private void LoadSource_Click(object sender, RoutedEventArgs e)
        {
            if (System.IO.File.Exists(Instance.EditorManiacINI.GetFilePath()))
            {
                System.Diagnostics.Process.Start("explorer.exe", "/select, " + Instance.EditorManiacINI.GetFilePath());
            }
            else
            {
                RSDKrU.MessageBox.Show("File does not exist at " + Instance.EditorManiacINI.GetFilePath(), "ERROR");
            }
        }

        private void HintsButton_Click(object sender, RoutedEventArgs e)
        {
            HintsButton.ContextMenu.IsOpen = true;
        }
    }
}
