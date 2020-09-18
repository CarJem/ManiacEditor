using RSDKv5;
using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class LightBarrier : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            DevicePanel d = properties.Graphics;
            Classes.Scene.EditorEntity entity = properties.EditorObject;
            int x = properties.DrawX;
            int y = properties.DrawY;
            int Transparency = properties.Transparency;
            bool fliph = false;
            bool flipv = false;
            bool enabled = entity.attributesMap["enabled"].ValueBool;
            int size = (int)entity.attributesMap["size"].ValueEnum;


            var editorAnim3 = LoadAnimation("FBZ/LightBarrier.bin", d, 0, 1);


            int y_start = y + (size / 2);
            int y_end = y - (size / 2);

            if (enabled == true)
            {
                int repeat = 0;
                int lengthMemory = size;
                int lengthLeft = size;
                bool finalLoop = false;
                int i = 0;
                int sprite_height = 64;


                while (lengthLeft > sprite_height)
                {
                    repeat++;
                    lengthLeft = lengthLeft - sprite_height;
                }

                DrawTexturePivotNormal(d, editorAnim3, editorAnim3.RequestedAnimID, editorAnim3.RequestedFrameID, x, y_end - (i * sprite_height), Transparency);

                for (i = 1; i < repeat + 1; i++)
                {
                    if (i == repeat)
                    {
                        finalLoop = true;
                    }

 
                    int height = (finalLoop ? lengthLeft : sprite_height);
                    DrawTexturePivotNormalCustom(d, editorAnim3, editorAnim3.RequestedAnimID, editorAnim3.RequestedFrameID, x, y_end + (i * sprite_height), Transparency, height);
                }
            }
            var editorAnim = LoadAnimation("FBZ/LightBarrier.bin", d, 0, 0);
            DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y + (size / 2), Transparency, false, false);
            DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y - (size / 2) + 16, Transparency, false, true);
        }

        public void DrawTexturePivotNormalCustom(DevicePanel Graphics, Methods.Drawing.ObjectDrawing.EditorAnimation Animation, int AnimID, int FrameID, int x, int y, int Transparency, int height, bool FlipH = false, bool FlipV = false, int rotation = 0, System.Drawing.Color? color = null)
        {
            if (EntityRenderer.IsValidated(Animation, new System.Tuple<int, int>(AnimID, FrameID)))
            {
                var Frame = Animation.Animation.Animations[AnimID].Frames[FrameID];
                Graphics.DrawBitmap(Animation.Spritesheets.ElementAt(Frame.SpriteSheet).Value, x + Frame.PivotX, y + Frame.PivotY, Frame.X, Frame.Y, Frame.Width, height, false, Transparency, FlipH, FlipV, rotation, color);
            }
        }

        public override string GetObjectName()
        {
            return "LightBarrier";
        }
    }
}
