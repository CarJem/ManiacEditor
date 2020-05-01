using RSDKv5;
using SystemColors = System.Drawing.Color;
using System;

namespace ManiacEditor.Entity_Renders
{
    public class LRZSpiral : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;

            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            int type = (int)e.attributesMap["type"].ValueUInt8;
            int multiplierX = 0;
            int multiplierY = 0;
            switch (type)
            {
                case 0:
                    multiplierX = 2;
                    multiplierY = 128;
                    break;
                case 1:
                    multiplierX = 2;
                    multiplierY = 2;
                    break;
                case 2:
                    multiplierX = 3;
                    multiplierY = 3;
                    break;
            }

            int radius = (int)e.attributesMap["radius"].ValueEnum;
            int height = (int)(e.attributesMap["height"].ValueEnum);

            if (type == 0)
            {
                var widthPixels = radius * 2;
                var heightPixels = height * 128;

                var sineHeightA = (height * 128) - (height % 2 == 0 ? 60 : 0);
                var sineHeightB = (height * 128) - (height % 2 == 0 ? 60 : 0) + 27;

                var sine_amplitude = (radius);
                int sine_period = 128;

                int sine_start_y1 = y - ((height * 128) / 2) + 108;
                int sine_start_y2 = y - ((height * 128) / 2) + 81;


                int sine_center_x = x;
                int sine_center_y = sine_start_y2 + (sineHeightB / 2);

                int sine_start_x = x;

                DrawBounds(d, sine_center_x, sine_center_y, widthPixels, sineHeightB, Transparency, SystemColors.White, SystemColors.Transparent);

                DrawSine(d, sine_start_x, sine_start_y2, sine_amplitude, sine_period, sineHeightA, SystemColors.Red);
                DrawSine(d, sine_start_x, sine_start_y1, sine_amplitude, sine_period, sineHeightA, SystemColors.YellowGreen);
            }
            else
            {
                var widthPixels = radius * multiplierX;
                var heightPixels = height * multiplierY;

                DrawBounds(d, x, y, widthPixels, heightPixels, Transparency, SystemColors.White, SystemColors.Transparent);
            }
        }

        public void DrawSine(DevicePanel d, int x, int y, int amplitude, int period, int height, SystemColors colors)
        {
            int x1 = x, y2 = y;
            int max = height + y;

            while (y2 < max)
            {
                var y1 = y2++;
                var radians = (y2 - y) * 2 * Math.PI / period;
                var x2 = x + (int)Math.Round(amplitude * Math.Sin(radians));
                d.DrawLine(x1, y1, x2, y2, colors, 2);
                x1 = x2;
            }
        }

        public override bool isObjectOnScreen(DevicePanel d, Classes.Scene.EditorEntity entity, int x, int y, int Transparency)
        {
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int multiplierX = 0;
            int multiplierY = 0;
            switch (type)
            {
                case 0:
                    multiplierX = 2;
                    multiplierY = 128;
                    break;
                case 1:
                    multiplierX = 2;
                    multiplierY = 2;
                    break;
                case 2:
                    multiplierX = 3;
                    multiplierY = 3;
                    break;
            }
            var widthPixels = (int)(entity.attributesMap["radius"].ValueEnum) * multiplierX;
            var heightPixels = (int)(entity.attributesMap["height"].ValueEnum) * multiplierY;
            if (widthPixels != 0 && heightPixels != 0)
            {
                return d.IsObjectOnScreen(x - widthPixels / 2, y - heightPixels / 2, widthPixels, heightPixels);
            }
            else
            {
                return d.IsObjectOnScreen(x - 16, y - 16, 32, 32);
            }

        }

        public override string GetObjectName()
        {
            return "LRZSpiral";
        }
    }
}
