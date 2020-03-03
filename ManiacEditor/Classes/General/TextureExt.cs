using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D9;

namespace ManiacEditor.Classes.General
{
    public class TextureExt : SharpDX.Direct3D9.Texture
    {
        public int TextureWidth { get; }
        public int TextureHeight { get; }
        public TextureExt(IntPtr nativePtr) : base(nativePtr)
        {

        }
        public TextureExt(Device device, int width, int height, int levelCount, Usage usage, Format format, Pool pool) : base(device, width, height, levelCount, usage, format, pool)
        {
            TextureWidth = width;
            TextureHeight = height;
        }
        public TextureExt(Device device, int width, int height, int levelCount, Usage usage, Format format, Pool pool, ref IntPtr sharedHandle) : base(device, width, height, levelCount, usage, format, pool, ref sharedHandle)
        {
            TextureWidth = width;
            TextureHeight = height;
        }
    }
}
