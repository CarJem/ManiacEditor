using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor.Entity_Renders
{
    public class TileObject
    {
        private int LayerIndex { get; set; } = 0;
        public TileObject(int _index)
        {
            LayerIndex = _index;
        }
        private Classes.Scene.EditorLayer ScatchLayer
        {
            get
            {
                try
                {
                    return Methods.Solution.CurrentSolution.CurrentScene?.AllLayers[LayerIndex];
                }
                catch
                {
                    return null;
                }

            }
        }
        private Classes.Rendering.TextureExt PlatformImage { get; set; }
        private ushort[][] TileMap { get; set; }
        public void DrawTileMap(DevicePanel d, int x, int y, int platform_x, int platform_y, int platform_width, int platform_height, System.Drawing.Color colors, int transparency, bool TilesNeedUpdate = false)
        {
            if (ScatchLayer != null)
            {
                if (ScatchLayer.Visible)
                {
                    if (TileMap == null || TileMap != ScatchLayer.Layer.Tiles || TilesNeedUpdate || PlatformImage == null)
                    {
                        if (PlatformImage != null)
                        {
                            PlatformImage.Dispose();
                            PlatformImage = null;
                        }

                        TileMap = ScatchLayer.Tiles;

                        var bitmap = new System.Drawing.Bitmap(ScatchLayer.Width * Methods.Solution.SolutionConstants.TILE_SIZE, ScatchLayer.Height * Methods.Solution.SolutionConstants.TILE_SIZE);
                        System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
                        ScatchLayer.Draw(g);
                        PlatformImage = Methods.Drawing.TextureCreator.FromBitmap(d._device, bitmap);
                        g.Clear(System.Drawing.Color.Transparent);
                        bitmap.Dispose();
                        g.Dispose();
                    }

                    d.DrawBitmap(PlatformImage, x, y, platform_x, platform_y, platform_width, platform_height, false, transparency, false, false, 0, colors);
                }
            }

        }
    }
}
