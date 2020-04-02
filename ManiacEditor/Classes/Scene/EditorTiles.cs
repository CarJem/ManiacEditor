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
        public Methods.Draw.GIF ImageTransparent { get; set; }
        public Methods.Draw.GIF IDImage { get; set; }
        public Methods.Draw.GIF EditorImage { get; set; }
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
        public EditorTiles(string stage_directory, string palleteDir = null)
		{
			Image = new Methods.Draw.GIF(Path.Combine(stage_directory, "16x16Tiles.gif"), palleteDir);
            ImageTransparent = new Methods.Draw.GIF(SetImageOpacity(Image.ToBitmap(), (float)0.1));
            IDImage = new Methods.Draw.GIF(Environment.CurrentDirectory + "\\Resources\\Tile Overlays\\" + "16x16Tiles_ID.gif");
			EditorImage = new Methods.Draw.GIF(Environment.CurrentDirectory + "\\Resources\\Tile Overlays\\" + "16x16Tiles_Edit.gif");
        }
        #endregion

        #region Extensions

        public Image SetImageOpacity(Image image, float opacity)
        {
            //create a Bitmap the size of the image provided  
            Bitmap bmp = new Bitmap(image.Width, image.Height);

            //create a graphics object from the image  
            using (Graphics gfx = Graphics.FromImage(bmp))
            {

                //create a color matrix object  
                ColorMatrix matrix = new ColorMatrix();

                //set the opacity  
                matrix.Matrix33 = opacity;

                //create image attributes  
                ImageAttributes attributes = new ImageAttributes();

                //set the color(opacity) of the image  
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                //now draw the image  
                gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
            }
            return bmp;
        }

        #endregion

        #region Collision

        public void UpdateConfigCollision()
        {
            try
            {

                if (CollisionMaskA != null) CollisionMaskA.Dispose();
                if (CollisionMaskB != null) CollisionMaskB.Dispose();

                CollisionMaskA = DrawCollisionMaskA();
                CollisionMaskB = DrawCollisionMaskB();
            }
            catch (Exception ex)
            {
                throw new EventHandlers.TileConfigException("Unable to load Tileconfig.bin!" + Environment.NewLine + "Full Exception Details: " + ex.Message);
            }

        }
        private Methods.Draw.GIF DrawCollisionMaskA()
        {
            Bitmap bitmap = new Bitmap(16, 16384);
            using (Graphics gfx = Graphics.FromImage(bitmap))
            {
                for (int i = 0; i < 1024; i++)
                {
                    gfx.DrawImage(TileConfig.CollisionPath1[i].DrawCMask(Color.FromArgb(0, 0, 0, 0), Color.White), new Rectangle(0,(16 * i),16,16));
                }
            }
            Methods.Draw.GIF gif = new Methods.Draw.GIF(bitmap);
            return gif;
        }
        private Methods.Draw.GIF DrawCollisionMaskB()
        {
            Bitmap bitmap = new Bitmap(16, 16384);
            using (Graphics gfx = Graphics.FromImage(bitmap))
            {
                for (int i = 0; i < 1024; i++)
                {
                    gfx.DrawImage(TileConfig.CollisionPath2[i].DrawCMask(Color.FromArgb(0, 0, 0, 0), Color.White), new Rectangle(0, (16 * i), 16, 16));
                }
            }
            Methods.Draw.GIF gif = new Methods.Draw.GIF(bitmap);
            return gif;
        }

        #endregion


        public void Write(string filename)
        {
            Bitmap write = Image.GetBitmap(new Rectangle(0,0,16,16384));
            write.Save(filename);
                
        }
        public void Dispose()
        {
            if (Image != null) Image.Dispose();
            if (ImageTransparent != null) ImageTransparent.Dispose();
            if (IDImage != null) IDImage.Dispose();
            if (EditorImage != null) EditorImage.Dispose();
            if (CollisionMaskA != null) CollisionMaskA.Dispose();
            if (CollisionMaskB != null) CollisionMaskB.Dispose();
        }
    }
}
