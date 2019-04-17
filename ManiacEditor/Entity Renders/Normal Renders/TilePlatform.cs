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
using System.Windows.Forms;
using System.Diagnostics;
using IronPython.Compiler.Ast;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ManiacEditor.Entity_Renders
{
    public class TilePlatform : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;



            int width = (int)entity.attributesMap["size"].ValuePosition.X.High;
            int height = (int)entity.attributesMap["size"].ValuePosition.Y.High;
            int TargetX = (int)entity.attributesMap["targetPos"].ValuePosition.X.High;
            int TargetY = (int)entity.attributesMap["targetPos"].ValuePosition.Y.High;
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("EditorIcons2", d.DevicePanel, 0, 7, fliph, flipv, false);

            width = RoundNum(width, EditorConstants.TILE_SIZE) / EditorConstants.TILE_SIZE;
            height = RoundNum(height, EditorConstants.TILE_SIZE) / EditorConstants.TILE_SIZE;
            TargetX = RoundNum(TargetX, EditorConstants.TILE_SIZE) / EditorConstants.TILE_SIZE;
            TargetY = RoundNum(TargetY, EditorConstants.TILE_SIZE) / EditorConstants.TILE_SIZE;

            //d.DrawRectangle(x - width / 2, y - height / 2, x + width/2, y + height/2, System.Drawing.Color.White);
            // The position for some platforms are still off a bit (but it's very decent)
            if (Editor.Instance.EditorScene?.Move != null)
            {
                SceneLayer Move = Editor.Instance.EditorScene?.Move.Layer;

                DrawPlatformTiles(d, Move, x, y, TargetX, TargetY, width, height, Transparency, e.Selected);

            }
        }

        private void DrawPlatformTiles(GraphicsHandler d, SceneLayer _layer, int ObjectX, int ObjectY, int CenterX, int CenterY, int width, int height, int Transperncy, bool selected = false)
        {
            //Rectangle rect = GetTileArea(x2, y2, width, height, _layer);

            if (width == 0 || height == 0) return;

            int x = ObjectX - ((width * EditorConstants.TILE_SIZE) / 2);
            int y = ObjectY - ((height * EditorConstants.TILE_SIZE) / 2);

            int TargetX = CenterX - width / 2;
            int TargetY = CenterY - height / 2;

            if (TargetY < 0 || TargetX < 0 || CenterX + width > GetColumnLength(_layer.Tiles, 1) || CenterY + height > GetColumnLength(_layer.Tiles, 0)) return;
            for (int ty = 0; ty < height; ++ty)
            {
                for (int tx = 0; tx < width; ++tx)
                {
                    int TileX = TargetX + tx;
                    int TileY = TargetY + ty;

                    // We will draw those later
                    if (_layer.Tiles?[TileY][TileX] != 0xffff)
                    {
                        DrawTile(d, _layer.Tiles[TileY][TileX], x + (tx * EditorConstants.TILE_SIZE), y + (ty * EditorConstants.TILE_SIZE), selected, Transperncy);
                    }
                }
            }
        }

        private int GetColumnLength(ushort[][] jaggedArray, int columnIndex)
        {
            int count = 0;
            foreach (ushort[] row in jaggedArray)
            {
                if (columnIndex < row.Length) count++;
            }
            return count;
        }

        private Rectangle GetTileArea(int x, int y, int width, int height, SceneLayer _layer)
        {
            int startX = x - width;
            int endX = x + width;

            int startY = y - height;
            int endY = y + height;

            return new Rectangle(startX, startY, endX, endY);
        }


        public void DrawTile(GraphicsHandler d, ushort tile, int x, int y, bool selected, int Transperncy)
        {
            bool flipX = ((tile >> 10) & 1) == 1;
            bool flipY = ((tile >> 11) & 1) == 1;
            d.DrawBitmap(Editor.Instance.EditorTiles.StageTiles.Image.GetTexture(d.DevicePanel._device, new Rectangle(0, (tile & 0x3ff) * EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE), flipX, flipY),
            x, y, EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE, selected, Transperncy);
        }

        private int RoundNum(int num, int step)
        {
            if (num >= 0)
                return ((num + (step / 2)) / step) * step;
            else
                return ((num - (step / 2)) / step) * step;
        }




        public override string GetObjectName()
        {
            return "TilePlatform";
        }
    }
}
