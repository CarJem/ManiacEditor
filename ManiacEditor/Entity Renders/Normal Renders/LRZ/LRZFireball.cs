using System.Linq;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class LRZFireball : EntityRenderer
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
            int type = (int)(entity.attributesMap["type"].ValueUInt8);
            int rotation = (int)(entity.attributesMap["rotation"].ValueInt32 / 1.42);
            int pageID = GetRotationFrame(rotation);

            bool fliph = FlippedH(rotation);
            bool flipv = FlippedV(rotation);



            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation("LRZFireball", d.DevicePanel, 1, 0, fliph, flipv, false, rotation, true, false, EditorEntityDrawing.Flag.PartialEngineRotation, false);

            if (editorAnim != null && editorAnim.Frames.Count != 0 && type != 0)
            {
                var frame = editorAnim.Frames[0];
                int thickness = (pageID == 1 ? frame.Frame.Width : frame.Frame.Height);
                int offset = thickness / 2;

                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x - (fliph ? offset : -offset) - (int)(frame.ImageWidth / 2),
                    y - (flipv ? -offset : 0)  - (int)(frame.ImageHeight / 2),
                    frame.ImageWidth, frame.ImageHeight, false, Transparency);

            }
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

        public override string GetObjectName()
        {
            return "LRZFireball";
        }
    }
}
