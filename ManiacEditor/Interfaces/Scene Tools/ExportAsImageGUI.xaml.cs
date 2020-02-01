using System.Windows;
using System.Windows.Controls;

namespace ManiacEditor.Interfaces
{
    /// <summary>
    /// Interaction logic for ExportAsImageGUI.xaml
    /// </summary>
    public partial class ExportAsImageGUI : Window
    {
        public ExportAsImageGUI(Classes.Edit.Solution.EditorScene editorScene)
        {
            InitializeComponent();
            foreach (var layer in editorScene.OtherLayers)
            {
                CheckBox layerCheckbox = new CheckBox()
                {
                    Content = layer.Name
                };
                ExtraLayersSection.Children.Add(layerCheckbox);
            }

            foreach (var obj in editorScene.Objects)
            {
                CheckBox entityCheckbox = new CheckBox()
                {
                    Content = obj.Name
                };

                RenderableObjects.Items.Add(entityCheckbox);
            }
        }

        private void RenderableObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RenderableObjects.SelectedIndex = -1;
        }

        private void CheckAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var checkbox in RenderableObjects.Items)
            {
                if (checkbox is CheckBox)
                {
                    (checkbox as CheckBox).IsChecked = true;
                }
            }
        }

        private void UncheckAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var checkbox in RenderableObjects.Items)
            {
                if (checkbox is CheckBox)
                {
                    (checkbox as CheckBox).IsChecked = false;
                }
            }
        }

        private void RenderableObjectRenders_Click(object sender, RoutedEventArgs e)
        {
            RenderableObjects.IsDropDownOpen = true;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (OnlyRenderSpecificObjects.IsChecked.Value)
            {
                RenderableObjectsDropDownButton.IsEnabled = true;
            }
            else
            {
                RenderableObjectsDropDownButton.IsEnabled = false;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
