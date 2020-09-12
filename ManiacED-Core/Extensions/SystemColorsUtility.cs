using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Control = System.Windows.Forms.Control;
using Color = System.Drawing.Color;
using MenuItem = System.Windows.Controls.MenuItem;
using Button = System.Windows.Controls.Button;
using ManiacEditor.Extensions;

namespace ManiacEditor.Extensions
{
    public class SystemColorsUtility
    {
        public SystemColorsUtility()
        {
            // force init color table
            byte unused = SystemColors.Window.R;

            var colorTableField = typeof(Color).Assembly.GetType("System.Drawing.KnownColorTable")
                .GetField("colorTable", BindingFlags.Static | BindingFlags.NonPublic);

            _colorTable = (int[])colorTableField.GetValue(null);
        }

        public void SetColor(KnownColor knownColor, Color value)
        {
            _colorTable[(int)knownColor] = value.ToArgb();
        }

        private readonly int[] _colorTable;
    }
}
