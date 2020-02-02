using RSDKv5;
using SystemColors = System.Drawing.Color;


namespace ManiacEditor.Entity_Renders
{
    public class LEDPanel : EntityRenderer
    {

        public override void Draw(Structures.EntityLoadOptions properties)
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
            var widthPixels = (int)(entity.attributesMap["size"].ValueVector2.X.High) - 16;
            var heightPixels = (int)(entity.attributesMap["size"].ValueVector2.Y.High);
            var width = (int)widthPixels / 16;
            var height = (int)heightPixels / 16;

            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 0, 1, false, false, false);

            if (width != 0 && height != 0)
            {
                if (width != -1 && height != -1)
                {
                    int x1 = x + widthPixels / -2;
                    int x2 = x + widthPixels / 2 - 1;
                    int y1 = y + heightPixels / -2;
                    int y2 = y + heightPixels / 2 - 1;


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
                            Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                            d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                                (x + widthPixels / (right ? 2 : -2)) - (right ? frame.Frame.Width : 0),
                                (y + heightPixels / (bottom ? 2 : -2) - (bottom ? frame.Frame.Height : 0)),
                                frame.Frame.Width, frame.Frame.Height, false, Transparency);

                        }
                    }
                    /*
                    // draw top and bottom
                    for (int i = 0; i < 2; i++)
                    {
                        bool bottom = (i & 1) > 0;

                        editorAnim = EditorInstancEditorEntity_ini.LoadAnimation2("EditorAssets", d.DevicePanel, 0, 1, false, bottom, false);
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

                        editorAnim = EditorInstancEditorEntity_ini.LoadAnimation2("EditorAssets", d.DevicePanel, 0, 2, right, false, false);
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
                    */
                }
            }
        }

        public override string GetObjectName()
        {
            return "LEDPanel";
        }
    }
}
