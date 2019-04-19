using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using ManiacEditor;
using Microsoft.Xna.Framework;
using RSDKv5;
using Xceed.Wpf.Toolkit;
using System.Collections.Specialized;
using System.Configuration;
using System.Windows.Data;
using System.Xml.Serialization;

namespace ManiacEditor
{


    public static class Extensions
    {
        public static System.Drawing.Color Blend(this System.Drawing.Color color, System.Drawing.Color backcolor, double amount)
        {
            byte r = (byte)((color.R * amount) + backcolor.R * (1 - amount));
            byte g = (byte)((color.G * amount) + backcolor.G * (1 - amount));
            byte b = (byte)((color.B * amount) + backcolor.B * (1 - amount));
            return System.Drawing.Color.FromArgb(r, g, b);
        }

        public static void GetRowColIndex(this System.Windows.Controls.Grid @this, System.Windows.Point position, out int row, out int column)
        {
            column = -1;
            double total = 0;
            foreach (System.Windows.Controls.ColumnDefinition clm in @this.ColumnDefinitions)
            {
                if (position.X < total)
                {
                    break;
                }
                column++;
                total += clm.ActualWidth;
            }
            row = -1;
            total = 0;
            foreach (System.Windows.Controls.RowDefinition rowDef in @this.RowDefinitions)
            {
                if (position.Y < total)
                {
                    break;
                }
                row++;
                total += rowDef.ActualHeight;
            }
        }
        public static System.Drawing.Color FindNearestMatch(this System.Drawing.Color col, out int distance, params System.Drawing.Color[] palette)
        {
            if (Array.IndexOf(palette, col) != -1)
            {
                distance = 0;
                return col;
            }
            System.Drawing.Color nearest_color = System.Drawing.Color.Empty;
            distance = int.MaxValue;
            foreach (System.Drawing.Color o in palette)
            {
                int test_red = o.R - col.R;
                test_red *= test_red;
                int test_green = o.G - col.G;
                test_green *= test_green;
                int test_blue = o.B - col.B;
                test_blue *= test_blue;
                int temp = test_blue + test_green + test_red;
                if (temp == 0)
                    return o;
                else if (temp < distance)
                {
                    distance = temp;
                    nearest_color = o;
                }
            }
            return nearest_color;
        }
        public static List<System.Drawing.Point> CreateDataPoints(float[] x, float[] y)
        {
            System.Diagnostics.Debug.Assert(x.Length == y.Length);
            List<System.Drawing.Point> points = new List<System.Drawing.Point>();

            for (int i = 0; i < x.Length; i++)
            {
                points.Add(new System.Drawing.Point((int)x[i], (int)y[i]));
            }

            return points;
        }
        public static IEnumerable<IEnumerable<int>> GroupConsecutive(this IEnumerable<int> list)
        {
            var group = new List<int>();
            foreach (var i in list)
            {
                if (group.Count == 0 || i - group[group.Count - 1] <= 1)
                    group.Add(i);
                else
                {
                    yield return group;
                    group = new List<int> { i };
                }
            }
            yield return group;
        }

        public static void EnableButtonList(object[] allItems)
        {
            foreach (var item in allItems)
            {
                if (item is ToggleButton)
                {
                    var button = item as ToggleButton;
                    button.IsEnabled = true;
                }
                else if (item is UIElement)
                {
                    var button = item as UIElement;
                    button.IsEnabled = true;
                }
                else if (item is Button)
                {
                    var button = item as Button;
                    button.IsEnabled = true;
                }
                else if (item is SplitButton)
                {
                    var button = item as SplitButton;
                    button.IsEnabled = true;
                }

            }
        }

		public static bool MouseIsOverGraphicsPanel(DevicePanel btn)
		{
			if (btn.ClientRectangle.Contains(btn.PointToClient(System.Windows.Forms.Cursor.Position)))
			{
				return true;
			}
			return false;
		}
		public static System.Windows.Media.Color ColorConvertToMedia(System.Drawing.Color input)
		{
			return System.Windows.Media.Color.FromArgb(input.A, input.R, input.G, input.B);
		}

