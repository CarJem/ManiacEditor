using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Media;
using ManiacEditor.Converters;

namespace ManiacEditor.Interfaces
{
	/// <summary>
	/// Converts a number of tiles, in any one dimension, to the equivalent number of pixels.
	/// </summary>


	/// <summary>
	/// Interaction logic for LayerEditor.xaml
	/// </summary>
	public partial class LayerManager : Window
	{
		private EditorSolution.EditorScene _editorScene;
		private IList<EditorLayer> Layers
		{
			get => _editorScene?.AllLayersList;
		}

		private int SelectedLayerIndex
		{
			get => GetSelectedLayer();
		}

		private int SelectedLayerHorizontalIndex
		{
			get => GetSelectedHorizontalMap();
		}

		private int SelectedLayerScrollIndex
		{
			get => GetSelectedLayerScroll();
		}

        private EditorLayer SelectedLayer;

		private bool LockSelectionChangedTriggers = false;
		private bool _layerArrangementChanged = false;
		bool initilzing = true;

		//Previous Values for Specific Varriables
		int nudStartLineTemp = 0;
		int nudLineCountTemp = 0;

		//Clipboards
		private EditorLayer LayerClipboard;



		// I clearly have no understanding of WinForms Data Binding
		public LayerManager(EditorSolution.EditorScene editorScene)
		{
			InitializeComponent();
			_editorScene = editorScene;
			if (Settings.MySettings.NightMode)
			{
				SetRTFText(ManiacEditor.Properties.Resources.LayerManagerWarningDarkTheme);
			}
			else
			{
				SetRTFText(ManiacEditor.Properties.Resources.LayerManagerWarning);
			}

			contextMenu1.Foreground = (SolidColorBrush)FindResource("NormalText");
			contextMenu1.Background = (SolidColorBrush)FindResource("ToolbarBackground");
			contextMenu1.BorderBrush = (SolidColorBrush)FindResource("ButtonBorder");

			lbLayers.SelectionChanged += OversizeTextUpdater;
			lbHorizontalRules.SelectionChanged += OversizeTextUpdater;
			lbMappings.SelectionChanged += OversizeTextUpdater;
			nudStartLine.ValueChanged += OversizeTextUpdater;
			nudLineCount.ValueChanged += OversizeTextUpdater;

			SelectionChanged();

		}

		public void SetRTFText(string text)
		{
			MemoryStream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(text));
			this.rtbWarn.Selection.Load(stream, DataFormats.Rtf);
		}

		public void BindControl(DependencyObject control, DependencyProperty prop, string path)
		{
			var binding = new Binding(path);
			binding.Source = DataContext;
			binding.Mode = BindingMode.TwoWay;
			binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			BindingOperations.SetBinding(control, prop, binding);
		}

		public void BindControlOneWay(DependencyObject control, DependencyProperty prop, string path)
		{
			var binding = new Binding(path);
			binding.Source = DataContext;
			binding.Mode = BindingMode.OneWay;
			binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			BindingOperations.SetBinding(control, prop, binding);
		}

		public void BindControlEffictiveSize(DependencyObject control, DependencyProperty prop, string path)
		{
			var binding = new Binding(path);
			binding.Source = DataContext;
			binding.Mode = BindingMode.OneWay;
			binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Converter = new TilePixelConverter();
			BindingOperations.SetBinding(control, prop, binding);
		}

		public int GetSelectedLayer()
		{
			if (lbLayers == null) return 0;
			if (lbLayers.SelectedIndex != -1)
			{
				return lbLayers.SelectedIndex;
			}
			else return 0;
		}

		public int GetSelectedLayerScroll()
		{
			if (lbHorizontalRules == null) return 0;
			if (lbHorizontalRules.SelectedIndex != -1)
			{
				return lbHorizontalRules.SelectedIndex;
			}
			else return 0;
		}

		public int GetSelectedHorizontalMap()
		{
			if (lbMappings == null) return 0;
			if (lbMappings.SelectedIndex != -1)
			{
				return lbMappings.SelectedIndex;
			}
			else return 0;
		}


