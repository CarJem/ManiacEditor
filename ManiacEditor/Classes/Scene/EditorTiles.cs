using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace ManiacEditor.Classes.Scene
{
    public class EditorTiles : IDisposable
    {
        /// <summary>
        /// the 16x16Tiles
        /// </summary>
        public readonly Methods.Draw.GIF Image;
        /// <summary>
        /// the 16x16Tiles (Semi-Transparent)
        /// </summary>
        public readonly Methods.Draw.GIF ImageTransparent;
        /// <summary>
        /// IDs for each tile
        /// </summary>
        public readonly Methods.Draw.GIF IDImage;
        /// <summary>
        /// Tiles for Maniac Editor to Use
        /// </summary>
        public readonly Methods.Draw.GIF EditorImage;
        
        
        #region Tile Config
        /// <summary>
        /// the stage's tileconfig data
        /// </summary>
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
        public Methods.Draw.GIF CollisionMaskA { get; private set; }
        public Methods.Draw.GIF CollisionMaskB { get; private set; }



        public EditorTiles(string stage_directory, string palleteDir = null)
		{
			Image = new Methods.Draw.GIF(Path.Combine(stage_directory, "16x16Tiles.gif"), palleteDir);
            ImageTransparent = new Methods.Draw.GIF(SetImageOpacity(Image.ToBitmap(), (float)0.1));
            IDImage = new Methods.Draw.GIF(Environment.CurrentDirectory + "\\Resources\\Tile Overlays\\" + "16x16Tiles_ID.gif");
			EditorImage = new Methods.Draw.GIF(Environment.CurrentDirectory + "\\Resources\\Tile Overlays\\" + "16x16Tiles_Edit.gif");
        }

        public void UpdateConfigCollision()
        {
            try
            {
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



        private const int bytesPerPixel = 4;
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

        public EditorTiles()
		{
			Image = new Methods.Draw.GIF(Path.Combine(Environment.CurrentDirectory, "16x16Tiles_ID.gif"));
			TileConfig = new RSDKv5.Tileconfig();
		}

		public void Write(string filename)
        {
            Bitmap write = Image.GetBitmap(new Rectangle(0,0,16,16384));
            write.Save(filename);
                
        }

        public void Dispose()
        {
            Image.Dispose();
        }

        public void DisposeTextures()
        {
            Image.Dispose();
        }
    }
}
