using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ManiacEditor;
using Microsoft.Xna.Framework;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIButtonLabel : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            string text = "Text" + Editor.Instance.UIModes.CurrentLanguage;
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int frameID = (int)entity.attributesMap["frameID"].ValueVar;
            int listID = (int)entity.attributesMap["listID"].ValueVar;
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation(text, d.DevicePanel, listID, frameID, false, false, false);
            var editorAnimType = Editor.Instance.EntityDrawing.LoadAnimation("ButtonLabel", d.DevicePanel, 0, type, false, false, false);
            if (editorAnimType != null && editorAnimType.Frames.Count != 0)
            {
                var frame = editorAnimType.Frames[Animation.index];
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }



        }

        public override string GetObjectName()
        {
            return "UIButtonLabel";
        }
    }
}
