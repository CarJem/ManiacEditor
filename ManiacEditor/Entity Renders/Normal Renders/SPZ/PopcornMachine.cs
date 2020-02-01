using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class PopcornMachine : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            //int type = (int)entity.attributesMap["type"].ValueUInt8;
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int height = (int)entity.attributesMap["height"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            var editorAnim = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("PopcornMachine", d.DevicePanel, 0, 0, fliph, flipv, false);
            var editorAnim2 = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("PopcornMachine", d.DevicePanel, 0, 1, fliph, flipv, false);
            var editorAnim3 = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("PopcornMachine", d.DevicePanel, 0, 2, false, false, false);
            var editorAnim4 = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("PopcornMachine", d.DevicePanel, 0, 3, false, false, false);
            var editorAnim5 = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("PopcornMachine", d.DevicePanel, 0, 6, false, false, false);
            var editorAnim6 = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("PopcornMachine", d.DevicePanel, 0, 7, false, false, false);
            var editorAnim7 = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("PopcornMachine", d.DevicePanel, 0, 4, false, false, false);
            var editorAnim8 = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("PopcornMachine", d.DevicePanel, 0, 5, false, false, false);
            var editorAnim9 = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("PopcornMachine", d.DevicePanel, 0, 9, false, false, false);
            var editorAnim10 = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("PopcornMachine", d.DevicePanel, 0, 10, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0 && editorAnim4 != null && editorAnim4.Frames.Count != 0 && editorAnim5 != null && editorAnim5.Frames.Count != 0 && editorAnim6 != null && editorAnim6.Frames.Count != 0 && editorAnim7 != null && editorAnim7.Frames.Count != 0 && editorAnim8 != null && editorAnim8.Frames.Count != 0 && editorAnim9 != null && editorAnim9.Frames.Count != 0 && editorAnim10 != null && editorAnim10.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frame2 = editorAnim2.Frames[Animation.index];
                var frame3 = editorAnim3.Frames[Animation.index];
                var frame4 = editorAnim4.Frames[Animation.index];
                var frame5 = editorAnim5.Frames[Animation.index];
                var frame6 = editorAnim6.Frames[Animation.index];
                var frame7 = editorAnim7.Frames[Animation.index];
                var frame8 = editorAnim8.Frames[Animation.index];
                var frame9 = editorAnim9.Frames[Animation.index];
                var frame10 = editorAnim10.Frames[Animation.index];

                //ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                        x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                        y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame2),
                        x + frame2.Frame.PivotX - (fliph ? (frame2.Frame.Width - editorAnim2.Frames[0].Frame.Width) : 0),
                        y + frame2.Frame.PivotY + (flipv ? (frame2.Frame.Height - editorAnim2.Frames[0].Frame.Height) : 0),
                        frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                if (type == 0 || type == 2)
                {
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame3),
                        x + frame3.Frame.PivotX - (fliph ? (frame3.Frame.Width - editorAnim3.Frames[0].Frame.Width) : 0),
                        y + frame3.Frame.PivotY + (flipv ? (frame3.Frame.Height - editorAnim3.Frames[0].Frame.Height) : 0),
                        frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                }
                if (type == 1 || type == 2)
                {
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame4),
                        x + frame4.Frame.PivotX - (fliph ? (frame4.Frame.Width - editorAnim3.Frames[0].Frame.Width) : 0),
                        y + frame4.Frame.PivotY + (flipv ? (frame4.Frame.Height - editorAnim3.Frames[0].Frame.Height) : 0),
                        frame4.Frame.Width, frame4.Frame.Height, false, Transparency);
                }
                for (int i = 0; i <= height; i++)
                {
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame5),
                        x + frame5.Frame.PivotX - (fliph ? (frame5.Frame.Width - editorAnim3.Frames[0].Frame.Width) : 0),
                        y - 208 - (i * 160) + frame5.Frame.PivotY + (flipv ? (frame5.Frame.Height - editorAnim3.Frames[0].Frame.Height) : 0),
                        frame5.Frame.Width, frame5.Frame.Height, false, Transparency);
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame6),
                        x + frame6.Frame.PivotX - (fliph ? (frame6.Frame.Width - editorAnim3.Frames[0].Frame.Width) : 0),
                        y - 208 - (i * 160) + frame6.Frame.PivotY + (flipv ? (frame6.Frame.Height - editorAnim3.Frames[0].Frame.Height) : 0),
                        frame6.Frame.Width, frame6.Frame.Height, false, Transparency);
                }
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame7),
                    x + frame7.Frame.PivotX - (fliph ? (frame7.Frame.Width - editorAnim3.Frames[0].Frame.Width) : 0),
                    y - 208 - (height * 160) + frame7.Frame.PivotY + (flipv ? (frame7.Frame.Height - editorAnim3.Frames[0].Frame.Height) : 0),
                    frame7.Frame.Width, frame7.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame8),
                    x + frame8.Frame.PivotX - (fliph ? (frame8.Frame.Width - editorAnim3.Frames[0].Frame.Width) : 0),
                    y - 208 - (height * 160) + frame8.Frame.PivotY + (flipv ? (frame8.Frame.Height - editorAnim3.Frames[0].Frame.Height) : 0),
                    frame8.Frame.Width, frame8.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame9),
                    x + frame9.Frame.PivotX - (fliph ? (frame9.Frame.Width - editorAnim9.Frames[0].Frame.Width) : 0),
                    y - 95 + frame9.Frame.PivotY + (flipv ? (frame9.Frame.Height - editorAnim9.Frames[0].Frame.Height) : 0),
                    frame9.Frame.Width, frame9.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame10),
                    x + frame10.Frame.PivotX - (fliph ? (frame10.Frame.Width - editorAnim10.Frames[0].Frame.Width) : 0),
                    y - 64 + frame10.Frame.PivotY + (flipv ? (frame10.Frame.Height - editorAnim10.Frames[0].Frame.Height) : 0),
                    frame10.Frame.Width, frame10.Frame.Height, false, Transparency);




            }
        }

        public override bool isObjectOnScreen(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency)
        {
            //TO-DO: Improve
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int height = (int)entity.attributesMap["height"].ValueUInt8;

            int boundsX = 256;
            int boundsY = 160*height + 96;
            return d.IsObjectOnScreen(x - boundsX * height, y - boundsY * height, boundsX * height, boundsY * height);
        }

        public override string GetObjectName()
        {
            return "PopcornMachine";
        }
    }
}
