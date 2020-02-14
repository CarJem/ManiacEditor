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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace ManiacEditor.Controls.Global.Controls.RenderedText
{
    /// <summary>
    /// Interaction logic for RenderedTextBox.xaml
    /// </summary>
    public partial class RenderedTextBox : UserControl
    {
        string TextAOtherChars = "*+,-./: \'\"_^]\\[)(";
        string TextANormalChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        string TextHUDNormalChars = "0123456789ABCDEF-abcdef";
        string TextHUDOtherChars = "\'\"-.";
        string TextHUDBlueNormalChars = "0123456789";

        public static readonly DependencyProperty OverlayBackgroundProperty = DependencyProperty.Register("OverlayBackground", typeof(Brush), typeof(RenderedTextBox), new UIPropertyMetadata(DefaultOverlayBackground));

        public Brush OverlayBackground
        {
            get { return (Brush)GetValue(OverlayBackgroundProperty); }
            set { SetValue(OverlayBackgroundProperty, value); }
        }

        public static readonly Brush DefaultOverlayBackground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#90000000"));

        public RenderedTextBox()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            RenderText();
        }

        public enum TextType
        {
            TextA, TextAHighlight, HUD, HUD_Blue
        }


        private string GetTextTypeString(TextType type)
        {
            switch (type)
            {
                case (TextType.TextA):
                    return "TextA";
                case (TextType.TextAHighlight):
                    return "TextA_Highlighted";
                case (TextType.HUD):
                    return "HUD";
                case (TextType.HUD_Blue):
                    return "HUD_Blue";
                default:
                    return "TextA";
            }
        }
        #region Get Char Path
        private string GetPathToChar(char character)
        {
            if (Type == TextType.TextA || Type == TextType.TextAHighlight)
            {
                return GetTextAPathToChar(character);
            }
            else if (Type == TextType.HUD || Type == TextType.HUD_Blue)
            {
                return GetHUDPathToChar(character);
            }
            else return GetTextAPathToChar(character);
        }
        public string GetHUDPathToChar(char character)
        {
            if (Type == TextType.HUD_Blue)
            {
                if (TextHUDBlueNormalChars.Contains(character))
                {
                    return GetResourcePath(character.ToString());
                }
            }
            else
            {
                if (TextHUDNormalChars.Contains(character))
                {
                    return GetResourcePath(character.ToString());
                }
                else if (TextHUDOtherChars.Contains(character))
                {
                    switch (character)
                    {
                        case ('-'):
                            return GetResourcePath("Minus");
                        case ('.'):
                            return GetResourcePath("Period");
                        case ('\''):
                            return GetResourcePath("Quote");
                        case ('\"'):
                            return GetResourcePath("DoubleQuote");
                    }

                }
            }


            return null;
        }
        public string GetTextAPathToChar(char character)
        {
            if (TextANormalChars.Contains(character))
            {
                return GetResourcePath(character.ToString());
            }
            else if (TextAOtherChars.Contains(character))
            {
                switch (character)
                {
                    case ('*'):
                        return GetResourcePath("Star");
                    case ('+'):
                        return GetResourcePath("Plus");
                    case ('-'):
                        return GetResourcePath("Minus");
                    case ('.'):
                        return GetResourcePath("Period");
                    case ('/'):
                        return GetResourcePath("ForwardSlash");
                    case (':'):
                        return GetResourcePath("Colon");
                    case (' '):
                        return GetResourcePath("Space");
                    case ('\''):
                        return GetResourcePath("Quote");
                    case ('\"'):
                        return GetResourcePath("DoubleQuote");
                    case ('_'):
                        return GetResourcePath("Underscore");
                    case ('^'):
                        return GetResourcePath("Carrot");
                    case (']'):
                        return GetResourcePath("BracketR");
                    case ('['):
                        return GetResourcePath("BracketL");
                    case (')'):
                        return GetResourcePath("ParR");
                    case ('('):
                        return GetResourcePath("ParL");
                    case ('\\'):
                        return GetResourcePath("BackwardSlash");
                }

            }

            return null;
        }

        public string GetResourcePath(string name)
        {
            //String appUri = "/" + System.Reflection.Assembly.GetEntryAssembly().GetName().Name + ";" + "component";
            String appUri = "pack://application:,,,";
            String path = appUri + $"/Resources/HUD/{GetTextTypeString(Type)}/" + name + ".png";
            return path;
        }

        private UriKind UriKind { get; set; } = UriKind.Absolute;
        #endregion

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(RenderedTextBox), new FrameworkPropertyMetadata("", OnTextChanged));

        private static void OnTextChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            RenderedTextBox renderText = source as RenderedTextBox;
            renderText.RenderText();
        }

        public TextType Type
        {
            get { return (TextType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }


        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(TextType), typeof(RenderedTextBox), new FrameworkPropertyMetadata(TextType.TextA, OnTextTypeChanged));

        private static void OnTextTypeChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            RenderedTextBox renderText = source as RenderedTextBox;
            renderText.RenderText();
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }


        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(RenderedTextBox), new FrameworkPropertyMetadata(16.0, OnScaleChanged));

        private static void OnScaleChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            RenderedTextBox renderText = source as RenderedTextBox;
            renderText.RenderText();
        }

        public double Scale
        {
            get 
            { 
                return (double)GetValue(ScaleProperty); 
            }
            set 
            { 
                SetValue(ScaleProperty, value);
                this.Height = value;
            }
        }

        public void Refresh()
        {
            RenderText();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                this.Background = Brushes.Yellow;
            }

        }

        public void RenderText()
        {
            Canvas.Background = OverlayBackground;
            Canvas.Items.Clear();

            this.MaxHeight = Scale;
            this.Height = Scale;
            this.MinHeight = Scale;

            Canvas.ClipToBounds = true;
            Canvas.MaxHeight = Scale;
            Canvas.Height = Scale;
            Canvas.MinHeight = Scale;

            foreach (var character in Text) 
            {
                string path = GetPathToChar(character);
                if (path != null)
                {
                    Image img = new Image();
                    RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.NearestNeighbor);
                    img.Stretch = Stretch.Uniform;
                    
                    Uri uri = new Uri(path, UriKind);
                    var finalImg = new BitmapImage(uri);
                    finalImg.Freeze();
                    img.Source = finalImg;

                    Canvas.Items.Add(img);
                }

            }
            Canvas.InvalidateVisual();
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RenderText();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RenderText();
        }
    }
}
