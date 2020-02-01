using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIVsCharSelector : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Edit.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            string text = "Text" + Classes.Editor.SolutionState.CurrentLanguage;
            int playerID = (int)entity.attributesMap["playerID"].ValueUInt8;
            int player = 8;
            switch (playerID)
            {
                case 0:
                    player = 8;
                    break;
                case 1:
                    player = 9;
                    break;
                case 2:
                    player = 10;
                    break;
                case 3:
                    player = 11;
                    break;

            }
            var editorAnimWaiting = Editor.Instance.EntityDrawing.LoadAnimation(text, d.DevicePanel, 12, 7, false, false, false);
            var editorAnimBackground = Editor.Instance.EntityDrawing.LoadAnimation("SaveSelect", d.DevicePanel, 14, 7, false, false, false);
            var editorAnimPlayerText = Editor.Instance.EntityDrawing.LoadAnimation(text, d.DevicePanel, 12, player, false, false, false);
            var editorAnimFrame = Editor.Instance.EntityDrawing.LoadAnimation("EditorUIRender", d.DevicePanel, 1, 0, false, false, false);

            d.DrawRectangle(x - 48, y - 48, x + 48, y + 48, System.Drawing.Color.FromArgb(128, 255, 255, 255));

            if (editorAnimFrame != null && editorAnimFrame.Frames.Count != 0)
            {
                var frame = editorAnimFrame.Frames[Animation.index];
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }

            if (editorAnimBackground != null && editorAnimBackground.Frames.Count != 0)
            {
                var frame = editorAnimBackground.Frames[Animation.index];
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY - 8,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }

            if (editorAnimWaiting != null && editorAnimWaiting.Frames.Count != 0)
            {
                var frame = editorAnimWaiting.Frames[Animation.index];
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY - 8,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }

            if (editorAnimPlayerText != null && editorAnimPlayerText.Frames.Count != 0)
            {
                var frame = editorAnimPlayerText.Frames[Animation.index];
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX + 36, y + frame.Frame.PivotY - 38,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }





        }

        public override string GetObjectName()
        {
            return "UIVsCharSelector";
        }
    }
}
