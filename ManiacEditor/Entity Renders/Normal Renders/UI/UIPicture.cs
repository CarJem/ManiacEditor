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

            string binFile = "Icons";
            switch (Methods.Editor.Solution.Entities.SetupObject) {
                case "MenuSetup":
                    binFile = "Picture";
                    break;
                case "ThanksSetup":
                    binFile = "Thanks/Decorations";
                    break;
                case "LogoSetup":
                    binFile = "Logos";
                    break;

            }

            int frameID = (int)e.attributesMap["frameID"].ValueEnum;
            int listID = (int)e.attributesMap["listID"].ValueEnum;

            var Animation = LoadAnimation(binFile, d, listID, frameID);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);

        }

        public override string GetObjectName()
        {
            return "UIPicture";
        }
    }
}
