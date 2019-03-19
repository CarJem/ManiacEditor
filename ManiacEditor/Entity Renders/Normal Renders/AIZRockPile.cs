﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ManiacEditor;
using Microsoft.Xna.Framework;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class AIZRockPile : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            int size = (int)entity.attributesMap["size"].ValueVar;
            var editorAnim = e.EditorInstance.EntityDrawing.LoadAnimation2("AIZ\\Platform", d, 0, size + 3, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            bool knux = entity.attributesMap["onlyKnux"].ValueBool;
            bool mighty = entity.attributesMap.ContainsKey("onlyMighty") && entity.attributesMap["onlyMighty"].ValueBool;

            // draw Knuckles icon
            if (knux)
            {
                editorAnim = e.EditorInstance.EntityDrawing.LoadAnimation2("HUD", d, 2, 2, false, false, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    var frame = editorAnim.Frames[Animation.index];
                    Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                    d.DrawBitmap(frame.Texture, x - frame.Frame.Width / (mighty ? 1 : 2), y - frame.Frame.Height / 2, frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
            }

            // draw Mighty icon
            if (mighty)
            {
                editorAnim = e.EditorInstance.EntityDrawing.LoadAnimation2("HUD", d, 2, 3, false, false, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    var frame = editorAnim.Frames[Animation.index];
                    Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                    d.DrawBitmap(frame.Texture, x - (knux ? 0 : frame.Frame.Width / 2), y - frame.Frame.Height / 2, frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
            }
        }

        public override string GetObjectName()
        {
            return "AIZRockPile";
        }
    }
}
