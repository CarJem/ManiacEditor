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
using System.IO;

namespace ManiacEditor
{
    /// <summary>
    /// Interaction logic for ModPackEditor.xaml
    /// </summary>
    public partial class RenderListEditor : Window
    {
        private Editor Instance;
        private bool updatingKeys { get; set; } = false;
        private bool updatingValues { get; set; } = false;
        private List<string> RenderInformationUnedited;

        public RenderListEditor(Editor instance)
        {
            InitializeComponent();
            Instance = instance;
            if (Instance.entityRenderingObjects != null) RenderInformationUnedited = Instance.entityRenderingObjects;
            RefreshKeyList();

            UpdateValueModButtons();
            UpdateKeyModButtons();
        }

        private void RefreshKeyList()
        {
            updatingKeys = true;
            if (ValueList.Items != null) ValueList.Items.Clear();
            if (KeyList.Items != null) KeyList.Items.Clear();
            foreach (var item in Instance.entityRenderingObjects)
            {
                KeyList.Items.Add(item);
            }
            updatingKeys = false;

            RefreshUnusedRenders();
        }

        private void RefreshUnusedRenders()
        {
           if (ValueList.Items != null) ValueList.Items.Clear();

            if (Instance.EntityDrawing.EntityRenderers.Count == 0)
            {
                var types = GetType().Assembly.GetTypes().Where(t => t.BaseType == typeof(ManiacEditor.Entity_Renders.EntityRenderer)).ToList();
                foreach (var type in types)
                    Instance.EntityDrawing.EntityRenderers.Add((ManiacEditor.Entity_Renders.EntityRenderer)Activator.CreateInstance(type));
            }

            List<string> AllRenders = new List<string>();
            foreach (var item in Instance.EntityDrawing.EntityRenderers) AllRenders.Add(item.GetObjectName());

            List<string> UnusedRenders = AllRenders.Except(Instance.entityRenderingObjects).ToList();

            foreach (var item in UnusedRenders) ValueList.Items.Add(item);
        }

        private bool KeyIndexValid()
        {
            if (KeyList.SelectedItem == null) return false;
            if (Instance.entityRenderingObjects.Count > KeyList.SelectedIndex)
            {
                if (Instance.entityRenderingObjects[KeyList.SelectedIndex] != null)
                {
                    return true;
                }
                else return false;
            }
            else return false;            
        }

        private void KeyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!updatingKeys)
            {
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
        }

        public void UpdateValueModButtons()
        {

        }

        private void RefreshKeyValues()
        {
            if (KeyIndexValid() == false) return;
            KeyNameTextBox.Text = Instance.entityRenderingObjects[KeyList.SelectedIndex];
        }

        private void RefreshValueValues()
        {

        }

        private void ClearKeyValues()
        {
            KeyNameTextBox.Text = "";
        }

        private void ClearValueValues()
        {

        }

        private void ClearValueList()
        {
            ValueList.Items.Clear();
        }

        private void AddKeyButton_Click(object sender, RoutedEventArgs e)
        {
            Instance.entityRenderingObjects.Add(KeyNameTextBox.Text);
            RefreshKeyList();
        }

        private void RemoveKeyButton_Click(object sender, RoutedEventArgs e)
        {
            if (KeyIndexValid() == false) return;
            MessageBoxResult result = RSDKrU.MessageBox.Show("Are you sure you want to delete this entry?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            if (result == MessageBoxResult.Yes)
            {
                Instance.entityRenderingObjects.RemoveAt(KeyList.SelectedIndex);
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
            if (KeyList.SelectedIndex + 1 > Instance.entityRenderingObjects.Count) return;
            MoveKey(KeyList.SelectedIndex, KeyList.SelectedIndex + 1);
            RefreshKeyList();
        }


        public void MoveKey(int oldIndex, int newIndex)
        {
            string item = Instance.entityRenderingObjects[oldIndex];
            Instance.entityRenderingObjects.RemoveAt(oldIndex);
            Instance.entityRenderingObjects.Insert(newIndex, item);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = RSDKrU.MessageBox.Show("Are you sure you want to save?", "Confirm Save", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            if (result == MessageBoxResult.Yes)
            {
                SavetoFile();
            }

        }

        private void SavetoFile()
        {
            if (File.Exists(System.IO.Path.Combine(Environment.CurrentDirectory, "Resources", "objectRenderList.ini")))
            {
                TextWriter tw = new StreamWriter(System.IO.Path.Combine(Environment.CurrentDirectory, "Resources", "objectRenderList.ini"));

                foreach (String s in Instance.entityRenderingObjects)
                    tw.WriteLine(s);

                tw.Close();
            }
            else
            {
                RSDKrU.MessageBox.Show("File does not exist at " + System.IO.Path.Combine(Environment.CurrentDirectory, "Resources", "objectRenderList.ini"), "ERROR");
            }

        }

        private void ChangeNameButton_Click(object sender, RoutedEventArgs e)
        {
            if (KeyIndexValid() == false) return;
            var itemToEdit = Instance.entityRenderingObjects[KeyList.SelectedIndex];
            Instance.entityRenderingObjects[KeyList.SelectedIndex] = KeyNameTextBox.Text;
            RefreshKeyList();
        }

        private void ChangeValueButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeValueButton_Click(sender, e, true);
        }

        private void ChangeValueButton_Click(object sender, RoutedEventArgs e, bool refreshList = true)
        {

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

        }

        private void RemoveValueButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MoveUpValueButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MoveDownValueButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Instance.entityRenderingObjects != RenderInformationUnedited)
            {
                MessageBoxResult result = RSDKrU.MessageBox.ShowYesNoCancel("You haven't saved your changes yet! Would you like to save your changes?", "Unsaved Changes", "Save and Exit", "Exit without Saving", "Cancel", MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.Yes)
                {
                    SavetoFile();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
                else if (result == MessageBoxResult.No)
                {
                    Instance.entityRenderingObjects = RenderInformationUnedited;
                }
                else
                {
                    e.Cancel = true;
                }
            }

        }

        private void LoadSource_Click(object sender, RoutedEventArgs e)
        {
            if (System.IO.File.Exists(System.IO.Path.Combine(Environment.CurrentDirectory, "Resources", "objectRenderList.ini")))
            {
                System.Diagnostics.Process.Start("explorer.exe", "/select, " + System.IO.Path.Combine(Environment.CurrentDirectory, "Resources", "objectRenderList.ini"));
            }
            else
            {
                RSDKrU.MessageBox.Show("File does not exist at " + System.IO.Path.Combine(Environment.CurrentDirectory, "Resources", "objectRenderList.ini"), "ERROR");
            }
        }

        private void HintsButton_Click(object sender, RoutedEventArgs e)
        {
            HintsButton.ContextMenu.IsOpen = true;
        }
    }
}
