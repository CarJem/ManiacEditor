using RSDKv5;
using System;
using System.Linq;
using ManiacEditor.Classes.Scene;

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

        public virtual bool isObjectOnScreen(DevicePanel d, EditorEntity entity, int x, int y, int Transparency)
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

        public static void DrawTexturePivotPlus(DevicePanel Graphics, Methods.Entities.EntityDrawing.EditorAnimation Animation, int AnimID, int FrameID, int x, int y, int x2, int y2, int Transparency, bool FlipH = false, bool FlipV = false, int rotation = 0, System.Drawing.Color? color = null)
        {
            if (EntityRenderer.IsValidated(Animation, new System.Tuple<int, int>(AnimID, FrameID)))
            {
                var Frame = Animation.Animation.Animations[AnimID].Frames[FrameID];
                Graphics.DrawTexture(Animation.Spritesheets.ElementAt(Frame.SpriteSheet).Value, x + Frame.PivotX + x2, y + Frame.PivotY + y2, Frame.X, Frame.Y, Frame.Width, Frame.Height, false, Transparency, FlipH, FlipV, rotation, color);
            }
        }

        public static void DrawBounds(DevicePanel d, int x, int y, int width, int height, int Transparency, System.Drawing.Color outline, System.Drawing.Color fill)
        {
            if (width != 0 && height != 0)
            {
                int x1 = x + width / -2;
                int x2 = x + width / 2 - 1;
                int y1 = y + height / -2;
                int y2 = y + height / 2 - 1;

                d.DrawRectangle(x1, y1, x2, y2, fill, outline, 2);

                var Animation = Methods.Entities.EntityDrawing.LoadAnimation(d, "EditorAssets", 0, 1);

                // draw corners
                for (int i = 0; i < 4; i++)
                {
                    bool right = (i & 1) > 0;
                    bool bottom = (i & 2) > 0;

                    int realX = (right ? x2 - 15 : x1 - 1);
                    int realY = (bottom ? y2 - 15 : y1 - 1);
                    DrawTexture(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, realX, realY, Transparency, right, bottom);


                }
            }
        }

        public static void DrawTexturePivotNormal(DevicePanel Graphics, Methods.Entities.EntityDrawing.EditorAnimation Animation, int AnimID, int FrameID, int x, int y, int Transparency, bool FlipH = false, bool FlipV = false, int rotation = 0, System.Drawing.Color? color = null)
        {
            if (EntityRenderer.IsValidated(Animation, new System.Tuple<int, int>(AnimID, FrameID)))
            {
                var Frame = Animation.Animation.Animations[AnimID].Frames[FrameID];
                Graphics.DrawTexture(Animation.Spritesheets.ElementAt(Frame.SpriteSheet).Value, x + Frame.PivotX, y + Frame.PivotY, Frame.X, Frame.Y, Frame.Width, Frame.Height, false, Transparency, FlipH, FlipV, rotation, color);
            }
        }

        public static void DrawTexturePivotLengthLimit(DevicePanel Graphics, Methods.Entities.EntityDrawing.EditorAnimation Animation, int AnimID, int FrameID, int x, int y, int Transparency, int Height)
        {
            if (EntityRenderer.IsValidated(Animation, new System.Tuple<int, int>(AnimID, FrameID)))
            {
                var Frame = Animation.Animation.Animations[AnimID].Frames[FrameID];
                Graphics.DrawTexture(Animation.Spritesheets.ElementAt(Frame.SpriteSheet).Value, x + Frame.PivotX, y, Frame.X, Frame.Y, Frame.Width, Height, false, Transparency);
            }
        }

        public static void DrawTexturePivotForced(DevicePanel Graphics, Methods.Entities.EntityDrawing.EditorAnimation Animation, int AnimID, int FrameID, int x, int y, int Transparency, bool FlipH = false, bool FlipV = false, int rotation = 0, System.Drawing.Color? color = null)
        {
            if (EntityRenderer.IsValidated(Animation, new System.Tuple<int, int>(AnimID, FrameID)))
            {
                var Frame = Animation.Animation.Animations[AnimID].Frames[FrameID];
                Graphics.DrawTexture(Animation.Spritesheets.ElementAt(Frame.SpriteSheet).Value, x - (Frame.Width / 2), y - (Frame.Height / 2), Frame.X, Frame.Y, Frame.Width, Frame.Height, false, Transparency, FlipH, FlipV, rotation, color);
            }
        }

        public static void DrawHitbox(DevicePanel Graphics, Methods.Entities.EntityDrawing.EditorAnimation Animation, string HitboxName, System.Drawing.Color color, int AnimID, int FrameID, int x, int y, int Transparency, bool FlipH = false, bool FlipV = false, int rotation = 0)
        {
            if (EntityRenderer.IsValidated(Animation, new System.Tuple<int, int>(AnimID, FrameID)))
            {
                var Frame = Animation.Animation.Animations[AnimID].Frames[FrameID];

                if (Animation.Animation.CollisionBoxes.Contains(HitboxName))
                {
                    int hitboxIndex = Animation.Animation.CollisionBoxes.IndexOf(HitboxName);
                    int x1 = x + (int)Frame.HitBoxes[hitboxIndex].Left;
                    int y1 = y + (int)Frame.HitBoxes[hitboxIndex].Bottom;
                    int x2 = x + (int)Frame.HitBoxes[hitboxIndex].Right;
                    int y2 = y + (int)Frame.HitBoxes[hitboxIndex].Top;

                    Graphics.DrawRectangle(x1, y1, x2, y2, color);
                }
            }
        }

        public static void DrawTexture(DevicePanel Graphics, Methods.Entities.EntityDrawing.EditorAnimation Animation, int AnimID, int FrameID, int x, int y, int Transparency, bool FlipH = false, bool FlipV = false, int rotation = 0, System.Drawing.Color? color = null)
        {
            if (EntityRenderer.IsValidated(Animation, new System.Tuple<int, int>(AnimID, FrameID)))
            {
                var Frame = Animation.Animation.Animations[AnimID].Frames[FrameID];
                Graphics.DrawTexture(Animation.Spritesheets.ElementAt(Frame.SpriteSheet).Value, x, y, Frame.X, Frame.Y, Frame.Width, Frame.Height, false, Transparency, FlipH, FlipV, rotation, color);
            }
        }

        public static void DrawTexture(DevicePanel Graphics, Methods.Entities.EntityDrawing.EditorAnimation Animation, int AnimID, int FrameID, int x, int y, int width, int height, int Transparency, bool FlipH = false, bool FlipV = false, int rotation = 0, System.Drawing.Color? color = null)
        {
            if (EntityRenderer.IsValidated(Animation, new System.Tuple<int, int>(AnimID, FrameID)))
            {
                var Frame = Animation.Animation.Animations[AnimID].Frames[FrameID];
                Graphics.DrawTexture(Animation.Spritesheets.ElementAt(Frame.SpriteSheet).Value, x, y, Frame.X, Frame.Y, width, height, false, Transparency, FlipH, FlipV, rotation, color);
            }
        }

        #endregion


    }
}
