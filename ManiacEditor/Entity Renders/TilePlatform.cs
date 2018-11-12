using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ManiacEditor;
using System.Drawing;
using SharpDX.Direct3D9;
using RSDKv5;
using CubeBuild.Helpers;
using System.Windows.Forms;
using System.Diagnostics;
using IronPython.Compiler.Ast;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ManiacEditor.Entity_Renders
{
    public class TilePlatform : EntityRenderer
    {
            internal EditorLayer Move => Editor.Instance.EditorScene?.Move;

            private SceneLayer _layer;
            internal SceneLayer Layer { get => _layer; }

            static int TILE_SIZE = 16;

            public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
            {
                _layer = Move.Layer;
                bool fliph = false;
                bool flipv = false;
                int dev = Properties.EditorState.Default.developerInt;
                int width = (int)entity.attributesMap["size"].ValuePosition.X.High;
                int height = (int)entity.attributesMap["size"].ValuePosition.Y.High;
                int x2 = (int)entity.attributesMap["targetPos"].ValuePosition.X.High;
                int y2 = (int)entity.attributesMap["targetPos"].ValuePosition.Y.High;
                var editorAnim = e.LoadAnimation2("EditorIcons2", d, 0, 7, fliph, flipv, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    d.DrawRectangle(x - width / 2, y - height / 2, x + width/2, y + height/2, System.Drawing.Color.White);
                    DrawTileGroup(d, x, y, x2 / 16, y2 / 16, height / 16, width / 16, Transparency, entity);

                }
            }

        public void DrawTileGroup(DevicePanel d, int x, int y, int x2, int y2, int height, int width, int Transperncy, SceneEntity entity)
        {
            Texture GroupTexture;
            //Rectangle rect = GetTileArea(x2 - width/2, y2 - height/2, width, height);
            Rectangle rect = GetTileArea(0, 0, _layer.Height, _layer.Width);

            try
            {
                using (Bitmap bmp = new Bitmap(rect.Width * 16, rect.Height * 16, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        for (int ty = y2 - height/2; ty <= y2 + height/2; ++ty)
                        {
                            for (int tx = x2 - width/2; tx <= x2 + width/2; ++tx)
                            {
                                // We will draw those later
                                if (this._layer.Tiles?[ty][tx] != 0xffff)
                                {
                                    //DrawTile(g, this._layer.Tiles[ty][tx], (x/16) + tx - x2, (y/16) + ty - y2, false, Transperncy);
                                    DrawTile(g, this._layer.Tiles[ty][tx], tx, ty);
                                }
                            }
                        }
                    }
                    GroupTexture = TextureCreator.FromBitmap(d._device, bmp);
                    d.DrawBitmap(GroupTexture, x - x2 - height / 2, y - y2 - height / 2, bmp.Width, bmp.Height, false, Transperncy);
                }

            }
            catch
            {

            }

        }

        public void DrawTile(DevicePanel d, ushort tile, int x, int y, bool selected, int Transperncy)
        {
            bool flipX = ((tile >> 10) & 1) == 1;
            bool flipY = ((tile >> 11) & 1) == 1;
            d.DrawBitmap(Editor.Instance.StageTiles.Image.GetTexture(d._device, new Rectangle(0, (tile & 0x3ff) * TILE_SIZE, TILE_SIZE, TILE_SIZE), flipX, flipY),
            x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE, selected, Transperncy);
        }
        public void DrawTile(Graphics g, ushort tile, int x, int y)
        {
            ushort TileIndex = (ushort)(tile & 0x3ff);
            int TileIndexInt = (int)TileIndex;
            bool flipX = ((tile >> 10) & 1) == 1;
            bool flipY = ((tile >> 11) & 1) == 1;
            bool SolidTopA = ((tile >> 12) & 1) == 1;
            bool SolidLrbA = ((tile >> 13) & 1) == 1;
            bool SolidTopB = ((tile >> 14) & 1) == 1;
            bool SolidLrbB = ((tile >> 15) & 1) == 1;

            g.DrawImage(Editor.Instance.StageTiles.Image.GetBitmap(new Rectangle(0, TileIndex * TILE_SIZE, TILE_SIZE, TILE_SIZE), flipX, flipY),
                new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE));
        }

            private Rectangle GetTileArea(int x, int y, int width, int height)
        {
            int y_start = y * height;
            int y_end = Math.Min((y + 1) * height, _layer.Height);

            int x_start = x * width;
            int x_end = Math.Min((x + 1) * width, _layer.Width);

            return new Rectangle(x, y,width,height);
        }


        public override string GetObjectName()
        {
            return "TilePlatform";
        }
    }
}
