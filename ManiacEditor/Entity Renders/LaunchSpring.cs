using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D9;
using ManiacEditor;
using Microsoft.Xna.Framework;
using RSDKv5;


namespace ManiacEditor.Entity_Renders
{
    public class LaunchSpring : EntityRenderer
    {
        //Shorthanding Setting Files
        Properties.Settings mySettings = Properties.Settings.Default;
        Properties.EditorState myEditorState = Properties.EditorState.Default;
        Properties.KeyBinds myKeyBinds = Properties.KeyBinds.Default;

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int angle = (int)entity.attributesMap["angle"].ValueInt32;
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int rotation = 0;
            switch (angle)
            {
                case 0:
                    rotation = 0;
                    break;
                case 1:
                    rotation = 45;
                    break;
                case 2:
                    rotation = 90;
                    break;
                case 3:
                    rotation = 135;
                    break;
                case 4:
                   rotation = 180;
                   break;
                case 5:
                    rotation = 225;
                    break;
                case 6:
                    rotation = 270;
                    break;
                case 7:
                    rotation = 315;
                    break;
            }

            var editorAnim = EditorEntity_ini.LoadAnimation2("LaunchSpring", d, 0, -1, false, false, true, rotation);
            var editorAnim2 = EditorEntity_ini.LoadAnimation2("LaunchSpring", d, 0, -1, true, false, true, rotation);
            var editorAnim3 = EditorEntity_ini.LoadAnimation2("LaunchSpring", d, 1, -1, false, false, false, 0);
            var editorAnim4 = EditorEntity_ini.LoadAnimation2("LaunchSpring", d, 2, -1, false, false, true, rotation);

