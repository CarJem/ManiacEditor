using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class ItemBox : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
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
                var editorAnimBox = Editor.Instance.EntityDrawing.LoadAnimation2("ItemBox", d.DevicePanel, 0, 0, fliph, flipv, false);
                var editorAnimEffect = Editor.Instance.EntityDrawing.LoadAnimation2("ItemBox", d.DevicePanel, 2, (int)value.ValueEnum, fliph, flipv, false);
                if (editorAnimBox != null && editorAnimEffect != null && editorAnimEffect.Frames.Count != 0)
                {
                    var frameBox = editorAnimBox.Frames[0];
                    var frameEffect = editorAnimEffect.Frames[0];
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameBox), x + frameBox.Frame.PivotX, y + frameBox.Frame.PivotY,
                        frameBox.Frame.Width, frameBox.Frame.Height, false, Transparency);
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameEffect), x + frameEffect.Frame.PivotX, y + frameEffect.Frame.PivotY - (flipv ? (-3) : 3),
                        frameEffect.Frame.Width, frameEffect.Frame.Height, false, Transparency);
                }
        }

        public void IceDraw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int forceType = 0)
        {
            var value = (forceType == -1 ? 0 : forceType);
            bool fliph = false;
            bool flipv = false;
            var editorAnimBox = Editor.Instance.EntityDrawing.LoadAnimation2("ItemBox", d.DevicePanel, 0, 0, fliph, flipv, false);
            var editorAnimEffect = Editor.Instance.EntityDrawing.LoadAnimation2("ItemBox", d.DevicePanel, 2, (int)value, fliph, flipv, false);
            if (editorAnimBox != null)
            {
                var frameBox = editorAnimBox.Frames[0];

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameBox), x + frameBox.Frame.PivotX, y + frameBox.Frame.PivotY,
                    frameBox.Frame.Width, frameBox.Frame.Height, false, Transparency);

            }
            if (editorAnimEffect != null && editorAnimEffect.Frames.Count != 0 && forceType != -1)
            {
                var frameEffect = editorAnimEffect.Frames[0];
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameEffect), x + frameEffect.Frame.PivotX, y + frameEffect.Frame.PivotY - (flipv ? (-3) : 3), frameEffect.Frame.Width, frameEffect.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "ItemBox";
        }
    }
}
