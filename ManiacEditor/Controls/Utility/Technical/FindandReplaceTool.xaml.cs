using System.Windows;

namespace ManiacEditor.Controls.Utility
{
    /// <summary>
    /// Interaction logic for FindandReplaceTool.xaml
    /// </summary>
    public partial class FindandReplaceTool : Window
	{
		int FindValue = 0;
		int ReplaceValue = 0;
		bool replaceMode = false;
		bool copyResults = false;
		int applyToState = 0;
		string specificLayer = "";
		bool readyState = false;
		bool perserveOptions = false;

		public FindandReplaceTool()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			if (replaceModeCheckBox.IsChecked.Value)
			{
				replaceMode = true;
			}
			if (clipboardResultsCheckbox.IsChecked.Value)
			{
				copyResults = true;
			}
			if (allLayersRadio.IsChecked.Value)
			{
				applyToState = 1; //All Layers
			}
			if (specificLayerRadio.IsChecked.Value)
			{
				applyToState = 2; //Specific Layer
			}
			if (activeLayerRadio.IsChecked.Value)
			{
				applyToState = 0; //Active Edit Layer Only
			}
			//if (textBox3.Text != "") (To Reimplement)
			//{
			//	textBox3.Text = specificLayer;
			//}
			if (preserveCollisionCheckbox.IsChecked.Value)
			{
				perserveOptions = true;
			}
			readyState = true;
			DialogResult = true;
		}

		public int GetFindValue()
		{
			return FindValue;
		}
		public bool GetReadyState()
		{
			return readyState;
		}
		public int GetReplaceValue()
		{
			return ReplaceValue;
		}

		public string GetSpecificLayer()
		{
			return specificLayer;
		}

		public bool IsReplaceMode()
		{
			return replaceMode;
		}

		public bool CopyResultsOrNot()
		{
			return copyResults;
		}

		public int GetApplyState()
		{
			return applyToState;
		}

		public bool PerservingCollision()
		{
			return perserveOptions;
		}

		private void radioButton2_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (specificLayerRadio.IsChecked.Value)
			{
				specificLayerSplitBox.IsEnabled = true;
			}
			else
			{
				specificLayerSplitBox.IsEnabled = false;
			}
		}

		private void checkBox1_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (replaceModeCheckBox.IsChecked.Value)
			{
				ReplaceNUD.IsEnabled = true;
				ReplaceLabel.IsEnabled = true;
			}
			else
			{
				ReplaceNUD.IsEnabled = false;
				ReplaceLabel.IsEnabled = false;
			}
		}

		private void checkBox6_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (enableCollisionCheckbox.IsChecked.Value)
			{
				optionBox1.IsEnabled = true;
				optionBox2.IsEnabled = true;
				optionBox3.IsEnabled = true;
				optionBox4.IsEnabled = true;
				optionBox5.IsEnabled = true;
				optionBox6.IsEnabled = true;
			}
			else
			{
				optionBox1.IsEnabled = false;
				optionBox2.IsEnabled = false;
				optionBox3.IsEnabled = false;
				optionBox4.IsEnabled = false;
				optionBox5.IsEnabled = false;
				optionBox6.IsEnabled = false;
			}
		}

		private void FindNUD_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			FindValue = (int)FindNUD.Value;
		}

		private void ReplaceNUD_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			ReplaceValue = (int)ReplaceNUD.Value;
		}


	}
}
