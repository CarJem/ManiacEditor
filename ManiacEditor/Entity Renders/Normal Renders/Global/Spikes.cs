using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Spikes : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            Methods.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            Methods.Entities.EntityAnimator Animation = properties.Animations;
            bool selected  = properties.isSelected;
            var value = entity.attributesMap["type"];
            bool fliph = false;
            bool flipv = false;
			bool isFBZ = (Methods.Editor.Solution.Entities.SetupObject == "FBZSetup" ? true : false);
            int animID = 0;

            // Down
            if (value.ValueEnum == 1)
            {
                flipv = true;
                animID = 0;
            }
            // Right
            if (value.ValueEnum == 2)
            {
                animID = 1;
            }
            // Left
            if (value.ValueEnum == 3)
            {
                fliph = true;
                animID = 1;
            }

            int count = (entity.attributesMap.ContainsKey("count") ? (int)entity.attributesMap["count"].ValueUInt8 : 2);

            // Is it a value that defaults to 2?
            if (count < 2)
                count = 2;

            int offset1 = 0, offset2 = 0;
            bool extra = false;
            count *= 2; // I made all this with an incorrect assumption so here's a cheap fix
            int count2 = count >> 2;
            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("Spikes", d.DevicePanel, animID, 0, fliph, flipv, false);
            if (isFBZ)
            {
                editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("Spikes", d.DevicePanel, animID, -1, fliph, flipv, false);
            }

            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                if (isFBZ)
                {
                    frame = editorAnim.Frames[Animation.index];
                    Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                }


                if (value.ValueEnum == 0 || value.ValueEnum == 1)
                {
                    // Is count indivisible by 4?
                    if (count % 4 != 0)
                    {
                        offset1 = frame.Frame.Width / 4;
                        count -= 2;
                        extra = true;
                    }

                    // Is count divisible by 8?
                    if (count % 8 == 0)
                    {
                        offset2 = frame.Frame.Width / 2;
                    }

                    // Draw each set of spikes
                    int max = (count2 + 1) / 2;
                    for (int i = -count2 / 2; i < max; ++i)
                    {
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX + (i * (frame.Frame.Width)) - offset1 + offset2, y + frame.Frame.PivotY,
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }

                    // Draw one more overlapping if needed
                    if (extra)
                    {
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX + ((max - 1) * (frame.Frame.Width)) + offset1 + offset2, y + frame.Frame.PivotY,
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }
                else if (value.ValueEnum == 2 || value.ValueEnum == 3)
                {
                    // Is count indivisible by 4?
                    if (count % 4 != 0)
                    {
                        offset1 = frame.Frame.Height / 4;
                        count -= 2;
                        extra = true;
                    }

                    // Is count divisible by 8?
                    if (count % 8 == 0)
                    {
                        offset2 = frame.Frame.Height / 2;
                    }

                    // Draw each set of spikes
                    int max = (count2 + 1) / 2;
                    for (int i = -count2 / 2; i < max; ++i)
                    {
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY + (i * (frame.Frame.Height)) - offset1 + offset2,
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }

                    // Draw one more overlapping if needed
                    if (extra)
                    {
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY + ((max - 1) * (frame.Frame.Height)) + offset1 + offset2,
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }

            }
        }


        public void IceDraw(Methods.Draw.GraphicsHandler d, SceneEntity entity, Classes.Scene.Sets.EditorEntity e, int x, int y, int Transparency)
        {
            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("Spikes", d.DevicePanel, 0, 0, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }

            
        }

        public override bool isObjectOnScreen(Methods.Draw.GraphicsHandler d, SceneEntity entity, Classes.Scene.Sets.EditorEntity e, int x, int y, int Transparency)
        {
            var value = entity.attributesMap["type"];
            int count = (entity.attributesMap.ContainsKey("count") ? (int)entity.attributesMap["count"].ValueUInt8 : 0);
            if (count == 0)
            {
                count = 1;
            }
            int bounds = (32 * count);

            return d.IsObjectOnScreen(x - bounds/2, y - bounds / 2, bounds, bounds);
        }

        public override string GetObjectName()
        {
           return "Spikes";
        }
    }
}
