using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

namespace ManiacEditor.Controls.Utility
{
    /// <summary>
    /// Interaction logic for ExportAsImageGUI.xaml
    /// </summary>
    public partial class ExportAsImageGUI : Window
    {
        private Classes.Scene.EditorScene Scene { get; set; }
        public ExportAsImageGUI(Classes.Scene.EditorScene _Scene)
        {
            InitializeComponent();
            Scene = _Scene;
            foreach (var layer in _Scene.OtherLayers)
            {
                CheckBox layerCheckbox = new CheckBox()
                {
                    Content = layer.Name
                };
                ExtraLayersSection.Children.Add(layerCheckbox);
            }

            foreach (var obj in _Scene.Entities.SceneObjects)
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
            List<string> ExtraLayersForExport = new List<string>();
            foreach (var checkbox in ExtraLayersSection.Children)
            {
                if (checkbox is CheckBox)
                {
                    if ((checkbox as CheckBox).IsChecked.Value)
                    {
                        ExtraLayersForExport.Add((checkbox as CheckBox).Content.ToString());
                    }
                }
            }

            if (FGLowerCheckbox.IsChecked.Value) ExtraLayersForExport.Add("FGLower");
            if (FGLowCheckbox.IsChecked.Value) ExtraLayersForExport.Add("FGLow");
            if (FGHighCheckbox.IsChecked.Value) ExtraLayersForExport.Add("FGHigh");
            if (FGHigherCheckbox.IsChecked.Value) ExtraLayersForExport.Add("FGHigher");
            if (EntitiesCheckbox.IsChecked.Value) ExtraLayersForExport.Add("Entities");

            if (SingleImageModeRadioButton.IsChecked.Value)
            {
                bool result = ManiacEditor.Methods.Editor.SolutionLoader.ExportAsPNG(ExtraLayersForExport);
                if (result != false) DialogResult = true;
            }
            else
            {
                bool result = ManiacEditor.Methods.Editor.SolutionLoader.ExportLayersAsPNG(ExtraLayersForExport);
                if (result != false) DialogResult = true;
            }


        }
    }
}
