using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class WaterGush : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity entity = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;
            var length = (int)(entity.attributesMap["length"].ValueUInt32);
            int orientation = (int)(entity.attributesMap["orientation"].ValueUInt8);
            int animID = 0;
            int animID2 = 2;
            bool fliph = false;
            bool flipv = false;
            switch (orientation)
            {
                case 0:
                    animID = 0;
                    animID2 = 2;
                    break;
                case 1:
                    animID = 1;
                    animID2 = 3;
                    break;
                case 2:
                    animID = 1;
                    animID2 = 3;
                    fliph = true;
                    break;
                case 3:
                    animID = 0;
                    animID2 = 2;
                    flipv = true;
                    break;
            }



            if (orientation == 0)
            {
                var editorAnim = LoadAnimation("HCZ/WaterGush.bin", d, animID, 0);
                int height = editorAnim.RequestedFrame.Height;
                for (int i = -length + 1; i <= 0; ++i)
                {
                    DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y + i * editorAnim.RequestedFrame.Height, Transparency, fliph, flipv);
                }
                var editorAnimGush = LoadAnimation("HCZ/WaterGush.bin", d, animID2, 0);
                DrawTexturePivotNormal(d, editorAnimGush, editorAnimGush.RequestedAnimID, editorAnimGush.RequestedFrameID, x, y - length * height, Transparency, fliph, flipv);
            }
            else if (orientation == 1)
            {
                var editorAnim = LoadAnimation("HCZ/WaterGush.bin", d, animID, 0);
                int height = editorAnim.RequestedFrame.Width;
                for (int i = -length + 1; i <= 0; ++i)
                {
                    DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x - i * editorAnim.RequestedFrame.Width, y, Transparency, fliph, flipv);
                }
                var editorAnimGush = LoadAnimation("HCZ/WaterGush.bin", d, animID2, 0);
                DrawTexturePivotNormal(d, editorAnimGush, editorAnimGush.RequestedAnimID, editorAnimGush.RequestedFrameID, x + length * height, y, Transparency, fliph, flipv);
            }
            else if (orientation == 2)
            {
                var editorAnim = LoadAnimation("HCZ/WaterGush.bin", d, animID, 0);
                int height = editorAnim.RequestedFrame.Width;
                for (int i = -length + 1; i <= 0; ++i)
                {
                    DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x - editorAnim.RequestedFrame.Width + i * editorAnim.RequestedFrame.Width, y, Transparency, fliph, flipv);
                }
                var editorAnimGush = LoadAnimation("HCZ/WaterGush.bin", d, animID2, 0);
                DrawTexturePivotNormal(d, editorAnimGush, editorAnimGush.RequestedAnimID, editorAnimGush.RequestedFrameID, x - length * height, y, Transparency, fliph, flipv);
            }
            else if (orientation == 3)
            {
                var editorAnim = LoadAnimation("HCZ/WaterGush.bin", d, animID, 0);
                int height = editorAnim.RequestedFrame.Height;
                for (int i = -length + 1; i <= 0; ++i)
                {
                    DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y + editorAnim.RequestedFrame.Height - i * editorAnim.RequestedFrame.Height, Transparency, fliph, flipv);
                }
                var editorAnimGush = LoadAnimation("HCZ/WaterGush.bin", d, animID2, 0);
                DrawTexturePivotNormal(d, editorAnimGush, editorAnimGush.RequestedAnimID, editorAnimGush.RequestedFrameID, x, y + length * height, Transparency, fliph, flipv);
            }            
        }

        public override string GetObjectName()
        {
            return "WaterGush";
        }
    }
}
