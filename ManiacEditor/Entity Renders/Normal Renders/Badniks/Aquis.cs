﻿using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Aquis : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            var Animation = Methods.Drawing.ObjectDrawing.LoadAnimation(Properties.Graphics, "OOZ/Aquis.bin", 1, 0);
            DrawTexturePivotNormal(Properties.Graphics, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency);

            Animation = Methods.Drawing.ObjectDrawing.LoadAnimation(Properties.Graphics, "OOZ/Aquis.bin", 3, 0);
            DrawTexturePivotNormal(Properties.Graphics, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency);
        }

        public override string GetObjectName()
        {
            return "Aquis";
        }
    }
}
