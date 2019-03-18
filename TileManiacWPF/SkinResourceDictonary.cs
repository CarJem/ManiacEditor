using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TileManiac
{
	public class SkinResourceDictonary : ResourceDictionary
	{
		private Uri _DarkSource;
		private Uri _LightSource;

		public Uri DarkSource
		{
			get { return _DarkSource; }
			set
			{
				_DarkSource = value;
				UpdateSource();
			}
		}
		public Uri LightSource
		{
			get { return _LightSource; }
			set
			{
				_LightSource = value;
				UpdateSource();
			}
		}

		public void UpdateSource()
		{
			var val = App.Skin == Skin.Dark ? DarkSource : LightSource;
			if (val != null && base.Source != val)
				base.Source = val;
		}
	}
}
