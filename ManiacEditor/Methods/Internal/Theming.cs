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

namespace ManiacEditor.Methods.Internal
{
    public static class Theming
    {
        private static ManiacEditor.Controls.Editor.MainEditor Instance;


        private static Color GetResource(string resourceName)
        {
            try
            {
                var c = (SolidColorBrush)Instance.FindResource(resourceName);
                if (c != null) return Color.FromArgb(c.Color.A, c.Color.R, c.Color.G, c.Color.B);
                else return Color.Empty;
            }
            catch
            {
                return Color.Empty;
            }


        }

        public static Color ThemeBrush1
        {
            get
            {
                return GetResource("LegacyThemeBrush1");
            }
        }
        public static Color ThemeBrush2
        {
            get
            {
                return GetResource("LegacyThemeBrush2");
            }
        }
        public static Color ThemeBrush3
        {
            get
            {
                return GetResource("LegacyThemeBrush3");
            }
        }
        public static Color ThemeBrush4
        {
            get
            {
                return GetResource("LegacyThemeBrush4");
            }
        }
        public static Color ThemeBrush5
        {
            get
            {
                return GetResource("LegacyThemeBrush5");
            }
        }
        public static Color ThemeBrush6
        {
            get
            {
                return GetResource("LegacyThemeBrush6");
            }
        }

        public static Color TileManiac_CollisionColor
        {
            get
            {
                return GetResource("TileManiac_CollisionColor");
            }
        }

        public static Color ThemeBrushText
        {
            get
            {
                return GetResource("LegacyThemeBrushText");
            }
        }

        public static Color ThemeBrushBG
        {
            get
            {
                return GetResource("LegacyThemeBrushBG");
            }
        }

        public static void UpdateInstance(ManiacEditor.Controls.Editor.MainEditor _instance)
        {
            Instance = _instance;
        }

