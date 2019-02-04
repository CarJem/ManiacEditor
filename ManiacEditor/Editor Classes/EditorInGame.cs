using ManiacEditor.Entity_Renders;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManiacEditor
{
    public class EditorInGame
    {
        protected const int NAME_BOX_WIDTH = 20;
        protected const int NAME_BOX_HEIGHT = 20;

        public static int CheckpointBase = 0x00EBB6C4;
        public static int PlayerBase = 0x85E9A0;
        public static int Player1Base = PlayerBase + (0x458 * 0);
        public static int Player2Base = PlayerBase + (0x458 * 1);
        public static int Player3Base = PlayerBase + (0x458 * 2);
        public static int Player4Base = PlayerBase + (0x458 * 3);

        public Editor EditorInstance;

        public short Player1_State { get { return EditorInstance.GameMemory.ReadShort(Player1Base + 0xC0); } set { EditorInstance.GameMemory.WriteShort(Player1Base + 0xC0, value); } }
        public short Player1_X { get { return EditorInstance.GameMemory.ReadShort(Player1Base + 0x02); } set { EditorInstance.GameMemory.WriteShort(Player1Base + 0x02, value); } }
        public short Player1_Y { get { return EditorInstance.GameMemory.ReadShort(Player1Base + 0x06); } set { EditorInstance.GameMemory.WriteShort(Player1Base + 0x06, value); } }

        public short Player4_State { get { return EditorInstance.GameMemory.ReadShort(Player4Base + 0xC0); } set { EditorInstance.GameMemory.WriteShort(Player4Base + 0xC0, value); } }
        public short Player4_X { get { return EditorInstance.GameMemory.ReadShort(Player4Base + 0x02); } set { EditorInstance.GameMemory.WriteShort(Player4Base + 0x02, value); } }
        public short Player4_Y { get { return EditorInstance.GameMemory.ReadShort(Player4Base + 0x06); } set { EditorInstance.GameMemory.WriteShort(Player4Base + 0x06, value); } }

        public short Player3_State { get { return EditorInstance.GameMemory.ReadShort(Player3Base + 0xC0); } set { EditorInstance.GameMemory.WriteShort(Player3Base + 0xC0, value); } }
        public short Player3_X { get { return EditorInstance.GameMemory.ReadShort(Player3Base + 0x02); } set { EditorInstance.GameMemory.WriteShort(Player3Base + 0x02, value); } }
        public short Player3_Y { get { return EditorInstance.GameMemory.ReadShort(Player3Base + 0x06); } set { EditorInstance.GameMemory.WriteShort(Player3Base + 0x06, value); } }

        public short Player2_State { get { return EditorInstance.GameMemory.ReadShort(Player2Base + 0xC0); } set { EditorInstance.GameMemory.WriteShort(Player2Base + 0xC0, value); } }
        public short Player2_X { get { return EditorInstance.GameMemory.ReadShort(Player2Base + 0x02); } set { EditorInstance.GameMemory.WriteShort(Player2Base + 0x02, value); } }
        public short Player2_Y { get { return EditorInstance.GameMemory.ReadShort(Player2Base + 0x06); } set { EditorInstance.GameMemory.WriteShort(Player2Base + 0x06, value); } }

        public byte StarPostEnable { get { return EditorInstance.GameMemory.ReadByte(EditorInstance.GameMemory.ReadInt32(CheckpointBase) + 0x34); } set { EditorInstance.GameMemory.WriteByte(EditorInstance.GameMemory.ReadByte(CheckpointBase) + 0x34, value); } }
        public int StarPostX { get { return EditorInstance.GameMemory.ReadInt32(EditorInstance.GameMemory.ReadInt32(CheckpointBase) + 0x12); } set { EditorInstance.GameMemory.WriteInt32(EditorInstance.GameMemory.ReadInt32(CheckpointBase) + 0x12, value); } }
        public int StarPostY { get { return EditorInstance.GameMemory.ReadInt32(EditorInstance.GameMemory.ReadInt32(CheckpointBase) + 0x16); } set { EditorInstance.GameMemory.WriteInt32(EditorInstance.GameMemory.ReadInt32(CheckpointBase) + 0x16, value); } }

        public byte CurrentScene { get { return EditorInstance.GameMemory.ReadByte(0x00E48758); } set { EditorInstance.GameMemory.WriteByte(0x00E48758, value); } }
        public byte GameState { get { return EditorInstance.GameMemory.ReadByte(0x00E48776); } set { EditorInstance.GameMemory.WriteByte(0x00E48776, value); } }


        public EditorInGame(Editor instance)
        {
            EditorInstance = instance;
        }

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

        public bool CheckpointEnabled { get { return StarPostEnable >= (byte)65; } }


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
                    X = EditorInstance.P1_X;
                    break;
                case 2:
                    X = EditorInstance.P2_X;
                    break;
                case 3:
                    X = EditorInstance.P3_X;
                    break;
                case 4:
                    X = EditorInstance.P4_X;
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
                    Y = EditorInstance.P1_Y;
                    break;
                case 2:
                    Y = EditorInstance.P2_Y;
                    break;
                case 3:
                    Y = EditorInstance.P3_Y;
                    break;
                case 4:
                    Y = EditorInstance.P4_Y;
                    break;
            }
            return Y;
        }

        public void MovePlayer(Point p, double Zoom, int player)
        {
            if (EditorInstance.GameRunning)
            {
                // TODO: Find out if this is constent
                switch (player)
                {
                    case 0:
                        Player1_X = (short)(p.X / Zoom);
                        Player1_Y = (short)(p.Y / Zoom);
                        break;
                    case 1:
                        Player2_X = (short)(p.X / Zoom);
                        Player2_Y = (short)(p.Y / Zoom);
                        break;
                    case 2:
                        Player3_X = (short)(p.X / Zoom);
                        Player3_Y = (short)(p.Y / Zoom);
                        break;
                    case 3:
                        Player4_X = (short)(p.X / Zoom);
                        Player4_Y = (short)(p.Y / Zoom);
                        break;
                }

            }
        }

        public void DrawSinglePlayer(DevicePanel d, int playerID)
        {

            int x = 0;
            int y = 0;
            int ID = 0;
            switch (playerID)
            {
                case 1:
                    EditorInstance.P1_X = Player1_X;
                    EditorInstance.P1_Y = Player1_Y;
                    x = Player1_X;
                    y = Player1_Y;
                    ID = Player1_State;
                    break;
                case 2:
                    EditorInstance.P2_X = Player2_X;
                    EditorInstance.P2_Y = Player2_Y;
                    x = Player2_X;
                    y = Player2_Y;
                    ID = Player2_State;
                    break;
                case 3:
                    EditorInstance.P3_X = Player3_X;
                    EditorInstance.P3_Y = Player3_Y;
                    x = Player3_X;
                    y = Player3_Y;
                    ID = Player3_State;
                    break;
                case 4:
                    EditorInstance.P4_X = Player4_X;
                    EditorInstance.P4_Y = Player4_Y;
                    x = Player4_X;
                    y = Player4_Y;
                    ID = Player4_State;
                    break;

            }

            if (playerID <= 0 || playerID >= 5) return;

            if (playerID == EditorInstance.PlayerBeingTracked) EditorInstance.GoToPosition(x, y);

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
                    if (EditorInstance.GetZoom() >= 1) d.DrawTextSmall(name, x + 2, y + 2, NAME_BOX_WIDTH - 4, System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black), true);
                }
                else
                {
                    var editorAnim = EditorInstance.EditorEntity_ini.LoadAnimation2("HUD", d, 2, frameID, false, false, false);
                    if (editorAnim != null && editorAnim.Frames.Count != 0 && ID != 0)
                    {
                        var frame = editorAnim.Frames[0];
                        d.DrawBitmap(frame.Texture, x, y, frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }


            }
        }

        public void UpdateCheckpoint(Point p, bool enabled = true, bool forceStatus = false)
        {

            byte status = StarPostEnable;
            if (enabled == true)
            {
                status = 0x01;
            }
            StarPostX = p.X;
            StarPostY = p.Y;
            StarPostEnable = status;

        }

        public void AssetReset()
        {
            int x = Player1_X;
            int y = Player1_Y;

            byte oldScene = CurrentScene;
            CurrentScene = 0x03;
            GameState = 0;

            MessageBox.Show("Click OK to Finish Asset Reset");
            CurrentScene = oldScene;
            GameState = 0;
            UpdateCheckpoint(new Point(x, y), true, true);
        }

        public void RestartScene()
        {
            GameState = 0;
        }

        public void DrawCheckpoint(DevicePanel d)
        {
            // TODO: Find out if this is constent
            int x = StarPostX;
            int y = StarPostY;
            bool isEnabled = (StarPostEnable >= 1 ? true : false);


            int Transparency = 0xff;

            System.Drawing.Color color2 = System.Drawing.Color.DarkBlue;
            bool showFrame = false;
            string name = "Checkpoint";




            if (isEnabled)
            {
                if (showFrame)
                {
                    d.DrawRectangle(x, y, x + NAME_BOX_WIDTH, y + NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(0x00, System.Drawing.Color.MediumPurple));
                    d.DrawLine(x, y, x + NAME_BOX_WIDTH, y, System.Drawing.Color.FromArgb(Transparency, color2));
                    d.DrawLine(x, y, x, y + NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color2));
                    d.DrawLine(x, y + NAME_BOX_HEIGHT, x + NAME_BOX_WIDTH, y + NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color2));
                    d.DrawLine(x + NAME_BOX_WIDTH, y, x + NAME_BOX_WIDTH, y + NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color2));
                    if (EditorInstance.GetZoom() >= 1) d.DrawTextSmall(name, x + 2, y + 2, NAME_BOX_WIDTH - 4, System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black), true);
                }
                else
                {
                    var editorAnim = EditorInstance.EditorEntity_ini.LoadAnimation2("StarPost", d, 1, 0, false, false, false);
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

            DrawSinglePlayer(d, 1);
            DrawSinglePlayer(d, 2);
            DrawSinglePlayer(d, 3);
            DrawSinglePlayer(d, 4);

            DrawCheckpoint(d);


        }
    }
}
