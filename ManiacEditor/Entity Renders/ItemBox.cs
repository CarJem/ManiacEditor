using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ManiacEditor;
using Microsoft.Xna.Framework;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class ItemBox : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
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
                var editorAnimBox = e.EditorInstance.EditorEntity_ini.LoadAnimation2("ItemBox", d, 0, 0, fliph, flipv, false);
                var editorAnimEffect = e.EditorInstance.EditorEntity_ini.LoadAnimation2("ItemBox", d, 2, (int)value.ValueVar, fliph, flipv, false);
                if (editorAnimBox != null && editorAnimEffect != null && editorAnimEffect.Frames.Count != 0)
                {
                    var frameBox = editorAnimBox.Frames[0];
                    var frameEffect = editorAnimEffect.Frames[0];
                    d.DrawBitmap(frameBox.Texture, x + frameBox.Frame.CenterX, y + frameBox.Frame.CenterY,
                        frameBox.Frame.Width, frameBox.Frame.Height, false, Transparency);
                    d.DrawBitmap(frameEffect.Texture, x + frameEffect.Frame.CenterX, y + frameEffect.Frame.CenterY - (flipv ? (-3) : 3),
                        frameEffect.Frame.Width, frameEffect.Frame.Height, false, Transparency);
                }
        }

        public void IceDraw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int forceType = 0)
        {
            var value = (forceType == -1 ? 0 : forceType);
            bool fliph = false;
            bool flipv = false;
            var editorAnimBox = e.EditorInstance.EditorEntity_ini.LoadAnimation2("ItemBox", d, 0, 0, fliph, flipv, false);
            var editorAnimEffect = e.EditorInstance.EditorEntity_ini.LoadAnimation2("ItemBox", d, 2, (int)value, fliph, flipv, false);
            if (editorAnimBox != null)
            {
                var frameBox = editorAnimBox.Frames[0];

                d.DrawBitmap(frameBox.Texture, x + frameBox.Frame.CenterX, y + frameBox.Frame.CenterY,
                    frameBox.Frame.Width, frameBox.Frame.Height, false, Transparency);

            }
            if (editorAnimEffect != null && editorAnimEffect.Frames.Count != 0 && forceType != -1)
            {
                var frameEffect = editorAnimEffect.Frames[0];
                d.DrawBitmap(frameEffect.Texture, x + frameEffect.Frame.CenterX, y + frameEffect.Frame.CenterY - (flipv ? (-3) : 3), frameEffect.Frame.Width, frameEffect.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "ItemBox";
        }
    }
}