        #region Theming Stuff
        public static void SetThemeColors(bool UsingSystem = false)
        {
            if (!UsingSystem)
            {
                SystemColorsUtility systemColors = new SystemColorsUtility();
                systemColors.SetColor(KnownColor.Window, ThemeBrush2);
                systemColors.SetColor(KnownColor.Highlight, ThemeBrush6);
                systemColors.SetColor(KnownColor.WindowFrame, ThemeBrush3);
                systemColors.SetColor(KnownColor.GradientActiveCaption, ThemeBrush2);
                systemColors.SetColor(KnownColor.GradientInactiveCaption, ThemeBrush2);
                systemColors.SetColor(KnownColor.ControlText, ThemeBrush4);
                systemColors.SetColor(KnownColor.WindowText, ThemeBrush4);
                systemColors.SetColor(KnownColor.GrayText, ThemeBrush5);
                systemColors.SetColor(KnownColor.InfoText, ThemeBrush4);
                systemColors.SetColor(KnownColor.MenuText, ThemeBrush4);
                systemColors.SetColor(KnownColor.Control, ThemeBrush2);
                systemColors.SetColor(KnownColor.ButtonHighlight, ThemeBrush4);
                systemColors.SetColor(KnownColor.ButtonShadow, ThemeBrush3);
                systemColors.SetColor(KnownColor.ButtonFace, ThemeBrush2);
                systemColors.SetColor(KnownColor.Desktop, ThemeBrush2);
                systemColors.SetColor(KnownColor.ControlLightLight, ThemeBrush3);
                systemColors.SetColor(KnownColor.ControlLight, ThemeBrush2);
                systemColors.SetColor(KnownColor.ControlDark, ThemeBrush4);
                systemColors.SetColor(KnownColor.ControlDarkDark, ThemeBrush4);
                systemColors.SetColor(KnownColor.ActiveBorder, ThemeBrush2);
                systemColors.SetColor(KnownColor.ActiveCaption, ThemeBrush2);
                systemColors.SetColor(KnownColor.ActiveCaptionText, ThemeBrush4);
                systemColors.SetColor(KnownColor.InactiveBorder, ThemeBrush3);
                systemColors.SetColor(KnownColor.MenuBar, ThemeBrush2);
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


        public static bool UseExtendedColors
        {
            get
            {
                if (Properties.Settings.MySettings.UserTheme == Enums.Skin.Light) return false;
                else return true;
            }
        }

        public static void SetTheme()
        {
            
            if (Properties.Settings.MySettings.UserTheme == Enums.Skin.Light)
            {
                App.ChangeSkin(Properties.Settings.MySettings.UserTheme);
                SetThemeColors(true);
            }
            else
            {
                App.ChangeSkin(Properties.Settings.MySettings.UserTheme);
                SetThemeColors(false);
            }
        }
        public static Control UseExternaTheme(Control control)
        {
            foreach (Control c in control.Controls)
            {
                if (c is Cyotek.Windows.Forms.ColorEditor)
                {
                    foreach (Control c2 in c.Controls)
                    {
                        if (c2 is System.Windows.Forms.NumericUpDown)
                        {
                            c2.ForeColor = ThemeBrushText;
                            c2.BackColor = ThemeBrushBG;
                        }
                        if (c2 is System.Windows.Forms.ComboBox)
                        {
                            c2.ForeColor = ThemeBrushText;
                            c2.BackColor = ThemeBrushBG;
                        }
                    }
                }

                if (c is System.Windows.Forms.Button)
                {
                    c.ForeColor = ThemeBrushText;
                }
                if (c is NumericUpDown)
                {
                    c.ForeColor = ThemeBrushText;
                    c.BackColor = ThemeBrushBG;
                }
                if (c is System.Windows.Forms.ComboBox)
                {
                    c.ForeColor = ThemeBrushText;
                    c.BackColor = ThemeBrushBG;
                }
                if (c is System.Windows.Forms.TextBox)
                {
                    c.ForeColor = ThemeBrushText;
                    c.BackColor = ThemeBrushBG;
                }
            }
            return control;
        }
        public static void UpdateButtonColors()
        {
            var converter = new System.Windows.Media.BrushConverter();
            if (UseExtendedColors)
            {
                ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.FolderIcon.Fill = (System.Windows.Media.Brush)converter.ConvertFromString("#FFE793");
            }
            else
            {
                ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.FolderIcon.Fill = (System.Windows.Media.Brush)converter.ConvertFromString("#FAD962");
            }

        }
        #endregion

        #region Entities Related Color Fetching
        public static System.Drawing.Color GetSenstiveFilterColors(string colorID)
        {
            if (colorID == "Blue")
            {
                if (UseExtendedColors) return System.Drawing.Color.LightBlue;
                else return System.Drawing.Color.Blue;
            }
            else if (colorID == "Green")
            {
                if (UseExtendedColors) return System.Drawing.Color.LightGreen;
                else return System.Drawing.Color.Green;
            }
            else if (colorID == "Red")
            {
                if (UseExtendedColors) return System.Drawing.Color.FromArgb(211, 76, 49);
                else return System.Drawing.Color.Red;
            }
            else
            {
                if (UseExtendedColors) return System.Drawing.Color.White;
                else return System.Drawing.Color.Black;
            }
        }

        public static int GetFilter(RSDKv5.SceneEntity entity)
        {
            if (entity.attributesMap.ContainsKey("filter") && entity.attributesMap["filter"].Type == RSDKv5.AttributeTypes.UINT8)
            {
                int filter = entity.attributesMap["filter"].ValueUInt8;
                return filter;
            }
            else
            {
                return -1;
            }
        }

        public static SolidColorBrush GetColorBrush(RSDKv5.SceneEntity entity)
        {
            int filter = GetFilter(entity);
            SolidColorBrush ForeColor = (SolidColorBrush)Instance.FindResource("NormalText");
            switch (filter)
            {
                case var _ when (filter == 0 || filter >= 5 && filter != 255):
                    ForeColor = new SolidColorBrush(Extensions.Extensions.ColorConvertToMedia(System.Drawing.Color.Gold)); // Other Filter
                    break;
                case var _ when (filter == 1 || filter == 5):
                    ForeColor = new SolidColorBrush(Extensions.Extensions.ColorConvertToMedia(GetSenstiveFilterColors("Blue"))); // Both Filter
                    break;
                case 2:
                    ForeColor = new SolidColorBrush(Extensions.Extensions.ColorConvertToMedia(GetSenstiveFilterColors("Red"))); // Mania Filter
                    break;
                case 4:
                    ForeColor = new SolidColorBrush(Extensions.Extensions.ColorConvertToMedia(GetSenstiveFilterColors("Green"))); //Encore Filter
                    break;
                case 255:
                    ForeColor = new SolidColorBrush(Extensions.Extensions.ColorConvertToMedia(System.Drawing.Color.Violet)); // All Filter
                    break;
                default:
                    ForeColor = (SolidColorBrush)Instance.FindResource("NormalText"); // NULL Filter
                    break;
            }
            return ForeColor;
        }

        public static SolidColorBrush GetColorBrush(int filter)
        {
            SolidColorBrush ForeColor = (SolidColorBrush)Instance.FindResource("NormalText");
            switch (filter)
            {
                case var _ when (filter == 0 || filter >= 5 && filter != 255):
                    ForeColor = new SolidColorBrush(Extensions.Extensions.ColorConvertToMedia(System.Drawing.Color.Gold)); // Other Filter
                    break;
                case var _ when (filter == 1 || filter == 5):
                    ForeColor = new SolidColorBrush(Extensions.Extensions.ColorConvertToMedia(GetSenstiveFilterColors("Blue"))); // Both Filter
                    break;
                case 2:
                    ForeColor = new SolidColorBrush(Extensions.Extensions.ColorConvertToMedia(GetSenstiveFilterColors("Red"))); // Mania Filter
                    break;
                case 4:
                    ForeColor = new SolidColorBrush(Extensions.Extensions.ColorConvertToMedia(GetSenstiveFilterColors("Green"))); //Encore Filter
                    break;
                case 255:
                    ForeColor = new SolidColorBrush(Extensions.Extensions.ColorConvertToMedia(System.Drawing.Color.Violet)); // All Filter
                    break;
                default:
                    ForeColor = (SolidColorBrush)Instance.FindResource("NormalText"); // NULL Filter
                    break;
            }
            return ForeColor;
        }
        #endregion

        #region Theming Refresh

        public static bool TilesToolbarAwaitingRefresh = false;
        public static bool EntitiesToolbarAwaitingRefresh = false;
        public static bool FormsModelAwaitingRefresh = false;
        public static bool StartScreenAwaitingRefresh = false;
        public static bool TileManiacAwaitingRefresh = false;

        public static void UpdateThemeForItemsWaiting()
        {
            if (FormsModelAwaitingRefresh && ManiacEditor.Controls.Editor.MainEditor.Instance.ViewPanel.SharpPanel != null) RefreshFormsModel();
            if (TilesToolbarAwaitingRefresh && ManiacEditor.Controls.Editor.MainEditor.Instance.TilesToolbar != null) RefreshTilesToolbar();
            if (EntitiesToolbarAwaitingRefresh && ManiacEditor.Controls.Editor.MainEditor.Instance.EntitiesToolbar != null) RefreshEntitiesToolbar();
            if (StartScreenAwaitingRefresh && ManiacEditor.Controls.Editor.MainEditor.Instance.StartScreen != null) RefreshStartScreen();
        }

        public static void RefreshTheme()
        {
            ManiacEditor.Controls.Editor.MainEditor.Instance.Refresh();
            if (ManiacEditor.Controls.Editor.MainEditor.Instance.ViewPanel.SharpPanel != null) RefreshFormsModel();
            else FormsModelAwaitingRefresh = true;
            if (ManiacEditor.Controls.Editor.MainEditor.Instance.StartScreen != null) RefreshStartScreen();
            else StartScreenAwaitingRefresh = true;
            if (ManiacEditor.Controls.Editor.MainEditor.Instance.TilesToolbar != null) RefreshTilesToolbar();
            else TilesToolbarAwaitingRefresh = true;
            if (ManiacEditor.Controls.Editor.MainEditor.Instance.EntitiesToolbar != null) RefreshEntitiesToolbar();
            else EntitiesToolbarAwaitingRefresh = true;
        }

        public static void RefreshStartScreen()
        {
            ManiacEditor.Controls.Editor.MainEditor.Instance.StartScreen.SelectScreen.RefreshTheme();
            StartScreenAwaitingRefresh = false;
        }

        public static void RefreshTilesToolbar()
        {
            ManiacEditor.Controls.Editor.MainEditor.Instance.TilesToolbar.Refresh();
            ManiacEditor.Controls.Editor.MainEditor.Instance.TilesToolbar.ChunkList.Refresh();
            ManiacEditor.Controls.Editor.MainEditor.Instance.TilesToolbar.TilesList.Refresh();
            ManiacEditor.Controls.Editor.MainEditor.Instance.TilesToolbar.ChunkList.vScrollBar1Host.Refresh();
            ManiacEditor.Controls.Editor.MainEditor.Instance.TilesToolbar.TilesList.vScrollBar1Host.Refresh();
            ManiacEditor.Controls.Editor.MainEditor.Instance.TilesToolbar.UpdateThemeColors();
            TilesToolbarAwaitingRefresh = false;
        }

        public static void RefreshEntitiesToolbar()
        {
            ManiacEditor.Controls.Editor.MainEditor.Instance.EntitiesToolbar.Refresh();
            ManiacEditor.Controls.Editor.MainEditor.Instance.EntitiesToolbar.UpdatePropertyGridTheme(true);
            EntitiesToolbarAwaitingRefresh = false;
        }

        public static void RefreshFormsModel()
        {
            ManiacEditor.Controls.Editor.MainEditor.Instance.ViewPanel.SharpPanel.Refresh();
            FormsModelAwaitingRefresh = false;
        }



        #endregion


    }
}
