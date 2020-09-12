using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Clapperboard : EntityRenderer
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

            int direction = (int)e.attributesMap["direction"].ValueUInt8;
            int flipState = 0;
            if (direction == 1)
            {
                fliph = true;
                flipState = 3;
            }
            else
            {
                fliph = false;
                flipState = 2;
            }

            var Animation = LoadAnimation(GetSetupAnimation(), d, 1, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + (direction == 1 ? -104 : 0), y, Transparency, fliph, flipv);
            Animation = LoadAnimation(GetSetupAnimation(), d, 0, 1);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + (direction == 1 ? -104 : 0), y, Transparency, fliph, flipv);
            Animation = LoadAnimation(GetSetupAnimation(), d, 0, flipState);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, false, false);
        }

        public override string GetSetupAnimation()
        {
            string BinName = "/Clapperboard.bin";
            string UnlockName = "Clapperboard";

            if (Methods.Solution.CurrentSolution.IZ_Stage != null && Methods.Solution.CurrentSolution.IZ_Stage.Unlocks != null)
            {
                var unlocks = Methods.Solution.CurrentSolution.IZ_Stage.Unlocks;

                if (unlocks.Contains("SPZ2_" + UnlockName)) return "SPZ2" + BinName;
                else if (unlocks.Contains("SPZ1_" + UnlockName)) return "SPZ1" + BinName;
            }

            string SetupType = Methods.Solution.CurrentSolution.Entities.SetupObject.Replace("Setup", "");
            if (SetupType == "SPZ2") return "SPZ2" + BinName;
            else return "SPZ1" + BinName;
        }

        public override string GetObjectName()
        {
            return "Clapperboard";
        }
    }
}
