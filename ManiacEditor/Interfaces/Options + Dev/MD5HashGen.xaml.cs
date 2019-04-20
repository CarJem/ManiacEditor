using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ManiacEditor.Interfaces.WPF_UI.Options___Dev
{
    /// <summary>
    /// Interaction logic for MD5HashGen.xaml
    /// </summary>
    public partial class MD5HashGen : Window
	{

		public MD5HashGen(Editor instance)
		{
			InitializeComponent();
		}

		private void richTextBox1_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (GetRichText() != "")
			{
				SetRichText(CreateMD5(GetRichText()));
			}
			else
			{
				SetRichText("");
			}

		}

		public void SetRichText(string text)
		{
			richTextBox1.Document.Blocks.Clear();
			richTextBox1.Document.Blocks.Add(new Paragraph(new Run(text)));
		}

		public string GetRichText()
		{
			return new TextRange(richTextBox1.Document.ContentStart, richTextBox1.Document.ContentEnd).Text;
		}

		public static string CreateMD5(string input)
		{
			// Use input string to calculate MD5 hash
			using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
			{
				byte[] inputBytes = Encoding.ASCII.GetBytes(input);
				byte[] hashBytes = md5.ComputeHash(inputBytes);

				// Convert the byte array to hexadecimal string
				StringBuilder sb = new StringBuilder();
				foreach (byte b in Invert(hashBytes))
					sb.Append($"{b:X2}");

				return sb.ToString();
			}
		}

		/// <summary>
		/// Swaps nibbles position of a byte Array, Useful for Sonic Mania
		/// e.g IN: 0CBC OUT: C0CB
		/// </summary>
		/// <param name="b">Hash Array</param>
		/// <returns>Swaped IEnumerable</returns>
		public static IEnumerable<byte> Invert(byte[] b)
		{
			return b.Select(item => (byte)((item << 4) | (item >> 4)));
		}
	}
}
