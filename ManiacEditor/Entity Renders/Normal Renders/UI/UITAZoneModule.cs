using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UITAZoneModule : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            Classes.Editor.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Editor.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            Methods.Entities.EntityAnimator Animation = properties.Animations;
            bool selected  = properties.isSelected;
            var editorAnimFrame = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation("EditorUIRender", d.DevicePanel, 0, 0, false, false, false);
            var editorAnimBackground = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation("SaveSelect", d.DevicePanel, 10, -1, false, false, false);

            if (editorAnimFrame != null && editorAnimFrame.Frames.Count != 0)
            {
                var frame = editorAnimFrame.Frames[0];
                d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }

            if (editorAnimBackground != null && editorAnimBackground.Frames.Count != 0)
            {
                var frame = editorAnimBackground.Frames[Animation.index];
                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX - 107, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }

            string text1 = entity.attributesMap["text1"].ValueString;
            string text2 = entity.attributesMap["text2"].ValueString;

            int listID = 3;
            int text_X = x - 46;
            int text_YAdjust = (text2 != "" ? 14 : 0);
            int text_Y = y - 8;

            int spacingAmount = 0;
            foreach (char symb in text1)
            {
                int frameID = GetFrameID(symb, Classes.Editor.SolutionState.MenuChar);
                var editorAnim2 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation("UIElements", d.DevicePanel, listID, frameID, false, false, false);
                if (editorAnim2 != null && editorAnim2.Frames.Count != 0)
                {
                    var frame = editorAnim2.Frames[0];
                    d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame), text_X + spacingAmount, text_Y + frame.Frame.PivotY - text_YAdjust,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    spacingAmount = spacingAmount + frame.Frame.Width;
                }
            }
            spacingAmount = 0;
            foreach (char symb in text2)
            {
                int frameID = GetFrameID(symb, Classes.Editor.SolutionState.MenuChar);
                var editorAnim2 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation("UIElements", d.DevicePanel, listID, frameID, false, false, false);
                if (editorAnim2 != null && editorAnim2.Frames.Count != 0)
                {
                    var frame = editorAnim2.Frames[0];
                    d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame), text_X + 32 + spacingAmount, text_Y + frame.Frame.PivotY + 28 - text_YAdjust,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    spacingAmount = spacingAmount + frame.Frame.Width;
                }
            }


        }

        public int GetFrameID(char letter, char[] arry)
        {
            char[] symArray = arry;
            int position = 0;
            foreach (char sym in symArray)
            {
                if (sym == letter) return position;
                position++;
            }
            return position;
        }

        public override string GetObjectName()
        {
            return "UITAZoneModule";
        }
    }
}
