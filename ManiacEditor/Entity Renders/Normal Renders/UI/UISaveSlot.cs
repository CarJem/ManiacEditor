using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UISaveSlot : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            Methods.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            Methods.Entities.EntityAnimator Animation = properties.Animations;
            bool selected  = properties.isSelected;

            //int frameID = (int)entity.attributesMap["listID"].ValueEnum;
            int type = (int)entity.attributesMap["type"].ValueEnum;
            string text = "Text" + Methods.Editor.SolutionState.CurrentLanguage;
            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation("SaveSelect", d.DevicePanel, 0, 0, false, false, false);
            var editorAnimBorder = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation("SaveSelect", d.DevicePanel, 0, 1, false, false, false);
            var editorAnimBackground = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation("SaveSelect", d.DevicePanel, 0, 2, false, false, false);
            var editorAnimActualRender = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation("EditorUIRender", d.DevicePanel, 3, 0, false, false, false);
            var editorAnimActualRenderBorder = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation("EditorUIRender", d.DevicePanel, 3, 1, false, false, false);
            var editorAnimText = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation(text, d.DevicePanel, 2, 0, false, false, false);
            var editorAnimNoSave = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation(text, d.DevicePanel, 2, 2, false, false, false);
            if (type == 1)
            {
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    var frame = editorAnim.Frames[Animation.index];
                    d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
                if (editorAnimBorder != null && editorAnimBorder.Frames.Count != 0)
                {
                    var frame = editorAnimBorder.Frames[Animation.index];
                    d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
                if (editorAnimBackground != null && editorAnimBackground.Frames.Count != 0)
                {
                    var frame = editorAnimBackground.Frames[Animation.index];
                    d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
                if (editorAnimText != null && editorAnimText.Frames.Count != 0)
                {
                    var frame = editorAnimText.Frames[Animation.index];
                    d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
                if (editorAnimNoSave != null && editorAnimNoSave.Frames.Count != 0 && type == 1)
                {
                    var frame = editorAnimNoSave.Frames[Animation.index];
                    d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
            }
            else
            {
                if (editorAnimActualRender != null && editorAnimActualRender.Frames.Count != 0)
                {
                    var frame = editorAnimActualRender.Frames[Animation.index];
                    d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    if (editorAnimBackground != null && editorAnimBackground.Frames.Count != 0)
                    {
                        var frame2 = editorAnimBackground.Frames[Animation.index];
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame2), x + frame2.Frame.PivotX, y + frame2.Frame.PivotY + (frame.Frame.PivotY / 2) - 6,
                            frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                    }
                    if (editorAnimText != null && editorAnimText.Frames.Count != 0)
                    {
                        var frame2 = editorAnimText.Frames[Animation.index];
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame2), x + frame2.Frame.PivotX, y + frame2.Frame.PivotY + (frame.Frame.PivotY / 2) - 6,
                            frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                    }
                }

                if (editorAnimActualRenderBorder != null && editorAnimActualRenderBorder.Frames.Count != 0)
                {
                    var frame = editorAnimActualRenderBorder.Frames[Animation.index];
                    d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
            }



        }

        public override string GetObjectName()
        {
            return "UISaveSlot";
        }
    }
}
