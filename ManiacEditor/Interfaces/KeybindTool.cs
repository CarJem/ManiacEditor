using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Specialized;

namespace ManiacEditor.Interfaces
{
    public partial class KeybindTool : Form
    {
		List<string> KeyBindsList = new List<string>();
        public Keys CurrentBindingKey = Keys.None;
		public bool ctrlChecked = false;
		public bool altChecked = false;
		public bool tabChecked = false;
		public bool shiftChecked = false;

		public const string ctrl = "CTRL";
		public const string shift = "SHIFT";
		public const string alt = "ALT";
		public const string plus = " + ";

        public KeybindTool(string keyRefrence)
        {
            InitializeComponent();
            UpdateResultLabel();
			SetupExistingKeybinds(keyRefrence);
        }

		private void SetupExistingKeybinds(string keyRefrence)
		{
			KeysConverter kc = new KeysConverter();

			var keybindDict = Settings.myKeyBinds[keyRefrence] as StringCollection;
			if (keybindDict != null) KeyBindsList = keybindDict.Cast<string>().ToList();

			Keys? BindKey = (Keys)kc.ConvertFromString(KeyBindsList[0].ToString());

			if (BindKey != null) CurrentBindingKey = BindKey.Value;

			UpdateResultLabel();
		}


		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
			Keys keyData = e.KeyData;
			CurrentBindingKey = keyData;
			textBox1.Text = "";
			UpdateResultLabel();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.Text = CurrentBindingKey.ToString();
        }

        private void UpdateResultLabel()
        {
            //resultLabel.Text = CurrentBindingKey.ToString();
			textBox1.Text = CurrentBindingKey.ToString();

		}
    }
}
