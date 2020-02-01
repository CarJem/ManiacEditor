using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ManiacEditor
{
    /// <summary>
    /// Interaction logic for EditLayerToggleButton.xaml
    /// </summary>
    public partial class EditLayerToggleButton : UserControl
	{
        public string LayerName { get; set; }

        #region Layer Options
        public bool ShowLayerScrollLines { get => GetShowLayerScrollLines(); set => SetShowLayerScrollLines(value); }
        private bool _ShowLayerScrollLines = false;

        private bool GetShowLayerScrollLines()
        {
            try
            {
                if (Classes.Edit.Scene.Solution.CurrentScene.AllLayers.Any(x => x.Name == Text))
                {
                    var sourceLayer = Classes.Edit.Scene.Solution.CurrentScene.AllLayers.Single(el => el.Name.Equals(Text));
                    _ShowLayerScrollLines = Classes.Edit.Scene.Solution.CurrentScene.AllLayers.Single(el => el.Name.Equals(Text)).ShowLayerScrollLines;
                    ShowLayerScrollDetails.IsChecked = _ShowLayerScrollLines;
                    return _ShowLayerScrollLines;
                }
                else
                {
                    _ShowLayerScrollLines = false;
                    ShowLayerScrollDetails.IsChecked = _ShowLayerScrollLines;
                    return _ShowLayerScrollLines;
                }

            }
            catch
            {
                _ShowLayerScrollLines = false;
                ShowLayerScrollDetails.IsChecked = _ShowLayerScrollLines;
                return _ShowLayerScrollLines;
            }
        }


        private void SetShowLayerScrollLines(bool value)
        {
            Classes.Edit.Scene.Solution.CurrentScene.AllLayers.FirstOrDefault(x => x.Name == Text).ShowLayerScrollLines = value;
            _ShowLayerScrollLines = Classes.Edit.Scene.Solution.CurrentScene.AllLayers.FirstOrDefault(x => x.Name == Text).ShowLayerScrollLines;
            ShowLayerScrollDetails.IsChecked = _ShowLayerScrollLines;
        }


        public bool AllowParallaxAnimation { get => GetAllowParallaxAnimation(); set => SetAllowParallaxAnimation(value); }
        private bool _AllowParallaxAnimation = false;

        private bool GetAllowParallaxAnimation()
        {
            try
            {
                if (Classes.Edit.Scene.Solution.CurrentScene.AllLayers.Any(x => x.Name == Text))
                {
                    var sourceLayer = Classes.Edit.Scene.Solution.CurrentScene.AllLayers.Single(el => el.Name.Equals(Text));
                    _AllowParallaxAnimation = Classes.Edit.Scene.Solution.CurrentScene.AllLayers.Single(el => el.Name.Equals(Text)).AllowLayerToAnimateParallax;
                    AllowLayerToAnimateParallax.IsChecked = _AllowParallaxAnimation;
                    return _AllowParallaxAnimation;
                }
                else
                {
                    _AllowParallaxAnimation = false;
                    AllowLayerToAnimateParallax.IsChecked = _AllowParallaxAnimation;
                    return _AllowParallaxAnimation;
                }

            }
            catch
            {
                _AllowParallaxAnimation = false;
                AllowLayerToAnimateParallax.IsChecked = _AllowParallaxAnimation;
                return _AllowParallaxAnimation;
            }
        }


        private void SetAllowParallaxAnimation(bool value)
        {
            Classes.Edit.Scene.Solution.CurrentScene.AllLayers.FirstOrDefault(x => x.Name == Text).AllowLayerToAnimateParallax = value;
            _AllowParallaxAnimation = Classes.Edit.Scene.Solution.CurrentScene.AllLayers.FirstOrDefault(x => x.Name == Text).AllowLayerToAnimateParallax;
            if (value == true) Classes.Edit.Scene.Solution.CurrentScene.AllLayers.FirstOrDefault(x => x.Name == Text).HasHorizontalLayerScrollInitilized = false;
            AllowLayerToAnimateParallax.IsChecked = _AllowParallaxAnimation;
        }
        #endregion



        public bool IsLayerOptionsEnabled { get => GetLayerOptionsEnabled(); set => SetLayerOptionsEnabled(value); }
        private bool _IsLayerOptionsEnabled = false;

        public bool GetLayerOptionsEnabled()
        {
            var item = (LayerOptionsDropdownButton.Template.FindName("ToggleButtonChrome", LayerOptionsDropdownButton) as Xceed.Wpf.Toolkit.Chromes.ButtonChrome);
            if (item != null) item.IsEnabled = _IsLayerOptionsEnabled;
            return _IsLayerOptionsEnabled;
        }

        private void SetLayerOptionsEnabled(bool value)
        {
            var item = (LayerOptionsDropdownButton.Template.FindName("ToggleButtonChrome", LayerOptionsDropdownButton) as Xceed.Wpf.Toolkit.Chromes.ButtonChrome);
            if (item != null) item.IsEnabled = value;
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
			set { base.SetValue(DualSelectMode, value); }
		}

		public bool? IsCheckedA
		{
			get { return LayerAToggle.IsChecked; }
			set { LayerAToggle.IsChecked = value; }
		}

		public bool IsCheckedAll
		{
			get { return (LayerAToggle.IsChecked.Value || LayerToggle.IsChecked.Value || LayerBToggle.IsChecked.Value); }
		}

		public bool DualSelect
		{
			get { return DualSelectModeEnabled; }
			set { DualSelectModeEnabled = value;}
		}

		public bool? IsCheckedN
		{
			get { return LayerToggle.IsChecked; }
			set { LayerToggle.IsChecked = value; }
		}

		public Brush TextForeground
		{
			get { return (Brush)GetValue(TextForegroundColor);}
			set { SetValue(TextForegroundColor, value); }
		}

		public bool? IsCheckedB
		{
			get { return LayerBToggle.IsChecked; }
			set { LayerBToggle.IsChecked = value; }
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

		public static readonly DependencyProperty DualSelectMode =
DependencyProperty.Register("DualSelectModeEnabled", typeof(bool), typeof(EditLayerToggleButton), new UIPropertyMetadata(false));

		public EditLayerToggleButton()
		{
			InitializeComponent();
			DataContext = this;


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
			else SetLayerSelectedButtonColors(ToggleButton, 0);
		}

		private void LayerToggle_Unchecked(object sender, RoutedEventArgs e)
		{
			if (LayerToggle.IsChecked.Value) SetLayerSelectedButtonColors(ToggleButton, 1);
			else if (LayerAToggle.IsChecked.Value) SetLayerSelectedButtonColors(ToggleButton, 2);
			else if (LayerBToggle.IsChecked.Value) SetLayerSelectedButtonColors(ToggleButton, 3);
			else SetLayerSelectedButtonColors(ToggleButton, 0);
		}

		protected void SetLayerSelectedButtonColors(Button toggle, int mode = 0)
		{
			// 0 - Not Checked
			// 1 - Checked
			// 2 - Checked (Edit Layer A)
			// 3 - Checked (Edit Layer B)

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
					this.Click(this, e);
				}
				else if (e.ChangedButton == MouseButton.Right)
				{
					LayerBToggle.IsChecked = !LayerBToggle.IsChecked.Value;
					if (LayerAToggle.IsChecked.Value) LayerAToggle.IsChecked = false;
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


		}

		protected void ToggleButton_Click(object sender, RoutedEventArgs e)
		{
			if (DualSelect && !Keyboard.IsKeyDown(Key.LeftCtrl))
			{
				LayerAToggle.IsChecked = !LayerAToggle.IsChecked.Value;
				if (LayerBToggle.IsChecked.Value) LayerBToggle.IsChecked = false;
				if (this.Click != null) this.Click(this, e);
			}
            else if (!Keyboard.IsKeyDown(Key.LeftCtrl))
			{
				LayerToggle.IsChecked = !LayerToggle.IsChecked.Value;
				if (this.Click != null) this.Click(this, e);
			}

        }

		protected void ToggleButton_MouseLeave(object sender, MouseEventArgs e)
		{
            UpdateMouseOver();
            if (LayerToggle.IsChecked.Value) SetLayerSelectedButtonColors(ToggleButton, 1);
			else if (LayerAToggle.IsChecked.Value) SetLayerSelectedButtonColors(ToggleButton, 2);
			else if (LayerBToggle.IsChecked.Value) SetLayerSelectedButtonColors(ToggleButton, 3);
			else SetLayerSelectedButtonColors(ToggleButton, 0);
		}

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ShowLayerScrollDetails.IsChecked) ShowLayerScrollLines = true;
            else ShowLayerScrollLines = false;
        }

        private void ToggleButton_MouseMove(object sender, MouseEventArgs e)
        {
            UpdateMouseOver();
        }

        private void UpdateMouseOver()
        {
            if ((ToggleButton.IsMouseOver || LayerOptionsDropdownButton.IsMouseOver) && IsLayerOptionsEnabled) LayerOptionsDropdownButton.Visibility = Visibility.Visible;
            else LayerOptionsDropdownButton.Visibility = Visibility.Collapsed;
        }

        private void AllowLayerToAnimateParallax_Click(object sender, RoutedEventArgs e)
        {
            if (AllowLayerToAnimateParallax.IsChecked) AllowParallaxAnimation = true;
            else AllowParallaxAnimation = false;
        }

        private void LayerOptionsDropdownButton_Opened_1(object sender, RoutedEventArgs e)
        {
            GetShowLayerScrollLines();
            GetAllowParallaxAnimation();
        }
    }

}
