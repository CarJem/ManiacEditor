using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UITABanner : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            var editorAnimFrame = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation("EditorUIRender", d.DevicePanel, 2, 0, false, false, false);
            var editorAnimBackground = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation("SaveSelect", d.DevicePanel, 7, -1, false, false, false);

            if (editorAnimBackground != null && editorAnimBackground.Frames.Count != 0)
            {
                var frame = editorAnimBackground.Frames[Animation.index];
                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX - 107, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }

            if (editorAnimFrame != null && editorAnimFrame.Frames.Count != 0)   
            {
                var frame = editorAnimFrame.Frames[0];
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }



        }

        public override string GetObjectName()
        {
            return "UITABanner";
        }
    }
}
