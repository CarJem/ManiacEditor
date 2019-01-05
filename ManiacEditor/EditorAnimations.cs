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
         public int platformAngle = 0;

        //Type 4 Platforms
        bool reverseAngleRot = false;
        public int platformAngle4 = 0;
        public bool negX = false;

        public DateTime lastFrametime;
        public int index = 0;
        public DateTime lastFrametime2;
        public DateTime lastFrametime3;
        public int index2 = 0;
        public int index3 = 0;
        public int PaperRollerIndex = 0;

        //Parallax Sprite Location Storing
        public string parallaxSprite = "";

        /// <summary>
        /// Handles animation timing
        /// </summary>
        /// <param name="speed">Speed</param>
        /// <param name="frameCount">The total amount of frames</param>
        public void ProcessAnimation(int speed, int frameCount, int duration, int startFrame = 0)
        {
            // Playback
            if (Editor.Instance.ShowAnimations.Checked && Editor.Instance.AnnimationsChecked)
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

        public void ProcessMovingPlatform(int angleDefault, UInt32 speed = 3)
        {
            if (speed >= 4294967290)
            {
                speed = 10;
            }

            int duration = 1;
            // Playback
            if (Editor.Instance.ShowAnimations.Checked && Editor.Instance.MovingPlatformsChecked)
            {
                if (speed > 0)
                {
                    int speed1 = (int)speed * 64 / (duration == 0 ? 256 : duration);
                    if (speed1 == 0)
                        speed1 = 1;
                    if ((DateTime.Now - lastFrametime).TotalMilliseconds > 1024 / speed1)
                    {
                        platformAngle++;
                        lastFrametime = DateTime.Now;
                    }
                }
            }
            else platformAngle = angleDefault;
            if (platformAngle >= 768)
                platformAngle = 0;

        }

        public int[] ProcessMovingPlatform2(int ampX, int ampY, int x, int y, int width, int height, UInt32 speed = 1)
        {
            if (speed >= 4294967290)
            {
                speed = 10;
            }
            int slope = 0;
            int c = 0;
            if (ampX != 0 && ampY != 0)
            {
                slope = (ampX + ampX) / (ampY + ampY);
                c = ampX - (slope * ampY);
            }
            int duration = 1;
            int initalX = ampX;
            int initalY = ampY;

            // Playback || I disabled anything with both x and y values because they have way too many issues atm
            if (Editor.Instance.ShowAnimations.Checked && Editor.Instance.MovingPlatformsChecked)
            {
                if (speed > 0)
                {
                    int speed1 = (int)speed * 64 / (duration == 0 ? 256 : duration);
                    if (speed1 == 0)
                        speed1 = 1;
                    if ((DateTime.Now - lastFrametime3).TotalMilliseconds > 1024 / speed1)
                    {

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
                            positionY = slope * positionX + c;
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
                if (ampX != 0 && ampY != 0)
                {
                    positionX = ampX;
                    positionY = ampY;
                }
                else
                {
                    positionX = 0;
                    positionY = 0;
                }

            }
            int[] position = new int[2];
            position[0] = positionX;
            position[1] = positionY;
            return position;

        }

        public int[] ProcessMovingPlatform2D(int ampX, int ampY, int x, int y, int width, int height, UInt32 speed = 1)
        {
            positionX = 0;
            positionY = 0;

            int[] position = new int[2];
            position[0] = positionX;
            position[1] = positionY;
            return position;

            /*
            if (speed >= 4294967290)
            {
                speed = 10;
            }
            int slope = 0;
            int c = 0;
            if (ampX != 0 && ampY != 0)
            {
                slope =  (ampX + ampX) / (ampY + ampY);
                c = ampX - (slope * ampY);
            }
            int duration = 1;
            int initalX = ampX;
            int initalY = ampY;

            // Playback || I disabled anything with both x and y values because they have way too many issues atm
            if (Editor.Instance.ShowAnimations.Checked && Properties.EditorState.Default.movingPlatformsChecked)
            {
                if (speed > 0)
                {
                    int speed1 = (int)speed * 64 / (duration == 0 ? 256 : duration);
                    if (speed1 == 0)
                        speed1 = 1;
                    if ((DateTime.Now - lastFrametime3).TotalMilliseconds > 1024 / speed1)
                    {
                        if (reverseX)
                        {
                            if (positionX <= 0)
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
                            positionY = slope * positionX + c;
                        }
                        lastFrametime3 = DateTime.Now;
                    }
                }
            }
            else
            {
                positionX = ampX;
                positionY = ampY;
            }

            int[] position = new int[2];
            position[0] = positionX;
            position[1] = positionY;
            return position;
            */
        }

        public void ProcessMovingPlatform4(int ampX, int angleDefault, UInt32 speed = 3)
        {
            if (ampX <= -1)
            {
                negX = true;
                ampX = -ampX;
            }
            if (speed >= 4294967290)
            {
                speed = 10;
            }

            int duration = 1;
            // Playback
            if (Editor.Instance.ShowAnimations.Checked && Editor.Instance.MovingPlatformsChecked)
            {
                if (speed > 0)
                {
                    int speed1 = (int)speed * 64 / (duration == 0 ? 256 : duration);
                    if (speed1 == 0)
                        speed1 = 1;
                    if ((DateTime.Now - lastFrametime).TotalMilliseconds > 1024 / speed1)
                    {
                        if (!negX)
                        {
                            if (reverseAngleRot)
                            {
                                platformAngle4--;
                                lastFrametime = DateTime.Now;
                            }
                            else
                            {
                                platformAngle4++;
                                lastFrametime = DateTime.Now;
                            }
                        }
                        else
                        {
                            if (reverseAngleRot)
                            {
                                platformAngle4++;
                                lastFrametime = DateTime.Now;
                            }
                            else
                            {
                                platformAngle4--;
                                lastFrametime = DateTime.Now;
                            }
                        }

                    }
                }
            }
            else platformAngle4 = angleDefault;
            if (!negX)
            {
                if (platformAngle4 >= ampX)
                {
                    reverseAngleRot = true;
                }
                else if (platformAngle4 <= -ampX)
                {
                    reverseAngleRot = false;
                }
            }
            else
            {
                if (platformAngle4 >= ampX)
                {
                    reverseAngleRot = false;
                }
                else if (platformAngle4 <= -ampX)
                {
                    reverseAngleRot = true;
                }
            }


        }

        public void ProcessAnimation2(int speed, int frameCount, int duration, int startFrame = 0)
        {
            // Playback
            if (Editor.Instance.ShowAnimations.Checked && Editor.Instance.AnnimationsChecked)
            {
                if (speed > 0)
                {
                    int speed1 = speed * 64 / (duration == 0 ? 256 : duration);
                    if (speed1 == 0)
                        speed1 = 1;
                    if ((DateTime.Now - lastFrametime).TotalMilliseconds > 1024 / speed1)
                    {
                        index2++;
                        lastFrametime = DateTime.Now;
                    }
                }
            }
            else index2 = 0 + startFrame;
            if (index2 >= frameCount)
                index2 = 0;

        }
        public void ProcessAnimation3(int speed, int frameCount, int duration, int startFrame = 0)
        {
            // Playback
            if (Editor.Instance.ShowAnimations.Checked && Editor.Instance.AnnimationsChecked)
            {
                if (speed > 0)
                {
                    int speed1 = speed * 64 / (duration == 0 ? 256 : duration);
                    if (speed1 == 0)
                        speed1 = 1;
                    if ((DateTime.Now - lastFrametime2).TotalMilliseconds > 1024 / speed1)
                    {
                        index3++;
                        lastFrametime2 = DateTime.Now;
                    }
                }
            }
            else index3 = 0 + startFrame;
            if (index3 >= frameCount)
                index3 = 0;

        }

        public void ProcessPaperRollerRotatingColors()
        {
            if (Editor.Instance.ShowAnimations.Checked && Editor.Instance.AnnimationsChecked)
            {
                if ((DateTime.Now - lastFrametime2).TotalMilliseconds > 1024)
                {
                    if (PaperRollerIndex >= 3)
                    {
                        PaperRollerIndex = 0;
                    }
                    else
                    {
                        PaperRollerIndex++;
                    }
                }

            }
        }
    }
}
