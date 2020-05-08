using RSDKv5;
using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class Spikes : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            var value = e.attributesMap["type"];
            bool fliph = false;
            bool flipv = false;
			bool isFBZ = (Methods.Solution.CurrentSolution.Entities.SetupObject == "FBZSetup" ? true : false);
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

            int count = (e.attributesMap.ContainsKey("count") ? (int)e.attributesMap["count"].ValueUInt8 : 2);

            // Is it a value that defaults to 2?
            if (count < 2)
                count = 2;

            int offset1 = 0, offset2 = 0;
            bool extra = false;
            count *= 2; // I made all this with an incorrect assumption so here's a cheap fix
            int count2 = count >> 2;
            var Animation = Methods.Draw.ObjectDrawing.LoadAnimation(Properties.Graphics, "Spikes", animID, 0);

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


        public void IceDraw(DevicePanel d, int x, int y, int Transparency)
        {
            var Animation = LoadAnimation("Spikes", d, 0, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, false, false);
        }

        public override bool isObjectOnScreen(DevicePanel d, Classes.Scene.EditorEntity entity, int x, int y, int Transparency)
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
