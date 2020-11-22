using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor.Classes.Rendering
{
    public class ChunkVBO : IDisposable
    {
        public bool IsReady = false;
        public Classes.Rendering.TextureExt Texture;
        public bool HasBeenRendered = false;
        public bool HasBeenSelectedPrior = false;
        public bool Empty = false;

        public void Dispose()
        {
            if (this.Texture != null)
            {
                this.Texture.Dispose();
                this.Texture = null;
            }
            this.IsReady = false;
            this.HasBeenSelectedPrior = false;
        }
    }
}
