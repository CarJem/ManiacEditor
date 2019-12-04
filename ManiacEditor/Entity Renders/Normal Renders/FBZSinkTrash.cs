using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class FBZSinkTrash : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            var type = entity.attributesMap["type"].ValueEnum;
            var widthPixels = (int)(entity.attributesMap["size"].ValueVector3.X.High);
            var heightPixels = (int)(entity.attributesMap["size"].ValueVector3.Y.High);
            var width = (int)widthPixels / 16 - 1;
            var height = (int)heightPixels / 16 - 1;

            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("Trash", d.DevicePanel, 11, 1, false, false, false);

            if (width != -1 && height != -1)
            {
                // draw inside
                // TODO this is really heavy on resources, so maybe switch to just drawing a rectangle??
                for (int i = 0; i <= height; i++)
                {
                    editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("Trash", d.DevicePanel, 11, 1, false, false, false);
                    if (editorAnim != null && editorAnim.Frames.Count != 0)
                    {
                        var frame = editorAnim.Frames[Animation.index];
                        Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                        bool wEven = width % 2 == 0;
                        bool hEven = height % 2 == 0;
                        for (int j = 0; j <= width; j++)
                            d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                                (((width + 1) * 16) - widthPixels) / 2 + (x + (wEven ? frame.Frame.PivotX : -16) + (-width / 2 + j) * 16),
                                y + (hEven ? frame.Frame.PivotY : -16) + (-height / 2 + i) * 16,
                                16, 16, false, Transparency);
                    }
                }

                // draw top and botton
                for (int i = 0; i < 2; i++)
                {
                    bool bottom = !((i & 1) > 0);

                    editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("Trash", d.DevicePanel, 11, 1, false, false, false);
                    if (editorAnim != null && editorAnim.Frames.Count != 0)
                    {
                        var frame = editorAnim.Frames[Animation.index];
                        Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                        bool wEven = width % 2 == 0;
                        bool hEven = height % 2 == 0;
                        for (int j = 0; j <= width; j++)
                            d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                                (((width + 1) * 16) - widthPixels) / 2 + (x + (wEven ? frame.Frame.PivotX : -16) + (-width / 2 + j) * 16),
                                (y + heightPixels / (bottom ? 2 : -2) - (bottom ? 16 : 0)),
                                16, 16, false, Transparency);
                    }
                }
            }
        }

        public override string GetObjectName()
        {
            return "FBZSinkTrash";
        }
    }
}
