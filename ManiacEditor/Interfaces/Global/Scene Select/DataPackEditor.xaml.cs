using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ManiacEditor
{
    /// <summary>
    /// Interaction logic for ModPackEditor.xaml
    /// </summary>
    public partial class DataPackEditor : Window
    {
        private Interfaces.Base.MapEditor Instance;
        private bool updatingKeys { get; set; } = false;
        private bool updatingValues { get; set; } = false;
        private List<Tuple<string, List<Tuple<string, string>>>> ModListInformationUnedited;
        public DataPackEditor(Interfaces.Base.MapEditor instance)
        {
            InitializeComponent();
            Instance = instance;
            if (Instance.DataPacks.ModListInformation != null) ModListInformationUnedited = Instance.DataPacks.ModListInformation;
            RefreshKeyList();

            UpdateValueModButtons();
            UpdateKeyModButtons();
        }

        private void RefreshKeyList()
        {
            updatingKeys = true;
            if (ValueList.Items != null) ValueList.Items.Clear();
            if (KeyList.Items != null) KeyList.Items.Clear();
            foreach (var item in Instance.DataPacks.ModListInformation)
            {
                KeyList.Items.Add(item.Item1);
            }
            updatingKeys = false;
        }

        private bool KeyIndexValid()
        {
            if (KeyList.SelectedItem == null) return false;
            if (Instance.DataPacks.ModListInformation.Count > KeyList.SelectedIndex)
            {
                if (Instance.DataPacks.ModListInformation[KeyList.SelectedIndex] != null)
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
                if (Instance.DataPacks.ModListInformation[KeyList.SelectedIndex].Item2.Count > ValueList.SelectedIndex)
                {
                    if (Instance.DataPacks.ModListInformation[KeyList.SelectedIndex].Item2[ValueList.SelectedIndex] != null)
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

            foreach (var item in Instance.DataPacks.ModListInformation[KeyList.SelectedIndex].Item2)
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
            AddValueButton.IsEnabled = (ValueList.Items != null && ValueList.Items.Count >= 0 && KeyList.SelectedItem != null);
            RemoveValueButton.IsEnabled = (ValueList.SelectedIndex != -1 && ValueList.SelectedItem != null);
        }

        private void RefreshKeyValues()
        {
            if (KeyIndexValid() == false) return;
            KeyNameTextBox.Text = Instance.DataPacks.ModListInformation[KeyList.SelectedIndex].Item1;
        }

        private void RefreshValueValues()
        {
            if (ValueIndexValid() == false) return;
            ValueNameTextBox.Text = Instance.DataPacks.ModListInformation[KeyList.SelectedIndex].Item2[ValueList.SelectedIndex].Item1;
            ValueTextBox.Text = Instance.DataPacks.ModListInformation[KeyList.SelectedIndex].Item2[ValueList.SelectedIndex].Item2;
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
            Instance.DataPacks.ModListInformation.Add(new Tuple<string, List<Tuple<string, string>>>("New Entry", new List<Tuple<string, string>>()));
            RefreshKeyList();
        }

        private void RemoveKeyButton_Click(object sender, RoutedEventArgs e)
        {
            if (KeyIndexValid() == false) return;
            MessageBoxResult result = System.Windows.MessageBox.Show("Are you sure you want to delete this entry?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            if (result == MessageBoxResult.Yes)
            {
                Instance.DataPacks.ModListInformation.RemoveAt(KeyList.SelectedIndex);
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
            if (KeyList.SelectedIndex + 1 > Instance.DataPacks.ModListInformation.Count) return;
            MoveKey(KeyList.SelectedIndex, KeyList.SelectedIndex + 1);
            RefreshKeyList();
        }


        public void MoveKey(int oldIndex, int newIndex)
        {
            Tuple<string, List<Tuple<string, string>>> item = Instance.DataPacks.ModListInformation[oldIndex];
            Instance.DataPacks.ModListInformation.RemoveAt(oldIndex);
            Instance.DataPacks.ModListInformation.Insert(newIndex, item);
        }

        public void MoveValue(int oldIndex, int newIndex)
        {
            Tuple<string, string> item = Instance.DataPacks.ModListInformation[KeyList.SelectedIndex].Item2[oldIndex];
            Instance.DataPacks.ModListInformation[KeyList.SelectedIndex].Item2.RemoveAt(oldIndex);
            Instance.DataPacks.ModListInformation[KeyList.SelectedIndex].Item2.Insert(newIndex, item);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Are you sure you want to save?", "Confirm Save", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            if (result == MessageBoxResult.Yes)
            {
                Instance.DataPacks.SaveFile();
            }

        }

        private void ChangeNameButton_Click(object sender, RoutedEventArgs e)
        {
            if (KeyIndexValid() == false) return;
            var itemToEdit = Instance.DataPacks.ModListInformation[KeyList.SelectedIndex];
            Instance.DataPacks.ModListInformation[KeyList.SelectedIndex] = new Tuple<string, List<Tuple<string, string>>>(KeyNameTextBox.Text, itemToEdit.Item2);
            RefreshKeyList();
        }

        private void ChangeValueButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeValueButton_Click(sender, e, true);
        }

        private void ChangeValueButton_Click(object sender, RoutedEventArgs e, bool refreshList = true)
        {
            if (ValueIndexValid() == false) return;
            var itemToEdit = Instance.DataPacks.ModListInformation[KeyList.SelectedIndex];
            var subItemtoEdit = itemToEdit.Item2;
            var valueItemToEdit = subItemtoEdit[ValueList.SelectedIndex];
            var valueItemEdited = new Tuple<string, string>(valueItemToEdit.Item1, ValueTextBox.Text);
            subItemtoEdit[ValueList.SelectedIndex] = valueItemEdited;
            Instance.DataPacks.ModListInformation[KeyList.SelectedIndex] = new Tuple<string, List<Tuple<string, string>>>(itemToEdit.Item1, subItemtoEdit);
            if (refreshList) RefreshValueList();
        }

        private void ChangeValueNameButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeValueNameButton_Click(sender, e, true);
        }

        private void ChangeValueNameButton_Click(object sender, RoutedEventArgs e, bool refreshList = true)
        {
            if (ValueIndexValid() == false) return;
            var itemToEdit = Instance.DataPacks.ModListInformation[KeyList.SelectedIndex];
            var subItemtoEdit = itemToEdit.Item2;
            var valueItemToEdit = subItemtoEdit[ValueList.SelectedIndex];
            var valueItemEdited = new Tuple<string, string>(ValueNameTextBox.Text, valueItemToEdit.Item2);
            subItemtoEdit[ValueList.SelectedIndex] = valueItemEdited;
            Instance.DataPacks.ModListInformation[KeyList.SelectedIndex] = new Tuple<string, List<Tuple<string, string>>>(itemToEdit.Item1, subItemtoEdit);
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
            if (Instance.DataPacks.ModListInformation[KeyList.SelectedIndex].Item2 == null) Instance.DataPacks.ModListInformation[KeyList.SelectedIndex] = new Tuple<string, List<Tuple<string, string>>>(Instance.DataPacks.ModListInformation[KeyList.SelectedIndex].Item1, new List<Tuple<string, string>>());
            Instance.DataPacks.ModListInformation[KeyList.SelectedIndex].Item2.Add(new Tuple<string, string>("Mod", "n/a"));
            RefreshValueList();
        }

        private void RemoveValueButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValueIndexValid() == false) return;
            MessageBoxResult result = System.Windows.MessageBox.Show("Are you sure you want to delete this entry?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            if (result == MessageBoxResult.Yes)
            {
                Instance.DataPacks.ModListInformation[KeyList.SelectedIndex].Item2.RemoveAt(ValueList.SelectedIndex);
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
            if (ValueList.SelectedIndex + 1 > Instance.DataPacks.ModListInformation[KeyList.SelectedIndex].Item2.Count) return;
            MoveValue(ValueList.SelectedIndex, ValueList.SelectedIndex + 1);
            RefreshValueList();
        }

        private void FindDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            GenerationsLib.Core.FolderSelectDialog folderSelect = new GenerationsLib.Core.FolderSelectDialog();   
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
            if (Instance.DataPacks.ModListInformation != ModListInformationUnedited)
            {
                //MessageBoxResult result = System.Windows.MessageBox.ShowYesNoCancel("You haven't saved your changes yet! Would you like to save your changes?", "Unsaved Changes", "Save and Exit", "Exit without Saving", "Cancel", MessageBoxImage.Exclamation);
                MessageBoxResult result = System.Windows.MessageBox.Show("You haven't saved your changes yet! Would you like to save your changes?", "Unsaved Changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.Yes)
                {
                    Instance.DataPacks.SaveFile();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
                else if (result == MessageBoxResult.No)
                {
                    Instance.DataPacks.ModListInformation = ModListInformationUnedited;
                }
                else
                {
                    e.Cancel = true;
                }
            }

        }

        private void LoadSource_Click(object sender, RoutedEventArgs e)
        {
            if (System.IO.File.Exists(System.IO.Path.Combine(Environment.CurrentDirectory, "Resources", "ModPackLists.ini")))
            {
                System.Diagnostics.Process.Start("explorer.exe", "/select, " + System.IO.Path.Combine(Environment.CurrentDirectory, "Resources", "ModPackLists.ini"));
            }
            else
            {
                System.Windows.MessageBox.Show("File does not exist at " + System.IO.Path.Combine(Environment.CurrentDirectory, "Resources", "ModPackLists.ini"), "ERROR");
            }
        }

        private void HintsButton_Click(object sender, RoutedEventArgs e)
        {
            HintsButton.ContextMenu.IsOpen = true;
        }
    }
}
