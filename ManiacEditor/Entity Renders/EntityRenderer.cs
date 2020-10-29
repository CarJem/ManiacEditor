using RSDKv5;
using System;
using System.Linq;
using ManiacEditor.Classes.Scene;
using System.Windows.Documents;
using System.Collections.Generic;

namespace ManiacEditor.Entity_Renders
{
    [Serializable]
    public abstract class EntityRenderer
    {

        #region Abstracts/Overides

        public abstract string GetObjectName();
        public virtual void Draw(Structures.EntityRenderProp properties)
        {

        }
        public virtual bool isObjectOnScreen(DevicePanel d, EditorEntity entity, int x, int y, int Transparency)
        {
            return d.IsObjectOnScreen(x, y, 20, 20);
        }
        public virtual string GetSetupAnimation()
        {
            //return GetSpriteAnimationPath("/BuzzBomber.bin", "BuzzBomber", new string[] { "GHZ" }, "GHZ");
            return "NULLSPRITE";
        }

        #endregion

        #region Zone Unlocks / Asset Detection
        private static bool ValidUnlockCode(List<string> ZoneUnlocks, string UnlockName)
        {
            return ZoneUnlocks.Contains(UnlockName) && Structures.IZStageUnlocks.AllUnlocks.Contains(UnlockName);
        }
        private static bool AllowedSetupType(List<string> AllowedSetups, string SetupName, string DesiredSetup)
        {
            return DesiredSetup == SetupName && AllowedSetups.Contains(SetupName);
        }
        public static string GetSpriteAnimationPath(string BinName, string UnlockName, string[] AllowedSetupsArray, string FallBackFolder = "Blueprint")
        {
            List<string> AllowedSetups = AllowedSetupsArray.ToList();

            if (Methods.Solution.CurrentSolution.IZ_Stage != null && Methods.Solution.CurrentSolution.IZ_Stage.Unlocks != null)
            {
                var unlocks = Methods.Solution.CurrentSolution.IZ_Stage.Unlocks;

                if (ValidUnlockCode(unlocks, "GHZ" + "_" + UnlockName)) return "GHZ" + BinName;
                else if (ValidUnlockCode(unlocks, "CPZ" + "_" + UnlockName)) return "CPZ" + BinName;
                else if (ValidUnlockCode(unlocks, "SPZ1" + "_" + UnlockName)) return "SPZ1" + BinName;
                else if (ValidUnlockCode(unlocks, "SPZ2" + "_" + UnlockName)) return "SPZ2" + BinName;
                else if (ValidUnlockCode(unlocks, "FBZ" + "_" + UnlockName)) return "FBZ" + BinName;
                else if (ValidUnlockCode(unlocks, "PSZ1" + "_" + UnlockName)) return "PSZ1" + BinName;
                else if (ValidUnlockCode(unlocks, "PSZ2" + "_" + UnlockName)) return "PSZ2" + BinName;
                else if (ValidUnlockCode(unlocks, "SSZ1" + "_" + UnlockName)) return "SSZ1" + BinName;
                else if (ValidUnlockCode(unlocks, "SSZ2" + "_" + UnlockName)) return "SSZ2" + BinName;
                else if (ValidUnlockCode(unlocks, "SSZ3" + "_" + UnlockName)) return "SSZ3" + BinName;
                else if (ValidUnlockCode(unlocks, "HCZ" + "_" + UnlockName)) return "HCZ" + BinName;
                else if (ValidUnlockCode(unlocks, "MSZ" + "_" + UnlockName)) return "MSZ" + BinName;
                else if (ValidUnlockCode(unlocks, "OOZ" + "_" + UnlockName)) return "OOZ" + BinName;
                else if (ValidUnlockCode(unlocks, "LRZ1" + "_" + UnlockName)) return "LRZ1" + BinName;
                else if (ValidUnlockCode(unlocks, "LRZ2" + "_" + UnlockName)) return "LRZ2" + BinName;
                else if (ValidUnlockCode(unlocks, "LRZ3" + "_" + UnlockName)) return "LRZ3" + BinName;
                else if (ValidUnlockCode(unlocks, "MMZ" + "_" + UnlockName)) return "MMZ" + BinName;
                else if (ValidUnlockCode(unlocks, "TMZ1" + "_" + UnlockName)) return "TMZ1" + BinName;
                else if (ValidUnlockCode(unlocks, "AIZ" + "_" + UnlockName)) return "AIZ" + BinName;
                else if (ValidUnlockCode(unlocks, "HPZ" + "_" + UnlockName)) return "HPZ" + BinName;
                else if (ValidUnlockCode(unlocks, "Blueprint" + "_" + UnlockName)) return "Blueprint" + BinName;
            }
            string SetupType = "";
            if (Methods.Solution.CurrentSolution.Entities.SetupObject != null) SetupType = Methods.Solution.CurrentSolution.Entities.SetupObject.Replace("Setup", "");

            if (AllowedSetupType(AllowedSetups, SetupType, "GHZ")) return "GHZ" + BinName;
            else if (AllowedSetupType(AllowedSetups, SetupType, "CPZ")) return "CPZ" + BinName;
            else if (AllowedSetupType(AllowedSetups, SetupType, "SPZ1")) return "SPZ1" + BinName;
            else if (AllowedSetupType(AllowedSetups, SetupType, "SPZ2")) return "SPZ2" + BinName;
            else if (AllowedSetupType(AllowedSetups, SetupType, "FBZ")) return "FBZ" + BinName;
            else if (AllowedSetupType(AllowedSetups, SetupType, "PSZ1")) return "PSZ1" + BinName;
            else if (AllowedSetupType(AllowedSetups, SetupType, "PSZ2")) return "PSZ2" + BinName;
            else if (AllowedSetupType(AllowedSetups, SetupType, "SSZ1")) return "SSZ1" + BinName;
            else if (AllowedSetupType(AllowedSetups, SetupType, "SSZ2")) return "SSZ2" + BinName;
            else if (AllowedSetupType(AllowedSetups, SetupType, "SSZ3")) return "SSZ3" + BinName;
            else if (AllowedSetupType(AllowedSetups, SetupType, "HCZ")) return "HCZ" + BinName;
            else if (AllowedSetupType(AllowedSetups, SetupType, "MSZ")) return "MSZ" + BinName;
            else if (AllowedSetupType(AllowedSetups, SetupType, "OOZ")) return "OOZ" + BinName;
            else if (AllowedSetupType(AllowedSetups, SetupType, "LRZ1")) return "LRZ1" + BinName;
            else if (AllowedSetupType(AllowedSetups, SetupType, "LRZ2")) return "LRZ2" + BinName;
            else if (AllowedSetupType(AllowedSetups, SetupType, "LRZ3")) return "LRZ3" + BinName;
            else if (AllowedSetupType(AllowedSetups, SetupType, "MMZ")) return "MMZ" + BinName;
            else if (AllowedSetupType(AllowedSetups, SetupType, "TMZ1")) return "TMZ1" + BinName;
            else if (AllowedSetupType(AllowedSetups, SetupType, "AIZ")) return "AIZ" + BinName;
            else if (AllowedSetupType(AllowedSetups, SetupType, "HPZ")) return "HPZ" + BinName;
            else if (AllowedSetupType(AllowedSetups, SetupType, "Blueprint")) return "Blueprint" + BinName;
            else
            {
                if (AllowedSetupsArray.Length >= 1 && FallBackFolder == "Blueprint") return AllowedSetupsArray[0] + BinName;
                else return FallBackFolder + BinName;
            }
        }
        public static string GetParallaxSpritePath(string[] AllowedSetupsArray)
        {
            // Parallax Sprite's that Exist
            // "SPZ", "LRZ", "MSZ", "OOZ", "CPZ", "FBZ"

            List<string> AllowedSetups = AllowedSetupsArray.ToList();

            string BinName = "Parallax.bin";
            string UnlockName = "Parallax";
            if (Methods.Solution.CurrentSolution.IZ_Stage != null && Methods.Solution.CurrentSolution.IZ_Stage.Unlocks != null)
            {
                var unlocks = Methods.Solution.CurrentSolution.IZ_Stage.Unlocks;

                if (ValidUnlockCode(unlocks, "CPZ" + "_" + UnlockName)) return "CPZ" + "/" + "CPZ" + BinName;
                else if (ValidUnlockCode(unlocks, "SPZ1" + "_" + UnlockName)) return "SPZ1" + "/" + "SPZ" + BinName;
                else if (ValidUnlockCode(unlocks, "FBZ" + "_" + UnlockName)) return "FBZ" + "/" + "FBZ" + BinName;
                else if (ValidUnlockCode(unlocks, "MSZ" + "_" + UnlockName)) return "MSZ" + "/" + "MSZ" + BinName;
                else if (ValidUnlockCode(unlocks, "OOZ" + "_" + UnlockName)) return "OOZ" + "/" + "OOZ" + BinName;
                else if (ValidUnlockCode(unlocks, "LRZ2" + "_" + UnlockName)) return "LRZ2" + "/" + "LRZ" + BinName;
            }

            string SetupType = "";
            if (Methods.Solution.CurrentSolution.Entities.SetupObject != null) SetupType = Methods.Solution.CurrentSolution.Entities.SetupObject.Replace("Setup", "");

            if (AllowedSetupType(AllowedSetups, SetupType, "CPZ")) return "CPZ" + "/" + "CPZ" + BinName;
            else if (AllowedSetupType(AllowedSetups, SetupType, "SPZ1")) return "SPZ1" + "/" + "SPZ" + BinName;
            else if (AllowedSetupType(AllowedSetups, SetupType, "FBZ")) return "FBZ" + "/" + "FBZ" + BinName;
            else if (AllowedSetupType(AllowedSetups, SetupType, "MSZ")) return "MSZ" + "/" + "MSZ" + BinName;
            else if (AllowedSetupType(AllowedSetups, SetupType, "OOZ")) return "OOZ" + "/" + "OOZ" + BinName;
            else if (AllowedSetupType(AllowedSetups, SetupType, "LRZ2")) return "LRZ2" + "/" + "LRZ" + BinName;
            else
            {
                return "EditorIcons2";
            }
        }
        #endregion

