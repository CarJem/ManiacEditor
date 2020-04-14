using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace ManiacEditor.Classes.Scene
{
    public class EditorTiles : IDisposable
    {
        #region Definitions

        #region GIF Variables
        public Methods.Draw.GIF Image { get; set; }
        public Methods.Draw.GIF IDImage { get; set; }
        public Methods.Draw.GIF EditorImage { get; set; }
        public Methods.Draw.GIF SelectionImage { get; set; }
        public Methods.Draw.GIF CollisionMaskA { get; private set; }
        public Methods.Draw.GIF CollisionMaskB { get; private set; }
        #endregion

        #region Tile Config
        public RSDKv5.Tileconfig TileConfig
        {
            get
            {
                return _TileConfig;
            }
            set
            {
                _TileConfig = value;
                UpdateConfigCollision();
            }
        }
        private RSDKv5.Tileconfig _TileConfig { get; set; }
        #endregion

        #endregion

        #region Init
        public EditorTiles()
        {
            Image = new Methods.Draw.GIF(Path.Combine(Environment.CurrentDirectory, "16x16Tiles_ID.gif"));
            TileConfig = new RSDKv5.Tileconfig();
        }
        public EditorTiles(string StageDirectory, string PaletteDataPath = null)
		{
			Image = new Methods.Draw.GIF(Path.Combine(StageDirectory, "16x16Tiles.gif"), PaletteDataPath);
            IDImage = new Methods.Draw.GIF(Environment.CurrentDirectory + "\\Resources\\Tile Overlays\\" + "16x16Tiles_ID.gif");
			EditorImage = new Methods.Draw.GIF(Environment.CurrentDirectory + "\\Resources\\Tile Overlays\\" + "16x16Tiles_Edit.gif");
            SelectionImage = new Methods.Draw.GIF(Environment.CurrentDirectory + "\\Resources\\Tile Overlays\\" + "16x16Tiles_Selection.gif");
        }
        #endregion

        #region Collision

        public void UpdateConfigCollision()
        {
            try
            {

                if (CollisionMaskA != null) CollisionMaskA.Dispose();
                if (CollisionMaskB != null) CollisionMaskB.Dispose();

                CollisionMaskA = new Methods.Draw.GIF(DrawCollisionMaskA());
                CollisionMaskB = new Methods.Draw.GIF(DrawCollisionMaskB());
            }
            catch (Exception ex)
            {
                throw new EventHandlers.TileConfigException("Unable to load Tileconfig.bin!" + Environment.NewLine + "Full Exception Details: " + ex.Message);
            }

        }
        private Bitmap DrawCollisionMaskA()
        {
            Bitmap bitmap = new Bitmap(16, 16384);
            using (Graphics gfx = Graphics.FromImage(bitmap))
            {
                for (int i = 0; i < 1024; i++)
                {
                    gfx.DrawImage(TileConfig.CollisionPath1[i].DrawCMask(Color.FromArgb(0, 0, 0, 0), Color.White), new Rectangle(0,(16 * i),16,16));
                }
            }
            return bitmap;
        }
        private Bitmap DrawCollisionMaskB()
        {
            Bitmap bitmap = new Bitmap(16, 16384);
            using (Graphics gfx = Graphics.FromImage(bitmap))
            {
                for (int i = 0; i < 1024; i++)
                {
                    gfx.DrawImage(TileConfig.CollisionPath2[i].DrawCMask(Color.FromArgb(0, 0, 0, 0), Color.White), new Rectangle(0, (16 * i), 16, 16));
                }
            }
            return bitmap;
        }

        #endregion


        public void Write(string filename)
        {
            Bitmap write = Image.GetBitmap(new Rectangle(0,0,16,16384));
            write.Save(filename);
                
        }
        public void Reload(string PaletteDataPath = null)
        {
            if (Image != null) Image.Reload(PaletteDataPath);
            if (IDImage != null) IDImage.Reload();
            if (EditorImage != null) EditorImage.Reload();
            if (SelectionImage != null) SelectionImage.Reload();
            if (CollisionMaskA != null) CollisionMaskA.Reload(DrawCollisionMaskA());
            if (CollisionMaskB != null) CollisionMaskB.Reload(DrawCollisionMaskB());
        }
        public void Dispose()
        {
            if (Image != null) Image.Dispose();
            if (IDImage != null) IDImage.Dispose();
            if (EditorImage != null) EditorImage.Dispose();
            if (SelectionImage != null) SelectionImage.Dispose();
            if (CollisionMaskA != null) CollisionMaskA.Dispose();
            if (CollisionMaskB != null) CollisionMaskB.Dispose();
        }
    }
}
