using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ManiacEditor;
using System.Drawing;
using SharpDX.Direct3D9;
using RSDKv5;
using System.Windows.Forms;
using System.Diagnostics;
using IronPython.Compiler.Ast;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ManiacEditor.Entity_Renders
{
    public class TilePlatform : EntityRenderer
    {

            public override void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
            {
            bool fliph = false;
            bool flipv = false;
            int width = (int)entity.attributesMap["size"].ValuePosition.X.High - 1;
            int height = (int)entity.attributesMap["size"].ValuePosition.Y.High - 1;
            int x2 = (int)entity.attributesMap["targetPos"].ValuePosition.X.High - 1;
            int y2 = (int)entity.attributesMap["targetPos"].ValuePosition.Y.High - 1;
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("EditorIcons2", d.DevicePanel, 0, 7, fliph, flipv, false);

            //int widthD = (int)Math.Round(width / 16, MidpointRounding.ToEven);
            //int heightD = (int)Math.Round(height / 16, MidpointRounding.ToEven);
            //int x2D = (int)Math.Round(x2/16, MidpointRounding.ToEven);
            //int y2D = (int)Math.Round(y2/16, MidpointRounding.ToEven);
            int widthD = RoundNum(width, 16) / 16;
            int heightD = RoundNum(height, 16) / 16;
            int x2D = RoundNum(x2, 16) / 16;
            int y2D = RoundNum(y2, 16) / 16;

            //d.DrawRectangle(x - width / 2, y - height / 2, x + width/2, y + height/2, System.Drawing.Color.White);


            // The position for some platforms are still off a bit (but it's very decent)
            var GroupTexture = Editor.Instance.EntityDrawing.LoadTilePlatform(d.DevicePanel, x2D, y2D, widthD, heightD);
            if (GroupTexture != null)
            {
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(GroupTexture.Texture), x - (int)(RoundNum(width, 16)/ 2), y - (int)(RoundNum(height, 16) / 2), width, height, false, Transparency);
            }               
            }

        private int RoundNum(int num, int step)
        {
            if (num >= 0)
                return ((num + (step / 2)) / step) * step;
            else
                return ((num - (step / 2)) / step) * step;
        }




        public override string GetObjectName()
        {
            return "TilePlatform";
        }
    }
}
