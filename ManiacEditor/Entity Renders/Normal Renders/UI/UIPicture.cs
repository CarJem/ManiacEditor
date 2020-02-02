using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIPicture : EntityRenderer
    {

        public override void Draw(Classes.Core.Draw.GraphicsHandler d, SceneEntity entity, Classes.Core.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            string binFile = "Icons";
            switch (Classes.Core.Solution.Entities.SetupObject) {
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

            int frameID = (int)entity.attributesMap["frameID"].ValueEnum;
            int listID = (int)entity.attributesMap["listID"].ValueEnum;
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation(binFile, d.DevicePanel, listID, frameID, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                //Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "UIPicture";
        }
    }
}
