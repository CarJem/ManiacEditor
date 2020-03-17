using RSDKv5;
using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class Spikes : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            SceneEntity entity = e.Entity;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

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
            var Animation = Methods.Entities.EntityDrawing.LoadAnimation(Properties.Graphics, "Spikes", animID, 0);

            if (Animation.RequestedFrame.Width == 0 || Animation.RequestedFrame.Height == 0) return;

            if (value.ValueEnum == 0 || value.ValueEnum == 1)
            {
                // Is count indivisible by 4?
                if (count % 4 != 0)
                {
                    offset1 = Animation.RequestedFrame.Width / 4;
                    count -= 2;
                    extra = true;
                }

                // Is count divisible by 8?
                if (count % 8 == 0)
                {
                    offset2 = Animation.RequestedFrame.Width / 2;
                }

                // Draw each set of spikes
                int max = (count2 + 1) / 2;
                for (int i = -count2 / 2; i < max; ++i)
                {
                    DrawTexturePivotPlus(Properties.Graphics, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, (i * (Animation.RequestedFrame.Width)) - offset1 + offset2, 0, Transparency, fliph, flipv);
                }

                // Draw one more overlapping if needed
                if (extra)
                {
                    DrawTexturePivotPlus(Properties.Graphics, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, ((max - 1) * (Animation.RequestedFrame.Width)) + offset1 + offset2, 0, Transparency, fliph, flipv);
                }
            }
            else if (value.ValueEnum == 2 || value.ValueEnum == 3)
            {
                // Is count indivisible by 4?
                if (count % 4 != 0)
                {
                    offset1 = Animation.RequestedFrame.Height / 4;
                    count -= 2;
                    extra = true;
                }

                // Is count divisible by 8?
                if (count % 8 == 0)
                {
                    offset2 = Animation.RequestedFrame.Height / 2;
                }

                // Draw each set of spikes
                int max = (count2 + 1) / 2;
                for (int i = -count2 / 2; i < max; ++i)
                {
                    DrawTexturePivotPlus(Properties.Graphics, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, 0, (i * (Animation.RequestedFrame.Height)) - offset1 + offset2, Transparency, fliph, flipv);
                }

                // Draw one more overlapping if needed
                if (extra)
                {
                    DrawTexturePivotPlus(Properties.Graphics, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, 0, ((max - 1) * (Animation.RequestedFrame.Height)) + offset1 + offset2, Transparency, fliph, flipv);
                }
            }
        }


        public void IceDraw()
        {
            /*
            //(Methods.Draw.GraphicsHandler d, SceneEntity entity, Classes.Scene.Sets.EditorEntity e, int x, int y, int Transparency)
            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("Spikes", d.DevicePanel, 0, 0, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            */           
        }

        public override bool isObjectOnScreen(DevicePanel d, SceneEntity entity, Classes.Scene.EditorEntity e, int x, int y, int Transparency)
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
