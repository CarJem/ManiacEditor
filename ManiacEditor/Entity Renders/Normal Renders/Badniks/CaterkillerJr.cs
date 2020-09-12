using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class CaterkillerJr : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;

            Classes.Scene.EditorEntity e = Properties.EditorObject;

            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            bool fliph = false;
            bool flipv = false;

            int direction = (int)e.attributesMap["direction"].ValueUInt8;
            if (direction == 1)
            {
                int x = Properties.DrawX + 37;
                fliph = true;
                var Animation = LoadAnimation(GetSetupAnimation(), d, 0, 0);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, !fliph, flipv);

                Animation = LoadAnimation(GetSetupAnimation(), d, 1, 0);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - 12, y, Transparency, fliph, flipv);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - 24, y, Transparency, fliph, flipv);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - 36, y, Transparency, fliph, flipv);
                Animation = LoadAnimation(GetSetupAnimation(), d, 2, 0);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - 47, y, Transparency, fliph, flipv);
                Animation = LoadAnimation(GetSetupAnimation(), d, 3, 0);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - 55, y, Transparency, fliph, flipv);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - 62, y, Transparency, fliph, flipv);
            }
            else
            {
                int x = Properties.DrawX;
                var Animation = LoadAnimation(GetSetupAnimation(), d, 0, 0);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, !fliph, flipv);

                Animation = LoadAnimation(GetSetupAnimation(), d, 1, 0);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + 12, y, Transparency, fliph, flipv);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + 24, y, Transparency, fliph, flipv);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + 36, y, Transparency, fliph, flipv);
                Animation = LoadAnimation(GetSetupAnimation(), d, 2, 0);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + 47, y, Transparency, fliph, flipv);
                Animation = LoadAnimation(GetSetupAnimation(), d, 3, 0);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + 55, y, Transparency, fliph, flipv);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + 62, y, Transparency, fliph, flipv);
            }

        }
        public override string GetSetupAnimation()
        {
            string BinName = "/CaterkillerJr.bin";
            if (Methods.Solution.CurrentSolution.IZ_Stage != null && Methods.Solution.CurrentSolution.IZ_Stage.Unlocks != null)
            {
                var unlocks = Methods.Solution.CurrentSolution.IZ_Stage.Unlocks;

                if (unlocks.Contains("AIZ_CaterkillerJr")) return "AIZ" + BinName;
                else if (unlocks.Contains("CPZ_CaterkillerJr")) return "CPZ" + BinName;
            }

            string SetupType = Methods.Solution.CurrentSolution.Entities.SetupObject.Replace("Setup", "");
            if (SetupType == "AIZ") return "AIZ" + BinName;
            else return "CPZ" + BinName;
        }

        public override string GetObjectName()
        {
            return "CaterkillerJr";
        }
    }
}
