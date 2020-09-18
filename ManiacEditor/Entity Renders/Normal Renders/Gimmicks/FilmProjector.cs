using System;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class FilmProjector : EntityRenderer
    {
        public DateTime lastFrametime;
        public int index = 0;

        public override void Draw(Structures.EntityRenderProp properties)
        {
            DevicePanel d = properties.Graphics;
            Classes.Scene.EditorEntity entity = properties.EditorObject; 
            int x = properties.DrawX;
            int y = properties.DrawY;
            int Transparency = properties.Transparency;
            bool fliph = false;
            bool flipv = false;

            var editorAnim5 = LoadAnimation("SPZ1/FilmProjector.bin", d, 2, 0);
            DrawTexturePivotNormal(d, editorAnim5, editorAnim5.RequestedAnimID, editorAnim5.RequestedFrameID, x + 42, y - 68, Transparency, false, false);
            DrawTexturePivotNormal(d, editorAnim5, editorAnim5.RequestedAnimID, editorAnim5.RequestedFrameID, x - 60, y - 68, Transparency, false, false);
            var editorAnim4 = LoadAnimation("SPZ1/FilmProjector.bin", d, 1, 0);
            DrawTexturePivotNormal(d, editorAnim4, editorAnim4.RequestedAnimID, editorAnim4.RequestedFrameID, x + 42, y - 68, Transparency, false, false);
            DrawTexturePivotNormal(d, editorAnim4, editorAnim4.RequestedAnimID, editorAnim4.RequestedFrameID, x - 60, y - 68, Transparency, false, false);
            var editorAnim = LoadAnimation("SPZ1/FilmProjector.bin", d, 0, 0);
            DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency, false, false);
            var editorAnim2 = LoadAnimation("SPZ1/FilmProjector.bin", d, 0, 1);
            DrawTexturePivotNormal(d, editorAnim2, editorAnim2.RequestedAnimID, editorAnim2.RequestedFrameID, x + 185, y, Transparency, false, false);
            var editorAnim3 = LoadAnimation("SPZ1/FilmProjector.bin", d, 3, 0);
            DrawTexturePivotNormal(d, editorAnim3, editorAnim3.RequestedAnimID, editorAnim3.RequestedFrameID, x + 185, y, Transparency, false, false);            
        }

        public override string GetObjectName()
        {
            return "FilmProjector";
        }
    }
}
