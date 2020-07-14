﻿using System;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using ManiacEditor.Classes.Scene;

namespace ManiacEditor.Classes.Rendering
{
    /// <summary>
    /// Functions that provides color/texture rectangle data from tile map (or other source)
    /// </summary>
    public delegate void TileProvider(int x, int y, int layer, out Color color, out IntRect rec);

    /// <summary>
    /// Fast and universal renderer of tilemaps
    /// </summary>
    public class LayerRenderer : Drawable
    {
        private readonly float TileSize;
        public readonly int Layers;

        public double Zoom
        {
            get
            {
                return Methods.Solution.SolutionState.Main.Zoom;
            }
        }

        private int height;
        private int width;

        /// <summary>
        /// Points to the tile in the top left corner
        /// </summary>
        private Vector2i offset;
        private Vertex[] vertices;
        public Vector2f Position { get; set; } = new Vector2f(0, 0);
        public Vector2f Size { get; set; } = new Vector2f(0, 0);

        /// <summary>
        /// Provides Color and Texture Source from tile map
        /// </summary>
        private TileProvider provider;

        /// <summary>
        /// Holds spritesheet
        /// </summary>
        private Texture texture;

        /// <param name="texture">Spritesheet</param>
        /// <param name="provider">Accesor to tilemap data</param>
        /// <param name="tileSize">Size of one tile</param>
        /// <param name="layers">Numbers of layers</param>
        public LayerRenderer(Texture texture, TileProvider provider, float tileSize = 16, int layers = 1)
        {
            if (provider == null || layers <= 0) throw new ArgumentException();
            this.provider = provider;

            TileSize = tileSize;
            Layers = layers;

            vertices = new Vertex[0];
            this.texture = texture;

        }

        /// <summary>
        /// Redraws whole screen
        /// </summary>
        public void Refresh()
        {
            RefreshLocal(0, 0, width, height);
        }

        private void RefreshLocal(int left, int top, int right, int bottom)
        {
            for (var y = top; y < bottom; y++)
                for (var x = left; x < right; x++)
                {
                    Refresh(x + (int)(offset.X / Zoom), y + (int)(offset.Y / Zoom));
                }
        }

        /// <summary>
        /// Ensures that vertex array has enough space
        /// </summary>
        /// <param name="v">Size of the visible area</param>
        private void SetSize(Vector2f v)
        {
            var w = (int)((v.X / Zoom) / TileSize) + 2;
            var h = (int)((v.Y / Zoom) / TileSize) + 2;
            if (w == width && h == height) return;

            width = w;
            height = h;

            vertices = new Vertex[width * height * 4 * Layers];
            Refresh();
        }

        /// <summary>
        /// Sets offset
        /// </summary>
        /// <param name="v">World position of top left corner of the screen</param>
        private void SetCorner(Vector2f v)
        {
            var tile = GetTile(v);
            var dif = tile - offset;
            if (dif.X == 0 && dif.Y == 0) return;
            offset = tile;

            if (Math.Abs(dif.X) > width / 4 || Math.Abs(dif.Y) > height / 4)
            {
                //Refresh everyting if difference is too big
                Refresh();
                return;
            }

            //Refresh only tiles that appeared since last update

            if (dif.X > 0) RefreshLocal(width - dif.X, 0, width, height);
            else RefreshLocal(0, 0, -dif.X, height);

            if (dif.Y > 0) RefreshLocal(0, height - dif.Y, width, height);
            else RefreshLocal(0, 0, width, -dif.Y);
        }

        /// <summary>
        /// Transforms from world size to to tile that is under that position
        /// </summary>
        private Vector2i GetTile(Vector2f pos)
        {
            var x = (int)(pos.X / TileSize);
            var y = (int)(pos.Y / TileSize);
            if (pos.X < 0) x--;
            if (pos.Y < 0) y--;
            return new Vector2i(x, y);
        }

        /// <summary>
        /// Redraws one tile
        /// </summary>
        /// <param name="x">X coord of the tile</param>
        /// <param name="y">Y coord of the tile</param>
        public void Refresh(int x, int y)
        {
            if (x < (int)(offset.X / Zoom) || x >= (int)(offset.X / Zoom) + (int)(width) || y < (int)(offset.Y / Zoom) || y >= (int)(offset.Y / Zoom) + (int)(height)) return; //check if tile is visible

            //vertices works like 2d ring buffer
            var vx = x % width;
            var vy = y % height;
            if (vx < 0) vx += width;
            if (vy < 0) vy += height;

            var index = (vx + vy * width) * 4 * Layers;
            var rec = new FloatRect(x * TileSize, y * TileSize, TileSize, TileSize);

            for (int i = 0; i < Layers; i++)
            {
                Color color;
                IntRect src;
                provider(x, y, i, out color, out src);

                Draw(index, rec, src, color);
                index += 4;
            }
        }

        /// <summary>
        /// Inserts color and texture data into vertex array
        /// </summary>
        private unsafe void Draw(int index, FloatRect rec, IntRect src, Color color)
        {
            float rec_left = Position.X + (float)(rec.Left);
            float rec_top = Position.Y + (float)(rec.Top);
            float rec_width = (float)(rec.Width);
            float rec_height = (float)(rec.Height);

            float zoom = (float)Zoom;

            int src_left = src.Left;
            int src_top = src.Top;
            int src_width = src.Width;
            int src_height = src.Height;

            fixed (Vertex* fptr = vertices)
            {
                //use pointers to avoid array bound checks (optimization)


                var ptr = fptr + index;

                ptr->Position.X = rec_left * zoom;
                ptr->Position.Y = rec_top * zoom;
                ptr->TexCoords.X = src_left;
                ptr->TexCoords.Y = src_top;
                ptr->Color = color;
                ptr++;

                ptr->Position.X = (rec_left + rec_width) * zoom;
                ptr->Position.Y = (rec_top) * zoom;
                ptr->TexCoords.X = (src_left + src_width);
                ptr->TexCoords.Y = (src_top);
                ptr->Color = color;
                ptr++;

                ptr->Position.X = (rec_left + rec_width) * zoom;
                ptr->Position.Y = (rec_top + rec_height) * zoom;
                ptr->TexCoords.X = (src_left + src_width);
                ptr->TexCoords.Y = (src_top + src_height);
                ptr->Color = color;
                ptr++;

                ptr->Position.X = (rec_left) * zoom;
                ptr->Position.Y = (rec_top + rec_height) * zoom;
                ptr->TexCoords.X = (src_left);
                ptr->TexCoords.Y = (src_top + src_height);
                ptr->Color = color;
            }
        }

        /// <summary>
        /// Update offset (based on Target's position) and draw it
        /// </summary>
        public void Draw(RenderTarget rt, RenderStates states)
        {
            if (Position.X == 0 && Position.Y == 0)
            {
                var view = rt.GetView();
                SetSize(view.Size);
                SetCorner(rt.MapPixelToCoords(new Vector2i()));
                states.Texture = texture;
                rt.Draw(vertices, PrimitiveType.Quads, states);
            }
            else
            {
                SetSize(Size);
                SetCorner(new Vector2f());
                states.Texture = texture;
                rt.Draw(vertices, PrimitiveType.Quads, states);
            }

        }
    }
}