using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class EggTV : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Edit.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            var widthPixels = (int)(entity.attributesMap["size"].ValueVector2.X.High);
            var heightPixels = (int)(entity.attributesMap["size"].ValueVector2.Y.High);
            var width = (int)widthPixels / 16;
            var height = (int)heightPixels / 16;

            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 0, 1, false, false, false);

            if (width != 0 && height != 0)
            {
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
                            (x + widthPixels / (right ? 2 : -2)) - (right ? frame.Frame.Width : 0),
                            (y + heightPixels / (bottom ? 2 : -2) - (bottom ? frame.Frame.Height : 0)),
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);

                    }
                }

                // draw top and bottom
                for (int i = 0; i < 2; i++)
                {
                    bool bottom = (i & 1) > 0;

                    editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 0, 2, false, bottom, false);
                    if (editorAnim != null && editorAnim.Frames.Count != 0)
                    {
                        var frame = editorAnim.Frames[Animation.index];
                        Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                        bool wEven = width % 2 == 0;
                        for (int j = 1; j < width; j++)
                            d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                                (x + (wEven ? frame.Frame.PivotX : -frame.Frame.Width) + (-width / 2 + j) * frame.Frame.Width),
                                (y + heightPixels / (bottom ? 2 : -2) - (bottom ? frame.Frame.Height : 0)),
                                frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }

                // draw sides
                for (int i = 0; i < 2; i++)
                {
                    bool right = (i & 1) > 0;

                    editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 0, 3, right, false, false);
                    if (editorAnim != null && editorAnim.Frames.Count != 0)
                    {
                        var frame = editorAnim.Frames[Animation.index];
                        Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                        bool hEven = height % 2 == 0;
                        for (int j = 1; j < height; j++)
                            d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                                (x + widthPixels / (right ? 2 : -2)) - (right ? frame.Frame.Width : 0),
                                (y + (hEven ? frame.Frame.PivotY : -frame.Frame.Height) + (-height / 2 + j) * frame.Frame.Height),
                                frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }
            }
        }

        public override string GetObjectName()
        {
            return "EggTV";
        }
    }
}
