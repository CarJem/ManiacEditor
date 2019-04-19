using System;
using Microsoft.ClearScript.V8;
using RSDKv5;
using SystemColors = System.Drawing.Color;

namespace ManiacEditor
{
    public class ClearScript
	{
		public bool objectsAdded = false;
		public V8Script script;
		public V8ScriptEngine engine = new V8ScriptEngine();

        int script_x = 0;
        int script_y = 0;
        int script_Transparency = 255;


        public ClearScript()
		{
			//engine.AddHostType("int", typeof(int));
			engine.AddHostType("Console", typeof(Console));
			engine.AddHostType("Color", typeof(SystemColors));
			engine.AddHostType("EditorEntity", typeof(EditorEntity));
			engine.AddHostType("SceneEntity", typeof(SceneEntity));
			engine.AddHostType("DevicePanel", typeof(DevicePanel));
			engine.AddHostType("AttributeValidater", typeof(AttributeValidater));
			engine.AddHostType("EditorAnimations", typeof(EditorAnimations));
		}
		public void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
		{
            script_x = x;
            script_y = y;


            if (objectsAdded == false)
			{
				engine.AddHostObject("attribMap", attribMap);
				engine.AddHostObject("entity", entity);
				engine.AddHostObject("e", e);
				engine.AddHostObject("d", d);
				engine.AddHostObject("Animation", Animation);
				engine.AddHostObject("host", this);
                objectsAdded = true;
			}


			if (script == null)
			{
				script = engine.Compile("var x = host.GetX();" +
                                        "var y = host.GetY();" +
                                        "var type = attribMap.AttributesMapVar(\"type\", entity);" +
						                "var moveType = attribMap.AttributesMapVar(\"moveType\", entity);" +
						                "var angle = attribMap.AttributesMapInt32(\"angle\", entity);" +
						                "var speed = attribMap.AttributesMapUint32(\"speed\", entity);" +
						                "var fliph = new Boolean(false);" +
						                "var flipv = new Boolean(false);" +
						                "var amplitudeX = attribMap.AttributesMapPositionHighX(\"amplitude\", entity);" +
						                "var amplitudeY = attribMap.AttributesMapPositionHighY(\"amplitude\", entity);" +
						                "var editorAnim = EditorInstancEditorEntity_ini.LoadAnimation2(\"Ring\", d.DevicePanel, 0, -1, false, false, false);" +
						                "if (editorAnim != null && editorAnim.Frames.Count != 0)" +
						                "{" +
						                "var frame = editorAnim.Frames[0];" +
						                "d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), 0 + frame.Frame.PivotX, 0 + frame.Frame.PivotY, frame.Frame.Width, frame.Frame.Height, false, 255); " +
						                "}"
						                );
			}
            engine.Execute(script);

        }


        public int GetX()
        {
            return script_x;
        }

        public int GetY()
        {
            return script_y;
        }

        public int GetTransparency()
        {
            return script_Transparency;
        }
    }
}
