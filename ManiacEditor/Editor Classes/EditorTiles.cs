using RSDKv5;

namespace ManiacEditor
{
    public class EditorTiles
    {
        private Editor Instance;
        public StageTiles StageTiles;

        public EditorTiles(Editor instance)
        {
            Instance = instance;
        }

        public void Dispose()
        {
            if (StageTiles != null) StageTiles.Dispose();
        }

        public void DisposeTextures()
        {
            if (StageTiles != null) StageTiles.DisposeTextures();
        }
    }
}
