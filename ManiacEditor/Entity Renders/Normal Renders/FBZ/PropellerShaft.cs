using RSDKv5;
using SystemColors = System.Drawing.Color;

namespace ManiacEditor.Entity_Renders
{
    public class PropellerShaft : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("EditorIcons", d.DevicePanel, 0, 6, fliph, flipv, false);
            var height_value = (int)(entity.attributesMap["size"].ValueEnum);
            var height = (height_value > 0 ? height_value / 2 : height_value);
            var width = 6;

            if (width != -1 && height != -1)
            {
                bool wEven = width % 2 == 0;
                bool hEven = height % 2 == 0;

                int x1 = (x + width);
                int x2 = (x - width) - 1;
                int y1 = (y + height * 2);
                int y2 = (y - height * 2);


                d.DrawLine(x1, y1, x1, y2, SystemColors.White);
                d.DrawLine(x1, y1, x2, y1, SystemColors.White);
                d.DrawLine(x2, y2, x1, y2, SystemColors.White);
                d.DrawLine(x2, y2, x2, y1, SystemColors.White);

                // draw corners
                for (int i = 0; i < 4; i++)
                {
                    bool right = (i & 1) > 0;
                    bool bottom = (i & 2) > 0;

                    editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 0, 1, right, bottom, false);
                    if (editorAnim != null && editorAnim.Frames.Count != 0)
                    {
                        var frame = editorAnim.Frames[Animation.index];
                        d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                            (right ? x1 - frame.Frame.Width + 1 : x2),
                            (bottom ? y1 - frame.Frame.Height + 1 : y2),
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);

                    }
                }
            }

            editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("EditorIcons", d.DevicePanel, 0, 6, fliph, flipv, false);

            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }


        }

        public override bool isObjectOnScreen(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency)
        {
            var widthPixels = (int)(13) * 16;
            var heightPixels = (int)(entity.attributesMap["size"].ValueEnum * 2 - 1) * 16;
            return d.IsObjectOnScreen(x - widthPixels / 2, y - heightPixels / 2, widthPixels + 15, heightPixels + 15);
        }

        public override string GetObjectName()
        {
            return "PropellerShaft";
        }
    }
}
