using System.IO;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class ParallaxSprite : EntityRenderer
    {
        private string ParallaxSpritePath { get; set; } = "";
        public override void Draw(Structures.EntityRenderProp properties)
        {
            DevicePanel d = properties.Graphics;
            Classes.Scene.EditorEntity entity = properties.EditorObject; 
            int x = properties.DrawX;
            int y = properties.DrawY;
            int Transparency = properties.Transparency;

            bool fliph = false;
            bool flipv = false;
            int aniID = (int)entity.attributesMap["aniID"].ValueUInt8;
            int attribute = (int)entity.attributesMap["attribute"].ValueUInt8;
            RSDKv5.Position parallaxFactor = entity.attributesMap["parallaxFactor"].ValueVector2;
            RSDKv5.Position loopPoint = entity.attributesMap["loopPoint"].ValueVector2;

            if (ParallaxSpritePath == "") ParallaxSpritePath = GetParallaxSpritePath();
            
            if (Methods.Solution.SolutionState.Main.ShowParallaxSprites)
            {
                var editorAnim = LoadAnimation("EditorIcons2", d, 0, 12);
                DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency);
            }
            else
            {
                var editorAnim = LoadAnimation(ParallaxSpritePath, d, aniID, 0);
                DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency);
            }


        }

        public string GetParallaxSpritePath()
        {
            return GetParallaxSpritePath(new string[] { "SPZ", "LRZ", "MSZ", "OOZ", "CPZ", "FBZ" });
        }

        public override string GetObjectName()
        {
            return "ParallaxSprite";
        }
    }
}
