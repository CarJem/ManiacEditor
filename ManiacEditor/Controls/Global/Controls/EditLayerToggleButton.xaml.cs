using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ManiacEditor.Controls.Global
{
    /// <summary>
    /// Interaction logic for EditLayerToggleButton.xaml
    /// </summary>
    public partial class EditLayerToggleButton : UserControl
	{
        public string LayerName { get; set; }

        #region Layer Options

		private void GetIsLayerControlsHidden()
		{
			if (IsLayerControlsHidden)
			{
				LayerOptionsDropdownButton.IsEnabled = false;
			}
			else
			{
				LayerOptionsDropdownButton.IsEnabled = true;
			}
		}


        #endregion



        public bool IsLayerOptionsEnabled { get => GetLayerOptionsEnabled(); set => SetLayerOptionsEnabled(value); }
        private bool _IsLayerOptionsEnabled = false;

        public bool GetLayerOptionsEnabled()
        {
			if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
			{
				var item = (LayerOptionsDropdownButton.Template.FindName("ToggleButtonChrome", LayerOptionsDropdownButton) as Xceed.Wpf.Toolkit.Chromes.ButtonChrome);
				if (item != null) item.IsEnabled = _IsLayerOptionsEnabled;
			}
			return _IsLayerOptionsEnabled;
		}

        private void SetLayerOptionsEnabled(bool value)
        {
			if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
			{
				var item = (LayerOptionsDropdownButton.Template.FindName("ToggleButtonChrome", LayerOptionsDropdownButton) as Xceed.Wpf.Toolkit.Chromes.ButtonChrome);
				if (item != null) item.IsEnabled = value;
			}
			_IsLayerOptionsEnabled = value;
		}



        public string Text
		{
			get { return (string)GetValue(TextBlockTextProperty); }
			set { SetValue(TextBlockTextProperty, value); }
		}

		bool DualSelectModeEnabled
		{
			get { return (bool)base.GetValue(DualSelectMode); }
			set { if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) base.SetValue(DualSelectMode, value); }
		}

		public bool? IsCheckedA
		{
			get { return LayerAToggle.IsChecked; }
			set { if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) LayerAToggle.IsChecked = value; }
		}

		public bool IsCheckedAll
		{
			get { return (LayerAToggle.IsChecked.Value || LayerToggle.IsChecked.Value || LayerBToggle.IsChecked.Value); }
		}

		public bool DualSelect
		{
			get { return DualSelectModeEnabled; }
			set { if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) DualSelectModeEnabled = value;}
		}

		private bool _IsLayerControlsHidden = false;
		public bool IsLayerControlsHidden
		{
			get 
			{ 
				return _IsLayerControlsHidden; 
			}
			set 
			{
				if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
				{
					_IsLayerControlsHidden = value;
					GetIsLayerControlsHidden();
				}

			}
		}



		public void SetGlobalCheckedState(bool state)
		{
			IsCheckedN = state;
			IsCheckedA = state;
			IsCheckedB = state;
			IsCheckedC = state;
			IsCheckedD = state;
		}

		public bool? GetCheckState(char layer)
		{
			switch (layer)
			{
				case 'A':
					return IsCheckedA;
				case 'B':
					return IsCheckedB;
				case 'C':
					return IsCheckedC;
				case 'D':
					return IsCheckedD;
				default:
					return IsCheckedN;
			}
		}

		public bool? IsCheckedN
		{
			get { return LayerToggle.IsChecked; }
			set { if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) LayerToggle.IsChecked = value; }
		}

		public Brush TextForeground
		{
			get { return (Brush)GetValue(TextForegroundColor);}
			set { if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) SetValue(TextForegroundColor, value); }
		}

		public bool? IsCheckedB
		{
			get { return LayerBToggle.IsChecked; }
			set { if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) LayerBToggle.IsChecked = value; }
		}

		public bool? IsCheckedC
		{
			get { return LayerCToggle.IsChecked; }
			set { if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) LayerCToggle.IsChecked = value; }
		}

		public bool? IsCheckedD
		{
			get { return LayerDToggle.IsChecked; }
			set { if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) LayerDToggle.IsChecked = value; }
		}

		public static readonly Brush DefaultForeText = Brushes.White;

		public event RoutedEventHandler Click;
		public event RoutedEventHandler RightClick;

		public static readonly DependencyProperty TextForegroundColor =
DependencyProperty.Register("TextForeground", typeof(Brush), typeof(EditLayerToggleButton), new UIPropertyMetadata(DefaultForeText));

		public static readonly DependencyProperty TextBlockTextProperty =
	DependencyProperty.Register("Text", typeof(string), typeof(EditLayerToggleButton), new UIPropertyMetadata(""));

		public static readonly DependencyProperty ToggleChecked =
