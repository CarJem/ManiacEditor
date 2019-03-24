using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Reflection;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Control = System.Windows.Forms.Control;
using Color = System.Drawing.Color;
using MenuItem = System.Windows.Controls.MenuItem;
using DataObject = System.Windows.DataObject;
using Button = System.Windows.Controls.Button;
using Cursors = System.Windows.Input.Cursors;
using RSDKrU;
using MessageBox = RSDKrU.MessageBox;

namespace ManiacEditor
{
    public class EditorTheming
    {
        private Editor Instance;
        //Dark Theme
        public static Color darkTheme0 = Color.FromArgb(255, 40, 40, 40);
        public static Color darkTheme1 = Color.FromArgb(255, 50, 50, 50);
        public static Color darkTheme2 = Color.FromArgb(255, 70, 70, 70);
        public static Color darkTheme3 = Color.White;
        public static Color darkTheme4 = Color.FromArgb(255, 49, 162, 247);
        public static Color darkTheme5 = Color.FromArgb(255, 80, 80, 80);

        public EditorTheming(Editor instance)
        {
            Instance = instance;
        }

        #region Theming Stuff
        public void UseDarkTheme(bool state = false)
        {
            if (state)
            {
                SystemColorsUtility systemColors = new SystemColorsUtility();
                systemColors.SetColor(KnownColor.Window, darkTheme1);
                systemColors.SetColor(KnownColor.Highlight, Color.Blue);
                systemColors.SetColor(KnownColor.WindowFrame, darkTheme2);
                systemColors.SetColor(KnownColor.GradientActiveCaption, darkTheme1);
                systemColors.SetColor(KnownColor.GradientInactiveCaption, darkTheme1);
                systemColors.SetColor(KnownColor.ControlText, darkTheme3);
                systemColors.SetColor(KnownColor.WindowText, darkTheme3);
                systemColors.SetColor(KnownColor.GrayText, Color.Gray);
                systemColors.SetColor(KnownColor.InfoText, darkTheme3);
                systemColors.SetColor(KnownColor.MenuText, darkTheme3);
                systemColors.SetColor(KnownColor.Control, darkTheme1);
                systemColors.SetColor(KnownColor.ButtonHighlight, darkTheme3);
                systemColors.SetColor(KnownColor.ButtonShadow, darkTheme2);
                systemColors.SetColor(KnownColor.ButtonFace, darkTheme1);
                systemColors.SetColor(KnownColor.Desktop, darkTheme1);
                systemColors.SetColor(KnownColor.ControlLightLight, darkTheme2);
                systemColors.SetColor(KnownColor.ControlLight, darkTheme1);
                systemColors.SetColor(KnownColor.ControlDark, darkTheme3);
                systemColors.SetColor(KnownColor.ControlDarkDark, darkTheme3);
                systemColors.SetColor(KnownColor.ActiveBorder, darkTheme1);
                systemColors.SetColor(KnownColor.ActiveCaption, darkTheme1);
                systemColors.SetColor(KnownColor.ActiveCaptionText, darkTheme3);
                systemColors.SetColor(KnownColor.InactiveBorder, darkTheme2);
                systemColors.SetColor(KnownColor.MenuBar, darkTheme1);
            }
            else
            {
                SystemColorsUtility systemColors = new SystemColorsUtility();
                systemColors.SetColor(KnownColor.Window, SystemColors.Window);
                systemColors.SetColor(KnownColor.Highlight, SystemColors.Highlight);
                systemColors.SetColor(KnownColor.WindowFrame, SystemColors.WindowFrame);
                systemColors.SetColor(KnownColor.GradientActiveCaption, SystemColors.GradientActiveCaption);
                systemColors.SetColor(KnownColor.GradientInactiveCaption, SystemColors.GradientInactiveCaption);
                systemColors.SetColor(KnownColor.ControlText, SystemColors.ControlText);
                systemColors.SetColor(KnownColor.WindowText, SystemColors.WindowText);
                systemColors.SetColor(KnownColor.GrayText, SystemColors.GrayText);
                systemColors.SetColor(KnownColor.InfoText, SystemColors.InfoText);
                systemColors.SetColor(KnownColor.MenuText, SystemColors.MenuText);
                systemColors.SetColor(KnownColor.Control, SystemColors.Control);
                systemColors.SetColor(KnownColor.ButtonHighlight, SystemColors.ButtonHighlight);
                systemColors.SetColor(KnownColor.ButtonShadow, SystemColors.ButtonShadow);
                systemColors.SetColor(KnownColor.ButtonFace, SystemColors.ButtonFace);
                systemColors.SetColor(KnownColor.Desktop, SystemColors.Desktop);
                systemColors.SetColor(KnownColor.ControlLightLight, SystemColors.ControlLightLight);
                systemColors.SetColor(KnownColor.ControlLight, SystemColors.ControlLight);
                systemColors.SetColor(KnownColor.ControlDark, SystemColors.ControlDark);
                systemColors.SetColor(KnownColor.ControlDarkDark, SystemColors.ControlDarkDark);
                systemColors.SetColor(KnownColor.ActiveBorder, SystemColors.ActiveBorder);
                systemColors.SetColor(KnownColor.ActiveCaption, SystemColors.ActiveCaption);
                systemColors.SetColor(KnownColor.ActiveCaptionText, SystemColors.ActiveCaptionText);
                systemColors.SetColor(KnownColor.InactiveBorder, SystemColors.InactiveBorder);
                systemColors.SetColor(KnownColor.MenuBar, SystemColors.MenuBar);
            }

        }
        public void UseDarkTheme_WPF(bool state = false)
        {
            if (state)
            {
                App.ChangeSkin(Skin.Dark);
                UseDarkTheme(true);
            }
            else
            {
                App.ChangeSkin(Skin.Light);
                UseDarkTheme(false);
            }
        }
        public Control UseExternalDarkTheme(Control control)
        {
            foreach (Control c in control.Controls)
            {
                if (c is Cyotek.Windows.Forms.ColorEditor)
                {
                    foreach (Control c2 in c.Controls)
                    {
                        if (c2 is System.Windows.Forms.NumericUpDown)
                        {
                            c2.ForeColor = Color.Black;
                            c2.BackColor = Color.White;
                        }
                        if (c2 is System.Windows.Forms.ComboBox)
                        {
                            c2.ForeColor = Color.Black;
                            c2.BackColor = Color.White;
                        }
                    }
                }

                if (c is System.Windows.Forms.Button)
                {
                    c.ForeColor = Color.Black;
                }
                if (c is NumericUpDown)
                {
                    c.ForeColor = Color.Black;
                    c.BackColor = Color.White;
                }
                if (c is System.Windows.Forms.ComboBox)
                {
                    c.ForeColor = Color.Black;
                    c.BackColor = Color.White;
                }
                if (c is System.Windows.Forms.TextBox)
                {
                    c.ForeColor = Color.Black;
                    c.BackColor = Color.White;
                }
            }
            return control;
        }
        public void SetButtonColors(object sender, Color OverallColor)
        {
            if (sender is ToggleButton)
            {

                var item = (sender as ToggleButton);
                if (item == null) return;
                if (item.Content == null) return;
                var objContent = (sender as ToggleButton).Content;
                if (objContent == null) return;
                if (objContent is System.Windows.Shapes.Rectangle)
                {
                    System.Windows.Shapes.Rectangle content = objContent as System.Windows.Shapes.Rectangle;
                    Color DisabledOpacity = Color.FromArgb(128, 0, 0, 0);
                    System.Windows.Media.Color ConvertedColor = System.Windows.Media.Color.FromArgb((item.IsEnabled ? OverallColor.A : DisabledOpacity.A), OverallColor.R, OverallColor.G, OverallColor.B);
                    content.Fill = new SolidColorBrush(ConvertedColor);

                }


            }

            if (sender is Button)
            {

                var item = (sender as Button);
                if (item == null) return;
                if (item.Content == null) return;
                var objContent = (sender as Button).Content;
                if (objContent == null) return;
                if (objContent is System.Windows.Shapes.Rectangle)
                {
                    System.Windows.Shapes.Rectangle content = objContent as System.Windows.Shapes.Rectangle;
                    Color DisabledOpacity = Color.FromArgb(128, 0, 0, 0);
                    System.Windows.Media.Color ConvertedColor = System.Windows.Media.Color.FromArgb((item.IsEnabled ? OverallColor.A : DisabledOpacity.A), OverallColor.R, OverallColor.G, OverallColor.B);
                    content.Fill = new SolidColorBrush(ConvertedColor);

                }

            }

            if (sender is MenuItem)
            {

                var item = (sender as MenuItem);
                if (item == null) return;
                if (item.Header == null) return;
                var objContent = (sender as MenuItem).Header;
                if (objContent == null) return;
                if (objContent is System.Windows.Shapes.Rectangle)
                {
                    System.Windows.Shapes.Rectangle content = objContent as System.Windows.Shapes.Rectangle;
                    Color DisabledOpacity = Color.FromArgb(128, 0, 0, 0);
                    System.Windows.Media.Color ConvertedColor = System.Windows.Media.Color.FromArgb((item.IsEnabled ? OverallColor.A : DisabledOpacity.A), OverallColor.R, OverallColor.G, OverallColor.B);
                    content.Fill = new SolidColorBrush(ConvertedColor);

                }


            }

            if (sender is Xceed.Wpf.Toolkit.SplitButton)
            {
                var item = (sender as Xceed.Wpf.Toolkit.SplitButton);
                if (item == null) return;
                if (item.Content == null) return;
                var objContent = (sender as Xceed.Wpf.Toolkit.SplitButton).Content;
                if (objContent == null) return;
                if (objContent is System.Windows.Shapes.Rectangle)
                {
                    System.Windows.Shapes.Rectangle content = objContent as System.Windows.Shapes.Rectangle;
                    Color DisabledOpacity = Color.FromArgb(128, 0, 0, 0);
                    System.Windows.Media.Color ConvertedColor = System.Windows.Media.Color.FromArgb((item.IsEnabled ? OverallColor.A : DisabledOpacity.A), OverallColor.R, OverallColor.G, OverallColor.B);
                    content.Fill = new SolidColorBrush(ConvertedColor);

                }

            }
        }
        public void UpdateButtonColors()
        {
            SetButtonColors(Instance.New, MainThemeColor());
            SetButtonColors(Instance.Open, MainThemeColor(Color.FromArgb(255, 231, 147), Color.FromArgb(250, 217, 98)));
            SetButtonColors(Instance.RecentDataDirectories, MainThemeColor(Color.FromArgb(255, 231, 147), Color.FromArgb(250, 217, 98)));
            SetButtonColors(Instance.Save, Color.RoyalBlue);
            SetButtonColors(Instance.ZoomInButton, Color.SlateBlue);
            SetButtonColors(Instance.ZoomOutButton, Color.SlateBlue);
            SetButtonColors(Instance.FreezeDeviceButton, Color.Red);
            SetButtonColors(Instance.UndoButton, Color.RoyalBlue);
            SetButtonColors(Instance.RedoButton, Color.RoyalBlue);
            SetButtonColors(Instance.ReloadButton, Color.RoyalBlue);
            SetButtonColors(Instance.PointerToolButton, MainThemeColor());
            SetButtonColors(Instance.SelectToolButton, Color.MediumPurple);
            SetButtonColors(Instance.PlaceTilesButton, Color.Green);
            SetButtonColors(Instance.InteractionToolButton, Color.Gold);
            SetButtonColors(Instance.MagnetMode, Color.Red);
            SetButtonColors(Instance.ChunksToolButton, Color.SandyBrown);
            SetButtonColors(Instance.ShowTileIDButton, MainThemeColor());
            SetButtonColors(Instance.ShowGridButton, MainThemeColor(Color.Gray));
            SetButtonColors(Instance.ShowCollisionAButton, Color.DeepSkyBlue);
            SetButtonColors(Instance.ShowCollisionBButton, Color.DeepSkyBlue);
            SetButtonColors(Instance.FlipAssistButton, MainThemeColor());
            SetButtonColors(Instance.MagnetModeSplitButton, MainThemeColor());
            SetButtonColors(Instance.GridSizeButton, MainThemeColor());
            SetButtonColors(Instance.RunSceneDropDown, MainThemeColor());
            SetButtonColors(Instance.RecentDataDirectories_DropDown, MainThemeColor());
            SetButtonColors(Instance.MagnetModeSplitDropDown, MainThemeColor());
            SetButtonColors(Instance.GridSizeButton, MainThemeColor());
            SetButtonColors(Instance.animationsSplitButton_Dropdown, MainThemeColor());
            SetButtonColors(Instance.MoreSettingsButton, MainThemeColor());

        }
        public Color MainThemeColor(Color? CDC = null, Color? CWC = null)
        {
            Color NightColor;
            Color NormalColor;
            if (CDC != null) NightColor = CDC.Value;
            else NightColor = Color.White;

            if (CWC != null) NormalColor = CWC.Value;
            else NormalColor = Color.Black;

            return (Settings.MySettings.NightMode ? NightColor : NormalColor);
        }
        #endregion

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
}
