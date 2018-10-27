﻿using System;
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

namespace ManiacEditor.Entity_Renders
{
    public class EncoreRoute : EntityRenderer
    {
        internal EditorLayer Scratch => Editor.Instance.EditorScene?.Scratch;

        private SceneLayer _layer;
        internal SceneLayer Layer { get => _layer; }

        static int TILE_SIZE = 16;

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            _layer = Scratch.Layer;
            bool fliph = false;
            bool flipv = false;
            int width = (int)entity.attributesMap["size"].ValuePosition.X.High;
            int height = (int)entity.attributesMap["size"].ValuePosition.Y.High;
            int x2 = (int)entity.attributesMap["offset"].ValuePosition.X.High;
            int y2 = (int)entity.attributesMap["offset"].ValuePosition.Y.High;

            // Prevents Out of Bounds with EncoreRoute
            if (entity.Object.Name.Name == "EncoreRoute")
            {
                bool outOfBoundsX = false;
                bool outOfBoundsY = false;
                if (x2 > Editor.Instance.ScratchLayer.Width)
                {
                    outOfBoundsX = true;
                }
                if (y2 > Editor.Instance.ScratchLayer.Height)
                {
                    outOfBoundsY = true;
                }
                if ((y2 + height) > Scratch.Layer.Height)
                {
                    if (outOfBoundsY)
                    {
                        MessageBox.Show("Layer Out of Bounds!    " + "\n" + "Y2: " + y2 + "\n" + "Height: " + height + "\n" + "Combined: " + (y2 + height) + "\n" + "Layer Height: " + Scratch.Layer.Height, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        entity.attributesMap["offset"].ValuePosition = new Position((short)x2, 0);
                    }
                    else
                    {
                        MessageBox.Show("Layer Out of Bounds!    " + "\n" + "Y2: " + y2 + "\n" + "Height: " + height + "\n" + "Combined: " + (y2 + height) + "\n" + "Layer Height: " + Scratch.Layer.Height, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        entity.attributesMap["size"].ValuePosition = new Position((short)width, 0);
                    }
                }
                if ((x2 + width) > Scratch.Layer.Width)
                {
                    if (outOfBoundsX)
                    {
                        MessageBox.Show("Layer Out of Bounds!    " + "\n" + "X2: " + x2 + "\n" + "Width: " + width + "\n" + "Combined: " + (x2 + width) + "\n" + "Layer Width: " + Scratch.Layer.Width, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        entity.attributesMap["offset"].ValuePosition = new Position(0, (short)y2);
                    }
                    else
                    {
                        MessageBox.Show("Layer Out of Bounds!    " + "\n" + "X2: " + x2 + "\n" + "Width: " + width + "\n" + "Combined: " + (x2 + width) + "\n" + "Layer Width: " + Scratch.Layer.Width);
                        entity.attributesMap["size"].ValuePosition = new Position(0, (short)height);
                    }
                }
            }

            var editorAnim = e.LoadAnimation2("EditorIcons2", d, 0, 7, fliph, flipv, false);

            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                //Draw the Encore Route Tiles
                DrawTileGroup(d, x / 16, y / 16, x2, y2, height, width, Transparency, entity);

                var frame = editorAnim.Frames[e.index];

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);



            }
        }
        public void DrawTileGroup(DevicePanel d, int x, int y, int x2, int y2, int height, int width, int Transperncy, SceneEntity entity)
        {

            Rectangle rect = GetTileArea(x2, y2, width, height);

            try
            {
                for (int ty = rect.Y; ty < rect.Y + rect.Height; ++ty)
                {
                    for (int tx = rect.X; tx < rect.X + rect.Width; ++tx)
                    {
                        // We will draw those later
                        if (this._layer.Tiles?[ty][tx] != 0xffff)
                        {
                            DrawTile(d, this._layer.Tiles[ty][tx], (x) + tx - x2, (y) + ty - y2, false, Transperncy);
                        }
                    }
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

        private Rectangle GetTileArea(int x, int y, int width, int height)
        {
            int y_start = y * height;
            int y_end = Math.Min((y + 1) * height, _layer.Height);

            int x_start = x * width;
            int x_end = Math.Min((x + 1) * width, _layer.Width);

            return new Rectangle(x, y, x + width, y + height);
        }

        public override string GetObjectName()
        {
            return "EncoreRoute";
        }
    }
}
