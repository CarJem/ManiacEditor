using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Player : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            SceneEntity entity = e.Entity;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            int id = (int)entity.attributesMap["characterID"].ValueEnum;
            if (id > 7)
            {
                entity.attributesMap["characterID"].ValueEnum = 7;
            }

            var Animation = Methods.Entities.EntityDrawing.LoadAnimation(d, "PlayerIcons", 0, id);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency);
        }

        public override string GetObjectName()
        {
            return "Player";
        }
    }
}
