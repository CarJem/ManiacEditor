using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class BigSqueeze : EntityRenderer
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
            Methods.Entities.EntityAnimator Animation = properties.Animations;
            bool selected  = properties.isSelected;
            int type = (int)entity.attributesMap["type"].ValueEnum;
            bool fliph = false;
            bool flipv = false;
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("BigSqueeze", d.DevicePanel, 0, -1, fliph, flipv, false);
            var editorAnim2 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("BigSqueeze", d.DevicePanel, 0, -1, true, flipv, false);
            var editorAnim3 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("BigSqueeze", d.DevicePanel, 1, -1, fliph, flipv, false);
            var editorAnim4 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("BigSqueeze", d.DevicePanel, 2, -1, fliph, flipv, false);
            var editorAnim5 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("BigSqueeze", d.DevicePanel, 3, -1, fliph, flipv, false);
            var editorAnim6 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("BigSqueeze", d.DevicePanel, 4, -1, fliph, flipv, false);
            var editorAnim7 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("BigSqueeze", d.DevicePanel, 4, -1, true, flipv, false);
            var editorAnim8 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("BigSqueeze", d.DevicePanel, 5, -1, true, flipv, false);
            var editorAnim9 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("EditorIcons2", d.DevicePanel, 0, 14, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0 && editorAnim4 != null && editorAnim4.Frames.Count != 0 && editorAnim5 != null && editorAnim5.Frames.Count != 0 && editorAnim6 != null && editorAnim6.Frames.Count != 0 && editorAnim7 != null && editorAnim7.Frames.Count != 0 && editorAnim8 != null && editorAnim8.Frames.Count != 0 && editorAnim9 != null && editorAnim9.Frames.Count != 0)
            {
                var wall1 = editorAnim.Frames[0];
                var wall2 = editorAnim.Frames[1];
                var wall3 = editorAnim.Frames[2];
                var wall1r = editorAnim2.Frames[0];
                var wall2r = editorAnim2.Frames[1];
                var wall3r = editorAnim2.Frames[2];
                var electroMagBase = editorAnim3.Frames[0];
                var electroMagDome = editorAnim4.Frames[Animation.index];
                var electroMagProngs = editorAnim5.Frames[Animation.index];
                var wheels = editorAnim6.Frames[Animation.index];
                var wheelsr = editorAnim7.Frames[Animation.index];
                var shocking = editorAnim8.Frames[Animation.index];
                var frame = editorAnim9.Frames[0];

                Animation.ProcessAnimation(electroMagDome.Entry.SpeedMultiplyer, electroMagDome.Entry.Frames.Count, electroMagDome.Frame.Delay);
                Animation.ProcessAnimation(electroMagProngs.Entry.SpeedMultiplyer, electroMagProngs.Entry.Frames.Count, electroMagProngs.Frame.Delay);
                Animation.ProcessAnimation(wheels.Entry.SpeedMultiplyer, wheels.Entry.Frames.Count, wheels.Frame.Delay);
                Animation.ProcessAnimation(wheelsr.Entry.SpeedMultiplyer, wheelsr.Entry.Frames.Count, wheelsr.Frame.Delay);
                //Animation.ProcessAnimation(shocking.Entry.SpeedMultiplyer, shocking.Entry.Frames.Count, shocking.Frame.Delay);

                if (type == 2)
                {
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(wall1),
                        x + wall1.Frame.PivotX - (fliph ? (wall1.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                        y + wall1.Frame.PivotY + (flipv ? (wall1.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                        wall1.Frame.Width, wall1.Frame.Height, false, Transparency);
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(wall2),
                        x + wall2.Frame.PivotX - (fliph ? (wall2.Frame.Width - editorAnim.Frames[1].Frame.Width) : 0),
                        y + wall2.Frame.PivotY + (flipv ? (wall2.Frame.Height - editorAnim.Frames[1].Frame.Height) : 0),
                        wall2.Frame.Width, wall2.Frame.Height, false, Transparency);
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(wall3),
                        x + wall3.Frame.PivotX - (fliph ? (wall3.Frame.Width - editorAnim.Frames[2].Frame.Width) : 0),
                        y + wall3.Frame.PivotY + (flipv ? (wall3.Frame.Height - editorAnim.Frames[2].Frame.Height) : 0),
                        wall3.Frame.Width, wall3.Frame.Height, false, Transparency);
                }
                else if (type == 3)
                {
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(wall1r),
                        x + wall1r.Frame.PivotX,
                        y + wall1r.Frame.PivotY + (flipv ? (wall1r.Frame.Height - editorAnim2.Frames[0].Frame.Height) : 0),
                        wall1r.Frame.Width, wall1r.Frame.Height, false, Transparency);
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(wall2r),
                        x + wall2r.Frame.PivotX + -wall2r.Frame.PivotX + 23,
                        y + wall2r.Frame.PivotY + (flipv ? (wall2r.Frame.Height - editorAnim2.Frames[1].Frame.Height) : 0),
                        wall2.Frame.Width, wall2.Frame.Height, false, Transparency);
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(wall3r),
                        x + wall3r.Frame.PivotX + -wall3r.Frame.PivotX + 55,
                        y + wall3r.Frame.PivotY + (flipv ? (wall3r.Frame.Height - editorAnim2.Frames[2].Frame.Height) : 0),
                        wall3r.Frame.Width, wall3r.Frame.Height, false, Transparency);
                }

                else if (type == 1)
                {
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(electroMagBase),
                        x + electroMagBase.Frame.PivotX - (fliph ? (electroMagBase.Frame.Width - editorAnim3.Frames[0].Frame.Width) : 0),
                        y + electroMagBase.Frame.PivotY + (flipv ? (electroMagBase.Frame.Height - editorAnim3.Frames[0].Frame.Height) : 0),
                        electroMagBase.Frame.Width, electroMagBase.Frame.Height, false, Transparency);
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(electroMagDome),
                        x + electroMagDome.Frame.PivotX - (fliph ? (electroMagDome.Frame.Width - editorAnim4.Frames[0].Frame.Width) : 0),
                        y + electroMagDome.Frame.PivotY + (flipv ? (electroMagDome.Frame.Height - editorAnim4.Frames[0].Frame.Height) : 0),
                        electroMagDome.Frame.Width, electroMagDome.Frame.Height, false, Transparency);
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(electroMagProngs),
                        x + electroMagProngs.Frame.PivotX - (fliph ? (electroMagProngs.Frame.Width - editorAnim5.Frames[0].Frame.Width) : 0),
                        y + electroMagProngs.Frame.PivotY + (flipv ? (electroMagProngs.Frame.Height - editorAnim5.Frames[0].Frame.Height) : 0),
                        electroMagProngs.Frame.Width, electroMagProngs.Frame.Height, false, Transparency);
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(wheels),
                        x + wheels.Frame.PivotX - (fliph ? (wheels.Frame.Width - editorAnim6.Frames[0].Frame.Width) : 0) - 20,
                        y + wheels.Frame.PivotY + (flipv ? (wheels.Frame.Height - editorAnim6.Frames[0].Frame.Height) : 0),
                        wheels.Frame.Width, wheels.Frame.Height, false, Transparency);
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(wheelsr),
                        x + wheelsr.Frame.PivotX + 20,
                        y + wheelsr.Frame.PivotY + (flipv ? (wheelsr.Frame.Height - editorAnim7.Frames[0].Frame.Height) : 0),
                        wheelsr.Frame.Width, wheelsr.Frame.Height, false, Transparency);
                    /*
                    if (Editor.Instance.ShowAnimations.Checked && Properties.EditorState.Default.annimationsChecked)
                    {

                        d.DrawBitmap(new GraphicsHandler.GraphicsInfo(shocking),
                            x + shocking.Frame.PivotX,
                            y + shocking.Frame.PivotY + (flipv ? (shocking.Frame.Height - editorAnim8.Frames[0].Frame.Height) : 0),
                            shocking.Frame.Width, shocking.Frame.Height, false, Transparency);

                    }
                    */

                    // TODO: Make sure this does not crash the editor.
                }

                else if (type == 0)
                {
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                        x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                        y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }



            }
        }

        public override string GetObjectName()
        {
            return "BigSqueeze";
        }
    }
}