		private void DesiredSizeChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			lblResizedEffective.Content = $"Effective Width {(nudWidth.Value * 16):N0}, " +
										$"Effective Height {(nudHeight.Value * 16):N0}";
		}

		private void SelectionChanged(int itemChanged = 0)
		{

			if (itemChanged != 1)
			{
				lbLayers.Refresh();
				lbLayers.Items.Refresh();
			}
			if (itemChanged != 2)
			{
				lbHorizontalRules.Refresh();
				lbHorizontalRules.Items.Refresh();
			}
			if (itemChanged != 3)
			{
				lbMappings.Refresh();
				lbMappings.Items.Refresh();
			}


            SelectedLayer = Layers.ElementAtOrDefault(SelectedLayerIndex);


                DataContext = new
				{
					bsLayers = Layers.ElementAtOrDefault(SelectedLayerIndex),
					_bsHorizontal = Layers.ElementAtOrDefault(SelectedLayerIndex).HorizontalLayerScroll.ElementAtOrDefault(SelectedLayerScrollIndex),
					_bsHorizontalMap = Layers.ElementAtOrDefault(SelectedLayerIndex).HorizontalLayerScroll.ElementAtOrDefault(SelectedLayerScrollIndex).LinesMapList.ElementAtOrDefault(SelectedLayerHorizontalIndex)
				};


				if (itemChanged != 1)  lbLayers.ItemsSource = Layers;
				if (itemChanged != 2) lbHorizontalRules.ItemsSource = Layers.ElementAtOrDefault(SelectedLayerIndex).HorizontalLayerScroll;
				if (itemChanged != 3) lbMappings.ItemsSource = Layers.ElementAtOrDefault(SelectedLayerIndex).HorizontalLayerScroll.ElementAtOrDefault(SelectedLayerScrollIndex).LinesMapList;


				lbLayers.DisplayMemberPath = "Name";

				BindControlOneWay(lblRawWidthValue, Label.ContentProperty, "bsLayers.Width");
				BindControlOneWay(lblRawHeightValue, Label.ContentProperty, "bsLayers.Height");

				BindControlEffictiveSize(lblEffSizeWidth, Label.ContentProperty, "bsLayers.Width");
				BindControlEffictiveSize(lblEffSizeHeight, Label.ContentProperty, "bsLayers.Height");

				BindControlOneWay(nudWidth, Xceed.Wpf.Toolkit.IntegerUpDown.ValueProperty, "bsLayers.Width");
				BindControlOneWay(nudHeight, Xceed.Wpf.Toolkit.IntegerUpDown.ValueProperty, "bsLayers.Height");
				BindControl(tbName, TextBox.TextProperty, "bsLayers.Name");

				BindControl(nudVerticalScroll, Xceed.Wpf.Toolkit.IntegerUpDown.ValueProperty, "bsLayers.Behaviour");
				BindControl(nudUnknownByte2, Xceed.Wpf.Toolkit.IntegerUpDown.ValueProperty, "bsLayers.DrawingOrder");
				BindControl(nudUnknownWord1, Xceed.Wpf.Toolkit.IntegerUpDown.ValueProperty, "bsLayers.RelativeSpeed");
				BindControl(nudUnknownWord2, Xceed.Wpf.Toolkit.IntegerUpDown.ValueProperty, "bsLayers.ConstantSpeed");
			

				lbHorizontalRules.DisplayMemberPath = "Id";

				BindControl(nudHorizontalEffect, Xceed.Wpf.Toolkit.IntegerUpDown.ValueProperty, "_bsHorizontal.Behavior");
				BindControl(nudHorizByte2, Xceed.Wpf.Toolkit.IntegerUpDown.ValueProperty, "_bsHorizontal.DrawOrder");
				BindControl(nudHorizVal1, Xceed.Wpf.Toolkit.IntegerUpDown.ValueProperty, "_bsHorizontal.RelativeSpeed");
				BindControl(nudHorizVal2, Xceed.Wpf.Toolkit.IntegerUpDown.ValueProperty, "_bsHorizontal.ConstantSpeed");

				BindControl(nudStartLine, Xceed.Wpf.Toolkit.IntegerUpDown.ValueProperty, "_bsHorizontalMap.StartIndex");
				BindControl(nudLineCount, Xceed.Wpf.Toolkit.IntegerUpDown.ValueProperty, "_bsHorizontalMap.LineCount");
			


		}

		private void LbHorizontalRules_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (LockSelectionChangedTriggers) return;

			LockSelectionChangedTriggers = true;
			SelectionChanged(2);
			LockSelectionChangedTriggers = false;
		}

		private void LbMappings_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

			if (LockSelectionChangedTriggers) return;
			LockSelectionChangedTriggers = true;
			SelectionChanged(3);
			LockSelectionChangedTriggers = false;
		}

		private void LbLayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (LockSelectionChangedTriggers) return;

			LockSelectionChangedTriggers = true;
			lbHorizontalRules.SelectedIndex = 0;
			lbMappings.SelectedIndex = 0;
			SelectionChanged(1);
			LockSelectionChangedTriggers = false;
		}

		private void btnResize_Click(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show(@"Resizing a layer can not be undone. You really should save what you have and take a backup first. Proceed with the resize?","Caution!",MessageBoxButton.YesNo,MessageBoxImage.Warning) == MessageBoxResult.Yes)
			{
				var layer = Layers.ElementAt(GetSelectedLayer());
				layer.Resize(Convert.ToUInt16(nudWidth.Value), Convert.ToUInt16(nudHeight.Value));
			}

			SelectionChanged();
		}

		private void btnUp_Click(object sender, RoutedEventArgs e)
		{
			var current = Layers.ElementAt(GetSelectedLayer());
			if (current == null) return;

			int index = lbLayers.SelectedIndex;
			if (index == 0) return;
			Layers.Remove(current);
			Layers.Insert(--index, current);
			lbLayers.SelectedIndex = index;

			_layerArrangementChanged = true;
			SelectionChanged();
		}

		private void btnDown_Click(object sender, RoutedEventArgs e)
		{
			var current = Layers.ElementAt(GetSelectedLayer());
			if (current == null) return;

			int index = lbLayers.SelectedIndex;
			if (index == Layers.Count - 1) return;
			Layers.Remove(current);
			Layers.Insert(++index, current);
			lbLayers.SelectedIndex = index;

			_layerArrangementChanged = true;
			SelectionChanged();

		}

		private void btnAdd_Click(object sender, RoutedEventArgs e)
		{
			EditorLayer newEditorLayer = _editorScene.ProduceLayer();
			Layers.Add(newEditorLayer);
			int newIndex = Layers.IndexOf(newEditorLayer);
			lbLayers.SelectedIndex = newIndex;

			_layerArrangementChanged = true;
			SelectionChanged();
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			var current = Layers.ElementAt(GetSelectedLayer());
			if (null == current) return;

			if (MessageBox.Show($@"Deleting a layer can not be undone! Are you sure you want to delete the [{current.Name}] layer?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
			{

				Layers.Remove(current);

				_layerArrangementChanged = true;
				SelectionChanged();
			}
		}
		
		private void btnCut_Click(object sender, RoutedEventArgs e)
		{
			var current = Layers.ElementAt(GetSelectedLayer());
			if (null == current) return;
			CopyLayerToClipboard(current);
			Layers.Remove(current);
			_layerArrangementChanged = true;
			SelectionChanged();
		}

		private void btnCopy_Click(object sender, RoutedEventArgs e)
		{
			var current = Layers.ElementAt(GetSelectedLayer());
			if (null == current) return;
			CopyLayerToClipboard(current);
		}

		private void btnDuplicate_Click(object sender, RoutedEventArgs e)
		{
			var current = Layers.ElementAt(GetSelectedLayer());
			if (null == current) return;
			Layers.Insert(Layers.IndexOf(current), current);
			_layerArrangementChanged = true;
			SelectionChanged();
		}

		private void btnPaste_Click(object sender, RoutedEventArgs e)
		{
			PasteLayerFromClipboard();
		}

		private void LayerManager_FormClosed(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!_layerArrangementChanged) return;

			MessageBox.Show(@"If you have changed the number, or order of the layers, 
			you may need to update any layer switching entities/objects in the scene too.

			If you don't, strange things may well happen.
			They may well happen anway, this is all experimental!",
			"Don't forget!",
			MessageBoxButton.OK,
			MessageBoxImage.Information);
		}


		private void btnAddHorizontalRule_Click(object sender, RoutedEventArgs e)
		{
			// create the horizontal rule set
			var layer = Layers.ElementAt(GetSelectedLayer());
			layer.ProduceHorizontalLayerScroll();

			// make sure our view of the underlying set of rules is refreshed
			lbHorizontalRules.Items.Refresh();

			// and select the one we just made
			lbHorizontalRules.SelectedIndex = lbHorizontalRules.Items.Count - 1;
			SelectionChanged();
		}


		private void btnRemoveHorizontalRule_Click(object sender, RoutedEventArgs e)
		{
			if (Layers.ElementAt(GetSelectedLayer()).HorizontalLayerScroll.Count == 1)
			{
			MessageBox.Show("There must be at least one set of horizontal scrolling rules.",
			"Delete not allowed.",
			MessageBoxButton.OK,
			MessageBoxImage.Error);
			return;
			}

		var current = Layers.ElementAt(GetSelectedLayer()).HorizontalLayerScroll.ElementAt(GetSelectedLayerScroll());
		if (null == current) return;

			if (MessageBox.Show($@"Deleting a set of horizontal scrolling rules can not be undone!
			Are you sure you want to delete the set of rules with id '{current.Id}'?
			All mappings for this rule will be deleted too!",
			"Confirm Deletion",
			MessageBoxButton.YesNo,
			MessageBoxImage.Warning) == MessageBoxResult.Yes)
			{
				Layers.ElementAt(GetSelectedLayer()).HorizontalLayerScroll.Remove(current);
				SelectionChanged();
			}
		}


		private void btnAddHorizontalMapping_Click(object sender, RoutedEventArgs e)
		{
			var hls = Layers.ElementAt(GetSelectedLayer()).HorizontalLayerScroll.ElementAt(GetSelectedLayerScroll());
			if (null == hls) return;

			hls.AddMapping();
			SelectionChanged();
		}


		private void btnRemoveHorizontalMapping_Click(object sender, RoutedEventArgs e)
		{
			var current = Layers.ElementAt(GetSelectedLayer()).HorizontalLayerScroll.ElementAt(GetSelectedLayerScroll()).LinesMapList.ElementAt(GetSelectedHorizontalMap()) as ScrollInfoLines;
			if (null == current) return;

			if (MessageBox.Show($@"Deleting a set of horizontal scrolling rule mappings can not be undone!
			Are you sure you want to delete this mapping?",
			"Confirm Deletion",
			MessageBoxButton.YesNo,
			MessageBoxImage.Warning) == MessageBoxResult.Yes)
			{
				Layers.ElementAt(GetSelectedLayer()).HorizontalLayerScroll.ElementAt(GetSelectedLayerScroll()).LinesMapList.Remove(current);
				SelectionChanged();
			}
		}


		private void OversizeTextUpdater(object sender, RoutedEventArgs e)
		{
			if (!initilzing)
			{
				if (nudStartLine.Value + nudLineCount.Value > (nudHeight.Value * 16))
				{
					overflowMessage.Content = new TextBlock
					{
						Text = $@"The Start Line Value plus the Line Count Value must not Exceed the Maximum Layer Height! (" + nudStartLine.Value + "+" + nudLineCount.Value + " (" + (nudStartLine.Value + nudLineCount.Value) + ") " + "> " + (nudHeight.Value * 16) + ") You won't be able to save!",
						TextAlignment = TextAlignment.Center,
						TextWrapping = TextWrapping.WrapWithOverflow,
						Foreground = new SolidColorBrush(Colors.Red)
					};
					//SelectionChanged();
				}
				else
				{
					overflowMessage.Content = new TextBlock
					{
						Text = "Make sure the Start Line Value plus the Line Count Value does not Exceed the Maximum Layer Height! Otherwise, you will be unable to save!",
						TextAlignment = TextAlignment.Center,
						TextWrapping = TextWrapping.WrapWithOverflow,
						Foreground = (SolidColorBrush)FindResource("NormalText")
					};
					//SelectionChanged();
				}
			}

		}

		private void LayerManager_Load(object sender, RoutedEventArgs e)
		{
			if (nudStartLine.Value + nudLineCount.Value > (nudHeight.Value * 16))
			{
				nudStartLineTemp = (int)nudStartLine.Value;
				nudLineCountTemp = (int)nudLineCount.Value;
			}
			initilzing = false;
		}

		private void lbLayers_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.RightButton == MouseButtonState.Pressed)
			{
				if (lbLayers.SelectedItem != null)
				{
					lbLayers.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
					lbLayers.ContextMenu.IsOpen = true;
				}

			}
		}

		private void CopyLayerToClipboard(EditorLayer layerToCopy)
		{
			EditorLayer copyData = layerToCopy;

			// Make a DataObject for the copied data and send it to the Windows clipboard for cross-instance copying
			Clipboard.SetDataObject(new DataObject("ManiacLayer", copyData), true);

			// Also copy to Maniac's clipboard in case it gets overwritten elsewhere
			LayerClipboard = copyData;

		}

		public void PasteLayerFromClipboard()
		{
			// check if there is a layer on the Windows clipboard; if so, use those

			// For Some reason this isn't working, please check this out campbell. (And no, I put in false to prevent it from running, that's not the problem)
			if (Clipboard.ContainsData("ManiacLayer") && false)
			{
				var layerToPaste = (EditorLayer)Clipboard.GetDataObject().GetData("ManiacLayer");

				Layers.Insert(Layers.Count - 1, layerToPaste);

				_layerArrangementChanged = true;

				SelectionChanged();
			}


			// if there's none, use the internal clipboard
			else if (LayerClipboard != null)
			{

				var layerToPaste = LayerClipboard;
				Layers.Insert(Layers.Count - 1, layerToPaste);
				_layerArrangementChanged = true;

				SelectionChanged();

			}
		}

        private void SaveLayers_Click(object sender, RoutedEventArgs e)
        {
        }

        private void HorizontalMappingChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            lbMappings.Items.Refresh();
        }
    }
}
