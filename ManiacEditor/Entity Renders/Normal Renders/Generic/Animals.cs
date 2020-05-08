using RSDKv5;
using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class Animals : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            bool fliph = false;
            bool flipv = false;
            int type = (int)e.attributesMap["type"].ValueEnum;
            var Animation = Methods.Draw.ObjectDrawing.LoadAnimation(Properties.Graphics, "Animals", type, 0);
            DrawTexturePivotNormal(Properties.Graphics, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency);
        }

        public override string GetObjectName()
        {
            return "Animals";
        }
    }
}
