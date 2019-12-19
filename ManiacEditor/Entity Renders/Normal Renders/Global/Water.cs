﻿using RSDKv5;
using SystemColors = System.Drawing.Color;

namespace ManiacEditor.Entity_Renders
{
    public class Water : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int type = (int)entity.attributesMap["type"].ValueEnum;
            var widthPixels = (int)(entity.attributesMap["size"].ValueVector2.X.High);
            var heightPixels = (int)(entity.attributesMap["size"].ValueVector2.Y.High);
            var heightY = (int)(entity.attributesMap["height"].ValueVector2.Y.High);
            var heightX = (int)(entity.attributesMap["height"].ValueVector2.X.High);
			int r = (int)(entity.attributesMap["r"].ValueUInt8);
			int g = (int)(entity.attributesMap["g"].ValueUInt8);
			int b = (int)(entity.attributesMap["b"].ValueUInt8);
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

            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("Water", d.DevicePanel, animID, -1, fliph, flipv, false);
            var editorAnim2 = Editor.Instance.EntityDrawing.LoadAnimation2("Water", d.DevicePanel, 0, -1, fliph, flipv, false);

            // Base Water + Bubble Source
            if (editorAnim != null && editorAnim.Frames.Count != 0 && animID >= 0 && (type == 2 || type == 0))
            {
                var frame = editorAnim.Frames[Animation.index];

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
            x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
            y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
            frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }


            // HCZ Big Bubbles
            else if (HCZBubbles == true)
            {
                editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("BigBubble", d.DevicePanel, 7, -1, fliph, flipv, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    var frame = editorAnim.Frames[Animation.index];

                    Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                        x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                        y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
            }

            // Bounded Water
            if (width != 0 && height != 0 && showBounds == true && HCZBubbles == false)
            {
                //Draw Icon
                editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("EditorIcons2", d.DevicePanel, 0, 8, fliph, flipv, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    var frame = editorAnim.Frames[Animation.index];

                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                        x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                        y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
                int x1 = x + widthPixels / -2;
                int x2 = x + widthPixels / 2 - 1;
                int y1 = y + heightPixels / -2;
                int y2 = y + heightPixels / 2 - 1;

				if (type != 1)
				{
					if (Editor.Instance.UIModes.ShowWaterLevel)
					{
						if (selected)
						{
							if (!Settings.MyPerformance.UseSimplifedWaterRendering)
							{
								d.DrawRectangle(0, heightX, Editor.Instance.SceneWidth, heightX, Editor.Instance.UIModes.waterColor);
								d.DrawLine(0, heightX, Editor.Instance.SceneWidth, heightX, SystemColors.White);
								if (editorAnim2 != null && editorAnim2.Frames.Count != 0)
								{
									var frame = editorAnim2.Frames[Animation.index];

									Animation.ProcessAnimation2(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

									for (int i = 0; i < Editor.Instance.SceneWidth; i = i + frame.Frame.Width)
										d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
											i + frame.Frame.PivotX,
											heightX + frame.Frame.PivotY,
											frame.Frame.Width, frame.Frame.Height, false, Transparency);
								}
							}
							else
							{
								d.DrawRectangle(0, heightX, Editor.Instance.SceneWidth, heightX, Editor.Instance.UIModes.waterColor);
								d.DrawLine(0, heightX, Editor.Instance.SceneWidth, heightX, SystemColors.White);
							}
						}
						else
						{
							if (!Settings.MyPerformance.UseSimplifedWaterRendering)
							{
								if (Editor.Instance.UIModes.AlwaysShowWaterLevel)
								{
									int startX = (Editor.Instance.UIModes.SizeWaterLevelwithBounds ? x1 : 0);
									int endX = (Editor.Instance.UIModes.SizeWaterLevelwithBounds ? x2 : Editor.Instance.SceneWidth);

									d.DrawRectangle(startX, heightX, endX, Editor.Instance.SceneHeight, Editor.Instance.UIModes.waterColor);
									d.DrawLine(startX, heightX, endX, heightX, SystemColors.White);
									if (editorAnim2 != null && editorAnim2.Frames.Count != 0)
									{
										var frame = editorAnim2.Frames[Animation.index];

										Animation.ProcessAnimation2(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

										for (int i = startX; i < endX; i = i + frame.Frame.Width)
											d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
												i + frame.Frame.PivotX,
												heightX + frame.Frame.PivotY,
												frame.Frame.Width, frame.Frame.Height, false, Transparency);
									}
								}

							}
							else
							{
								if (Editor.Instance.UIModes.AlwaysShowWaterLevel)
								{
									int startX = (Editor.Instance.UIModes.SizeWaterLevelwithBounds ? x1 : 0);
									int endX = (Editor.Instance.UIModes.SizeWaterLevelwithBounds ? x2 : Editor.Instance.SceneWidth);
									d.DrawRectangle(startX, heightX, endX, Editor.Instance.SceneHeight, Editor.Instance.UIModes.waterColor);
									d.DrawLine(startX, heightX, endX, heightX, SystemColors.White);
								}
							}
						}
					}
				}
				else
				{
					int red = r - 150;
					int blue = b - 150;
					int green = g - 150;
					if (red > 255) red = 255;
					if (blue > 255) blue = 255;
					if (green > 255) green = 255;
					if (red < 0) red = 0;
					if (blue < 0) blue = 0;
					if (green < 0) green = 0;
					d.DrawRectangle(x1, y1, x2, y2, SystemColors.FromArgb(128, red, green, blue));
				}

                d.DrawLine(x1, y1, x1, y2, SystemColors.Aqua);
                d.DrawLine(x1, y1, x2, y1, SystemColors.Aqua);
                d.DrawLine(x2, y2, x1, y2, SystemColors.Aqua);
                d.DrawLine(x2, y2, x2, y1, SystemColors.Aqua);


                // draw corners

                editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 0, 1, false, false, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    var frame = editorAnim.Frames[Animation.index];
                    Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                        (x + widthPixels / (false ? 2 : -2)) - (false ? frame.Frame.Width : 0),
                        (y + heightPixels / (false ? 2 : -2) - (false ? frame.Frame.Height : 0)),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);

                }

                editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 0, 1, false, true, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    var frame = editorAnim.Frames[Animation.index];
                    Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                        (x + widthPixels / (false ? 2 : -2)) - (false ? frame.Frame.Width : 0),
                        (y + heightPixels / (true ? 2 : -2) - (true ? frame.Frame.Height : 0)),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);

                }

                editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 0, 1, true, false, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    var frame = editorAnim.Frames[Animation.index];
                    Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                        (x + widthPixels / (true ? 2 : -2)) - (true ? frame.Frame.Width : 0),
                        (y + heightPixels / (false ? 2 : -2) - (false ? frame.Frame.Height : 0)),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);

                }

                editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d.DevicePanel, 0, 1, true, true, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    var frame = editorAnim.Frames[Animation.index];
                    Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                        (x + widthPixels / (true ? 2 : -2)) - (true ? frame.Frame.Width : 0),
                        (y + heightPixels / (true ? 2 : -2) - (true ? frame.Frame.Height : 0)),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);

                }
            }
        }

        public override bool isObjectOnScreen(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            var widthPixels = (int)(entity.attributesMap["size"].ValueVector2.X.High);
            var heightPixels = (int)(entity.attributesMap["size"].ValueVector2.Y.High);
            return d.IsObjectOnScreen(x - widthPixels / 2, y - heightPixels / 2, widthPixels, heightPixels);
        }

        public override string GetObjectName()
        {
            return "Water";
        }
    }
}