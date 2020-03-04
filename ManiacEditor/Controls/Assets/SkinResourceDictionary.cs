using System;
using System.Windows;

namespace ManiacEditor.Controls.Assets
{
    public class SkinResourceDictionary : ResourceDictionary
    {
        private Uri _DarkSource;
        private Uri _LightSource;
        private Uri _BetaSource;

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

        public Uri BetaSource
        {
            get { return _BetaSource; }
            set
            {
                _BetaSource = value;
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
            if (App.Skin == Enums.Skin.Light)
            {
                return LightSource;
            }
            else if (App.Skin == Enums.Skin.Dark)
            {
                return DarkSource;

            }
            else if (App.Skin == Enums.Skin.Beta)
            {
                return BetaSource;
            }
            else return DarkSource;
        }
    }
}
