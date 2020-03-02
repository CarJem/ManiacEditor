using System;
using SharpDX.Direct3D9;

namespace ManiacEditor.EventHandlers
{
    public class TileConfigException : Exception
    {
        public TileConfigException(string message) : base(message)
        {

        }
    }
}
