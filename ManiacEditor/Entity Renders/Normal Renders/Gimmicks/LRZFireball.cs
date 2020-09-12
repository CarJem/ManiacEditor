using System.Linq;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class LRZFireball : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;

            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            int type = (int)(e.attributesMap["type"].ValueUInt8);
            int rotation = (int)(e.attributesMap["rotation"].ValueInt32 / 1.42);
            int pageID = GetRotationFrame(rotation);

            bool fliph = FlippedH(rotation);
            bool flipv = FlippedV(rotation);


            var Animation = LoadAnimation(GetSetupAnimation(), d, 1, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv, rotation);
        }

        public enum Orientation : int
        {
            Horizontal = 0,
            Vertical = 1,
            HorizontalFlipped = 2,
            VerticalFlipped = 3,
            Unknown = -1
        }

        public int GetRotationFrame(int rotation)
        {
            Orientation Orientation = GetOrientation(rotation);
            if (Orientation == Orientation.Horizontal || Orientation == Orientation.HorizontalFlipped)
            {
                return 0;
            }
            else return 1;
        }

        public bool FlippedV(int rotation)
        {
            Orientation Orientation = GetOrientation(rotation);
            if (Orientation == Orientation.VerticalFlipped) return true;
            else return false;
        }

        public bool FlippedH(int rotation)
        {
            Orientation Orientation = GetOrientation(rotation);
            if (Orientation == Orientation.HorizontalFlipped) return true;
            else return false;
        }

        public Orientation GetOrientation(int rotation)
        {
            bool isNegative = false;
            if (rotation < 0) isNegative = true;
            if (isNegative) rotation = -rotation;

            while (rotation > 360) rotation -= 360;

            if (!isNegative)
            {
                if (Enumerable.Range(0, 90).Contains(rotation)) return Orientation.Vertical;
                else if (Enumerable.Range(90, 180).Contains(rotation)) return Orientation.HorizontalFlipped;
                else if (Enumerable.Range(180, 270).Contains(rotation)) return Orientation.VerticalFlipped;
                else if (Enumerable.Range(270, 360).Contains(rotation)) return Orientation.Horizontal;
                else return Orientation.Unknown;
            }
            else
            {
                if (Enumerable.Range(0, 90).Contains(rotation)) return Orientation.Vertical;
                else if (Enumerable.Range(90, 180).Contains(rotation)) return Orientation.Horizontal;
                else if (Enumerable.Range(180, 270).Contains(rotation)) return Orientation.VerticalFlipped;
                else if (Enumerable.Range(270, 360).Contains(rotation)) return Orientation.HorizontalFlipped;
                else return Orientation.Unknown;
            }

        }

        public override string GetSetupAnimation()
        {
            return GetSpriteAnimationPath("/LRZFireball.bin", "LRZFireball", new string[] { "LRZ2", "LRZ1" });
        }

        public override string GetObjectName()
        {
            return "LRZFireball";
        }
    }
}
