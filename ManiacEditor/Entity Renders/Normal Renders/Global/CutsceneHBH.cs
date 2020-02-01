using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class CutsceneHBH : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Edit.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            int characterID = (int)entity.attributesMap["characterID"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            string sprite = "";
            int animID = 0;
            bool multiFrame = false;
            switch (characterID)
            {
                case 0:
                    sprite = "Gunner";
                    animID = 0;
                    break;
                case 1:
                    sprite = "Shinobi";
                    animID = 0;
                    break;
                case 2:
                    sprite = "Mystic";
                    animID = 0;
                    break;
                case 3:
                    sprite = "Rider";
                    animID = 0;
                    break;
                case 4:
                    sprite = "King";
                    animID = 0;
                    multiFrame = true;
                    break;
                case 5:
                    sprite = "Rogues";
                    animID = 0;
                    break;
                case 6:
                    sprite = "Rogues";
                    animID = 5;
                    break;
                case 7:
                    sprite = "Rogues";
                    animID = 12;
                    break;
                case 8:
                    sprite = "DamagedKing";
                    animID = 0;
                    multiFrame = true;
                    break;
                case 9:
                    sprite = "HBHPile";
                    animID = 0;
                    break;
                default:
                    sprite = "Gunner";
                    animID = 0;
                    break;
            }
            if (direction == 1)
            {
                fliph = true;
            }
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2(sprite, d.DevicePanel, animID, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (multiFrame)
            {
                string sprite2 = "";
                int animID2 = 0;
                switch (characterID)
                {
                    case 4:
                        sprite2 = "King";
                        animID2 = 14;
                        break;
                    case 8:
                        sprite = "DamagedKing";
                        animID2 = 1;
                        break;
                }
                var editorAnim2 = Editor.Instance.EntityDrawing.LoadAnimation2(sprite2, d.DevicePanel, animID2, -1, fliph, flipv, false);
                if (editorAnim2 != null && editorAnim2.Frames.Count != 0)
                {
                    var frame = editorAnim2.Frames[Animation.index];

                    Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                        x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim2.Frames[0].Frame.Width) : 0),
                        y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim2.Frames[0].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
            }

        }

        public override string GetObjectName()
        {
            return "CutsceneHBH";
        }
    }
}
