using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class TimeAttackGate : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            Classes.Core.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Core.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            EditorAnimations Animation = properties.Animations;
            bool selected  = properties.isSelected;
            bool finish = entity.attributesMap["finishLine"].ValueBool;
            var editorAnimBase = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("SpeedGate", d.DevicePanel, 0, 0, false, false, false);
            var editorAnimTop = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("SpeedGate", d.DevicePanel, 1, 0, false, false, false);
            var editorAnimFins = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("SpeedGate", d.DevicePanel, finish ? 4 : 3, -1, false, false, false);
            if (editorAnimBase != null && editorAnimTop != null && editorAnimFins != null && editorAnimFins.Frames.Count != 0 && editorAnimTop.Frames.Count != 0 && editorAnimTop.Frames.Count != 0)
            {
                var frameBase = editorAnimBase.Frames[0];
                var frameTop = editorAnimTop.Frames[0];
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frameBase), x + frameBase.Frame.PivotX, y + frameBase.Frame.PivotY,
                    frameBase.Frame.Width, frameBase.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frameTop), x + frameTop.Frame.PivotX, y + frameTop.Frame.PivotY,
                    frameTop.Frame.Width, frameTop.Frame.Height, false, Transparency);
                for (int i = 0; i < editorAnimFins.Frames.Count; ++i)
                {
                    var frame = editorAnimFins.Frames[i];
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
            }
        }

        public override string GetObjectName()
        {
            return "TimeAttackGate";
        }
    }
}
