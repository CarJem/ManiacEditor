using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ClearScript.V8;
using RSDKv5;
using SystemColors = System.Drawing.Color;

namespace ManiacEditor
{
	public class ClearScript
	{
		public int x1;
		public int y1;
		public int Transparency1;
		public bool objectsAdded = false;
		public V8Script script;
		public V8ScriptEngine engine = new V8ScriptEngine();

		public ClearScript()
		{
			//engine.AddHostType("Int32", typeof(int));
			engine.AddHostType("Console", typeof(Console));
			engine.AddHostType("Color", typeof(SystemColors));
			engine.AddHostType("EditorEntity", typeof(EditorEntity));
			engine.AddHostType("SceneEntity", typeof(SceneEntity));
			engine.AddHostType("DevicePanel", typeof(DevicePanel));
			engine.AddHostType("AttributeValidater", typeof(AttributeValidater));
			engine.AddHostType("EditorAnimations", typeof(EditorAnimations));
			engine.AddHostType("RenderingHost", typeof(ClearScript));
		}
		public void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
		{
			x1 = x;
			y1 = y;
			Transparency1 = Transparency;
			SystemColors lineTest = SystemColors.Wheat;


			if (objectsAdded == false)
			{
				engine.AddHostObject("attribMap", attribMap);
				engine.AddHostObject("entity", entity);
				engine.AddHostObject("e", e);
				engine.AddHostObject("d", d);
				engine.AddHostObject("Animation", Animation);
				engine.AddHostObject("host", this);
				engine.AddHostObject("lineTest", lineTest);
				objectsAdded = true;
			}


				if (script == null)
				{
					script = engine.Compile("var type = attribMap.AttributesMapVar(\"type\", entity);" +
								"var moveType = attribMap.AttributesMapVar(\"moveType\", entity);" +
								"var angle = attribMap.AttributesMapInt32(\"angle\", entity);" +
								"var speed = attribMap.AttributesMapUint32(\"speed\", entity);" +
								"var fliph = new Boolean(false);" +
								"var flipv = new Boolean(false);" +
								"var amplitudeX = attribMap.AttributesMapPositionHighX(\"amplitude\", entity);" +
								"var amplitudeY = attribMap.AttributesMapPositionHighY(\"amplitude\", entity);" +
								//"d.DrawLine(0,0,100,100, lineTest);" +
								"host.DrawingTest(d, e);"
								//"var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2(\"Ring\", d, animID, -1, fliph, flipv, false);" +
								//"if (editorAnim != null && editorAnim.Frames.Count != 0 && animID >= 0)" +
								//"{" +
								//"var frame = editorAnim.Frames[Animation.index];" +
								//"d.DrawBitmap(frame.Texture, 0 + frame.Frame.PivotX, 0 + frame.Frame.PivotY, frame.Frame.Width, frame.Frame.Height, false, 255); " +
								//"}" +
								//"" +
								//"" +
								//""
								//"Console.WriteLine(type);" +
								//"Console.WriteLine(moveType);" +
								//"Console.WriteLine(angle);"
								);
				}
				engine.Execute(script);

			
		}

		public void DrawingTest(DevicePanel d, EditorEntity e)
		{
			var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Ring", d, 0, -1, false, false, false);
			if (editorAnim != null && editorAnim.Frames.Count != 0)
			{
				var frame = editorAnim.Frames[0];
				d.DrawBitmap(frame.Texture, x1 + frame.Frame.PivotX, y1 + frame.Frame.PivotY,
					frame.Frame.Width, frame.Frame.Height, false, Transparency1);
			}
		}
	}
}
