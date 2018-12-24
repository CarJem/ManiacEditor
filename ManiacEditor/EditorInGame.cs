using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor
{
    public class EditorInGame
    {
        protected const int NAME_BOX_WIDTH = 20;
        protected const int NAME_BOX_HEIGHT = 20;

        public int StarPostEnable = 0x00EBB6C4 + 34;
        public int StarPostX = 0x00EBB6C4 + 12;
        public int StarPostY = 0x00EBB6C4 + 16;


        public int GetPlayerAt(Point point)
        {
            for (int i = 1; i < 5; i++)
                if (PlayerContainsPoint(point, i))
                    return i;
            return -1;
        }

        public bool PlayerContainsPoint(Point point, int player)
        {
            return GetDimensions(player).Contains(point);
        }

        public Rectangle GetDimensions(int player)
        {
            return new Rectangle(GetX(player), GetY(player), 20, 20);
        }

        public int GetX(int player)
        {
            int X = 0;
            switch (player)
            {
                case 1:
                    X = Editor.Instance.P1_X;
                    break;
                case 2:
                    X = Editor.Instance.P2_X;
                    break;
                case 3:
                    X = Editor.Instance.P3_X;
                    break;
                case 4:
                    X = Editor.Instance.P4_X;
                    break;
            }
            return X;
        }

        public int GetY(int player)
        {
            int Y = 0;
            switch (player)
            {
                case 1:
                    Y = Editor.Instance.P1_Y;
                    break;
                case 2:
                    Y = Editor.Instance.P2_Y;
                    break;
                case 3:
                    Y = Editor.Instance.P3_Y;
                    break;
                case 4:
                    Y = Editor.Instance.P4_Y;
                    break;
            }
            return Y;
        }

        public void MovePlayer(Point p, double Zoom, int player)
        {
            if (Editor.GameRunning)
            {
                // TODO: Find out if this is constent
                int ObjectAddress = GetPlayerBase(player);
                Editor.GameMemory.WriteInt16(ObjectAddress + 2, (short)(p.X / Zoom));
                Editor.GameMemory.WriteInt16(ObjectAddress + 6, (short)(p.Y / Zoom));
            }
        }

        public int GetPlayerBase(int player)
        {
            return 0x85E9A0 + (0x458 * player--);
        }

        public void DrawSinglePlayer(DevicePanel d, int playerID, int playerBase)
        {

            int ObjectAddress = playerBase;
            IntPtr processHandle = Editor.GameMemory.ProcessHandle;
            byte[] xb = ReadMemory(ObjectAddress + 2, 2, (int)processHandle);
            byte[] yb = ReadMemory(ObjectAddress + 6, 2, (int)processHandle);
            byte[] PID = ReadMemory(ObjectAddress + 0xC0, 2, (int)processHandle);
            int x = BitConverter.ToUInt16(xb, 0);
            int y = BitConverter.ToUInt16(yb, 0);
            int ID = BitConverter.ToUInt16(PID, 0);
            switch (playerID)
            {
                case 1:
                    Editor.Instance.P1_X = x;
                    Editor.Instance.P1_Y = y;
                    break;
                case 2:
                    Editor.Instance.P2_X = x;
                    Editor.Instance.P2_Y = y;
                    break;
                case 3:
                    Editor.Instance.P3_X = x;
                    Editor.Instance.P3_Y = y;
                    break;
                case 4:
                    Editor.Instance.P4_X = x;
                    Editor.Instance.P4_Y = y;
                    break;

            }

            int Transparency = 0xff;
            string name = "Player " + playerID;

            System.Drawing.Color color2 = System.Drawing.Color.DarkBlue;
            int frameID = 0;
            bool showFrame = false;

            switch (ID)
            {
                case 1:
                    color2 = System.Drawing.Color.SkyBlue;
                    frameID = 0;
                    break;
                case 2:
                    color2 = System.Drawing.Color.Orange;
                    frameID = 1;
                    break;
                case 4:
                    color2 = System.Drawing.Color.Red;
                    frameID = 2;
                    break;
                case 8:
                    color2 = System.Drawing.Color.Yellow;
                    frameID = 3;
                    break;
                case 16:
                    color2 = System.Drawing.Color.DarkRed;
                    frameID = 4;
                    break;
                default:
                    color2 = System.Drawing.Color.White;
                    frameID = 0;
                    showFrame = true;
                    break;

            }




            if (x != 0 || y != 0)
            {
                if (showFrame)
                {
                    d.DrawRectangle(x, y, x + NAME_BOX_WIDTH, y + NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(0x00, System.Drawing.Color.MediumPurple));
                    d.DrawLine(x, y, x + NAME_BOX_WIDTH, y, System.Drawing.Color.FromArgb(Transparency, color2));
                    d.DrawLine(x, y, x, y + NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color2));
                    d.DrawLine(x, y + NAME_BOX_HEIGHT, x + NAME_BOX_WIDTH, y + NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color2));
                    d.DrawLine(x + NAME_BOX_WIDTH, y, x + NAME_BOX_WIDTH, y + NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color2));
                    if (Editor.Instance.GetZoom() >= 1) d.DrawTextSmall(name, x + 2, y + 2, NAME_BOX_WIDTH - 4, System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black), true);
                }
                else
                {
                    var editorAnim = EditorEntity_ini.LoadAnimation2("HUD", d, 2, frameID, false, false, false);
                    if (editorAnim != null && editorAnim.Frames.Count != 0 && ID != 0)
                    {
                        var frame = editorAnim.Frames[0];
                        d.DrawBitmap(frame.Texture, x, y, frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }


            }
        }

        public void UpdateCheckpoint(Point p, bool enabled = true)
        {       

            //byte status = 0x00;
            //if (enabled == true) status = 0x01;
            Editor.GameMemory.WriteByte(StarPostEnable, 0x01);
            Editor.GameMemory.WriteInt16(StarPostX, (short)p.X);
            Editor.GameMemory.WriteInt16(StarPostY, (short)p.Y);
            
        }

        public void DrawCheckpoint(DevicePanel d)
        {
            // TODO: Find out if this is constent
            IntPtr processHandle = Editor.GameMemory.ProcessHandle;
            byte[] xb = ReadMemory(StarPostX, 2, (int)processHandle);
            byte[] yb = ReadMemory(StarPostY, 2, (int)processHandle);
            byte[] isEnabled = ReadMemory(StarPostEnable, 1, (int)processHandle);
            int x = BitConverter.ToUInt16(xb, 0);
            int y = BitConverter.ToUInt16(yb, 0);
            

            int Transparency = 0xff;

            System.Drawing.Color color2 = System.Drawing.Color.DarkBlue;
            bool showFrame = false;
            string name = "Checkpoint";




            if (isEnabled[0] == 1)
            {
                if (showFrame)
                {
                    d.DrawRectangle(x, y, x + NAME_BOX_WIDTH, y + NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(0x00, System.Drawing.Color.MediumPurple));
                    d.DrawLine(x, y, x + NAME_BOX_WIDTH, y, System.Drawing.Color.FromArgb(Transparency, color2));
                    d.DrawLine(x, y, x, y + NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color2));
                    d.DrawLine(x, y + NAME_BOX_HEIGHT, x + NAME_BOX_WIDTH, y + NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color2));
                    d.DrawLine(x + NAME_BOX_WIDTH, y, x + NAME_BOX_WIDTH, y + NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color2));
                    if (Editor.Instance.GetZoom() >= 1) d.DrawTextSmall(name, x + 2, y + 2, NAME_BOX_WIDTH - 4, System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black), true);
                }
                else
                {
                    var editorAnim = EditorEntity_ini.LoadAnimation2("StarPost", d, 1, 0, false, false, false);
                    if (editorAnim != null && editorAnim.Frames.Count != 0)
                    {
                        var frame = editorAnim.Frames[0];
                        d.DrawBitmap(frame.Texture, x, y, frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }


            }
        }

        public byte[] ReadMemory(int adress, int processSize, int processHandle)
        {
            byte[] buffer = new byte[processSize];
            Editor.ReadProcessMemory(processHandle, adress, buffer, processSize, 0);
            return buffer;
        }

        public void DrawGameElements(DevicePanel d)
        {
            int P1 = 0x85E9A0;
            int P2 = 0x85E9A0 + (0x458 * 1);
            int P3 = 0x85E9A0 + (0x458 * 2);
            int P4 = 0x85E9A0 + (0x458 * 3);

            DrawSinglePlayer(d, 1, P1);
            DrawSinglePlayer(d, 2, P2);
            DrawSinglePlayer(d, 3, P3);
            DrawSinglePlayer(d, 4, P4);

            //DrawCheckpoint(d);


        }
    }
}
