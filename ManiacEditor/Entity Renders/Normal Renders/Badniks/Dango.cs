using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Dango : EntityRenderer
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

            var Animation = LoadAnimation(GetSetupAnimation(), d, 1, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetSetupAnimation()
        {
            string BinName = "/Dango.bin";
            string UnlockName = "Dango";

            if (Methods.Solution.CurrentSolution.IZ_Stage != null && Methods.Solution.CurrentSolution.IZ_Stage.Unlocks != null)
            {
                var unlocks = Methods.Solution.CurrentSolution.IZ_Stage.Unlocks;

                if (unlocks.Contains("SSZ2_" + UnlockName)) return "SSZ2" + BinName;
                else if (unlocks.Contains("SSZ1_" + UnlockName)) return "SSZ1" + BinName;
                else if (unlocks.Contains("SSZ_" + UnlockName)) return "SSZ" + BinName;
            }

            string SetupType = Methods.Solution.CurrentSolution.Entities.SetupObject.Replace("Setup", "");
            if (SetupType == "SSZ2") return "SPZ2" + BinName;
            else if (SetupType == "SSZ1") return "SPZ2" + BinName;
            else return "SSZ" + BinName;
        }

        public override string GetObjectName()
        {
            return "Dango";
        }
    }
}
