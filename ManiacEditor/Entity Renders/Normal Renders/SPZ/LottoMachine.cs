using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class LottoMachine : EntityRenderer
    {

        public override void Draw(Classes.Core.Draw.GraphicsHandler d, SceneEntity entity, Classes.Core.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            //int type = (int)entity.attributesMap["type"].ValueUInt8;
            //int type = (int)entity.attributesMap["type"].ValueUInt8;
            //int height = (int)entity.attributesMap["height"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            bool allowToRender = false;
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("LottoMachine", d.DevicePanel, 0, 0, fliph, flipv, false);
            var editorAnim2 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("LottoMachine", d.DevicePanel, 0, 1, fliph, flipv, false);
            var editorAnim3 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("LottoMachine", d.DevicePanel, 0, 2, fliph, flipv, false);
            var editorAnim4 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("LottoMachine", d.DevicePanel, 0, 3, fliph, flipv, false);
            var editorAnim5 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("LottoMachine", d.DevicePanel, 0, 4, fliph, flipv, false);
            var editorAnim3_2 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("LottoMachine", d.DevicePanel, 0, 2, true, flipv, false);
            var editorAnim4_2 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("LottoMachine", d.DevicePanel, 0, 3, true, flipv, false);
            var editorAnim5_2 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("LottoMachine", d.DevicePanel, 0, 4, true, flipv, false);
            var editorAnim6 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("LottoMachine", d.DevicePanel, 0, 5, fliph, flipv, false);
            var editorAnim7 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("LottoMachine", d.DevicePanel, 0, 6, fliph, flipv, false);
            var editorAnim8 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("LottoMachine", d.DevicePanel, 0, 7, fliph, flipv, false);
            var editorAnim9 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("LottoMachine", d.DevicePanel, 0, 8, fliph, flipv, false);
            var editorAnim10 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("LottoMachine", d.DevicePanel, 0, 9, fliph, flipv, false);
            var editorAnim11 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("LottoMachine", d.DevicePanel, 0, 8, true, false, false);
            var editorAnim12 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("LottoMachine", d.DevicePanel, 0, 8, true, true, false);
            var editorAnim13 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("LottoMachine", d.DevicePanel, 0, 8, false, true, false);
            var editorAnim14 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("LottoMachine", d.DevicePanel, 0, 1, fliph, flipv, false);
            var editorAnim15 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("LottoMachine", d.DevicePanel, 0, 2, fliph, flipv, false);
            var editorAnim16 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("LottoMachine", d.DevicePanel, 0, 0, fliph, flipv, false);
            var editorAnim17 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("LottoMachine", d.DevicePanel, 2, 0, fliph, flipv, false);
            var editorAnim18 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("LottoMachine", d.DevicePanel, 0, 1, fliph, flipv, false);

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

                //ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame9),
x + frame9.Frame.PivotX - (fliph ? (frame9.Frame.Width - editorAnim9.Frames[0].Frame.Width) : 0),
y + frame9.Frame.PivotY + (flipv ? (frame9.Frame.Height - editorAnim9.Frames[0].Frame.Height) : 0),
frame9.Frame.Width, frame9.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame10),
                    x + frame10.Frame.PivotX - (fliph ? (frame10.Frame.Width - editorAnim10.Frames[0].Frame.Width) : 0),
                    y - 4 + frame10.Frame.PivotY + (flipv ? (frame10.Frame.Height - editorAnim10.Frames[0].Frame.Height) : 0),
                    frame10.Frame.Width, frame10.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame11),
