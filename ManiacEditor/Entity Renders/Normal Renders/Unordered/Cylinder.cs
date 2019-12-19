using RSDKv5;
using SystemColors = System.Drawing.Color;

namespace ManiacEditor.Entity_Renders
{
    public class Cylinder : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int multiplierX = 0;
            int multiplierY = 0;
            int widthPixels = 0;
            int heightPixels = 0;
            switch (type)
            {
                case 0:
                    multiplierX = 2;
                    multiplierY = 2;
                    widthPixels = (int)(entity.attributesMap["length"].ValueEnum) * multiplierX;
                    heightPixels = (int)(entity.attributesMap["radius"].ValueEnum) * multiplierY;
                    break;
                case 1:
                    multiplierX = 2;
                    multiplierY = 2;
                    widthPixels = (int)(entity.attributesMap["radius"].ValueEnum) * multiplierX;
                    heightPixels = (int)(entity.attributesMap["length"].ValueEnum) * multiplierY;
                    break;
                case 2:
                    multiplierX = 2;
                    multiplierY = 2;
                    widthPixels = (int)(entity.attributesMap["radius"].ValueEnum) * multiplierX;
                    heightPixels = (int)(entity.attributesMap["length"].ValueEnum) * multiplierY;
                    break;
                case 3: //Reverse Direction of 3
                    multiplierX = 2;
                    multiplierY = 2;
                    widthPixels = (int)(entity.attributesMap["radius"].ValueEnum) * multiplierX;
                    heightPixels = (int)(entity.attributesMap["length"].ValueEnum) * multiplierY;
                    break;
                case 4:
                    multiplierX = 2;
                    multiplierY = 2;
                    widthPixels = (int)(entity.attributesMap["length"].ValueEnum) * multiplierX;
                    heightPixels = (int)(entity.attributesMap["radius"].ValueEnum) * multiplierY;
                    break;
                case 5: // Nothing Apparently
                    widthPixels = (int)(entity.attributesMap["length"].ValueEnum) * multiplierX;
                    heightPixels = (int)(entity.attributesMap["radius"].ValueEnum) * multiplierY;
                    break;
                case 6: // Nothing Apparently
                    widthPixels = (int)(entity.attributesMap["length"].ValueEnum) * multiplierX;
                    heightPixels = (int)(entity.attributesMap["radius"].ValueEnum) * multiplierY;
                    break;
                case 7:
                    multiplierX = 2;
                    multiplierY = 2;
                    widthPixels = (int)(entity.attributesMap["radius"].ValueEnum) * multiplierX;
                    heightPixels = (int)(entity.attributesMap["length"].ValueEnum) * multiplierY;
                    break;
            }

            var width = (int)widthPixels / 16;
            var height = (int)heightPixels / 16;

            var editorAnim2 = Editor.Instance.EntityDrawing.LoadAnimation2("Cylinder", d.DevicePanel, 0, 0, false, false, false);
            if (editorAnim2 != null && editorAnim2.Frames.Count > 0)
            {
                var frame = editorAnim2.Frames[0];
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            
            if (type == 5 || type == 6 || type > 7)
            {
                var editorAnim3 = Editor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 6, 0, false, false, false);
                if (editorAnim3 != null && editorAnim3.Frames.Count > 0)
                {
                    var frame = editorAnim3.Frames[0];
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                        x + frame.Frame.PivotX,
                        y + frame.Frame.PivotY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
            }

            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 0, 1, false, false, false);

            if (width != 0 && height != 0)
            {
                int x1 = x + widthPixels / -2;
                int x2 = x + widthPixels / 2 - 1;
                int y1 = y + heightPixels / -2;
                int y2 = y + heightPixels / 2 - 1;

                SystemColors color1 = SystemColors.White;
                SystemColors color2 = SystemColors.White;
                SystemColors color3 = SystemColors.White;
                SystemColors color4 = SystemColors.White;
                switch (type)
                {
                    case 2:
                        color1 = SystemColors.Red;
                        color2 = SystemColors.White;
                        color3 = SystemColors.White;
                        color4 = SystemColors.Green;
                        break;
                    case 3:
                        color1 = SystemColors.Green;
                        color2 = SystemColors.White;
                        color3 = SystemColors.White;
                        color4 = SystemColors.Red;
                        break;
                }

                d.DrawLine(x1, y1, x1, y2, color1);
                d.DrawLine(x1, y1, x2, y1, color2);
                d.DrawLine(x2, y2, x1, y2, color3);
                d.DrawLine(x2, y2, x2, y1, color4);

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
            }
        }

        public override bool isObjectOnScreen(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int multiplierX = 0;
            int multiplierY = 0;
            int widthPixels = 0;
            int heightPixels = 0;
            switch (type)
            {
                case 0:
                    multiplierX = 2;
                    multiplierY = 2;
                    widthPixels = (int)(entity.attributesMap["length"].ValueEnum) * multiplierX;
                    heightPixels = (int)(entity.attributesMap["radius"].ValueEnum) * multiplierY;
                    break;
                case 1:
                    multiplierX = 2;
                    multiplierY = 2;
                    widthPixels = (int)(entity.attributesMap["radius"].ValueEnum) * multiplierX;
                    heightPixels = (int)(entity.attributesMap["length"].ValueEnum) * multiplierY;
                    break;
                case 2:
                    multiplierX = 2;
                    multiplierY = 2;
                    widthPixels = (int)(entity.attributesMap["radius"].ValueEnum) * multiplierX;
                    heightPixels = (int)(entity.attributesMap["length"].ValueEnum) * multiplierY;
                    break;
                case 3: //Reverse Direction of 3
                    multiplierX = 2;
                    multiplierY = 2;
                    widthPixels = (int)(entity.attributesMap["radius"].ValueEnum) * multiplierX;
                    heightPixels = (int)(entity.attributesMap["length"].ValueEnum) * multiplierY;
                    break;
                case 4:
                    multiplierX = 2;
                    multiplierY = 2;
                    widthPixels = (int)(entity.attributesMap["length"].ValueEnum) * multiplierX;
                    heightPixels = (int)(entity.attributesMap["radius"].ValueEnum) * multiplierY;
                    break;
                case 5: // Nothing Apparently
                    widthPixels = (int)(entity.attributesMap["length"].ValueEnum) * multiplierX;
                    heightPixels = (int)(entity.attributesMap["radius"].ValueEnum) * multiplierY;
                    break;
                case 6: // Nothing Apparently
                    widthPixels = (int)(entity.attributesMap["length"].ValueEnum) * multiplierX;
                    heightPixels = (int)(entity.attributesMap["radius"].ValueEnum) * multiplierY;
                    break;
                case 7:
                    multiplierX = 2;
                    multiplierY = 2;
                    widthPixels = (int)(entity.attributesMap["radius"].ValueEnum) * multiplierX;
                    heightPixels = (int)(entity.attributesMap["length"].ValueEnum) * multiplierY;
                    break;
            }
            if (widthPixels != 0 && heightPixels != 0)
            {
                return d.IsObjectOnScreen(x - widthPixels / 2, y - heightPixels / 2, widthPixels, heightPixels);
            }
            else
            {
                return d.IsObjectOnScreen(x - 16, y - 16, 32, 32);
            }

        }

        public override string GetObjectName()
        {
            return "Cylinder";
        }
    }
}
