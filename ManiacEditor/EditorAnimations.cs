using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor
{
    public partial class EditorAnimations
    {

        //Rotating/Moving Platforms
        static int positionX = 0;
        static int positionY = 0;
        static bool reverseX = false;
        static bool reverseY = false;
        public static EditorAnimations Instance;

        public DateTime lastFrametime;
        public int index = 0;
        public DateTime lastFrametime2;
        public DateTime lastFrametime3;
        public int index2 = 0;

        public EditorAnimations()
        {
            Instance = this;
        }

        public int[] ProcessMovingPlatform2(int ampX, int ampY, int speed = 1)
        {

            int duration = 1;
            // Playback
            if (Editor.Instance.ShowAnimations.Checked && Properties.EditorState.Default.movingPlatformsChecked)
            {
                if (speed > 0)
                {
                    int speed1 = speed * 64 / (duration == 0 ? 256 : duration);
                    if (speed1 == 0)
                        speed1 = 1;
                    if ((DateTime.Now - lastFrametime3).TotalMilliseconds > 1024 / speed1)
                    {
                        if (ampX <= -1 && ampX != 0)
                        {
                            reverseX = true;
                        }
                        if (ampY <= -1 && ampY != 0)
                        {
                            reverseY = true;
                        }

                        if (reverseX)
                        {
                            if (positionX <= -ampX)
                            {
                                reverseX = false;
                            }
                            else
                            {
                                positionX--;
                            }
                        }
                        else
                        {
                            if (positionX >= ampX)
                            {
                                reverseX = true;
                            }
                            else
                            {
                                positionX++;
                            }
                        }


                        if (reverseY)
                        {
                            if (positionY <= -ampY)
                            {
                                reverseY = false;
                            }
                            else
                            {
                                positionY--;
                            }
                        }
                        else
                        {
                            if (positionY >= ampY)
                            {
                                reverseY = true;
                            }
                            else
                            {
                                positionY++;
                            }
                        }

                        lastFrametime3 = DateTime.Now;
                    }
                }
            }
            else
            {
                positionX = 0;
                positionY = 0;
            }
            int[] position = new int[2];
            position[0] = positionX;
            position[1] = positionY;
            return position;

        }

        public void ProcessAnimation2(int speed, int frameCount, int duration, int startFrame = 0)
        {
            // Playback
            if (Editor.Instance.ShowAnimations.Checked && Properties.EditorState.Default.annimationsChecked)
            {
                if (speed > 0)
                {
                    int speed1 = speed * 64 / (duration == 0 ? 256 : duration);
                    if (speed1 == 0)
                        speed1 = 1;
                    if ((DateTime.Now - lastFrametime).TotalMilliseconds > 1024 / speed1)
                    {
                        index++;
                        lastFrametime = DateTime.Now;
                    }
                }
            }
            else index = 0 + startFrame;
            if (index >= frameCount)
                index = 0;

        }
        public void ProcessAnimation3(int speed, int frameCount, int duration, int startFrame = 0)
        {
            // Playback
            if (Editor.Instance.ShowAnimations.Checked && Properties.EditorState.Default.annimationsChecked)
            {
                if (speed > 0)
                {
                    int speed1 = speed * 64 / (duration == 0 ? 256 : duration);
                    if (speed1 == 0)
                        speed1 = 1;
                    if ((DateTime.Now - lastFrametime2).TotalMilliseconds > 1024 / speed1)
                    {
                        index2++;
                        lastFrametime2 = DateTime.Now;
                    }
                }
            }
            else index2 = 0 + startFrame;
            if (index2 >= frameCount)
                index2 = 0;

        }
    }
}