x + frame11.Frame.Width + 32 + frame11.Frame.PivotX - (true ? (frame11.Frame.Width - editorAnim11.Frames[0].Frame.Width) : 0),
y + frame11.Frame.PivotY + (false ? (frame11.Frame.Height - editorAnim11.Frames[0].Frame.Height) : 0),
frame11.Frame.Width, frame11.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame12),
x + frame12.Frame.Width + 32 + frame12.Frame.PivotX - (true ? (frame12.Frame.Width - editorAnim12.Frames[0].Frame.Width) : 0),
y + frame12.Frame.Height + frame12.Frame.PivotY + (true ? (frame12.Frame.Height - editorAnim12.Frames[0].Frame.Height) : 0),
frame12.Frame.Width, frame12.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame13),
x + frame13.Frame.PivotX - (false ? (frame13.Frame.Width - editorAnim13.Frames[0].Frame.Width) : 0),
y + frame13.Frame.Height + frame13.Frame.PivotY + (true ? (frame13.Frame.Height - editorAnim13.Frames[0].Frame.Height) : 0),
frame13.Frame.Width, frame13.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(ballslot1),
                    x + ballslot1.Frame.PivotX - (fliph ? (ballslot1.Frame.Width - editorAnim3.Frames[0].Frame.Width) : 0),
                    y + ballslot1.Frame.PivotY + (flipv ? (ballslot1.Frame.Height - editorAnim3.Frames[0].Frame.Height) : 0),
                    ballslot1.Frame.Width, ballslot1.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(ballslot2),
                    x + ballslot2.Frame.PivotX - (fliph ? (ballslot2.Frame.Width - editorAnim4.Frames[0].Frame.Width) : 0),
                    y + ballslot2.Frame.PivotY + (flipv ? (ballslot2.Frame.Height - editorAnim4.Frames[0].Frame.Height) : 0),
                    ballslot2.Frame.Width, ballslot2.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(ballslot3),
x + ballslot3.Frame.PivotX - (fliph ? (ballslot3.Frame.Width - editorAnim5.Frames[0].Frame.Width) : 0),
y + ballslot3.Frame.PivotY + (flipv ? (ballslot3.Frame.Height - editorAnim5.Frames[0].Frame.Height) : 0),
ballslot3.Frame.Width, ballslot3.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(ballslot1_2),
x - ballslot1_2.Frame.Width - ballslot1_2.Frame.PivotX + (fliph ? (ballslot1_2.Frame.Width + editorAnim3.Frames[0].Frame.Width) : 0),
y + ballslot1_2.Frame.PivotY + (flipv ? (ballslot1_2.Frame.Height - editorAnim3.Frames[0].Frame.Height) : 0),
ballslot1_2.Frame.Width, ballslot1_2.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(ballslot2_2),
                    x - ballslot2_2.Frame.Width - ballslot2_2.Frame.PivotX + (fliph ? (ballslot2_2.Frame.Width + editorAnim4.Frames[0].Frame.Width) : 0),
                    y + ballslot2_2.Frame.PivotY + (flipv ? (ballslot2_2.Frame.Height - editorAnim4.Frames[0].Frame.Height) : 0),
                    ballslot2_2.Frame.Width, ballslot2_2.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(ballslot3_2),
x - ballslot3_2.Frame.Width - ballslot3_2.Frame.PivotX + (fliph ? (ballslot3_2.Frame.Width + editorAnim5.Frames[0].Frame.Width) : 0),
y + ballslot3_2.Frame.PivotY + (flipv ? (ballslot3_2.Frame.Height - editorAnim5.Frames[0].Frame.Height) : 0),
ballslot3_2.Frame.Width, ballslot3_2.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(galloplogo),
                    x + galloplogo.Frame.PivotX - (fliph ? (galloplogo.Frame.Width - editorAnim8.Frames[0].Frame.Width) : 0),
                    y + galloplogo.Frame.PivotY + (flipv ? (galloplogo.Frame.Height - editorAnim8.Frames[0].Frame.Height) : 0),
                    galloplogo.Frame.Width, galloplogo.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(chute),
x + chute.Frame.PivotX - (fliph ? (chute.Frame.Width - editorAnim17.Frames[0].Frame.Width) : 0),
y + chute.Frame.PivotY + 132 + (flipv ? (chute.Frame.Height - editorAnim17.Frames[0].Frame.Height) : 0),
chute.Frame.Width, chute.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(dispenser),
    x + dispenser.Frame.PivotX - (fliph ? (dispenser.Frame.Width - editorAnim2.Frames[0].Frame.Width) : 0),
    y + dispenser.Frame.PivotY + (flipv ? (dispenser.Frame.Height - editorAnim2.Frames[0].Frame.Height) : 0),
    dispenser.Frame.Width, dispenser.Frame.Height, false, Transparency);




            }
        }

        public override bool isObjectOnScreen(Classes.Core.Draw.GraphicsHandler d, SceneEntity entity, Classes.Core.Scene.Sets.EditorEntity e, int x, int y, int Transparency)
        {
            int bounds = 330;
            return d.IsObjectOnScreen(x - bounds / 2, y - bounds / 2, bounds, bounds);
        }

        public override string GetObjectName()
        {
            return "LottoMachine";
        }
    }
}
