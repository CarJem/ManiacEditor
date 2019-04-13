using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ManiacEditor
{
    public class SkinResourceDictionary : ResourceDictionary
    {
        private Uri _DarkSource;
        private Uri _LightSource;
        private Uri _JemSource;

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

        public Uri JemSource
        {
            get { return _JemSource; }
            set
            {
                _JemSource = value;
                UpdateSource();
            }
        }

        public void UpdateSource()
        {
            var val = GetSkin();
            if (val != null && base.Source != val)
                base.Source = val;
        }

        public Uri GetSkin()
        {
            if (App.Skin == Skin.Light)
            {
                return LightSource;
            }
            else if (App.Skin == Skin.Dark)
            {
                return DarkSource;

            }
            else if (App.Skin == Skin.CarJem)
            {
                return JemSource;
            }
            else return LightSource;
        }
    }
}
