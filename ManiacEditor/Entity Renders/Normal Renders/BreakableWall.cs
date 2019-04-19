using RSDKv5;
using SystemColors = System.Drawing.Color;

namespace ManiacEditor.Entity_Renders
{
    public class BreakableWall : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            var type = entity.attributesMap["type"].ValueUInt8;
            var width = (int)(entity.attributesMap["size"].ValuePosition.X.High) - 1;
			var height = (int)(entity.attributesMap["size"].ValuePosition.Y.High) - 1;


            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 0, 1, false, false, false);

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
                for (int i = 0; i < 4; i++)
                {
                    bool right = (i & 1) > 0;
                    bool bottom = (i & 2) > 0;

                    editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 0, 1, right, bottom, false);
                    if (editorAnim != null && editorAnim.Frames.Count != 0)
                    {
                        var frame = editorAnim.Frames[Animation.index];
                        Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                        d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                            (x + (wEven ? frame.Frame.PivotX : -frame.Frame.Width) + (-width / 2 + (right ? width : 0)) * frame.Frame.Width),
                            (y + (hEven ? frame.Frame.PivotY : -frame.Frame.Height) + (-height / 2 + (bottom ? height : 0)) * frame.Frame.Height),
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);

                    }
                }
            }

            bool knux = entity.attributesMap["onlyKnux"].ValueBool;
            bool mighty = entity.attributesMap.ContainsKey("onlyMighty") && entity.attributesMap["onlyMighty"].ValueBool;

            // draw Knuckles icon
            if (knux)
            {
                editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("HUD", d.DevicePanel, 2, 2, false, false, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    var frame = editorAnim.Frames[Animation.index];
                    Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x - frame.Frame.Width / (mighty ? 1 : 2), y - frame.Frame.Height / 2, frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
            }

            // draw Mighty icon
            if (mighty)
            {
                editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("HUD", d.DevicePanel, 2, 3, false, false, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    var frame = editorAnim.Frames[Animation.index];
                    Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x - (knux ? 0 : frame.Frame.Width / 2), y - frame.Frame.Height / 2, frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
            }
        }

        public override bool isObjectOnScreen(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            var widthPixels = (int)(entity.attributesMap["size"].ValuePosition.X.High * 2 - 1) * 16;
            var heightPixels = (int)(entity.attributesMap["size"].ValuePosition.Y.High * 2 - 1) * 16;
            return d.IsObjectOnScreen(x - widthPixels / 2, y - heightPixels / 2, widthPixels + 15, heightPixels + 15);
        }

        public override string GetObjectName()
        {
            return "BreakableWall";
        }
    }
}
