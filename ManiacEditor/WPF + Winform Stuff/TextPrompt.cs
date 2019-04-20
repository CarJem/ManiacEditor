using System.Windows.Forms;

namespace ManiacEditor
{
    public static class TextPrompt
    {
        public static string ShowDialog(string caption, string text, string preInput = "")
        {
            Form prompt = new Form();
            prompt.Width = 500;
            prompt.Height = 150;
            prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
            prompt.Text = caption;
            prompt.StartPosition = FormStartPosition.CenterParent;
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text, AutoSize = true };
            TextBox inputBox = new TextBox() { Left = 50, Top = 50, Width = 400, BackColor = System.Drawing.Color.White, ForeColor = System.Drawing.Color.Black, Text = preInput };
            Button confirmation = new Button() { Text = "OK", Left = 350, Width = 100, Top = 70, ForeColor = System.Drawing.Color.Black };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(inputBox);
            prompt.ShowDialog();
            return inputBox.Text;
        }
    }
}
