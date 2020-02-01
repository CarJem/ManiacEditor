using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class EggLoco : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("Train", d.DevicePanel, 0, -1, fliph, flipv, false);
            var editorAnim2 = Editor.Instance.EntityDrawing.LoadAnimation2("Train", d.DevicePanel, 1, -1, fliph, flipv, false);
            var editorAnim3 = Editor.Instance.EntityDrawing.LoadAnimation2("Train", d.DevicePanel, 2, -1, fliph, flipv, false);
            var editorAnim4 = Editor.Instance.EntityDrawing.LoadAnimation2("Train", d.DevicePanel, 5, 0, false, false, false);
            var editorAnim5 = Editor.Instance.EntityDrawing.LoadAnimation2("Train", d.DevicePanel, 7, -1, fliph, flipv, false);
            var editorAnim6 = Editor.Instance.EntityDrawing.LoadAnimation2("Train", d.DevicePanel, 6, 0, false, false, false);
            var editorAnim7 = Editor.Instance.EntityDrawing.LoadAnimation2("Train", d.DevicePanel, 3, -1, fliph, flipv, false);
            var editorAnim8 = Editor.Instance.EntityDrawing.LoadAnimation2("Train", d.DevicePanel, 4, -1, fliph, flipv, false);
            var editorAnim9 = Editor.Instance.EntityDrawing.LoadAnimation2("Train", d.DevicePanel, 10, -1, fliph, flipv, false);
            var editorAnim10 = Editor.Instance.EntityDrawing.LoadAnimation2("Train", d.DevicePanel, 12, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0 && editorAnim4 != null && editorAnim4.Frames.Count != 0 && editorAnim5 != null && editorAnim5.Frames.Count != 0 && editorAnim6 != null && editorAnim6.Frames.Count != 0 && editorAnim7 != null && editorAnim7.Frames.Count != 0 && editorAnim8 != null && editorAnim8.Frames.Count != 0 && editorAnim9 != null && editorAnim9.Frames.Count != 0 && editorAnim10 != null && editorAnim10.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[0];
                var frame3 = editorAnim2.Frames[1];
                var frame4 = editorAnim3.Frames[Animation.index2];
                var frame5 = editorAnim4.Frames[0];
                var frame6 = editorAnim5.Frames[Animation.index];
                var frame7 = editorAnim6.Frames[0];
                var frame8 = editorAnim7.Frames[0];
                var frame9 = editorAnim7.Frames[1];
                var frame10 = editorAnim8.Frames[0];
                var frame11 = editorAnim8.Frames[1];
                var frame12 = editorAnim9.Frames[Animation.index3];
                var frame13 = editorAnim10.Frames[0];

                Animation.ProcessAnimation(frame6.Entry.SpeedMultiplyer, frame6.Entry.Frames.Count, frame6.Frame.Delay);
                Animation.ProcessAnimation2(frame4.Entry.SpeedMultiplyer, frame4.Entry.Frames.Count, frame4.Frame.Delay);
                Animation.ProcessAnimation3(frame12.Entry.SpeedMultiplyer, frame12.Entry.Frames.Count, frame12.Frame.Delay);

                //Eggman
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame12),
                    x + frame12.Frame.PivotX,
                    y + frame12.Frame.PivotY,
                    frame12.Frame.Width, frame12.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame13),
                    x + frame13.Frame.PivotX,
                    y + frame13.Frame.PivotY,
                    frame13.Frame.Width, frame13.Frame.Height, false, Transparency);

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame2),
                    x + frame2.Frame.PivotX - 104,
                    y + frame2.Frame.PivotY + 56,
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame2),
                    x + frame2.Frame.PivotX - 30,
                    y + frame2.Frame.PivotY + 56,
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame3),
                    x + frame3.Frame.PivotX - 104,
                    y + frame3.Frame.PivotY + 56,
                    frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame3),
                    x + frame3.Frame.PivotX - 30,
                    y + frame3.Frame.PivotY + 56,
                    frame3.Frame.Width, frame3.Frame.Height, false, Transparency);

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame4),
                    x + frame4.Frame.PivotX + 22,
                    y + frame4.Frame.PivotY + 74,
                    frame4.Frame.Width, frame4.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame4),
                    x + frame4.Frame.PivotX + 84,
                    y + frame4.Frame.PivotY + 74,
                    frame4.Frame.Width, frame4.Frame.Height, false, Transparency);

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame5),
                    x + frame5.Frame.PivotX,
                    y + frame5.Frame.PivotY,
                    frame5.Frame.Width, frame5.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame6),
                    x + frame6.Frame.PivotX,
                    y + frame6.Frame.PivotY,
                    frame6.Frame.Width, frame6.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame7),
                    x + frame7.Frame.PivotX,
                    y + frame7.Frame.PivotY,
                    frame7.Frame.Width, frame7.Frame.Height, false, Transparency);

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame9),
                    x + frame9.Frame.PivotX - 46,
                    y + frame9.Frame.PivotY + 54,
                    frame9.Frame.Width, frame9.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame8),
                    x + frame8.Frame.PivotX - 122,
                    y + frame8.Frame.PivotY + 54,
                    frame8.Frame.Width, frame8.Frame.Height, false, Transparency);

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame10),
                    x + frame10.Frame.PivotX + 7,
                    y + frame10.Frame.PivotY,
                    frame10.Frame.Width, frame10.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame11),
                    x + frame11.Frame.PivotX,
                    y + frame11.Frame.PivotY,
                    frame11.Frame.Width, frame11.Frame.Height, false, Transparency);




            }
        }

        public override string GetObjectName()
        {
            return "EggLoco";
        }
    }
}
