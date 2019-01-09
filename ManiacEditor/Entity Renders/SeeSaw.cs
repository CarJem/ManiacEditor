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
    public class SeeSaw : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            if (e.EditorInstance.entities.SetupObject != "MMZSetup")
            {
                int side = (int)entity.attributesMap["side"].ValueUInt8;
                bool fliph = false;
                switch (side)
                {
                    case 0:
                        fliph = false;
                        break;
                    case 1:
                        fliph = true;
                        break;
                    default:
                        fliph = false;
                        break;
                }
                var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("SeeSaw", d, 0, 0, false, false, false);
                var editorAnim2 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("SeeSaw", d, 1, 0, false, false, false);
                var editorAnim3 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("SeeSaw", d, 2, 0, false, false, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0)
                {
                    var frame = editorAnim.Frames[0];
                    var frame2 = editorAnim2.Frames[0];
                    var frame3 = editorAnim3.Frames[0];

                    d.DrawBitmap(frame3.Texture,
                        x + frame3.Frame.CenterX - (fliph ? -35 : 35),
                        y + frame3.Frame.CenterY - 15,
                        frame3.Frame.Width, frame3.Frame.Height, false, Transparency);

                    d.DrawBitmap(frame2.Texture,
                        x + frame2.Frame.CenterX,
                        y + frame2.Frame.CenterY,
                        frame2.Frame.Width, frame2.Frame.Height, false, Transparency);

                    d.DrawBitmap(frame.Texture,
                        x + frame.Frame.CenterX,
                        y + frame.Frame.CenterY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);


                }
            }

        }

        public override string GetObjectName()
        {
            return "SeeSaw";
        }
    }
}
