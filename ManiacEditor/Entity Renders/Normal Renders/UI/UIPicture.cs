using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIPicture : EntityRenderer
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

            string binFile = GetSetupAnimation();

            int frameID = (int)e.attributesMap["frameID"].ValueEnum;
            int listID = (int)e.attributesMap["listID"].ValueEnum;

            var Animation = LoadAnimation(binFile, d, listID, frameID);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);

        }

        public override string GetSetupAnimation()
        {
            string binFile = "LSelect/Icons.bin";
            switch (Methods.Solution.CurrentSolution.Entities.SetupObject)
            {
                case "MenuSetup":
                    binFile = "UI/Picture.bin";
                    break;
                case "ThanksSetup":
                    binFile = "Thanks/Decorations.bin";
                    break;
                case "LogoSetup":
                    binFile = "Logos/Icons.bin";
                    break;
                default:
                    binFile = "LSelect/Icons.bin";
                    break;

            }


            if (Methods.Solution.CurrentSolution.IZ_Stage != null && Methods.Solution.CurrentSolution.IZ_Stage.Unlocks != null)
            {
                if (Methods.Solution.CurrentSolution.IZ_Stage.Unlocks.Contains("Summary_UIPicture")) binFile = "LSelect/Icons.bin";
            }
            return binFile;
        }

        public override string GetObjectName()
        {
            return "UIPicture";
        }
    }
}