        #region LoadAnimation/Validation
        public static bool IsValidated(Methods.Drawing.ObjectDrawing.EditorAnimation Animation, Tuple<int, int> AnimPos = null)
        {
            if (Animation != null)
            {
                if (Animation.Animation != null)
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
                    else return false;
                }
            }
            return false;
        }
        public static Methods.Drawing.ObjectDrawing.EditorAnimation LoadAnimation(string name, DevicePanel d, int AnimID = 0, int FrameID = 0)
        {
            return Methods.Drawing.ObjectDrawing.LoadAnimation(d, name, AnimID, FrameID);
        }
        public static Methods.Drawing.ObjectDrawing.EditorAnimation LoadAnimation(DevicePanel d, string name, int AnimID = 0, int FrameID = 0)
        {
            return Methods.Drawing.ObjectDrawing.LoadAnimation(d, name, AnimID, FrameID);
        }
        #endregion

        #region DrawTexture
        public static void DrawTexture(DevicePanel Graphics, Methods.Drawing.ObjectDrawing.EditorAnimation Animation, int AnimID, int FrameID, int x, int y, int Transparency, bool FlipH = false, bool FlipV = false, int rotation = 0, System.Drawing.Color? color = null)
        {
            if (EntityRenderer.IsValidated(Animation, new System.Tuple<int, int>(AnimID, FrameID)))
            {
                var Frame = Animation.Animation.Animations[AnimID].Frames[FrameID];
                Graphics.DrawBitmap(Animation.Spritesheets.ElementAt(Frame.SpriteSheet).Value, x, y, Frame.X, Frame.Y, Frame.Width, Frame.Height, false, Transparency, FlipH, FlipV, rotation, color);
            }
        }
        public static void DrawTexture(DevicePanel Graphics, Methods.Drawing.ObjectDrawing.EditorAnimation Animation, int AnimID, int FrameID, int x, int y, int width, int height, int Transparency, bool FlipH = false, bool FlipV = false, int rotation = 0, System.Drawing.Color? color = null)
        {
            if (EntityRenderer.IsValidated(Animation, new System.Tuple<int, int>(AnimID, FrameID)))
            {
                var Frame = Animation.Animation.Animations[AnimID].Frames[FrameID];
                Graphics.DrawBitmap(Animation.Spritesheets.ElementAt(Frame.SpriteSheet).Value, x, y, Frame.X, Frame.Y, width, height, false, Transparency, FlipH, FlipV, rotation, color);
            }
        }
        public static void DrawTexturePivotPlus(DevicePanel Graphics, Methods.Drawing.ObjectDrawing.EditorAnimation Animation, int AnimID, int FrameID, int x, int y, int x2, int y2, int Transparency, bool FlipH = false, bool FlipV = false, int rotation = 0, System.Drawing.Color? color = null)
        {
            if (EntityRenderer.IsValidated(Animation, new System.Tuple<int, int>(AnimID, FrameID)))
            {
                var Frame = Animation.Animation.Animations[AnimID].Frames[FrameID];
                Graphics.DrawBitmap(Animation.Spritesheets.ElementAt(Frame.SpriteSheet).Value, x + Frame.PivotX + x2, y + Frame.PivotY + y2, Frame.X, Frame.Y, Frame.Width, Frame.Height, false, Transparency, FlipH, FlipV, rotation, color);
            }
        }
        public static void DrawTexturePivotNormal(DevicePanel Graphics, Methods.Drawing.ObjectDrawing.EditorAnimation Animation, int AnimID, int FrameID, int x, int y, int Transparency, bool FlipH = false, bool FlipV = false, int rotation = 0, System.Drawing.Color? color = null)
        {
            if (EntityRenderer.IsValidated(Animation, new System.Tuple<int, int>(AnimID, FrameID)))
            {
                var Frame = Animation.Animation.Animations[AnimID].Frames[FrameID];
                Graphics.DrawBitmap(Animation.Spritesheets.ElementAt(Frame.SpriteSheet).Value, x + Frame.PivotX, y + Frame.PivotY, Frame.X, Frame.Y, Frame.Width, Frame.Height, false, Transparency, FlipH, FlipV, rotation, color);
            }
        }
        public static void DrawTexturePivotLengthLimit(DevicePanel Graphics, Methods.Drawing.ObjectDrawing.EditorAnimation Animation, int AnimID, int FrameID, int x, int y, int Transparency, int Height)
        {
            if (EntityRenderer.IsValidated(Animation, new System.Tuple<int, int>(AnimID, FrameID)))
            {
                var Frame = Animation.Animation.Animations[AnimID].Frames[FrameID];
                Graphics.DrawBitmap(Animation.Spritesheets.ElementAt(Frame.SpriteSheet).Value, x + Frame.PivotX, y, Frame.X, Frame.Y, Frame.Width, Height, false, Transparency);
            }
        }
        public static void DrawTexturePivotForced(DevicePanel Graphics, Methods.Drawing.ObjectDrawing.EditorAnimation Animation, int AnimID, int FrameID, int x, int y, int Transparency, bool FlipH = false, bool FlipV = false, int rotation = 0, System.Drawing.Color? color = null)
        {
            if (EntityRenderer.IsValidated(Animation, new System.Tuple<int, int>(AnimID, FrameID)))
            {
                var Frame = Animation.Animation.Animations[AnimID].Frames[FrameID];
                Graphics.DrawBitmap(Animation.Spritesheets.ElementAt(Frame.SpriteSheet).Value, x - (Frame.Width / 2), y - (Frame.Height / 2), Frame.X, Frame.Y, Frame.Width, Frame.Height, false, Transparency, FlipH, FlipV, rotation, color);
            }
        }
        #endregion

