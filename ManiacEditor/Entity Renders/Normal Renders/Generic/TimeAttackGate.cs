using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class TimeAttackGate : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            bool finish = e.attributesMap["finishLine"].ValueBool;

            var Animation1 = LoadAnimation("SpeedGate", d, 0, 0);
            DrawTexturePivotNormal(Properties.Graphics, Animation1, Animation1.RequestedAnimID, Animation1.RequestedFrameID, x, y, Transparency);
            var Animation2 = LoadAnimation("SpeedGate", d, 1, 0);
            DrawTexturePivotNormal(Properties.Graphics, Animation2, Animation2.RequestedAnimID, Animation2.RequestedFrameID, x, y, Transparency);

            var Animation3 = LoadAnimation("SpeedGate", d, (finish ? 4 : 3), 0);
            for (int FrameID = 0; FrameID < Animation3.RequestedAnimation.Frames.Count; FrameID++)
            {
                DrawTexturePivotNormal(Properties.Graphics, Animation3, Animation3.RequestedAnimID, FrameID, x, y, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "TimeAttackGate";
        }
    }
}
