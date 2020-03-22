using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class ChemicalPool : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity entity = Properties.EditorObject;

            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            Transparency = 95;
            var type = entity.attributesMap["type"].ValueEnum;
            var widthPixels = (int)(entity.attributesMap["size"].ValueVector2.X.High);
            var heightPixels = (int)(entity.attributesMap["size"].ValueVector2.Y.High);
            var width = (int)widthPixels / 16 - 1;
            var height = (int)heightPixels / 16 - 1;

            var Animation = LoadAnimation("EditorAssets", d, 1, 1 + (int)type * 2);

            if (width != -1 && height != -1)
            {
                // draw inside
                // TODO this is really heavy on resources, so maybe switch to just drawing a rectangle??
                for (int i = 0; i <= height; i++)
                {
                    bool wEven = width % 2 == 0;
                    bool hEven = height % 2 == 0;
                    for (int j = 0; j <= width; j++)
                        DrawTexture(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID,
                            (((width + 1) * 16) - widthPixels) / 2 + (x + (wEven ? Animation.RequestedFrame.PivotX : -Animation.RequestedFrame.Width) + (-width / 2 + j) * Animation.RequestedFrame.Width),
                            y + (hEven ? Animation.RequestedFrame.PivotY : -Animation.RequestedFrame.Height) + (-height / 2 + i) * Animation.RequestedFrame.Height, Transparency);
                }

                // draw top and botton
                for (int i = 0; i < 2; i++)
                {
                    bool bottom = !((i & 1) > 0);

                    Animation = LoadAnimation("EditorAssets", d, 1, (bottom ? 1 : 0) + (int)type * 2);
                    bool wEven = width % 2 == 0;
                    bool hEven = height % 2 == 0;
                    for (int j = 0; j <= width; j++)
                        DrawTexture(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID,
                                (((width + 1) * 16) - widthPixels) / 2 + (x + (wEven ? Animation.RequestedFrame.PivotX : -Animation.RequestedFrame.Width) + (-width / 2 + j) * Animation.RequestedFrame.Width),
                            (y + heightPixels / (bottom ? 2 : -2) - (bottom ? Animation.RequestedFrame.Height : 0)), Transparency);
                }
            }
        }

        public override bool isObjectOnScreen(DevicePanel d, Classes.Scene.EditorEntity entity, int x, int y, int Transparency)
        {
            var widthPixels = (int)(entity.attributesMap["size"].ValueVector2.X.High);
            var heightPixels = (int)(entity.attributesMap["size"].ValueVector2.Y.High);
            return d.IsObjectOnScreen(x - widthPixels / 2, y - heightPixels / 2, widthPixels, heightPixels);
        }

        public override string GetObjectName()
        {
            return "ChemicalPool";
        }
    }
}
