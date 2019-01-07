using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ManiacEditor;
using Microsoft.Win32.SafeHandles;
using Microsoft.Xna.Framework;
using RSDKv5;
using SystemColors = System.Drawing.Color;

namespace ManiacEditor.Entity_Renders
{
    public class Water : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int type = (int)entity.attributesMap["type"].ValueVar;
            var widthPixels = (int)(entity.attributesMap["size"].ValuePosition.X.High);
            var heightPixels = (int)(entity.attributesMap["size"].ValuePosition.Y.High);
            var heightY = (int)(entity.attributesMap["height"].ValuePosition.Y.High);
            var heightX = (int)(entity.attributesMap["height"].ValuePosition.X.High);
            var width = (int)widthPixels / 16;
            var height = (int)heightPixels / 16;
            bool fliph = false;
            bool flipv = false;
            bool showBounds = false;
            bool HCZBubbles = false;
            int animID = 0;
            switch (type)
            {
                case 0:
                    showBounds = false;
                    break;
                case 1:
                    showBounds = true;
                    break;
                case 2:
                    showBounds = false;
                    animID = 2;
                    break;
                case 3:
                    showBounds = true;
                    break;
                case 4:
                    showBounds = true;
                    break;
                case 5:
                    showBounds = false;
                    HCZBubbles = true;
                    break;
            }

            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Water", d, animID, -1, fliph, flipv, false);
            var editorAnim2 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Water", d, 0, -1, fliph, flipv, false);

            // Base Water + Bubble Source
            if (editorAnim != null && editorAnim.Frames.Count != 0 && animID >= 0 && (type == 2 || type == 0))
            {
                var frame = editorAnim.Frames[Animation.index];

                Animation.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);

                d.DrawBitmap(frame.Texture,
            x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
            y + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
            frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }


            // HCZ Big Bubbles
            else if (HCZBubbles == true)
            {
                editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("BigBubble", d, 7, -1, fliph, flipv, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    var frame = editorAnim.Frames[Animation.index];

                    Animation.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);

                    d.DrawBitmap(frame.Texture,
                        x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                        y + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
            }

            // Bounded Water
            if (width != 0 && height != 0 && showBounds == true && HCZBubbles == false)
            {
                //Draw Icon
                editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("EditorIcons2", d, 0, 8, fliph, flipv, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    var frame = editorAnim.Frames[Animation.index];

                    d.DrawBitmap(frame.Texture,
                        x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                        y + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
                int x1 = x + widthPixels / -2;
                int x2 = x + widthPixels / 2 - 1;
                int y1 = y + heightPixels / -2;
                int y2 = y + heightPixels / 2 - 1;

                if (e.EditorInstance.showWaterLevel)
                {
                    if (selected)
                    {
                        if (!Properties.Settings.Default.SimplifiedWaterLevelRendering)
                        {
                            d.DrawRectangle(0, heightX, e.EditorInstance.SceneWidth, e.EditorInstance.SceneHeight, e.EditorInstance.waterColor);
                            d.DrawLine(0, heightX, e.EditorInstance.SceneWidth, heightX, SystemColors.White);
                            if (editorAnim2 != null && editorAnim2.Frames.Count != 0)
                            {
                                var frame = editorAnim2.Frames[Animation.index];

                                Animation.ProcessAnimation2(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);

                                for (int i = 0; i < e.EditorInstance.SceneWidth; i = i + frame.Frame.Width)
                                    d.DrawBitmap(frame.Texture,
                                        i + frame.Frame.CenterX,
                                        heightX + frame.Frame.CenterY,
                                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                            }
                        }
                        else
                        {
                            d.DrawRectangle(0, heightX, e.EditorInstance.SceneWidth, e.EditorInstance.SceneHeight, e.EditorInstance.waterColor);
                            d.DrawLine(0, heightX, e.EditorInstance.SceneWidth, heightX, SystemColors.White);
                        }
                    }
                    else
                    {
                        if (!Properties.Settings.Default.SimplifiedWaterLevelRendering)
                        {
                            if (e.EditorInstance.alwaysShowWaterLevel)
                            {
                                int startX = (e.EditorInstance.sizeWaterLevelwithBounds ? x1 : 0);
                                int endX = (e.EditorInstance.sizeWaterLevelwithBounds ? x2 : e.EditorInstance.SceneWidth);

                                d.DrawRectangle(startX, heightX, endX, e.EditorInstance.SceneHeight, e.EditorInstance.waterColor);
                                d.DrawLine(startX, heightX, endX, heightX, SystemColors.White);
                                if (editorAnim2 != null && editorAnim2.Frames.Count != 0)
                                {
                                    var frame = editorAnim2.Frames[Animation.index];

                                    Animation.ProcessAnimation2(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);

                                    for (int i = startX; i < endX; i = i + frame.Frame.Width)
                                        d.DrawBitmap(frame.Texture,
                                            i + frame.Frame.CenterX,
                                            heightX + frame.Frame.CenterY,
                                            frame.Frame.Width, frame.Frame.Height, false, Transparency);
                                }
                            }

                        }
                        else
                        {
                            if (e.EditorInstance.alwaysShowWaterLevel)
                            {
                                int startX = (e.EditorInstance.sizeWaterLevelwithBounds ? x1 : 0);
                                int endX = (e.EditorInstance.sizeWaterLevelwithBounds ? x2 : e.EditorInstance.SceneWidth);
                                d.DrawRectangle(startX, heightX, endX, e.EditorInstance.SceneHeight, e.EditorInstance.waterColor);
                                d.DrawLine(startX, heightX, endX, heightX, SystemColors.White);
                            }
                        }
                    }
                }


                d.DrawLine(x1, y1, x1, y2, SystemColors.Aqua);
                d.DrawLine(x1, y1, x2, y1, SystemColors.Aqua);
                d.DrawLine(x2, y2, x1, y2, SystemColors.Aqua);
                d.DrawLine(x2, y2, x2, y1, SystemColors.Aqua);


                // draw corners
                for (int i = 0; i < 4; i++)
                {
                    bool right = (i & 1) > 0;
                    bool bottom = (i & 2) > 0;

                    editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("EditorAssets", d, 0, 1, right, bottom, false);
                    if (editorAnim != null && editorAnim.Frames.Count != 0)
                    {
                        var frame = editorAnim.Frames[Animation.index];
                        Animation.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);
                        d.DrawBitmap(frame.Texture,
                            (x + widthPixels / (right ? 2 : -2)) - (right ? frame.Frame.Width : 0),
                            (y + heightPixels / (bottom ? 2 : -2) - (bottom ? frame.Frame.Height : 0)),
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);

                    }
                }
            }
        }

        public override bool isObjectOnScreen(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            var widthPixels = (int)(entity.attributesMap["size"].ValuePosition.X.High);
            var heightPixels = (int)(entity.attributesMap["size"].ValuePosition.Y.High);
            return d.IsObjectOnScreen(x - widthPixels / 2, y - heightPixels / 2, widthPixels, heightPixels);
        }

        public override string GetObjectName()
        {
            return "Water";
        }
    }
}
