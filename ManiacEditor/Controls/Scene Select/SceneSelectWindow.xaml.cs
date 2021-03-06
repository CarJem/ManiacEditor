﻿using System.Windows;

namespace ManiacEditor.Controls.SceneSelect
{
    /// <summary>
    /// Interaction logic for SceneSelectWindow.xaml
    /// </summary>
    public partial class SceneSelectWindow : Window
	{
		public Controls.SceneSelect.SceneSelectHost SceneSelect;
		public SceneSelectWindow(Controls.Editor.MainEditor instance = null, bool selectDirectory = false, bool isSceneLoad = true)
		{
			InitializeComponent();
			SceneSelect = new Controls.SceneSelect.SceneSelectHost(instance, this, isSceneLoad);
			FrameHost.Children.Add(SceneSelect);
			if (selectDirectory)
			{
				SceneSelect.AddANewDataDirectory();
			}
		}
	}
}
