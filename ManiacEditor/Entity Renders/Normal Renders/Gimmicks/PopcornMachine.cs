using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class PopcornMachine : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            //int type = (int)entity.attributesMap["type"].ValueUInt8;
            int type = (int)e.attributesMap["type"].ValueUInt8;
            int height = (int)e.attributesMap["height"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;

            var editorAnim = LoadAnimation("SPZ1/PopcornMachine.bin", d, 0, 0);
            DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency, fliph, flipv);
            editorAnim = LoadAnimation("SPZ1/PopcornMachine.bin", d, 0, 1);
            DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency, fliph, flipv);






            if (type == 0 || type == 2)
            {
                editorAnim = LoadAnimation("SPZ1/PopcornMachine.bin", d, 0, 2);
                DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency, fliph, flipv);
            }
            if (type == 1 || type == 2)
            {
                editorAnim = LoadAnimation("SPZ1/PopcornMachine.bin", d, 0, 3);
                DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency, fliph, flipv);
            }
            for (int i = 0; i <= height; i++)
            {
                int y_mod = y - 208 - (i * 160);
                editorAnim = LoadAnimation("SPZ1/PopcornMachine.bin", d, 0, 6);
                DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y_mod, Transparency, fliph, flipv);
                editorAnim = LoadAnimation("SPZ1/PopcornMachine.bin", d, 0, 7);
                DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y_mod, Transparency, fliph, flipv);
            }


            editorAnim = LoadAnimation("SPZ1/PopcornMachine.bin", d, 0, 4);
            DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y - 208 - (height * 160), Transparency, fliph, flipv);
            editorAnim = LoadAnimation("SPZ1/PopcornMachine.bin", d, 0, 5);
            DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y - 208 - (height * 160), Transparency, fliph, flipv);
            editorAnim = LoadAnimation("SPZ1/PopcornMachine.bin", d, 0, 9);
            DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y - 95, Transparency, fliph, flipv);
            editorAnim = LoadAnimation("SPZ1/PopcornMachine.bin", d, 0, 10);
            DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y - 64, Transparency, fliph, flipv);
        }

        public override bool isObjectOnScreen(DevicePanel d, Classes.Scene.EditorEntity entity, int x, int y, int Transparency)
        {
            //TO-DO: Improve
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int height = (int)entity.attributesMap["height"].ValueUInt8;

            int boundsX = 256;
            int boundsY = 160 * height + 96;
            return d.IsObjectOnScreen(x - boundsX * height, y - boundsY * height, boundsX * height, boundsY * height);
        }

        public override string GetObjectName()
        {
            return "PopcornMachine";
        }
    }
}
