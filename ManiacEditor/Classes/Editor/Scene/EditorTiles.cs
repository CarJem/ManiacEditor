using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace ManiacEditor.Classes.Editor.Scene
{
    public class EditorTiles : IDisposable
    {
        /// <summary>
        /// the 16x16Tiles
        /// </summary>
        public readonly Classes.Editor.Draw.GIF Image;
        /// <summary>
        /// the 16x16Tiles (Semi-Transparent)
        /// </summary>
        public readonly Classes.Editor.Draw.GIF ImageTransparent;
        /// <summary>
        /// IDs for each tile
        /// </summary>
        public readonly Classes.Editor.Draw.GIF IDImage;
        /// <summary>
        /// Tiles for Maniac Editor to Use
        /// </summary>
        public readonly Classes.Editor.Draw.GIF EditorImage;
        /// <summary>
        /// the stage's tileconfig data
        /// </summary>
        public readonly RSDKv5.Tileconfig Config;
        public readonly Classes.Editor.Draw.GIF CollisionMaskA;
        public readonly Classes.Editor.Draw.GIF CollisionMaskB;

		public EditorTiles(string stage_directory, string palleteDir = null)
		{
			Image = new Classes.Editor.Draw.GIF(Path.Combine(stage_directory, "16x16Tiles.gif"), palleteDir);
            ImageTransparent = new Classes.Editor.Draw.GIF(SetImageOpacity(Image.ToBitmap(), (float)0.1));
            IDImage = new Classes.Editor.Draw.GIF(Environment.CurrentDirectory + "\\Resources\\Tile Overlays\\" + "16x16Tiles_ID.gif");
			EditorImage = new Classes.Editor.Draw.GIF(Environment.CurrentDirectory + "\\Resources\\Tile Overlays\\" + "16x16Tiles_Edit.gif");
			if (File.Exists(Path.Combine(stage_directory, "TileConfig.bin")))
			{
				Config = new RSDKv5.Tileconfig(Path.Combine(stage_directory, "TileConfig.bin"));
                CollisionMaskA = DrawCollisionMaskA();
                CollisionMaskB = DrawCollisionMaskB();
            }

        }

        private Classes.Editor.Draw.GIF DrawCollisionMaskA()
        {
            Bitmap bitmap = new Bitmap(16, 16384);
            using (Graphics gfx = Graphics.FromImage(bitmap))
            {
                for (int i = 0; i < 1024; i++)
                {
                    gfx.DrawImage(Config.CollisionPath1[i].DrawCMask(Color.FromArgb(0, 0, 0, 0), Color.White), new Rectangle(0,(16 * i),16,16));
                }
            }
            Classes.Editor.Draw.GIF gif = new Classes.Editor.Draw.GIF(bitmap);
            return gif;
        }

        private Classes.Editor.Draw.GIF DrawCollisionMaskB()
        {
            Bitmap bitmap = new Bitmap(16, 16384);
            using (Graphics gfx = Graphics.FromImage(bitmap))
            {
                for (int i = 0; i < 1024; i++)
                {
                    gfx.DrawImage(Config.CollisionPath2[i].DrawCMask(Color.FromArgb(0, 0, 0, 0), Color.White), new Rectangle(0, (16 * i), 16, 16));
                }
            }
            Classes.Editor.Draw.GIF gif = new Classes.Editor.Draw.GIF(bitmap);
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
			Image = new Classes.Editor.Draw.GIF(Path.Combine(Environment.CurrentDirectory, "16x16Tiles_ID.gif"));
			Config = new RSDKv5.Tileconfig();
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
