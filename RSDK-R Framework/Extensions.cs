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
using System.Collections.Specialized;
using System.Configuration;
using System.Windows.Data;


namespace RSDK_R_Framework
{
    public static class Extensions
    {
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
			foreach (SettingsProperty currentProperty in KeyBinding.Keybindings)
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

}
