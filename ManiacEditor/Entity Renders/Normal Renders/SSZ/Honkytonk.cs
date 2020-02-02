using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Honkytonk : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            Classes.Editor.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Editor.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            Methods.Entities.EntityAnimator Animation = properties.Animations;
            bool selected  = properties.isSelected;
            bool fliph = false;
            bool flipv = false;
            int angle = (int)entity.attributesMap["angle"].ValueEnum;
            int rotation = (int)(angle / -0.71);


            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation("HonkyTonk", d.DevicePanel, 0, 1, fliph, flipv, true, rotation, true, false, Methods.Entities.EntityDrawing.Flag.PartialEngineRotation, true);


            if (editorAnim != null && editorAnim.Frames.Count != 0)
			{
				var frame = editorAnim.Frames[0];

				d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame),
					x - (int)(frame.ImageWidth / 2),
					y - (int)(frame.ImageHeight / 2),
					frame.ImageWidth, frame.ImageHeight, false, Transparency);
			}


        }

        public override string GetObjectName()
        {
            return "Honkytonk";
        }
    }
}
