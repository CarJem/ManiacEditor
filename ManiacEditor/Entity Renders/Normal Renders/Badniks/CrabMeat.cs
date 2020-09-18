﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor.Entity_Renders
{
    public class CrabMeat : EntityRenderer
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

            int FrameID = 0;
            int AnimID = 0;

            var animation = LoadAnimation(d, GetSetupAnimation(), AnimID, FrameID);
            Entity_Renders.EntityRenderer.DrawTexturePivotNormal(d, animation, animation.RequestedAnimID, animation.RequestedFrameID, x, y, Transparency);
        }

        public override string GetObjectName()
        {
            return "Crabmeat";
        }

        public override string GetSetupAnimation()
        {
            return GetSpriteAnimationPath("/Crabmeat.bin", "Crabmeat", new string[] { "GHZ" }, "GHZ");
        }

    }
}