DependencyProperty.Register("IsCheckedN", typeof(bool), typeof(EditLayerToggleButton), new UIPropertyMetadata(false));

		public static readonly DependencyProperty ToggleAChecked =
DependencyProperty.Register("IsCheckedA", typeof(bool), typeof(EditLayerToggleButton), new UIPropertyMetadata(false));

		public static readonly DependencyProperty ToggleBChecked =
DependencyProperty.Register("IsCheckedB", typeof(bool), typeof(EditLayerToggleButton), new UIPropertyMetadata(false));

		public static readonly DependencyProperty ToggleCChecked =
DependencyProperty.Register("IsCheckedC", typeof(bool), typeof(EditLayerToggleButton), new UIPropertyMetadata(false));

		public static readonly DependencyProperty ToggleDChecked =
DependencyProperty.Register("IsCheckedD", typeof(bool), typeof(EditLayerToggleButton), new UIPropertyMetadata(false));

		public static readonly DependencyProperty DualSelectMode =
DependencyProperty.Register("DualSelectModeEnabled", typeof(bool), typeof(EditLayerToggleButton), new UIPropertyMetadata(false));

		public EditLayerToggleButton()
		{
			if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
			{
				InitializeComponent();
				DataContext = this;
			}
		}

		public void ClearCheckedItems(int layerSetting = 0)
		{
			switch (layerSetting)
			{
				case 0:
					LayerAToggle.IsChecked = false;
					LayerBToggle.IsChecked = false;
					LayerToggle.IsChecked = false;
					break;
				case 1:
					LayerAToggle.IsChecked = false;
					LayerToggle.IsChecked = false;
					break;
				case 2:
					LayerBToggle.IsChecked = false;
					LayerToggle.IsChecked = false;
					break;
				case 3:
					LayerToggle.IsChecked = false;
					break;
			}

		}

		public void SwapDefaultToA(bool reverseSwap = false)
		{
			if (!reverseSwap)
			{
				if (LayerToggle.IsChecked.Value)
				{
					LayerAToggle.IsChecked = true;
					LayerToggle.IsChecked = false;
				}
			}
			else
			{
				if (LayerAToggle.IsChecked.Value)
				{
					LayerAToggle.IsChecked = false;
					LayerToggle.IsChecked = true;
				}
				LayerBToggle.IsChecked = false;
			}


		}

		private void LayerToggle_Checked(object sender, RoutedEventArgs e)
		{		
			if (LayerToggle.IsChecked.Value) SetLayerSelectedButtonColors(ToggleButton, 1);
			else if (LayerAToggle.IsChecked.Value) SetLayerSelectedButtonColors(ToggleButton, 2);
			else if (LayerBToggle.IsChecked.Value) SetLayerSelectedButtonColors(ToggleButton, 3);
			else if (LayerCToggle.IsChecked.Value) SetLayerSelectedButtonColors(ToggleButton, 4);
			else if (LayerDToggle.IsChecked.Value) SetLayerSelectedButtonColors(ToggleButton, 5);
			else SetLayerSelectedButtonColors(ToggleButton, 0);
		}

		private void LayerToggle_Unchecked(object sender, RoutedEventArgs e)
		{
			if (LayerToggle.IsChecked.Value)
			{
				SetLayerSelectedButtonColors(ToggleButton, 1);
				SetLayerSelectedCheckState(1);
			}
			else if (LayerAToggle.IsChecked.Value)
			{
				SetLayerSelectedButtonColors(ToggleButton, 2);
				SetLayerSelectedCheckState(2);
			}
			else if (LayerBToggle.IsChecked.Value)
			{
				SetLayerSelectedButtonColors(ToggleButton, 3);
				SetLayerSelectedCheckState(3);
			}
			else if (LayerCToggle.IsChecked.Value)
			{
				SetLayerSelectedButtonColors(ToggleButton, 4);
				SetLayerSelectedCheckState(4);
			}
			else if (LayerDToggle.IsChecked.Value)
			{
				SetLayerSelectedButtonColors(ToggleButton, 5);
				SetLayerSelectedCheckState(5);
			}
			else
			{
				SetLayerSelectedButtonColors(ToggleButton, 0);
				SetLayerSelectedCheckState(0);
			}
		}

		protected void SetLayerSelectedCheckState(int mode)
		{
			// 0 - Not Checked
			// 1 - Checked
			// 2 - Checked (Edit Layer A)
			// 3 - Checked (Edit Layer B)
			// 4 - Checked (Edit Layer C)
			// 5 - Checked (Edit Layer D)

			switch (mode)
			{
				case 0:
					SetAsEditLayerA.IsChecked = false;
					SetAsEditLayerB.IsChecked = false;
					SetAsEditLayerC.IsChecked = false;
					SetAsEditLayerD.IsChecked = false;
					break;
				case 1:
					SetAsEditLayerA.IsChecked = false;
					SetAsEditLayerB.IsChecked = false;
					SetAsEditLayerC.IsChecked = false;
					SetAsEditLayerD.IsChecked = false;
					break;
				case 2:
					SetAsEditLayerA.IsChecked = true;
					SetAsEditLayerB.IsChecked = false;
					SetAsEditLayerC.IsChecked = false;
					SetAsEditLayerD.IsChecked = false;
					break;
				case 3:
					SetAsEditLayerA.IsChecked = false;
					SetAsEditLayerB.IsChecked = true;
					SetAsEditLayerC.IsChecked = false;
					SetAsEditLayerD.IsChecked = false;
					break;
				case 4:
					SetAsEditLayerA.IsChecked = false;
					SetAsEditLayerB.IsChecked = false;
					SetAsEditLayerC.IsChecked = true;
					SetAsEditLayerD.IsChecked = false;
					break;
				case 5:
					SetAsEditLayerA.IsChecked = false;
					SetAsEditLayerB.IsChecked = false;
					SetAsEditLayerC.IsChecked = false;
					SetAsEditLayerD.IsChecked = true;
					break;
			}
		}

		protected void SetLayerSelectedButtonColors(Button toggle, int mode = 0)
		{
			// 0 - Not Checked
			// 1 - Checked
			// 2 - Checked (Edit Layer A)
			// 3 - Checked (Edit Layer B)
			// 4 - Checked (Edit Layer C)
			// 5 - Checked (Edit Layer D)

			switch (mode)
			{
				case 0:
					toggle.Background = System.Windows.Media.Brushes.Transparent;
					toggle.BorderBrush = System.Windows.Media.Brushes.Transparent;
					break;
				case 1:
					toggle.Background = (SolidColorBrush)FindResource("EditLayerSelectedColorBack");
					toggle.BorderBrush = (SolidColorBrush)FindResource("EditLayerSelectedColor");
					break;
				case 2:
					toggle.Background = (SolidColorBrush)FindResource("EditLayerASelectedColorBack");
					toggle.BorderBrush = (SolidColorBrush)FindResource("EditLayerASelectedColor");
					break;
				case 3:
					toggle.Background = (SolidColorBrush)FindResource("EditLayerBSelectedColorBack");
					toggle.BorderBrush = (SolidColorBrush)FindResource("EditLayerBSelectedColor");
					break;
				case 4:
					toggle.Background = (SolidColorBrush)FindResource("EditLayerCSelectedColorBack");
					toggle.BorderBrush = (SolidColorBrush)FindResource("EditLayerCSelectedColor");
					break;
				case 5:
					toggle.Background = (SolidColorBrush)FindResource("EditLayerDSelectedColorBack");
					toggle.BorderBrush = (SolidColorBrush)FindResource("EditLayerDSelectedColor");
					break;
			}
		}

		protected void ToggleButton_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (DualSelect)
			{
				if (e.ChangedButton == MouseButton.Left && !Keyboard.IsKeyDown(Key.LeftCtrl))
				{
					LayerAToggle.IsChecked = !LayerAToggle.IsChecked.Value;
					if (LayerBToggle.IsChecked.Value) LayerBToggle.IsChecked = false;
					if (LayerCToggle.IsChecked.Value) LayerCToggle.IsChecked = false;
					if (LayerDToggle.IsChecked.Value) LayerDToggle.IsChecked = false;
					this.Click(this, e);
				}
				else if (e.ChangedButton == MouseButton.Right)
				{
					LayerBToggle.IsChecked = !LayerBToggle.IsChecked.Value;
					if (LayerAToggle.IsChecked.Value) LayerAToggle.IsChecked = false;
					if (LayerCToggle.IsChecked.Value) LayerCToggle.IsChecked = false;
					if (LayerDToggle.IsChecked.Value) LayerDToggle.IsChecked = false;
					this.RightClick(this, e);
				}
            }
			else
			{
				if (e.ChangedButton == MouseButton.Left && !Keyboard.IsKeyDown(Key.LeftCtrl))
				{
					LayerToggle.IsChecked = !LayerToggle.IsChecked.Value;
					this.Click(this, e);
				}
            }
			LayerToggle_Unchecked(null, null);

		}

		protected void ToggleButton_Click(object sender, RoutedEventArgs e)
		{
			if (DualSelect && !Keyboard.IsKeyDown(Key.LeftCtrl))
			{
				LayerAToggle.IsChecked = !LayerAToggle.IsChecked.Value;
				if (LayerBToggle.IsChecked.Value) LayerBToggle.IsChecked = false;
				if (LayerCToggle.IsChecked.Value) LayerCToggle.IsChecked = false;
				if (LayerDToggle.IsChecked.Value) LayerDToggle.IsChecked = false;
				if (this.Click != null) this.Click(this, e);
			}
            else if (!Keyboard.IsKeyDown(Key.LeftCtrl))
			{
				LayerToggle.IsChecked = !LayerToggle.IsChecked.Value;
				if (this.Click != null) this.Click(this, e);
			}
			LayerToggle_Unchecked(null, null);
		}

		protected void ToggleButton_MouseLeave(object sender, MouseEventArgs e)
		{
            UpdateMouseOver();
            if (LayerToggle.IsChecked.Value) SetLayerSelectedButtonColors(ToggleButton, 1);
			else if (LayerAToggle.IsChecked.Value) SetLayerSelectedButtonColors(ToggleButton, 2);
			else if (LayerBToggle.IsChecked.Value) SetLayerSelectedButtonColors(ToggleButton, 3);
			else if (LayerCToggle.IsChecked.Value) SetLayerSelectedButtonColors(ToggleButton, 4);
			else if (LayerDToggle.IsChecked.Value) SetLayerSelectedButtonColors(ToggleButton, 5);
			else SetLayerSelectedButtonColors(ToggleButton, 0);

		}

        private void ToggleButton_MouseMove(object sender, MouseEventArgs e)
        {
            UpdateMouseOver();
        }

        private void UpdateMouseOver()
        {
            //if ((ToggleButton.IsMouseOver || LayerOptionsDropdownButton.IsMouseOver) && IsLayerOptionsEnabled) LayerOptionsDropdownButton.Visibility = Visibility.Visible;
            //else LayerOptionsDropdownButton.Visibility = Visibility.Collapsed;
        }

        private void LayerOptionsDropdownButton_Opened_1(object sender, RoutedEventArgs e)
        {

        }


		private void SetEditLayerState(char Layer, RoutedEventArgs e)
		{
			if (Layer == 'A')
			{
				LayerAToggle.IsChecked = !LayerAToggle.IsChecked.Value;
				if (LayerBToggle.IsChecked.Value) LayerBToggle.IsChecked = false;
				if (LayerCToggle.IsChecked.Value) LayerCToggle.IsChecked = false;
				if (LayerDToggle.IsChecked.Value) LayerDToggle.IsChecked = false;
				if (this.Click != null) this.Click(this, e);
			}
			else if (Layer == 'B')
			{
				LayerBToggle.IsChecked = !LayerBToggle.IsChecked.Value;
				if (LayerAToggle.IsChecked.Value) LayerAToggle.IsChecked = false;
				if (LayerCToggle.IsChecked.Value) LayerCToggle.IsChecked = false;
				if (LayerDToggle.IsChecked.Value) LayerDToggle.IsChecked = false;
				if (this.Click != null) this.Click(this, e);
			}
			else if (Layer == 'C')
			{
				LayerCToggle.IsChecked = !LayerCToggle.IsChecked.Value;
				if (LayerAToggle.IsChecked.Value) LayerAToggle.IsChecked = false;
				if (LayerBToggle.IsChecked.Value) LayerBToggle.IsChecked = false;
				if (LayerDToggle.IsChecked.Value) LayerDToggle.IsChecked = false;
				if (this.Click != null) this.Click(this, e);
			}
			else
			{
				LayerDToggle.IsChecked = !LayerDToggle.IsChecked.Value;
				if (LayerAToggle.IsChecked.Value) LayerAToggle.IsChecked = false;
				if (LayerCToggle.IsChecked.Value) LayerCToggle.IsChecked = false;
				if (LayerBToggle.IsChecked.Value) LayerBToggle.IsChecked = false;
				if (this.Click != null) this.Click(this, e);
			}
			LayerToggle_Unchecked(null, null);

		}


		private void SetAsEditLayerA_Click(object sender, RoutedEventArgs e)
		{
			SetEditLayerState('A', e);
		}

		private void SetAsEditLayerB_Click(object sender, RoutedEventArgs e)
		{
			SetEditLayerState('B', e);
		}

		private void SetAsEditLayerC_Click(object sender, RoutedEventArgs e)
		{
			SetEditLayerState('C', e);
		}

		private void SetAsEditLayerD_Click(object sender, RoutedEventArgs e)
		{
			SetEditLayerState('D', e);
		}
	}

}
