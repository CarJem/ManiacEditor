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
    public class LottoMachine : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            //int type = (int)entity.attributesMap["type"].ValueUInt8;
            //int type = (int)entity.attributesMap["type"].ValueUInt8;
            //int height = (int)entity.attributesMap["height"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            bool allowToRender = false;
            var editorAnim = EditorEntity_ini.LoadAnimation2("LottoMachine", d, 0, 0, fliph, flipv, false);
            var editorAnim2 = EditorEntity_ini.LoadAnimation2("LottoMachine", d, 0, 1, fliph, flipv, false);
            var editorAnim3 = EditorEntity_ini.LoadAnimation2("LottoMachine", d, 0, 2, fliph, flipv, false);
            var editorAnim4 = EditorEntity_ini.LoadAnimation2("LottoMachine", d, 0, 3, fliph, flipv, false);
            var editorAnim5 = EditorEntity_ini.LoadAnimation2("LottoMachine", d, 0, 4, fliph, flipv, false);
            var editorAnim3_2 = EditorEntity_ini.LoadAnimation2("LottoMachine", d, 0, 2, true, flipv, false);
            var editorAnim4_2 = EditorEntity_ini.LoadAnimation2("LottoMachine", d, 0, 3, true, flipv, false);
            var editorAnim5_2 = EditorEntity_ini.LoadAnimation2("LottoMachine", d, 0, 4, true, flipv, false);
            var editorAnim6 = EditorEntity_ini.LoadAnimation2("LottoMachine", d, 0, 5, fliph, flipv, false);
            var editorAnim7 = EditorEntity_ini.LoadAnimation2("LottoMachine", d, 0, 6, fliph, flipv, false);
            var editorAnim8 = EditorEntity_ini.LoadAnimation2("LottoMachine", d, 0, 7, fliph, flipv, false);
            var editorAnim9 = EditorEntity_ini.LoadAnimation2("LottoMachine", d, 0, 8, fliph, flipv, false);
            var editorAnim10 = EditorEntity_ini.LoadAnimation2("LottoMachine", d, 0, 9, fliph, flipv, false);
            var editorAnim11 = EditorEntity_ini.LoadAnimation2("LottoMachine", d, 0, 8, true, false, false);
            var editorAnim12 = EditorEntity_ini.LoadAnimation2("LottoMachine", d, 0, 8, true, true, false);
            var editorAnim13 = EditorEntity_ini.LoadAnimation2("LottoMachine", d, 0, 8, false, true, false);
            var editorAnim14 = EditorEntity_ini.LoadAnimation2("LottoMachine", d, 0, 1, fliph, flipv, false);
            var editorAnim15 = EditorEntity_ini.LoadAnimation2("LottoMachine", d, 0, 2, fliph, flipv, false);
            var editorAnim16 = EditorEntity_ini.LoadAnimation2("LottoMachine", d, 0, 0, fliph, flipv, false);
            var editorAnim17 = EditorEntity_ini.LoadAnimation2("LottoMachine", d, 2, 0, fliph, flipv, false);
            var editorAnim18 = EditorEntity_ini.LoadAnimation2("LottoMachine", d, 0, 1, fliph, flipv, false);

            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0)
            {
                if (editorAnim3 != null && editorAnim3.Frames.Count != 0 && editorAnim4 != null && editorAnim4.Frames.Count != 0)
                {
                    if (editorAnim5 != null && editorAnim5.Frames.Count != 0 && editorAnim6 != null && editorAnim6.Frames.Count != 0)
                    {
                        if (editorAnim7 != null && editorAnim7.Frames.Count != 0 && editorAnim8 != null && editorAnim8.Frames.Count != 0)
                        {
                            if (editorAnim9 != null && editorAnim9.Frames.Count != 0 && editorAnim10 != null && editorAnim10.Frames.Count != 0)
                            {
                                if (editorAnim11 != null && editorAnim11.Frames.Count != 0 && editorAnim12 != null && editorAnim12.Frames.Count != 0)
                                {
                                    if (editorAnim13 != null && editorAnim13.Frames.Count != 0 && editorAnim14 != null && editorAnim14.Frames.Count != 0)
                                    {
                                        if (editorAnim15 != null && editorAnim15.Frames.Count != 0 && editorAnim16 != null && editorAnim16.Frames.Count != 0)
                                        {
                                            if (editorAnim17 != null && editorAnim17.Frames.Count != 0 && editorAnim18 != null && editorAnim18.Frames.Count != 0)
                                            {
                                                if (editorAnim3_2 != null && editorAnim3_2.Frames.Count != 0 && editorAnim4_2 != null && editorAnim4_2.Frames.Count != 0)
                                                {
                                                    if (editorAnim5_2 != null && editorAnim5_2.Frames.Count != 0)
                                                    {
                                                        allowToRender = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (allowToRender == true)
            {
                var frame = editorAnim.Frames[Animation.index];
                var dispenser = editorAnim2.Frames[Animation.index];
                var ballslot1 = editorAnim3.Frames[Animation.index];
                var ballslot2 = editorAnim4.Frames[Animation.index];
                var ballslot3 = editorAnim5.Frames[Animation.index];
                var frame6 = editorAnim6.Frames[Animation.index];
                var frame7 = editorAnim7.Frames[Animation.index];
                var galloplogo = editorAnim8.Frames[Animation.index];
                var frame9 = editorAnim9.Frames[Animation.index];
                var frame10 = editorAnim10.Frames[Animation.index];
                var frame11 = editorAnim11.Frames[Animation.index];
                var frame12 = editorAnim12.Frames[Animation.index];
                var frame13 = editorAnim13.Frames[Animation.index];
                var frame14 = editorAnim14.Frames[Animation.index];
                var frame15 = editorAnim15.Frames[Animation.index];
                var frame16 = editorAnim16.Frames[Animation.index];
                var chute = editorAnim17.Frames[Animation.index];
                var ballslot1_2 = editorAnim3_2.Frames[Animation.index];
                var ballslot2_2 = editorAnim4_2.Frames[Animation.index];
                var ballslot3_2 = editorAnim5_2.Frames[Animation.index];

                //ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);
                d.DrawBitmap(frame9.Texture,
x + frame9.Frame.CenterX - (fliph ? (frame9.Frame.Width - editorAnim9.Frames[0].Frame.Width) : 0),
y + frame9.Frame.CenterY + (flipv ? (frame9.Frame.Height - editorAnim9.Frames[0].Frame.Height) : 0),
frame9.Frame.Width, frame9.Frame.Height, false, Transparency);
                d.DrawBitmap(frame10.Texture,
                    x + frame10.Frame.CenterX - (fliph ? (frame10.Frame.Width - editorAnim10.Frames[0].Frame.Width) : 0),
                    y - 4 + frame10.Frame.CenterY + (flipv ? (frame10.Frame.Height - editorAnim10.Frames[0].Frame.Height) : 0),
                    frame10.Frame.Width, frame10.Frame.Height, false, Transparency);
                d.DrawBitmap(frame11.Texture,
x + frame11.Frame.Width + 32 + frame11.Frame.CenterX - (true ? (frame11.Frame.Width - editorAnim11.Frames[0].Frame.Width) : 0),
y + frame11.Frame.CenterY + (false ? (frame11.Frame.Height - editorAnim11.Frames[0].Frame.Height) : 0),
frame11.Frame.Width, frame11.Frame.Height, false, Transparency);
                d.DrawBitmap(frame12.Texture,
x + frame12.Frame.Width + 32 + frame12.Frame.CenterX - (true ? (frame12.Frame.Width - editorAnim12.Frames[0].Frame.Width) : 0),
y + frame12.Frame.Height + frame12.Frame.CenterY + (true ? (frame12.Frame.Height - editorAnim12.Frames[0].Frame.Height) : 0),
frame12.Frame.Width, frame12.Frame.Height, false, Transparency);
                d.DrawBitmap(frame13.Texture,
x + frame13.Frame.CenterX - (false ? (frame13.Frame.Width - editorAnim13.Frames[0].Frame.Width) : 0),
y + frame13.Frame.Height + frame13.Frame.CenterY + (true ? (frame13.Frame.Height - editorAnim13.Frames[0].Frame.Height) : 0),
frame13.Frame.Width, frame13.Frame.Height, false, Transparency);
                d.DrawBitmap(ballslot1.Texture,
                    x + ballslot1.Frame.CenterX - (fliph ? (ballslot1.Frame.Width - editorAnim3.Frames[0].Frame.Width) : 0),
                    y + ballslot1.Frame.CenterY + (flipv ? (ballslot1.Frame.Height - editorAnim3.Frames[0].Frame.Height) : 0),
                    ballslot1.Frame.Width, ballslot1.Frame.Height, false, Transparency);
                d.DrawBitmap(ballslot2.Texture,
                    x + ballslot2.Frame.CenterX - (fliph ? (ballslot2.Frame.Width - editorAnim4.Frames[0].Frame.Width) : 0),
                    y + ballslot2.Frame.CenterY + (flipv ? (ballslot2.Frame.Height - editorAnim4.Frames[0].Frame.Height) : 0),
                    ballslot2.Frame.Width, ballslot2.Frame.Height, false, Transparency);
                d.DrawBitmap(ballslot3.Texture,
x + ballslot3.Frame.CenterX - (fliph ? (ballslot3.Frame.Width - editorAnim5.Frames[0].Frame.Width) : 0),
y + ballslot3.Frame.CenterY + (flipv ? (ballslot3.Frame.Height - editorAnim5.Frames[0].Frame.Height) : 0),
ballslot3.Frame.Width, ballslot3.Frame.Height, false, Transparency);
                d.DrawBitmap(ballslot1_2.Texture,
x - ballslot1_2.Frame.Width - ballslot1_2.Frame.CenterX + (fliph ? (ballslot1_2.Frame.Width + editorAnim3.Frames[0].Frame.Width) : 0),
y + ballslot1_2.Frame.CenterY + (flipv ? (ballslot1_2.Frame.Height - editorAnim3.Frames[0].Frame.Height) : 0),
ballslot1_2.Frame.Width, ballslot1_2.Frame.Height, false, Transparency);
                d.DrawBitmap(ballslot2_2.Texture,
                    x - ballslot2_2.Frame.Width - ballslot2_2.Frame.CenterX + (fliph ? (ballslot2_2.Frame.Width + editorAnim4.Frames[0].Frame.Width) : 0),
                    y + ballslot2_2.Frame.CenterY + (flipv ? (ballslot2_2.Frame.Height - editorAnim4.Frames[0].Frame.Height) : 0),
                    ballslot2_2.Frame.Width, ballslot2_2.Frame.Height, false, Transparency);
                d.DrawBitmap(ballslot3_2.Texture,
x - ballslot3_2.Frame.Width - ballslot3_2.Frame.CenterX + (fliph ? (ballslot3_2.Frame.Width + editorAnim5.Frames[0].Frame.Width) : 0),
y + ballslot3_2.Frame.CenterY + (flipv ? (ballslot3_2.Frame.Height - editorAnim5.Frames[0].Frame.Height) : 0),
ballslot3_2.Frame.Width, ballslot3_2.Frame.Height, false, Transparency);
                d.DrawBitmap(galloplogo.Texture,
                    x + galloplogo.Frame.CenterX - (fliph ? (galloplogo.Frame.Width - editorAnim8.Frames[0].Frame.Width) : 0),
                    y + galloplogo.Frame.CenterY + (flipv ? (galloplogo.Frame.Height - editorAnim8.Frames[0].Frame.Height) : 0),
                    galloplogo.Frame.Width, galloplogo.Frame.Height, false, Transparency);
                d.DrawBitmap(chute.Texture,
x + chute.Frame.CenterX - (fliph ? (chute.Frame.Width - editorAnim17.Frames[0].Frame.Width) : 0),
y + chute.Frame.CenterY + 132 + (flipv ? (chute.Frame.Height - editorAnim17.Frames[0].Frame.Height) : 0),
chute.Frame.Width, chute.Frame.Height, false, Transparency);
                d.DrawBitmap(dispenser.Texture,
    x + dispenser.Frame.CenterX - (fliph ? (dispenser.Frame.Width - editorAnim2.Frames[0].Frame.Width) : 0),
    y + dispenser.Frame.CenterY + (flipv ? (dispenser.Frame.Height - editorAnim2.Frames[0].Frame.Height) : 0),
    dispenser.Frame.Width, dispenser.Frame.Height, false, Transparency);




            }
        }

        public override string GetObjectName()
        {
            return "LottoMachine";
        }
    }
}
