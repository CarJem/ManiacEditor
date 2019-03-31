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
    public class UIPicture : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            string binFile = "Icons";
            switch (Editor.Instance.Entities.SetupObject) {
                case "MenuSetup":
                    binFile = "Picture";
                    break;
                case "ThanksSetup":
                    binFile = "Thanks/Decorations";
                    break;
                case "LogoSetup":
                    binFile = "Logos";
                    break;

            }

            int frameID = (int)entity.attributesMap["frameID"].ValueVar;
            int listID = (int)entity.attributesMap["listID"].ValueVar;
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation(binFile, d, listID, frameID, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                //Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                d.DrawBitmap(frame.Texture, x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "UIPicture";
        }
    }
}
