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
    public class PSZDoor : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int orientation = entity.attributesMap["orientation"].ValueUInt8;
            bool open = entity.attributesMap["open"].ValueBool;
            bool fliph = false;
            bool flipv = false;
            int frameID = 0;
            switch (orientation)
            {
                case 0:
                    frameID = 0;
                    break;
                case 1:
                    frameID = 0;
                    fliph = true;
                    break;
                case 2:
                    frameID = 1;

                    break;
                case 3:
                    frameID = 1;
                    flipv = true;
                    break;
            }
            var editorAnim = EditorEntity_ini.LoadAnimation2("PSZDoor", d, 0, frameID, fliph, flipv, false);
            var cogPart2 = EditorEntity_ini.LoadAnimation2("PSZDoor", d, 0, 2, fliph, flipv, false);
            var cogPart1 = EditorEntity_ini.LoadAnimation2("PSZDoor", d, 1, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && frameID >= 0 && cogPart1 != null && cogPart1.Frames.Count != 0 && cogPart2 != null && cogPart2.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var cog1 = cogPart1.Frames[Animation.index];
                var cog2 = cogPart2.Frames[Animation.index];

                int COG_SPACE = 18;

                int cogSpaceH = 56;
                int cogSpaceW = -COG_SPACE;
                int cogSpaceH_2 = 56;
                int cogSpaceW_2 = -COG_SPACE;
                int doorAdjX = frame.Frame.Width;
                int doorAdjY = 0;

                switch (orientation)
                {
                    case 1:
                        // Upper
                        cogSpaceH = 56;
                        cogSpaceW = -COG_SPACE;
                        // Lower
                        cogSpaceH_2 = 56;
                        cogSpaceW_2 = COG_SPACE;


                        //Door
                        doorAdjX = 0;
                        doorAdjY = frame.Frame.Height;
                        break;
                    case 0:
                        // Upper
                        cogSpaceH = -56;
                        cogSpaceW = -COG_SPACE;
                        // Lower
                        cogSpaceH_2 = -56;
                        cogSpaceW_2 = COG_SPACE;


                        //Door
                        doorAdjX = 0;
                        doorAdjY = -frame.Frame.Height;
                        break;
                    case 2:
                        // Upper
                        cogSpaceW = -56;
                        cogSpaceH = COG_SPACE;
                        // Lower
                        cogSpaceW_2 = -56;
                        cogSpaceH_2 = -COG_SPACE;


                        //Door
                        doorAdjX = -frame.Frame.Width;
                        doorAdjY = 0;
                        break;
                    case 3:
                        // Upper
                        cogSpaceW = 56;
                        cogSpaceH = COG_SPACE;
                        // Lower
                        cogSpaceW_2 = 56;
                        cogSpaceH_2 = -COG_SPACE;


                        //Door
                        doorAdjX = frame.Frame.Width;
                        doorAdjY = 0;
                        break;
                }

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX + (open ? doorAdjX : 0),
                    y + frame.Frame.CenterY + (open ? doorAdjY : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

                //Upper Cog
                d.DrawBitmap(cog1.Texture,
                    x + cog1.Frame.CenterX + cogSpaceW,
                    y + cog1.Frame.CenterY + cogSpaceH,
                    cog1.Frame.Width, cog1.Frame.Height, false, Transparency);

                d.DrawBitmap(cog2.Texture,
                    x + cog2.Frame.CenterX + cogSpaceW,
                    y + cog2.Frame.CenterY + cogSpaceH,
                    cog2.Frame.Width, cog2.Frame.Height, false, Transparency);

                //Lower Cog
                d.DrawBitmap(cog1.Texture,
                    x + cog1.Frame.CenterX + cogSpaceW_2,
                    y + cog1.Frame.CenterY + cogSpaceH_2,
                    cog1.Frame.Width, cog1.Frame.Height, false, Transparency);

                d.DrawBitmap(cog2.Texture,
                    x + cog2.Frame.CenterX + cogSpaceW_2,
                    y + cog2.Frame.CenterY + cogSpaceH_2,
                    cog2.Frame.Width, cog2.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "PSZDoor";
        }
    }
}