		public static System.Drawing.Color ColorConvertToDrawing(System.Windows.Media.Color input)
		{
			return System.Drawing.Color.FromArgb(input.A, input.R, input.G, input.B);
		}

		// Or IsNanOrInfinity
		public static bool HasValue(this double value)
		{
			return !Double.IsNaN(value) && !Double.IsInfinity(value);
		}

		public static bool KeyBindsSettingExists(string name)
		{
			bool found = false;
			foreach (SettingsProperty currentProperty in Properties.KeyBinds.Default.Properties)
			{
				if (name == currentProperty.Name.ToString())
				{
					found = true;
				}

			}
			return found;
		}

		public static IEnumerable<T> FindVisualChildren<T>(DependencyObject rootObject) where T : DependencyObject
		{
			if (rootObject != null)
			{
				for (int i = 0; i < VisualTreeHelper.GetChildrenCount(rootObject); i++)
				{
					DependencyObject child = VisualTreeHelper.GetChild(rootObject, i);

					if (child != null && child is T)
						yield return (T)child;

					foreach (T childOfChild in FindVisualChildren<T>(child))
						yield return childOfChild;
				}
			}
		}

		public static string ReplaceLastOccurrence(string Source, string Find, string Replace)
		{
			int place = Source.LastIndexOf(Find);

			if (place == -1)
				return Source;

			string result = Source.Remove(place, Find.Length).Insert(place, Replace);
			return result;
		}

		private const int bytesPerPixel = 4;

		/// <summary>
		/// Change the opacity of an image
		/// </summary>
		/// <param name="originalImage">The original image</param>
		/// <param name="opacity">Opacity, where 1.0 is no opacity, 0.0 is full transparency</param>
		/// <returns>The changed image</returns>
		public static System.Drawing.Image ChangeImageOpacity(System.Drawing.Image originalImage, double opacity)
		{
			if ((originalImage.PixelFormat & System.Drawing.Imaging.PixelFormat.Indexed) == System.Drawing.Imaging.PixelFormat.Indexed)
			{
				// Cannot modify an image with indexed colors
				return originalImage;
			}

			Bitmap bmp = (Bitmap)originalImage.Clone();

			// Specify a pixel format.
			System.Drawing.Imaging.PixelFormat pxf = System.Drawing.Imaging.PixelFormat.Format32bppArgb;

			// Lock the bitmap's bits.
			System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
			System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, pxf);

			// Get the address of the first line.
			IntPtr ptr = bmpData.Scan0;

			// Declare an array to hold the bytes of the bitmap.
			// This code is specific to a bitmap with 32 bits per pixels 
			// (32 bits = 4 bytes, 3 for RGB and 1 byte for alpha).
			int numBytes = bmp.Width * bmp.Height * bytesPerPixel;
			byte[] argbValues = new byte[numBytes];

			// Copy the ARGB values into the array.
			System.Runtime.InteropServices.Marshal.Copy(ptr, argbValues, 0, numBytes);

			// Manipulate the bitmap, such as changing the
			// RGB values for all pixels in the the bitmap.
			for (int counter = 0; counter < argbValues.Length; counter += bytesPerPixel)
			{
				// argbValues is in format BGRA (Blue, Green, Red, Alpha)

				// If 100% transparent, skip pixel
				if (argbValues[counter + bytesPerPixel - 1] == 0)
					continue;

				int pos = 0;
				pos++; // B value
				pos++; // G value
				pos++; // R value

				argbValues[counter + pos] = (byte)(argbValues[counter + pos] * opacity);
			}

			// Copy the ARGB values back to the bitmap
			System.Runtime.InteropServices.Marshal.Copy(argbValues, 0, ptr, numBytes);

			// Unlock the bits.
			bmp.UnlockBits(bmpData);

			return bmp;
		}

