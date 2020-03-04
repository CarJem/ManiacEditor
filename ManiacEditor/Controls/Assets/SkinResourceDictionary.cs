using System;
using System.Windows;

namespace ManiacEditor.Controls.Assets
{
    public class SkinResourceDictionary : ResourceDictionary
    {
        #region Themes
        private Uri _DarkSource;
        private Uri _LightSource;
        private Uri _BetaSource;
        private Uri _ShardSource;
        private Uri _CarJemSource;

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
        public Uri ShardSource
        {
            get { return _ShardSource; }
            set
            {
                _ShardSource = value;
                UpdateSource();
            }
        }
        public Uri CarJemSource
        {
            get { return _CarJemSource; }
            set
            {
                _CarJemSource = value;
                UpdateSource();
            }
        }
        #endregion

        #region General
        public void UpdateSource()
        {
            var val = GetSkin();
            if (val != null && base.Source != val)
                base.Source = val;
        }
        public Uri GetSkin()
        {
            switch (App.Skin)
            {
                case Enums.Skin.Light:
                    return LightSource;
                case Enums.Skin.Dark:
                    return DarkSource;
                case Enums.Skin.Beta:
                    return BetaSource;
                case Enums.Skin.Shard:
                    return ShardSource;
                case Enums.Skin.CarJem:
                    return CarJemSource;
                default:
                    return DarkSource;
            }
        }
        #endregion
    }
}
