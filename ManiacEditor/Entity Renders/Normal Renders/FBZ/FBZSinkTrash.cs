using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class FBZSinkTrash : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            Classes.Core.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Core.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            EditorAnimations Animation = properties.Animations;
            bool selected  = properties.isSelected;
            var type = entity.attributesMap["type"].ValueEnum;
            var widthPixels = (int)(entity.attributesMap["size"].ValueVector2.X.High);
            var heightPixels = (int)(entity.attributesMap["size"].ValueVector2.Y.High);
            var width = (int)widthPixels / 16 - 1;
            var height = (int)heightPixels / 16 - 1;

            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Trash", d.DevicePanel, 11, 1, false, false, false);

            if (width != -1 && height != -1)
            {
                // draw inside
                // TODO this is really heavy on resources, so maybe switch to just drawing a rectangle??
                for (int i = 0; i <= height; i++)
                {
                    editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Trash", d.DevicePanel, 11, 1, false, false, false);
                    if (editorAnim != null && editorAnim.Frames.Count != 0)
                    {
                        var frame = editorAnim.Frames[Animation.index];
                        Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                        bool wEven = width % 2 == 0;
                        bool hEven = height % 2 == 0;
                        for (int j = 0; j <= width; j++)
                            d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                                (((width + 1) * 16) - widthPixels) / 2 + (x + (wEven ? frame.Frame.PivotX : -16) + (-width / 2 + j) * 16),
                                y + (hEven ? frame.Frame.PivotY : -16) + (-height / 2 + i) * 16,
                                16, 16, false, Transparency);
                    }
                }

                // draw top and botton
                for (int i = 0; i < 2; i++)
                {
                    bool bottom = !((i & 1) > 0);

                    editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Trash", d.DevicePanel, 11, 1, false, false, false);
                    if (editorAnim != null && editorAnim.Frames.Count != 0)
                    {
                        var frame = editorAnim.Frames[Animation.index];
                        Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                        bool wEven = width % 2 == 0;
                        bool hEven = height % 2 == 0;
                        for (int j = 0; j <= width; j++)
                            d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
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
