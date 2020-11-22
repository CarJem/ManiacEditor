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
using ManiacEditor.Extensions;
using ManiacEditor.Classes.Scene;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ManiacEditor.Controls.Toolbox
{
	public partial class LayerManager : Window
	{
		#region View Model
		public class ViewModel : INotifyPropertyChanged
		{
            #region Collections
            private IList<EditorLayer> _Layers { get; set; }
			public IList<EditorLayer> Layers
			{
				get
				{
					return _Layers;
				}
				set
				{
					_Layers = value;
					NotifyPropertyChanged("Layers");
				}
			}
			public IList<HorizontalLayerScroll> HorizontalLayerScroll
			{
				get
				{
					return GetHorizontalLayerScrolls();
				}
				set
				{
					SetHorizontalLayerScrolls(value);
					NotifyPropertyChanged("HorizontalLayerScroll");

				}
			}
			public IList<ScrollInfoLines> ScrollInfoLines
			{
				get
				{
					return GetScrollInfoLines();
				}
				set
				{
					SetScrollInfoLines(value);
					NotifyPropertyChanged("ScrollInfoLines");
				}
			}
            #endregion

            #region Current Items
            public EditorLayer CurrentLayer
			{
				get
				{
					return GetCurrentLayer();
				}
				set
				{
					SetCurrentLayer(value);
					NotifyPropertyChanged("CurrentLayer");
				}
			}
			public HorizontalLayerScroll CurrentHorizontalLayerScroll
			{
				get
				{
					return GetCurrentHorizontalLayerScroll();
				}
				set
				{
					SetCurrentHorizontalLayerScroll(value);
					NotifyPropertyChanged("CurrentHorizontalLayerScroll");
				}
			}
			public ScrollInfoLines CurrentScrollInfoLines
			{
				get
				{
					return GetCurrentScrollInfoLine();
				}
				set
				{
					SetCurrentScrollInfoLine(value);
					NotifyPropertyChanged("CurrentScrollInfoLines");
				}
			}
			#endregion

			#region Indexes
			private int _SelectedLayerIndex { get; set; } = 0;
			private int _SelectedLayerHorizontalIndex { get; set; } = 0;
			private int _SelectedLayerScrollIndex { get; set; } = 0;

			public int SelectedLayerIndex
			{
				get
				{
					return _SelectedLayerIndex;
				}
				set
				{
					_SelectedLayerIndex = value;
					UpdateSection(true, true, true);
				}
			}
			public int SelectedLayerHorizontalIndex
			{
				get
				{
					return _SelectedLayerHorizontalIndex;
				}
				set
				{
					_SelectedLayerHorizontalIndex = value;
					UpdateSection(false, true, true);
				}
			}
			public int SelectedLayerScrollIndex
			{
				get
				{
					return _SelectedLayerScrollIndex;
				}
				set
				{
					_SelectedLayerScrollIndex = value;
					UpdateSection(false, false, true);
				}
			}


            #endregion

            #region Index Helpers

			public bool OutsideOfLayerBounds
			{
				get 
				{
					if (Layers == null) return true;
					return (Layers.Count - 1 < SelectedLayerIndex || SelectedLayerIndex < 0);
				}
			}

			public bool OutsideOfHorizontalRuleBounds
			{
				get
				{
					if (Layers == null) return true;
					return (Layers[SelectedLayerIndex].HorizontalLayerRules.Count - 1 < SelectedLayerHorizontalIndex || SelectedLayerHorizontalIndex < 0);
				}
			}

			public bool OutsideOfScrollInfoBounds
			{
				get
				{
					if (Layers == null) return true;
					return (Layers[SelectedLayerIndex].HorizontalLayerRules[SelectedLayerHorizontalIndex].LinesMapList.Count - 1 < SelectedLayerScrollIndex || SelectedLayerScrollIndex < 0);
				}
			}

			#endregion

			#region Current Helper Methods
			private EditorLayer GetCurrentLayer()
			{
				if (OutsideOfLayerBounds) return null;
				return Layers[SelectedLayerIndex];
			}
			private void SetCurrentLayer(EditorLayer value)
			{
				if (OutsideOfLayerBounds) return;
				else Layers[SelectedLayerIndex] = value;

			}
			private HorizontalLayerScroll GetCurrentHorizontalLayerScroll()
			{
				if (OutsideOfLayerBounds) return null;
				else if (OutsideOfHorizontalRuleBounds) return null;
				return Layers[SelectedLayerIndex].HorizontalLayerRules[SelectedLayerHorizontalIndex];
			}
			private ScrollInfoLines GetCurrentScrollInfoLine()
			{
				if (OutsideOfLayerBounds) return null;
				else if (OutsideOfHorizontalRuleBounds) return null;
				else if (OutsideOfScrollInfoBounds) return null;
				else return Layers[SelectedLayerIndex].HorizontalLayerRules[SelectedLayerHorizontalIndex].LinesMapList[SelectedLayerScrollIndex];

			}
			private void SetCurrentHorizontalLayerScroll(HorizontalLayerScroll value)
			{
				if (OutsideOfLayerBounds) return;
				else if (OutsideOfHorizontalRuleBounds) return;
				else Layers[SelectedLayerIndex].HorizontalLayerRules[SelectedLayerHorizontalIndex] = value;
			}
			private void SetCurrentScrollInfoLine(ScrollInfoLines value)
			{
				if (OutsideOfLayerBounds) return;
				else if (OutsideOfHorizontalRuleBounds) return;
				else if (OutsideOfScrollInfoBounds) return;
				else Layers[SelectedLayerIndex].HorizontalLayerRules[SelectedLayerHorizontalIndex].LinesMapList[SelectedLayerScrollIndex] = value;
			}

            #endregion

            #region Collection Helper Methods

            private IList<HorizontalLayerScroll> GetHorizontalLayerScrolls()
			{
				if (OutsideOfLayerBounds) return null;
				else return Layers[SelectedLayerIndex].HorizontalLayerRules;
			}
			private IList<ScrollInfoLines> GetScrollInfoLines()
			{
				if (OutsideOfLayerBounds) return null;
				else if (OutsideOfHorizontalRuleBounds) return null;
				else return Layers[SelectedLayerIndex].HorizontalLayerRules[SelectedLayerHorizontalIndex].LinesMapList;

			}
			private void SetHorizontalLayerScrolls(IList<HorizontalLayerScroll> value)
			{
				if (OutsideOfLayerBounds) return;
				Layers[SelectedLayerIndex].HorizontalLayerRules = value;
			}
			private void SetScrollInfoLines(IList<ScrollInfoLines> value)
			{
				if (OutsideOfLayerBounds) return;
				else if (OutsideOfHorizontalRuleBounds) return;
				else Layers[SelectedLayerIndex].HorizontalLayerRules[SelectedLayerHorizontalIndex].LinesMapList = value;
			}

			#endregion

			#region Update Helper Methods

			private void UpdateSection(bool UpdateLayerProps, bool UpdateHorizontalProps, bool UpdateScrollMapProps)
			{
				if (UpdateLayerProps)
				{
					NotifyPropertyChanged("SelectedLayerIndex");
					NotifyPropertyChanged("CurrentLayer");
				}
				if (UpdateHorizontalProps)
				{
					NotifyPropertyChanged("SelectedLayerHorizontalIndex");
					NotifyPropertyChanged("CurrentHorizontalLayerScroll");
				}
				if (UpdateScrollMapProps)
				{
					NotifyPropertyChanged("SelectedLayerScrollIndex");
					NotifyPropertyChanged("CurrentScrollInfoLines");
				}
			}

			#endregion

			#region INotifyPropertyChanged Properties

			public event PropertyChangedEventHandler PropertyChanged;

			protected void NotifyPropertyChanged(String info)
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(info));
				}
			}

            #endregion
        }

        public ViewModel CurrentDataContext
		{
			get
			{
				return (this.DataContext as ViewModel);
			}
			set
			{
				this.DataContext = value;
			}
		}

        #endregion

        #region Base Definitions

        private Classes.Scene.EditorScene EditorScene;

		#endregion

		#region Index Definitions

		private int SelectedLayerIndex
		{
			get
			{
				return CurrentDataContext.SelectedLayerIndex;
			}
		}
		private int SelectedLayerScrollIndex
		{
			get
			{
				return CurrentDataContext.SelectedLayerScrollIndex;
			}
		}
		private int SelectedLayerHorizontalIndex
		{
			get
			{
				return CurrentDataContext.SelectedLayerHorizontalIndex;
			}
		}

		#endregion

		#region Definitions
		private bool HasLayerArangementChanged { get; set; } = false;
		private bool isLoaded { get; set; } = true;


        #endregion

        #region Init
        public LayerManager(Classes.Scene.EditorScene editorScene)
		{
			InitializeComponent();
			CurrentDataContext = new ViewModel();
			EditorScene = editorScene;

			LayerContextMenu.Foreground = (SolidColorBrush)FindResource("NormalText");
			LayerContextMenu.Background = (SolidColorBrush)FindResource("ToolbarBackground");
			LayerContextMenu.BorderBrush = (SolidColorBrush)FindResource("ButtonBorder");

			lbLayers.SelectionChanged += OversizeTextUpdater;
			lbHorizontalRules.SelectionChanged += OversizeTextUpdater;
			lbMappings.SelectionChanged += OversizeTextUpdater;
			nudStartLine.ValueChanged += OversizeTextUpdater;
			nudLineCount.ValueChanged += OversizeTextUpdater;


			InitDataBindings(EditorScene.AllLayers);
		}



		private void InitDataBindings(IList<EditorLayer> Layers)
		{

			BindControl(lbLayers, ListBox.SelectedIndexProperty, "SelectedLayerIndex");
			BindControl(lbHorizontalRules, ListBox.SelectedIndexProperty, "SelectedLayerHorizontalIndex");
			BindControl(lbMappings, ListBox.SelectedIndexProperty, "SelectedLayerScrollIndex");
			
			BindControl(lbLayers, ListBox.ItemsSourceProperty, "Layers");
			BindControl(lbHorizontalRules, ListBox.ItemsSourceProperty, "CurrentLayer.HorizontalLayerRules");
			BindControl(lbMappings, ListBox.ItemsSourceProperty, "CurrentHorizontalLayerScroll.LinesMapList");

			lbLayers.Items.Refresh();
			lbHorizontalRules.Items.Refresh();
			lbMappings.Items.Refresh();

			CurrentDataContext.Layers = Layers;

			lbLayers.DisplayMemberPath = "Name";

			BindControlOneWay(lblRawWidthValue, Label.ContentProperty, "CurrentLayer.Width");
			BindControlOneWay(lblRawHeightValue, Label.ContentProperty, "CurrentLayer.Height");

			BindControlEffictiveSize(lblEffSizeWidth, Label.ContentProperty, "CurrentLayer.Width");
			BindControlEffictiveSize(lblEffSizeHeight, Label.ContentProperty, "CurrentLayer.Height");

			BindControl(nudWidth, Xceed.Wpf.Toolkit.IntegerUpDown.ValueProperty, "CurrentLayer.WorkingWidth");
			BindControl(nudHeight, Xceed.Wpf.Toolkit.IntegerUpDown.ValueProperty, "CurrentLayer.WorkingHeight");
			BindControl(tbName, TextBox.TextProperty, "CurrentLayer.Name");

			BindControl(nudVerticalScroll, Xceed.Wpf.Toolkit.IntegerUpDown.ValueProperty, "CurrentLayer.Behaviour");
			BindControl(nudUnknownByte2, Xceed.Wpf.Toolkit.IntegerUpDown.ValueProperty, "CurrentLayer.DrawingOrder");
			BindControl(nudUnknownWord1, Xceed.Wpf.Toolkit.IntegerUpDown.ValueProperty, "CurrentLayer.RelativeSpeed");
			BindControl(nudUnknownWord2, Xceed.Wpf.Toolkit.IntegerUpDown.ValueProperty, "CurrentLayer.ConstantSpeed");

			BindControl(nudHorizontalEffect, Xceed.Wpf.Toolkit.IntegerUpDown.ValueProperty, "CurrentHorizontalLayerScroll.Behavior");
			BindControl(nudHorizByte2, Xceed.Wpf.Toolkit.IntegerUpDown.ValueProperty, "CurrentHorizontalLayerScroll.DrawOrder");
			BindControl(nudHorizVal1, Xceed.Wpf.Toolkit.IntegerUpDown.ValueProperty, "CurrentHorizontalLayerScroll.RelativeSpeed");
			BindControl(nudHorizVal2, Xceed.Wpf.Toolkit.IntegerUpDown.ValueProperty, "CurrentHorizontalLayerScroll.ConstantSpeed");

			BindControl(nudStartLine, Xceed.Wpf.Toolkit.IntegerUpDown.ValueProperty, "CurrentScrollInfoLines.StartIndex");
			BindControl(nudLineCount, Xceed.Wpf.Toolkit.IntegerUpDown.ValueProperty, "CurrentScrollInfoLines.LineCount");



		}
		#endregion

		#region Layer Manipulation
		private void ResizeLayer()
		{
			if (MessageBox.Show(@"Resizing a layer can not be undone. You really should save what you have and take a backup first. Proceed with the resize?", "Caution!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
			{
				var layer = CurrentDataContext.Layers.ElementAt(SelectedLayerIndex);
				layer.Resize(Convert.ToUInt16(nudWidth.Value), Convert.ToUInt16(nudHeight.Value));
				UpdateListDetails();
			}	
		}

		private void MoveLayerUp()
		{
			var current = CurrentDataContext.Layers.ElementAt(SelectedLayerIndex);
			if (current == null) return;

			int index = lbLayers.SelectedIndex;
			if (index == 0) return;
			CurrentDataContext.Layers.Remove(current);
			CurrentDataContext.Layers.Insert(--index, current);
			lbLayers.SelectedIndex = index;
			HasLayerArangementChanged = true;
			UpdateListDetails();

		}
		private void MoveLayerDown()
		{
			var current = CurrentDataContext.Layers.ElementAt(SelectedLayerIndex);
			if (current == null) return;

			int index = lbLayers.SelectedIndex;
			if (index == CurrentDataContext.Layers.Count - 1) return;
			CurrentDataContext.Layers.Remove(current);
			CurrentDataContext.Layers.Insert(++index, current);
			lbLayers.SelectedIndex = index;
			HasLayerArangementChanged = true;
			UpdateListDetails();
		}
		private void AddLayer()
		{
			Classes.Scene.EditorLayer newEditorLayer = EditorScene.ProduceLayer();
			CurrentDataContext.Layers.Add(newEditorLayer);
			int newIndex = CurrentDataContext.Layers.IndexOf(newEditorLayer);
			lbLayers.SelectedIndex = newIndex;
			HasLayerArangementChanged = true;
			UpdateListDetails();

		}
		private void DeleteLayer()
		{
			var current = CurrentDataContext.Layers.ElementAt(SelectedLayerIndex);
			if (null == current) return;

			if (MessageBox.Show($@"Deleting a layer can not be undone! Are you sure you want to delete the [{current.Name}] layer?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
			{
				EditorScene.DeleteLayer(current);
				HasLayerArangementChanged = true;
				UpdateListDetails();
			}
		}
		private void CutLayer()
		{
			var current = CurrentDataContext.Layers.ElementAt(SelectedLayerIndex);
			if (null == current) return;
			CopyLayerToClipboard(current);
			CurrentDataContext.Layers.Remove(current);
			HasLayerArangementChanged = true;
			UpdateListDetails();

		}
		private void CopyLayer()
		{
			var current = CurrentDataContext.Layers.ElementAt(SelectedLayerIndex);
			if (null == current) return;
			CopyLayerToClipboard(current);
			UpdateListDetails();
		}
		private void DuplicateLayer()
		{
			var current = CurrentDataContext.Layers.ElementAt(SelectedLayerIndex) as Classes.Scene.EditorLayer;
			if (null == current) return;
			CurrentDataContext.Layers.Insert(SelectedLayerIndex, current);
			HasLayerArangementChanged = true;
			UpdateListDetails();

		}
		private void PasteLayer()
		{
			PasteLayerFromClipboard();
			UpdateListDetails();
		}

		#endregion

		#region Horizontal Mapping Manipulation
		private void MoveHorizontalMappingUp()
		{
			var current = CurrentDataContext.Layers.ElementAt(SelectedLayerIndex).HorizontalLayerRules.ElementAt(SelectedLayerHorizontalIndex).LinesMapList.ElementAt(SelectedLayerScrollIndex);
			if (current == null) return;

			int index = lbMappings.SelectedIndex;
			if (index == 0) return;
			CurrentDataContext.ScrollInfoLines.Remove(current);
			CurrentDataContext.ScrollInfoLines.Insert(--index, current);
			lbMappings.SelectedIndex = index;
			HasLayerArangementChanged = true;
			UpdateListDetails();

		}
		private void MoveHorizontalMappingDown()
		{
			var current = CurrentDataContext.Layers.ElementAt(SelectedLayerIndex).HorizontalLayerRules.ElementAt(SelectedLayerHorizontalIndex).LinesMapList.ElementAt(SelectedLayerScrollIndex);
			if (current == null) return;

			int index = lbMappings.SelectedIndex;
			if (index == CurrentDataContext.ScrollInfoLines.Count - 1) return;
			CurrentDataContext.ScrollInfoLines.Remove(current);
			CurrentDataContext.ScrollInfoLines.Insert(++index, current);
			lbMappings.SelectedIndex = index;
			HasLayerArangementChanged = true;
			UpdateListDetails();
		}
		private void AddHorizontalMapping()
		{
			var hls = CurrentDataContext.Layers.ElementAt(SelectedLayerIndex).HorizontalLayerRules.ElementAt(SelectedLayerHorizontalIndex);
			if (null == hls) return;

			hls.AddMapping();			
		}
		private void RemoveHorizontalMapping()
		{
			var itemsToRemove = lbMappings.SelectedItems;
			if (null == itemsToRemove) return;

			if (MessageBox.Show($@"Deleting a set of horizontal scrolling rule mappings can not be undone!
			Are you sure you want to delete?",
			"Confirm Deletion",
			MessageBoxButton.YesNo,
			MessageBoxImage.Warning) == MessageBoxResult.Yes)
			{
				foreach (var entry in itemsToRemove)
				{
					CurrentDataContext.Layers.ElementAt(SelectedLayerIndex).HorizontalLayerRules.ElementAt(SelectedLayerHorizontalIndex).LinesMapList.Remove(entry as ScrollInfoLines);
				}

				lbMappings.Items.Refresh();
			}
		}


		#endregion

		#region Horizontal Rule Manipulation
		private void MoveHorizontalRuleUp()
		{
			var current = CurrentDataContext.Layers.ElementAt(SelectedLayerIndex).HorizontalLayerRules.ElementAt(SelectedLayerHorizontalIndex);
			if (current == null) return;

			int index = lbHorizontalRules.SelectedIndex;
			if (index == 0) return;
			CurrentDataContext.HorizontalLayerScroll.Remove(current);
			CurrentDataContext.HorizontalLayerScroll.Insert(--index, current);
			lbHorizontalRules.SelectedIndex = index;
			HasLayerArangementChanged = true;
			UpdateListDetails();

		}
		private void MoveHorizontalRuleDown()
		{
			var current = CurrentDataContext.Layers.ElementAt(SelectedLayerIndex).HorizontalLayerRules.ElementAt(SelectedLayerHorizontalIndex);
			if (current == null) return;

			int index = lbHorizontalRules.SelectedIndex;
			if (index == CurrentDataContext.HorizontalLayerScroll.Count - 1) return;
			CurrentDataContext.HorizontalLayerScroll.Remove(current);
			CurrentDataContext.HorizontalLayerScroll.Insert(++index, current);
			lbHorizontalRules.SelectedIndex = index;
			HasLayerArangementChanged = true;
			UpdateListDetails();
		}
		private void AddHorizontalRule()
		{

			// create the horizontal rule set
			var layer = CurrentDataContext.Layers.ElementAt(SelectedLayerIndex);
			layer.ProduceHorizontalLayerScroll();

			// make sure our view of the underlying set of rules is refreshed
			lbHorizontalRules.Items.Refresh();

			// and select the one we just made
			lbHorizontalRules.SelectedIndex = lbHorizontalRules.Items.Count - 1;
		}
		private void RemoveHorizontalRule()
		{
			int amountSelected = lbHorizontalRules.SelectedItems.Count;

			if (CurrentDataContext.Layers.ElementAt(SelectedLayerIndex).HorizontalLayerRules.Count - amountSelected < 1)
			{
				MessageBox.Show("There must be at least one set of horizontal scrolling rules.",
				"Delete not allowed.",
				MessageBoxButton.OK,
				MessageBoxImage.Error);
				return;
			}

			var itemsToRemove = lbHorizontalRules.SelectedItems;

			if (null == itemsToRemove) return;

			if (MessageBox.Show($@"Deleting a set of horizontal scrolling rules can not be undone!
			Are you sure you want to delete these set of rules?
			All mappings for this rule will be deleted too!",
			"Confirm Deletion",
			MessageBoxButton.YesNo,
			MessageBoxImage.Warning) == MessageBoxResult.Yes)
			{
				foreach (var entry in itemsToRemove)
				{
					CurrentDataContext.Layers.ElementAt(SelectedLayerIndex).HorizontalLayerRules.Remove(entry as HorizontalLayerScroll);
				}

				lbHorizontalRules.Items.Refresh();
			}
		}

		#endregion

		#region Clipboard

		private void CopyLayerToClipboard(Classes.Scene.EditorLayer layerToCopy)
		{
			Classes.Scene.EditorLayer copyData = layerToCopy.Clone();

			Methods.Solution.SolutionClipboard.SetLayerClipboard(new Classes.Clipboard.LayerClipboardEntry(copyData));

		}
		public void PasteLayerFromClipboard()
		{
			if (Clipboard.ContainsData("ManiacLayer") && false)
			{
				var layerToPaste = (Classes.Scene.EditorLayer)Clipboard.GetDataObject().GetData("ManiacLayer");

				CurrentDataContext.Layers.Insert(CurrentDataContext.Layers.Count - 1, layerToPaste);

				HasLayerArangementChanged = true;

				
			}
			else if (Methods.Solution.SolutionClipboard.LayerClipboard != null)
			{
				var layerToPaste = Methods.Solution.SolutionClipboard.LayerClipboard;
				CurrentDataContext.Layers.Insert(CurrentDataContext.Layers.Count - 1, layerToPaste.GetData());
				HasLayerArangementChanged = true;

				
			}
		}

		#endregion

		#region Events
		private void HorizontalMappingChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			lbMappings.Items.Refresh();
		}
		private void DesiredSizeChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			UpdateDesiredSizeLabel();
		}
		private void btnAddHorizontalRule_Click(object sender, RoutedEventArgs e)
		{
			AddHorizontalRule();
		}
		private void btnRemoveHorizontalRule_Click(object sender, RoutedEventArgs e)
		{
			RemoveHorizontalRule();
		}
		private void btnAddHorizontalMapping_Click(object sender, RoutedEventArgs e)
		{
			AddHorizontalMapping();
		}
		private void btnRemoveHorizontalMapping_Click(object sender, RoutedEventArgs e)
		{
			RemoveHorizontalMapping();
		}
		private void OversizeTextUpdater(object sender, RoutedEventArgs e)
		{
			UpdateLayerOversizedLabels();
		}

		private void btnResize_Click(object sender, RoutedEventArgs e)
		{
			ResizeLayer();
		}

		private void btnUp_Click(object sender, RoutedEventArgs e)
		{
			MoveLayerUp();
		}

		private void btnDown_Click(object sender, RoutedEventArgs e)
		{
			MoveLayerDown();
		}
		private void btnUpMappings_Click(object sender, RoutedEventArgs e)
		{
			MoveHorizontalMappingUp();
		}

		private void btnDownMappings_Click(object sender, RoutedEventArgs e)
		{
			MoveHorizontalMappingDown();
		}

		private void btnUpRules_Click(object sender, RoutedEventArgs e)
		{
			MoveHorizontalRuleUp();
		}

		private void btnDownRules_Click(object sender, RoutedEventArgs e)
		{
			MoveHorizontalRuleDown();
		}
		private void btnAdd_Click(object sender, RoutedEventArgs e)
		{
			AddLayer();
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			DeleteLayer();
		}

		private void btnCut_Click(object sender, RoutedEventArgs e)
		{
			CutLayer();
		}

		private void btnCopy_Click(object sender, RoutedEventArgs e)
		{
			CopyLayer();
		}

		private void btnDuplicate_Click(object sender, RoutedEventArgs e)
		{
			DuplicateLayer();
		}

		private void btnPaste_Click(object sender, RoutedEventArgs e)
		{
			PasteLayer();
		}

		#endregion

		#region UI Updating

		public void UpdateListDetails()
		{
			lbLayers.Items.Refresh();
			lbHorizontalRules.Items.Refresh();
			lbMappings.Items.Refresh();
		}
		public void UpdateDesiredSizeLabel()
		{
			lblResizedEffective.Text = $"Effective Width {(nudWidth.Value * 16):N0}, " + $"Effective Height {(nudHeight.Value * 16):N0}";
		}
		public void UpdateLayerOversizedLabels()
		{
			if (!isLoaded)
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
				}
			}
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

		#endregion

		#region Selection Changed

		private void LbHorizontalRules_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}
		private void LbMappings_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}
		private void LbLayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}
		#endregion

		#region Open/Close Events

		private void LayerManager_FormClosed(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!HasLayerArangementChanged) return;

			MessageBox.Show(@"If you have changed the number, or order of the layers, 
			you may need to update any layer switching entities/objects in the scene too.

			If you don't, strange things may well happen.
			They may well happen anway, this is all experimental!",
			"Don't forget!",
			MessageBoxButton.OK,
			MessageBoxImage.Information);
		}
		private void LayerManager_Load(object sender, RoutedEventArgs e)
		{
			isLoaded = false;
		}

		#endregion

		#region Context Menu

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


		#endregion


	}
}
