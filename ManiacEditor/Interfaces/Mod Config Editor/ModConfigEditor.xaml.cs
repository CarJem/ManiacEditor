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
using Path = System.IO.Path;
using System.ComponentModel;
using MessageBox = RSDKrU.MessageBox;


namespace ManiacEditor
{
	/// <summary>
	/// Interaction logic for ModConfigEditor.xaml
	/// </summary>
	public partial class ModConfigEditor : Window
	{
		bool isEditing = false;
		string originalName = "";

		string addConfigContents = null;
		string addConfigName = null;


		public ModConfigEditor(bool edit = false, int editIndex = 0)
		{
			InitializeComponent();
			if (edit == true)
			{
				richTextBox1.AppendText(Properties.Settings.Default.modConfigs[editIndex]);
				textBox1.Text = Properties.Settings.Default.modConfigsNames[editIndex];
				textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 4);
				originalName = textBox1.Text;
				isEditing = true;
			}
		}

		private void ModConfigCreator_Load(object sender, RoutedEventArgs e)
		{

		}

		private void create_Click(object sender, RoutedEventArgs e)
		{
			bool cancel = false;
			string richTextBox1Text = new TextRange(richTextBox1.Document.ContentStart, richTextBox1.Document.ContentEnd).Text;
			if (textBox1.Text == "" || textBox1.Text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
			{
				MessageBox.Show("You must type a valid file name!");
				cancel = true;
			}
			if (richTextBox1Text == "")
			{
				MessageBox.Show("You must have some content in your config!");
				cancel = true;
			}
			if (cancel == false)
			{
				if (isEditing == false)
				{
					addConfigContents = new TextRange(richTextBox1.Document.ContentStart, richTextBox1.Document.ContentEnd).Text;
					addConfigName = this.textBox1.Text;
					string file = Environment.CurrentDirectory + "\\Config\\" + addConfigName + ".ini";
					string directory = Environment.CurrentDirectory + "\\Config";
					DirectoryInfo di = Directory.CreateDirectory(directory);
					File.Create(file).Close();
					File.WriteAllText(file, addConfigContents);
					this.DialogResult = true;
				}
				else
				{
					addConfigContents = new TextRange(richTextBox1.Document.ContentStart, richTextBox1.Document.ContentEnd).Text;
					addConfigName = this.textBox1.Text;
					String fileNew = Environment.CurrentDirectory + "\\Config\\" + addConfigName + ".ini";
					if (addConfigName != originalName)
					{
						String fileOld = Environment.CurrentDirectory + "\\Config\\" + originalName + ".ini";
						File.Move(fileOld, fileNew);
					}
					File.WriteAllText(fileNew, addConfigContents);
					this.DialogResult = true;
				}

			}
		}

		private void cancel_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void ModConfigCreator_HelpButtonClicked(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("This config file will replace ManiaModLoader.ini when it is used, so look to that if you want to know what to input!");
		}

		private void import_Click(object sender, RoutedEventArgs e)
		{
			var ofd = new System.Windows.Forms.OpenFileDialog();
			ofd.Title = "Select ManiaModLoader.ini";
			ofd.Filter = "Windows PE Executable|*.ini";
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				string fileName = ofd.FileName;
				string fileContents = File.ReadAllText(fileName);
				richTextBox1.Document.Blocks.Clear();
				richTextBox1.AppendText(fileContents);
			}
		}
	}

}
