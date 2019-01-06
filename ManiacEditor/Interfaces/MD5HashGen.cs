using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManiacEditor
{
    public partial class MD5HashGen : Form
    {

        public MD5HashGen(Editor instance)
        {
            InitializeComponent();
            this.Owner = instance;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (richTextBox1.Text != "")
            {
                richTextBox2.Text = CreateMD5(richTextBox1.Text);
            }
            else
            {
                richTextBox2.Text = "";
            }

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