            if (editorAnim != null && editorAnim2 != null && editorAnim3 != null && editorAnim4 != null && editorAnim.Frames.Count != 0)
                {
                    var frame = editorAnim.Frames[0];
                    var frame2 = editorAnim2.Frames[0];
                    var frame3 = editorAnim3.Frames[0];
                    var frame4 = editorAnim4.Frames[1];

                switch (angle)
                {
                    case 0:
                        //Spring
                        d.DrawBitmap(frame4.Texture,
                                x + frame4.Frame.CenterX,
                                y + frame4.Frame.CenterY - 11,
                                frame4.ImageWidth, frame4.ImageHeight, false, Transparency);
                        //Universal Center Screw
                        d.DrawBitmap(frame3.Texture,
                            x + frame3.Frame.CenterX,
                            y + frame3.Frame.CenterY,
                            frame3.ImageWidth, frame3.ImageHeight, false, Transparency);
                        //Launcher
                        d.DrawBitmap(frame.Texture,
                            x + frame.Frame.CenterX - 20,
                            y + frame.Frame.CenterY + (type == 0 ? 46 : 0),
                            frame.ImageWidth, frame.ImageHeight, false, Transparency);
                        d.DrawBitmap(frame2.Texture,
                            x + frame2.Frame.CenterX + 20,
                            y + frame2.Frame.CenterY,
                            frame2.ImageWidth, frame2.ImageHeight, false, Transparency);
                        break;
                    case 1:
                        //Spring
                        d.DrawBitmap(frame4.Texture,
                                x + frame4.Frame.CenterX - 48,
                                y + frame4.Frame.CenterY - 73,
                                frame4.ImageWidth, frame4.ImageHeight, false, Transparency);
                        //Universal Center Screw
                        d.DrawBitmap(frame3.Texture,
                            x + frame3.Frame.CenterX,
                            y + frame3.Frame.CenterY,
                            frame3.ImageWidth, frame3.ImageHeight, false, Transparency);
                        //Launcher
                        d.DrawBitmap(frame.Texture,
                            x + frame.Frame.CenterX - 17 - (type == 0 ? 32 : 0),
                            y + frame.Frame.CenterY - 70 + (type == 0 ? 33 : 0),
                            frame.ImageWidth, frame.ImageHeight, false, Transparency);
                        d.DrawBitmap(frame2.Texture,
                            x + frame2.Frame.CenterX + 11,
                            y + frame2.Frame.CenterY - 43,
                            frame2.ImageWidth, frame2.ImageHeight, false, Transparency);
                        break;
                    case 2:
                        //Spring
                        d.DrawBitmap(frame4.Texture,
                                x + frame4.Frame.CenterX - 29,
                                y + frame4.Frame.CenterY - 64,
                                frame4.ImageWidth, frame4.ImageHeight, false, Transparency);
                        //Universal Center Screw
                        d.DrawBitmap(frame3.Texture,
                            x + frame3.Frame.CenterX,
                            y + frame3.Frame.CenterY,
                            frame3.ImageWidth, frame3.ImageHeight, false, Transparency);
                        //Launcher
                        d.DrawBitmap(frame.Texture,
                            x + frame.Frame.CenterX + 23 - (type == 0 ? 47 : 0),
                            y + frame.Frame.CenterY - 41,
                            frame.ImageWidth, frame.ImageHeight, false, Transparency);
                        d.DrawBitmap(frame2.Texture,
                            x + frame2.Frame.CenterX + 23,
                            y + frame2.Frame.CenterY - 2,
                            frame2.ImageWidth, frame2.ImageHeight, false, Transparency);
                        break;
                    case 3:
                        //Spring
                        d.DrawBitmap(frame4.Texture,
                                x + frame4.Frame.CenterX - 25,
                                y + frame4.Frame.CenterY - 49,
                                frame4.ImageWidth, frame4.ImageHeight, false, Transparency);
                        //Universal Center Screw
                        d.DrawBitmap(frame3.Texture,
                            x + frame3.Frame.CenterX,
                            y + frame3.Frame.CenterY,
                            frame3.ImageWidth, frame3.ImageHeight, false, Transparency);
                        //Launcher
                        d.DrawBitmap(frame.Texture,
                            x + frame.Frame.CenterX + 28 - (type == 0 ? 32 : 0),
                            y + frame.Frame.CenterY + 6 - (type == 0 ? 32 : 0),
                            frame.ImageWidth, frame.ImageHeight, false, Transparency);
                        d.DrawBitmap(frame2.Texture,
                            x + frame2.Frame.CenterX + 1,
                            y + frame2.Frame.CenterY + 34,
                            frame2.ImageWidth, frame2.ImageHeight, false, Transparency);
                        break;
                    case 4:
                        //Spring
                        d.DrawBitmap(frame4.Texture,
                                x + frame4.Frame.CenterX - 32,
                                y + frame4.Frame.CenterY - 30,
                                frame4.ImageWidth, frame4.ImageHeight, false, Transparency);
                        //Universal Center Screw
                        d.DrawBitmap(frame3.Texture,
                            x + frame3.Frame.CenterX,
                            y + frame3.Frame.CenterY,
                            frame3.ImageWidth, frame3.ImageHeight, false, Transparency);
                        //Launcher
                        d.DrawBitmap(frame.Texture,
                            x + frame.Frame.CenterX - 1,
                            y + frame.Frame.CenterY + 45 - (type == 0 ? 47 : 0),
                            frame.ImageWidth, frame.ImageHeight, false, Transparency);
                        d.DrawBitmap(frame2.Texture,
                            x + frame2.Frame.CenterX - 40,
                            y + frame2.Frame.CenterY + 45,
                            frame2.ImageWidth, frame2.ImageHeight, false, Transparency);
                        break;
                    case 5:
                        //Spring
                        d.DrawBitmap(frame4.Texture,
                                x + frame4.Frame.CenterX - 52,
                                y + frame4.Frame.CenterY - 23,
                                frame4.ImageWidth, frame4.ImageHeight, false, Transparency);
                        //Universal Center Screw
                        d.DrawBitmap(frame3.Texture,
                            x + frame3.Frame.CenterX,
                            y + frame3.Frame.CenterY,
                            frame3.ImageWidth, frame3.ImageHeight, false, Transparency);
                        //Launcher
                        d.DrawBitmap(frame.Texture,
                            x + frame.Frame.CenterX - 52 + (type == 0 ? 36 : 0),
                            y + frame.Frame.CenterY + 50 - (type == 0 ? 30 : 0),
                            frame.ImageWidth, frame.ImageHeight, false, Transparency);
                        d.DrawBitmap(frame2.Texture,
                            x + frame2.Frame.CenterX - 78,
                            y + frame2.Frame.CenterY + 24,
                            frame2.ImageWidth, frame2.ImageHeight, false, Transparency);
                        break;
                    case 6:
                        //Spring
                        d.DrawBitmap(frame4.Texture,
                                x + frame4.Frame.CenterX - 66,
                                y + frame4.Frame.CenterY - 32,
                                frame4.ImageWidth, frame4.ImageHeight, false, Transparency);
                        //Universal Center Screw
                        d.DrawBitmap(frame3.Texture,
                            x + frame3.Frame.CenterX,
                            y + frame3.Frame.CenterY,
                            frame3.ImageWidth, frame3.ImageHeight, false, Transparency);
                        //Launcher
                        d.DrawBitmap(frame.Texture,
                            x + frame.Frame.CenterX - 88 + (type == 0 ? 47 : 0),
                            y + frame.Frame.CenterY + 22,
                            frame.ImageWidth, frame.ImageHeight, false, Transparency);
                        d.DrawBitmap(frame2.Texture,
                            x + frame2.Frame.CenterX - 88,
                            y + frame2.Frame.CenterY - 17,
                            frame2.ImageWidth, frame2.ImageHeight, false, Transparency);
                        break;
                    case 7:
                        //Spring
                        d.DrawBitmap(frame4.Texture,
                                x + frame4.Frame.CenterX - 73,
                                y + frame4.Frame.CenterY - 48,
                                frame4.ImageWidth, frame4.ImageHeight, false, Transparency);
                        //Universal Center Screw
                        d.DrawBitmap(frame3.Texture,
                            x + frame3.Frame.CenterX,
                            y + frame3.Frame.CenterY,
                            frame3.ImageWidth, frame3.ImageHeight, false, Transparency);
                        //Launcher
                        d.DrawBitmap(frame.Texture,
                            x + frame.Frame.CenterX - 94,
                            y + frame.Frame.CenterY - 27,
                            frame.ImageWidth, frame.ImageHeight, false, Transparency);
                        d.DrawBitmap(frame2.Texture,
                            x + frame2.Frame.CenterX - 67 + (type == 0 ? 34 : 0),
                            y + frame2.Frame.CenterY - 55 + (type == 0 ? 34 : 0),
                            frame2.ImageWidth, frame2.ImageHeight, false, Transparency);
                        break;
                    default:
                        //Spring
                        d.DrawBitmap(frame4.Texture,
                                x + frame4.Frame.CenterX,
                                y + frame4.Frame.CenterY,
                                frame4.ImageWidth, frame4.ImageHeight, false, Transparency);
                        //Universal Center Screw
                        d.DrawBitmap(frame3.Texture,
                            x + frame3.Frame.CenterX,
                            y + frame3.Frame.CenterY,
                            frame3.ImageWidth, frame3.ImageHeight, false, Transparency);
                        //Launcher
                        d.DrawBitmap(frame.Texture,
                            x + frame.Frame.CenterX + (type == 0 ? 47 : 0),
                            y + frame.Frame.CenterY,
                            frame.ImageWidth, frame.ImageHeight, false, Transparency);
                        d.DrawBitmap(frame2.Texture,
                            x + frame2.Frame.CenterX + frame3.Frame.Width + 24,
                            y + frame2.Frame.CenterY,
                            frame2.ImageWidth, frame2.ImageHeight, false, Transparency);
                        break;
                }


            }
        }




        public override string GetObjectName()
        {
            return "LaunchSpring";
        }
    }
}
