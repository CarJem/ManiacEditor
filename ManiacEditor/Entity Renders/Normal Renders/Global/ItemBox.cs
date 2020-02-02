using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class ItemBox : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
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
                var value = entity.attributesMap["type"];
                int direction = (int)entity.attributesMap["direction"].ValueUInt8;
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
                var editorAnimBox = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("ItemBox", d.DevicePanel, 0, 0, fliph, flipv, false);
                var editorAnimEffect = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("ItemBox", d.DevicePanel, 2, (int)value.ValueEnum, fliph, flipv, false);
                if (editorAnimBox != null && editorAnimEffect != null && editorAnimEffect.Frames.Count != 0)
                {
                    var frameBox = editorAnimBox.Frames[0];
                    var frameEffect = editorAnimEffect.Frames[0];
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frameBox), x + frameBox.Frame.PivotX, y + frameBox.Frame.PivotY,
                        frameBox.Frame.Width, frameBox.Frame.Height, false, Transparency);
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frameEffect), x + frameEffect.Frame.PivotX, y + frameEffect.Frame.PivotY - (flipv ? (-3) : 3),
                        frameEffect.Frame.Width, frameEffect.Frame.Height, false, Transparency);
                }
        }

        public void IceDraw(Classes.Core.Draw.GraphicsHandler d, SceneEntity entity, Classes.Core.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int forceType = 0)
        {
            var value = (forceType == -1 ? 0 : forceType);
            bool fliph = false;
            bool flipv = false;
            var editorAnimBox = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("ItemBox", d.DevicePanel, 0, 0, fliph, flipv, false);
            var editorAnimEffect = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("ItemBox", d.DevicePanel, 2, (int)value, fliph, flipv, false);
            if (editorAnimBox != null)
            {
                var frameBox = editorAnimBox.Frames[0];

                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frameBox), x + frameBox.Frame.PivotX, y + frameBox.Frame.PivotY,
                    frameBox.Frame.Width, frameBox.Frame.Height, false, Transparency);

            }
            if (editorAnimEffect != null && editorAnimEffect.Frames.Count != 0 && forceType != -1)
            {
                var frameEffect = editorAnimEffect.Frames[0];
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frameEffect), x + frameEffect.Frame.PivotX, y + frameEffect.Frame.PivotY - (flipv ? (-3) : 3), frameEffect.Frame.Width, frameEffect.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "ItemBox";
        }
    }
}
