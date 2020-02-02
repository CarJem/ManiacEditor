using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class ChemicalPool : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            Transparency = 95;
            var type = entity.attributesMap["type"].ValueEnum;
            var widthPixels = (int)(entity.attributesMap["size"].ValueVector2.X.High);
            var heightPixels = (int)(entity.attributesMap["size"].ValueVector2.Y.High);
            var width = (int)widthPixels / 16 - 1;
            var height = (int)heightPixels / 16 - 1;

            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 1, 1 + (int)type * 2, false, false, false);

            if (width != -1 && height != -1)
            {
                // draw inside
                // TODO this is really heavy on resources, so maybe switch to just drawing a rectangle??
                for (int i = 0; i <= height; i++)
                {
                    editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 1, 1 + (int)type * 2, false, false, false);
                    if (editorAnim != null && editorAnim.Frames.Count != 0)
                    {
                        var frame = editorAnim.Frames[Animation.index];
                        Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                        bool wEven = width % 2 == 0;
                        bool hEven = height % 2 == 0;
                        for (int j = 0; j <= width; j++)
                            d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                                (((width + 1) * 16) - widthPixels) / 2 + (x + (wEven ? frame.Frame.PivotX : -frame.Frame.Width) + (-width / 2 + j) * frame.Frame.Width),
                                y + (hEven ? frame.Frame.PivotY : -frame.Frame.Height) + (-height / 2 + i) * frame.Frame.Height,
                                frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }

                // draw top and botton
                for (int i = 0; i < 2; i++)
                {
                    bool bottom = !((i & 1) > 0);

                    editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 1, (bottom ? 1 : 0) + (int)type * 2, false, false, false);
                    if (editorAnim != null && editorAnim.Frames.Count != 0)
                    {
                        var frame = editorAnim.Frames[Animation.index];
                        Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                        bool wEven = width % 2 == 0;
                        bool hEven = height % 2 == 0;
                        for (int j = 0; j <= width; j++)
                            d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                                (((width + 1) * 16) - widthPixels) / 2 + (x + (wEven ? frame.Frame.PivotX : -frame.Frame.Width) + (-width / 2 + j) * frame.Frame.Width),
                                (y + heightPixels / (bottom ? 2 : -2) - (bottom ? frame.Frame.Height : 0)),
                                frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }
            }
        }

        public override bool isObjectOnScreen(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency)
        {
            var widthPixels = (int)(entity.attributesMap["size"].ValueVector2.X.High);
            var heightPixels = (int)(entity.attributesMap["size"].ValueVector2.Y.High);
            return d.IsObjectOnScreen(x - widthPixels / 2, y - heightPixels / 2, widthPixels, heightPixels);
        }

        public override string GetObjectName()
        {
            return "ChemicalPool";
        }
    }
}
