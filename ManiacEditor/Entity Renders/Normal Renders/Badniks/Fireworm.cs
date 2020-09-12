using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor.Entity_Renders
{
    public class Fireworm : EntityRenderer
    {


        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;

            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            bool fliph = false;
            bool flipv = false;

            int FrameID = 1;
            int AnimID = 0;

            var animation = LoadAnimation(d, GetSetupAnimation(), AnimID, FrameID);
            DrawTexturePivotNormal(d, animation, animation.RequestedAnimID, animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "Fireworm";
        }

        public override string GetSetupAnimation()
        {
            return GetSpriteAnimationPath("/Fireworm.bin", "Fireworm", new string[] { "LRZ1", "LRZ2" }, "LRZ1");
        }

    }
}