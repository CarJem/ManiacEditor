using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class SignPost : EntityRenderer
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

            var Animation = LoadAnimation("SignPost", d, 0, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency);

            if (Methods.Runtime.GameHandler.SelectedGameVersion == "1.3")
            {
                Animation = LoadAnimation("SignPost", d, 4, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency);
            }
            else
            {
                Animation = LoadAnimation("SignPost", d, 6, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency);
            }

        }

        public override string GetObjectName()
        {
            return "SignPost";
        }
    }
}
