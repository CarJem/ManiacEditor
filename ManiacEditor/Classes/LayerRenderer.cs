using System;
using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace ManiacEditor.Classes
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

        private int height;
        private int width;

        /// <summary>
        /// Points to the tile in the top left corner
        /// </summary>
        private Vector2i offset;
        private Vertex[] vertices;

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
                    Refresh(x + offset.X, y + offset.Y);
                }
        }

        /// <summary>
        /// Ensures that vertex array has enough space
        /// </summary>
        /// <param name="v">Size of the visible area</param>
        private void SetSize(Vector2f v)
        {
            var w = (int)(v.X / TileSize) + 2;
            var h = (int)(v.Y / TileSize) + 2;
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
            if (x < offset.X || x >= offset.X + width || y < offset.Y || y >= offset.Y + height)
                return; //check if tile is visible

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
            fixed (Vertex* fptr = vertices)
            {
                //use pointers to avoid array bound checks (optimization)

                var ptr = fptr + index;

                ptr->Position.X = rec.Left;
                ptr->Position.Y = rec.Top;
                ptr->TexCoords.X = src.Left;
                ptr->TexCoords.Y = src.Top;
                ptr->Color = color;
                ptr++;

                ptr->Position.X = rec.Left + rec.Width;
                ptr->Position.Y = rec.Top;
                ptr->TexCoords.X = src.Left + src.Width;
                ptr->TexCoords.Y = src.Top;
                ptr->Color = color;
                ptr++;

                ptr->Position.X = rec.Left + rec.Width;
                ptr->Position.Y = rec.Top + rec.Height;
                ptr->TexCoords.X = src.Left + src.Width;
                ptr->TexCoords.Y = src.Top + src.Height;
                ptr->Color = color;
                ptr++;

                ptr->Position.X = rec.Left;
                ptr->Position.Y = rec.Top + rec.Height;
                ptr->TexCoords.X = src.Left;
                ptr->TexCoords.Y = src.Top + src.Height;
                ptr->Color = color;
            }
        }

        /// <summary>
        /// Update offset (based on Target's position) and draw it
        /// </summary>
        public void Draw(RenderTarget rt, RenderStates states)
        {
            var view = rt.GetView();

            SetSize(view.Size);
            SetCorner(rt.MapPixelToCoords(new Vector2i()));
            states.Texture = texture;
            rt.Draw(vertices, PrimitiveType.Quads, states);

        }
    }
}