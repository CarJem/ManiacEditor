using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class ItemBox : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            var value = e.attributesMap["type"];
            int direction = (int)e.attributesMap["direction"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            switch (direction)
            {
                case 0:
                    break;
                case 1:
                    flipv = true;
                    break;
                default:
                    break;

            }

            var Animation = Methods.Draw.ObjectDrawing.LoadAnimation(d, "ItemBox");
            DrawTexturePivotPlus(d, Animation, 0, 0, x, y, 0, 0, Transparency, fliph, flipv);
            DrawTexturePivotPlus(d, Animation, 2, value.ValueEnum, x, y, 0, -(flipv ? (-3) : 3), Transparency, fliph, flipv);
        }

        public void IceDraw(DevicePanel d, int x, int y, int Transparency, int forceType = 0)
        {
            
            var value = (forceType == -1 ? 0 : forceType);
            bool fliph = false;
            bool flipv = false;


            var Animation = Methods.Draw.ObjectDrawing.LoadAnimation(d, "ItemBox");
            DrawTexturePivotPlus(d, Animation, 0, 0, x, y, 0, 0, Transparency, fliph, flipv);
            DrawTexturePivotPlus(d, Animation, 2, value, x, y, 0, -(flipv ? (-3) : 3), Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "ItemBox";
        }
    }
}
