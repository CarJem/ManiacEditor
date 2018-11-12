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
    public class CutsceneHBH : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            int characterID = (int)entity.attributesMap["characterID"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            string sprite = "";
            int animID = 0;
            bool multiFrame = false;
            switch (characterID)
            {
                case 0:
                    sprite = "Gunner";
                    animID = 0;
                    break;
                case 1:
                    sprite = "Shinobi";
                    animID = 0;
                    break;
                case 2:
                    sprite = "Mystic";
                    animID = 0;
                    break;
                case 3:
                    sprite = "Rider";
                    animID = 0;
                    break;
                case 4:
                    sprite = "King";
                    animID = 0;
                    multiFrame = true;
                    break;
                case 5:
                    sprite = "Rogues";
                    animID = 0;
                    break;
                case 6:
                    sprite = "Rogues";
                    animID = 5;
                    break;
                case 7:
                    sprite = "Rogues";
                    animID = 12;
                    break;
                case 8:
                    sprite = "DamagedKing";
                    animID = 0;
                    multiFrame = true;
                    break;
                case 9:
                    sprite = "HBHPile";
                    animID = 0;
                    break;
                default:
                    sprite = "Gunner";
                    animID = 0;
                    break;
            }
            if (direction == 1)
            {
                fliph = true;
            }
            var editorAnim = e.LoadAnimation2(sprite, d, animID, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[e.index];

                e.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (multiFrame)
            {
                string sprite2 = "";
                int animID2 = 0;
                switch (characterID)
                {
                    case 4:
                        sprite2 = "King";
                        animID2 = 14;
                        break;
                    case 8:
                        sprite = "DamagedKing";
                        animID2 = 1;
                        break;
                }
                var editorAnim2 = e.LoadAnimation2(sprite2, d, animID2, -1, fliph, flipv, false);
                if (editorAnim2 != null && editorAnim2.Frames.Count != 0)
                {
                    var frame = editorAnim2.Frames[e.index];

                    e.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);

                    d.DrawBitmap(frame.Texture,
                        x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim2.Frames[0].Frame.Width) : 0),
                        y + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim2.Frames[0].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
            }

        }

        public override string GetObjectName()
        {
            return "CutsceneHBH";
        }
    }
}
