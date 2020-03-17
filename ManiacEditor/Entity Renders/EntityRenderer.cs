﻿using RSDKv5;
using System;
using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    [Serializable]
    public abstract class EntityRenderer
    {

        #region Common
        public abstract string GetObjectName();

        public virtual void Draw(Structures.EntityRenderProp properties)
        {

        }

        public virtual bool isObjectOnScreen(DevicePanel d, SceneEntity entity, Classes.Scene.EditorEntity e, int x, int y, int Transparency)
        {
            return d.IsObjectOnScreen(x, y, 20, 20);
        }
        #endregion

        #region Universal
        public static Methods.Entities.EntityDrawing.EditorAnimation LoadAnimation(string name, DevicePanel d, int AnimID = 0, int FrameID = 0)
        {
            return Methods.Entities.EntityDrawing.LoadAnimation(d, name, AnimID, FrameID);
        }
        public static Methods.Entities.EntityDrawing.EditorAnimation LoadAnimation(DevicePanel d, string name, int AnimID = 0, int FrameID = 0)
        {
            return Methods.Entities.EntityDrawing.LoadAnimation(d, name, AnimID, FrameID);
        }
        public static bool IsValidated(Methods.Entities.EntityDrawing.EditorAnimation Animation, Tuple<int,int> AnimPos = null)
        {
            if (Animation != null)
            {
                if (Animation.Animation != null)
                {
                    if (Animation.Animation.Animations.Count != 0 && Animation.Animation.Animations.Count >= 1 && Animation.Animation.Animations[0].Frames != null && Animation.Animation.Animations[0].Frames.Count >= 1)
                    {
                        if (AnimPos != null)
                        {
                            int AnimID = AnimPos.Item1, FrameID = AnimPos.Item2;

                            if (AnimID >= 0 && Animation.Animation.Animations.Count - 1 >= AnimID)
                            {
                                if (FrameID >= 0 && Animation.Animation.Animations[AnimID].Frames.Count - 1 >= FrameID)
                                {
                                    if (Animation.Animation.Animations[AnimID].Frames[FrameID].SpriteSheet <= Animation.Animation.SpriteSheets.Count - 1)
                                    {
                                        if (Animation.Animation.Animations[AnimID].Frames[FrameID].SpriteSheet <= Animation.Spritesheets.Count - 1) return true;
                                    }
                                }
                            }
                        }
                        else return true;
                    }
                }
            }
            return false;
        }

        public static void DrawTexturePivotPlus(DevicePanel Graphics, Methods.Entities.EntityDrawing.EditorAnimation Animation, int AnimID, int FrameID, int x, int y, int x2, int y2, int Transparency, bool FlipH = false, bool FlipV = false)
        {
            if (EntityRenderer.IsValidated(Animation, new System.Tuple<int, int>(AnimID, FrameID)))
            {
                var Frame = Animation.Animation.Animations[AnimID].Frames[FrameID];
                Graphics.DrawTexture(Animation.Spritesheets.ElementAt(Frame.SpriteSheet).Value, x + Frame.PivotX + x2, y + Frame.PivotY + y2, Frame.X, Frame.Y, Frame.Width, Frame.Height, false, Transparency, FlipH, FlipV);
            }
        }

        public static void DrawTexturePivotNormal(DevicePanel Graphics, Methods.Entities.EntityDrawing.EditorAnimation Animation, int AnimID, int FrameID, int x, int y, int Transparency, bool FlipH = false, bool FlipV = false)
        {
            if (EntityRenderer.IsValidated(Animation, new System.Tuple<int, int>(AnimID, FrameID)))
            {
                var Frame = Animation.Animation.Animations[AnimID].Frames[FrameID];
                Graphics.DrawTexture(Animation.Spritesheets.ElementAt(Frame.SpriteSheet).Value, x + Frame.PivotX, y + Frame.PivotY, Frame.X, Frame.Y, Frame.Width, Frame.Height, false, Transparency, FlipH, FlipV);
            }
        }
        public static void DrawTexture(DevicePanel Graphics, Methods.Entities.EntityDrawing.EditorAnimation Animation, int AnimID, int FrameID, int x, int y, int Transparency, bool FlipH = false, bool FlipV = false)
        {
            if (EntityRenderer.IsValidated(Animation, new System.Tuple<int, int>(AnimID, FrameID)))
            {
                var Frame = Animation.Animation.Animations[AnimID].Frames[FrameID];
                Graphics.DrawTexture(Animation.Spritesheets.ElementAt(Frame.SpriteSheet).Value, x, y, Frame.X, Frame.Y, Frame.Width, Frame.Height, false, Transparency, FlipH, FlipV);
            }
        }

        #endregion


    }
}