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

            var Animation = Methods.Entities.EntityDrawing.LoadAnimation(d, "ItemBox");
            DrawTexturePivotPlus(d, Animation, 0, 0, x, y, 0, 0, Transparency, fliph, flipv);
            DrawTexturePivotPlus(d, Animation, 2, value.ValueEnum, x, y, 0, -(flipv ? (-3) : 3), Transparency, fliph, flipv);
        }

        //public void IceDraw(Methods.Draw.GraphicsHandler d, SceneEntity entity, Classes.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int forceType = 0)
        //{
            /*
            var value = (forceType == -1 ? 0 : forceType);
            bool fliph = false;
            bool flipv = false;
            var editorAnimBox = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("ItemBox", d.DevicePanel, 0, 0, fliph, flipv, false);
            var editorAnimEffect = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("ItemBox", d.DevicePanel, 2, (int)value, fliph, flipv, false);
            if (editorAnimBox != null)
            {
                var frameBox = editorAnimBox.Frames[0];

                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frameBox), x + frameBox.Frame.PivotX, y + frameBox.Frame.PivotY,
                    frameBox.Frame.Width, frameBox.Frame.Height, false, Transparency);

            }
            if (editorAnimEffect != null && editorAnimEffect.Frames.Count != 0 && forceType != -1)
            {
                var frameEffect = editorAnimEffect.Frames[0];
                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frameEffect), x + frameEffect.Frame.PivotX, y + frameEffect.Frame.PivotY - (flipv ? (-3) : 3), frameEffect.Frame.Width, frameEffect.Frame.Height, false, Transparency);
            }*/
       //}

        public override string GetObjectName()
        {
            return "ItemBox";
        }
    }
}