		public static System.Drawing.Bitmap ChangeImageColor(Bitmap source, System.Drawing.Color OldColor, System.Drawing.Color NewColor)
		{
			Bitmap Result = new Bitmap(source.Width, source.Height);
			Graphics g = Graphics.FromImage(Result);
			using (Bitmap bmp = new Bitmap(source))
			{

				// Set the image attribute's color mappings
				System.Drawing.Imaging.ColorMap[] colorMap = new System.Drawing.Imaging.ColorMap[1];
				colorMap[0] = new System.Drawing.Imaging.ColorMap();
				colorMap[0].OldColor = OldColor;
				colorMap[0].NewColor = NewColor;
				System.Drawing.Imaging.ImageAttributes attr = new System.Drawing.Imaging.ImageAttributes();
				attr.SetRemapTable(colorMap);
				// Draw using the color map
				System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
				g.DrawImage(bmp, rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel, attr);
			}
			return Result;
		}
	}

    public static class ExtensionMethods
    {

        private static Action EmptyDelegate = delegate () { };


        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
	}

	public static class EnumerableExtensions
	{
		public static int IndexOf<T>(this IEnumerable<T> obj, T value)
		{
			return obj.IndexOf(value, null);
		}

		public static int IndexOf<T>(this IEnumerable<T> obj, T value, IEqualityComparer<T> comparer)
		{
			comparer = comparer ?? EqualityComparer<T>.Default;
			var found = obj
				.Select((a, i) => new { a, i })
				.FirstOrDefault(x => comparer.Equals(x.a, value));
			return found == null ? -1 : found.i;
		}
	}

	public class SettingBindingExtension : Binding
	{
		public SettingBindingExtension()
		{
			Initialize();
		}

		public SettingBindingExtension(string path)
			: base(path)
		{
			Initialize();
		}

		private void Initialize()
		{
			this.Source = ManiacEditor.Properties.Settings.Default;
			this.Mode = BindingMode.TwoWay;
		}
	}

    public class DefaultSettingsBindingExtension : Binding
    {
        public DefaultSettingsBindingExtension()
        {
            Initialize();
        }

        public DefaultSettingsBindingExtension(string path)
            : base(path)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.Source = ManiacEditor.Properties.Defaults.Default;
            this.Mode = BindingMode.TwoWay;
        }
    }

    public class PerformanceSettingsBindingExtension : Binding
    {
        public PerformanceSettingsBindingExtension()
        {
            Initialize();
        }

        public PerformanceSettingsBindingExtension(string path)
            : base(path)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.Source = ManiacEditor.Properties.Performance.Default;
            this.Mode = BindingMode.TwoWay;
        }
    }

    public class DevSettingsBindingExtension : Binding
    {
        public DevSettingsBindingExtension()
        {
            Initialize();
        }

        public DevSettingsBindingExtension(string path)
            : base(path)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.Source = ManiacEditor.Properties.DevSettings.Default;
            this.Mode = BindingMode.TwoWay;
        }
    }

    public class GameSettingsBindingExtension : Binding
    {
        public GameSettingsBindingExtension()
        {
            Initialize();
        }

        public GameSettingsBindingExtension(string path)
            : base(path)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.Source = ManiacEditor.Properties.GameOptions.Default;
            this.Mode = BindingMode.TwoWay;
        }
    }


    public static class ButtonHelper
	{
		// Boilerplate code to register attached property "bool? DialogResult"
		public static bool? GetDialogResult(DependencyObject obj) { return (bool?)obj.GetValue(DialogResultProperty); }
		public static void SetDialogResult(DependencyObject obj, bool? value) { obj.SetValue(DialogResultProperty, value); }
		public static readonly DependencyProperty DialogResultProperty = DependencyProperty.RegisterAttached("DialogResult", typeof(bool?), typeof(ButtonHelper), new UIPropertyMetadata
		{
			PropertyChangedCallback = (obj, e) =>
			{
				// Implementation of DialogResult functionality
				Button button = obj as Button;
				if (button == null)
					throw new InvalidOperationException(
					  "Can only use ButtonHelper.DialogResult on a Button control");
				button.Click += (sender, e2) =>
				{
					Window.GetWindow(button).DialogResult = GetDialogResult(button);
				};
			}
		});
	}

}
