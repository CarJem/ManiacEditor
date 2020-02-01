using RSDKv5;
using SystemColors = System.Drawing.Color;

namespace ManiacEditor.Entity_Renders
{
    public class WaterfallSound : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            var editorAnim = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("EditorIcons2", d.DevicePanel, 0, 6, fliph, flipv, false);
            var width = (int)(entity.attributesMap["size"].ValueVector2.X.High*2 - 1);
            var height = (int)(entity.attributesMap["size"].ValueVector2.Y.High*2 - 1);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }

            if (width != -1 && height != -1)
            {
                bool wEven = width % 2 == 0;
                bool hEven = height % 2 == 0;

                int x1 = (x + (wEven ? -8 : -16) + (-width / 2 + width) * 16) + 15;
                int x2 = (x + (wEven ? -8 : -16) + (-width / 2) * 16);
                int y1 = (y + (hEven ? -8 : -16) + (-height / 2 + height) * 16) + 15;
                int y2 = (y + (hEven ? -8 : -16) + (-height / 2) * 16);


                d.DrawLine(x1, y1, x1, y2, SystemColors.White);
                d.DrawLine(x1, y1, x2, y1, SystemColors.White);
                d.DrawLine(x2, y2, x1, y2, SystemColors.White);
                d.DrawLine(x2, y2, x2, y1, SystemColors.White);

                // draw corners

                editorAnim = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 0, 1, false, false, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    var frame = editorAnim.Frames[Animation.index];
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                        (x + (wEven ? frame.Frame.PivotX : -frame.Frame.Width) + (-width / 2 + (false ? width : 0)) * frame.Frame.Width),
                        (y + (hEven ? frame.Frame.PivotY : -frame.Frame.Height) + (-height / 2 + (false ? height : 0)) * frame.Frame.Height),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);

                }


                editorAnim = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 0, 1, false, true, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    var frame = editorAnim.Frames[Animation.index];
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                        (x + (wEven ? frame.Frame.PivotX : -frame.Frame.Width) + (-width / 2 + (false ? width : 0)) * frame.Frame.Width),
                        (y + (hEven ? frame.Frame.PivotY : -frame.Frame.Height) + (-height / 2 + (true ? height : 0)) * frame.Frame.Height),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);

                }


                editorAnim = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 0, 1, true, false, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    var frame = editorAnim.Frames[Animation.index];
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                        (x + (wEven ? frame.Frame.PivotX : -frame.Frame.Width) + (-width / 2 + (true ? width : 0)) * frame.Frame.Width),
                        (y + (hEven ? frame.Frame.PivotY : -frame.Frame.Height) + (-height / 2 + (false ? height : 0)) * frame.Frame.Height),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);

                }

                editorAnim = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 0, 1, true, true, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    var frame = editorAnim.Frames[Animation.index];
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                        (x + (wEven ? frame.Frame.PivotX : -frame.Frame.Width) + (-width / 2 + (true ? width : 0)) * frame.Frame.Width),
                        (y + (hEven ? frame.Frame.PivotY : -frame.Frame.Height) + (-height / 2 + (true ? height : 0)) * frame.Frame.Height),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);

                }
            }
        }

        public override bool isObjectOnScreen(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency)
        {
            var widthPixels = (int)(entity.attributesMap["size"].ValueVector2.X.High * 2 - 1) * 16;
            var heightPixels = (int)(entity.attributesMap["size"].ValueVector2.Y.High * 2 - 1) * 16;
            return d.IsObjectOnScreen(x - widthPixels / 2, y - heightPixels / 2, widthPixels + 15, heightPixels + 15);
        }

        public override string GetObjectName()
        {
            return "WaterfallSound";
        }
    }
}
