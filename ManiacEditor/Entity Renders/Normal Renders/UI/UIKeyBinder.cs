using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIKeyBinder : EntityRenderer
    {
        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            string text = "Text" + Classes.Editor.SolutionState.CurrentLanguage;
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int inputID = (int)entity.attributesMap["inputID"].ValueUInt8;
            int width = 48;
            int height = 12;
            int frameID = 1;
            int listID = 0;
            switch (type)
            {
                case 0:
                    frameID = 7;
                    break;
                case 1:
                    frameID = 8;
                    break;
                case 2:
                    frameID = 9;
                    break;
                case 3:
                    frameID = 10;
                    break;
                case 4:
                    frameID = 13;
                    break;
                case 5:
                    frameID = 1;
                    break;
                case 6:
                    frameID = 3;
                    break;
                case 7:
                    frameID = 11;
                    break;
                case 8:
                    frameID = 12;
                    break;


            }

            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation(text, d.DevicePanel, listID, frameID, false, false, false);
            var editorAnimKey = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation("Buttons", d.DevicePanel, 1, 0, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                e.DrawUIButtonBack(d, x, y, width, height, frame.Frame.Width, frame.Frame.Height, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (editorAnimKey != null && editorAnimKey.Frames.Count != 0)
            {
                var frame = editorAnimKey.Frames[Animation.index];
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX - 16, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }


        }

        public override string GetObjectName()
        {
            return "UIKeyBinder";
        }
    }
}
