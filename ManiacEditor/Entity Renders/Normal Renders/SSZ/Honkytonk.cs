using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Honkytonk : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            int angle = (int)entity.attributesMap["angle"].ValueEnum;
            int rotation = (int)(angle / -0.71);


            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation("HonkyTonk", d.DevicePanel, 0, 1, fliph, flipv, true, rotation, true, false, EditorEntityDrawing.Flag.PartialEngineRotation, true);


            if (editorAnim != null && editorAnim.Frames.Count != 0)
			{
				var frame = editorAnim.Frames[0];

				d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
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
