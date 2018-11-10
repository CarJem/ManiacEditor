using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor
{
    [Serializable]
    public class EditorAnimations
    {

        //Rotating/Moving Platforms
         int positionX = 0;
         int positionY = 0;
         bool reverseX = false;
         bool reverseY = false;
         EditorAnimations Instance;

        public DateTime lastFrametime;
        public int index = 0;
        public DateTime lastFrametime2;
        public DateTime lastFrametime3;
        public int index2 = 0;

        public EditorAnimations()
        {
            Instance = this;
        }


        public int[] ProcessMovingPlatform2(int ampX, int ampY, int x, int y, int height, int width, UInt32 speed = 1)
        {
            if (speed >= 4294967290)
            {
                speed = 10;
            }
            int slope = 0;
            int c = 0;
            if (ampX != 0 && ampY != 0)
            {
                slope = (-ampY / ampY) / (-ampX / ampX);
                c = ampY - ampX * slope;
            }
            int duration = 1;
            int initalX = ampX;
            int initalY = ampY;

            // Playback
            if (Editor.Instance.ShowAnimations.Checked && Properties.EditorState.Default.movingPlatformsChecked)
            {
                if (speed > 0)
                {
                    int speed1 = (int)speed * 64 / (duration == 0 ? 256 : duration);
                    if (speed1 == 0)
                        speed1 = 1;
                    if ((DateTime.Now - lastFrametime3).TotalMilliseconds > 1024 / speed1)
                    {
                        /*//Moving Platforms
                        if (ampX <= -1 && ampX != 0)
                        {
                            reverseX = true;
                        }
                        if (ampY <= -1 && ampY != 0)
                        {
                            reverseY = true;
                        }*/

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
                        if (ampX != 0 && ampY != 0)
                        {
                            positionY = slope * positionX + (int)Properties.Settings.Default.devInt;
                        }




                        if (!(ampX != 0 && ampY != 0))
                        {
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