        #region DrawBounds
        public static void DrawBounds(DevicePanel d, int x, int y, int width, int height, int Transparency, System.Drawing.Color outline, System.Drawing.Color fill)
        {
            if (width != 0 && height != 0)
            {
                int x1 = x + width / -2;
                int x2 = x + width / 2 - 1;
                int y1 = y + height / -2;
                int y2 = y + height / 2 - 1;

                d.DrawRectangle(x1, y1, x2, y2, fill, outline, 1);

                var Animation = Methods.Drawing.ObjectDrawing.LoadAnimation(d, "EditorAssets", 0, 1);

                // draw corners
                for (int i = 0; i < 4; i++)
                {
                    bool right = (i & 1) > 0;
                    bool bottom = (i & 2) > 0;

                    int realX = (right ? x2 - 15 : x1);
                    int realY = (bottom ? y2 - 15 : y1);
                    DrawTexture(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, realX, realY, Transparency, right, bottom);


                }
            }
        }
        public static void DrawBounds2(DevicePanel d, int x1, int y1, int x2, int y2, int Transparency, System.Drawing.Color outline, System.Drawing.Color fill)
        {
            d.DrawRectangle(x1, y1, x2, y2, fill, outline, 1);

            var Animation = Methods.Drawing.ObjectDrawing.LoadAnimation(d, "EditorAssets", 0, 1);

            // draw corners
            for (int i = 0; i < 4; i++)
            {
                bool right = (i & 1) > 0;
                bool bottom = (i & 2) > 0;

                int realX = (right ? x1 - 15 : x2);
                int realY = (bottom ? y1 - 15 : y2);
                DrawTexture(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, realX, realY, Transparency, right, bottom);
            }
        }
        #endregion

        #region DrawHitbox
        public static void DrawHitbox(DevicePanel Graphics, Methods.Drawing.ObjectDrawing.EditorAnimation Animation, string HitboxName, System.Drawing.Color color, int AnimID, int FrameID, int x, int y, int Transparency, bool FlipH = false, bool FlipV = false, int rotation = 0)
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
        #endregion

    }
}
