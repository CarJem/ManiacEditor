using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor.Editor_Classes
{
	
	public class EditorRenderScript
	{
		Dictionary<string, bool> Boolean = new Dictionary<string, bool>();
		Dictionary<string, int> Intergers = new Dictionary<string, int>();
		Dictionary<string, byte> Bytes = new Dictionary<string, byte>();
		Dictionary<string, double> Doubles = new Dictionary<string, double>();
		Dictionary<string, EditorEntity_ini.EditorAnimation> Animations = new Dictionary<string, EditorEntity_ini.EditorAnimation>();
		Dictionary<string, EditorEntity_ini.EditorAnimation.EditorFrame> Frames = new Dictionary<string, EditorEntity_ini.EditorAnimation.EditorFrame>();

		RSDKv5.SceneEntity RenderObject;

		AttributeValidater attributeValidater = new AttributeValidater();

		string code = "";
		//This is the goal I am setting for this language
		string testScript =
			@"byte(type) = attributesMap(""type"",uint8);
			int(width) = attributesMap(""size"",Pos.X.High);
			int(height) = attributesMap(""size"",Pos.Y.High);
			var(editorAnim) = LoadAnim2(""EditorAssets"",0,1,false,false,false);
			if (width != -1 && height != -1) {
				bool(wEven) = width % 2 == 0;
				bool(wEven) = width % 2 == 0;

                int(x1) = (x + (wEven ? -8 : -16) + (-width / 2 + width) * 16) + 15;
                int(x2) = (x + (wEven ? -8 : -16) + (-width / 2) * 16);
                int(y1) = (y + (hEven ? -8 : -16) + (-height / 2 + height) * 16) + 15;
                int(y2) = (y + (hEven ? -8 : -16) + (-height / 2) * 16);

                DrawLine(x1, y1, x1, y2, SystemColors.White);
                DrawLine(x1, y1, x2, y1, SystemColors.White);
                DrawLine(x2, y2, x1, y2, SystemColors.White);
                DrawLine(x2, y2, x2, y1, SystemColors.White);

				for (int i = 0; i < 4; i++)
                {
                    bool(right) = (i & 1) > 0;
                    bool(bottom) = (i & 2) > 0;

                    editorAnim = LoadAnim2(""EditorAssets"",0, 1, right, bottom, false);
                    if (ValidFrame(editorAnim))
                    {
                        var(frame) = GetFrame(editorAnim,Animation.index);
						ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
						DrawBitmap(frame.Texture, 
						(x + (wEven? frame.Frame.PivotX : -frame.Frame.Width) + (-width / 2 + (right? width : 0)) * frame.Frame.Width),
						(y + (hEven? frame.Frame.PivotY : -frame.Frame.Height) + (-height / 2 + (bottom? height : 0)) * frame.Frame.Height),
						frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
				}

				bool(knux) = attributesMap(""onlyKnux"",uint8);
				bool(mighty) = attributesMap(""onlyMighty"",uint8);

				// draw Knuckles icon
				if (knux)
				{
					editorAnim = LoadAnim2(""HUD"", 2, 2, false, false, false);
					if (ValidFrame(editorAnim))
					{
						var(frame) = GetFrame(editorAnim,Animation.index);
						ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
						DrawBitmap(frame.Texture, x - frame.Frame.Width / (mighty? 1 : 2), y - frame.Frame.Height / 2, frame.Frame.Width, frame.Frame.Height, false, Transparency);
					}

				}
				// draw Mighty icon
				if (mighty)
				{
					editorAnim = LoadAnim2(""HUD"", 2, 3, false, false, false);
					if (ValidFrame(editorAnim))
					{
						var(frame) = GetFrame(editorAnim,Animation.index);
						ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
						DrawBitmap(frame.Texture, x - (knux? 0 : frame.Frame.Width / 2), y - frame.Frame.Height / 2, frame.Frame.Width, frame.Frame.Height, false, Transparency);
					}
				}";



		public EditorRenderScript(string code, RSDKv5.SceneEntity entity, bool isPath = false)
		{
			RenderObject = entity;
		}

		public void Draw()
		{
			IList<string> CodeListed = code.Split(new[] { '\r', '\n' });
			foreach (string section in CodeListed)
			{
				Praser(section);
			}
		}

		public void Praser(string code)
		{
			if (code.StartsWith("int")) IntPraser(code);
			else if (code.StartsWith("var")) VarPraser(code);
		}

		
		public int IntReturnPraser(string code)
		{
			if (code.StartsWith("attributesMap")) return (int)AttributesMapPraser();
			else return 0;

		}

		public object VarReturnPraser(string code)
		{
			if (code.StartsWith("LoadAnim2")) return AnimationPraser();
			else return null;
		}

		public object AnimationPraser()
		{
			return null;
		}

		public object AttributesMapPraser()
		{
			string contents = code.Split('(', ')')[1];
			string[] attributeInformation = contents.Split(',');
			int? result = (int)AttributesMap(attributeInformation);
			if (result == null) return 0;
			else return result.Value;
		}

		public object AttributesMap(String[] attributeInformation)
		{
			object result;
			switch (attributeInformation[1])
			{
				case "uint8":
					result = attributeValidater.AttributesMapUint8(attributeInformation[0], RenderObject);
					break;
				case "uint16":
					result = attributeValidater.AttributesMapUint16(attributeInformation[0], RenderObject);
					break;
				case "uint32":
					result = attributeValidater.AttributesMapUint32(attributeInformation[0], RenderObject);
					break;
				case "int8":
					result = attributeValidater.AttributesMapInt8(attributeInformation[0], RenderObject);
					break;
				case "int16":
					result = attributeValidater.AttributesMapInt16(attributeInformation[0], RenderObject);
					break;
				case "int32":
					result = attributeValidater.AttributesMapInt32(attributeInformation[0], RenderObject);
					break;
				case "var":
					result = attributeValidater.AttributesMapVar(attributeInformation[0], RenderObject);
					break;
				case "bool":
					result = attributeValidater.AttributesMapBool(attributeInformation[0], RenderObject);
					break;
				case "string":
					result = attributeValidater.AttributesMapString(attributeInformation[0], RenderObject);
					break;
				case "color":
					result = attributeValidater.AttributesMapColor(attributeInformation[0], RenderObject);
					break;
				case "Pos.High.X":
					result = attributeValidater.AttributesMapPositionHighX(attributeInformation[0], RenderObject);
					break;
				case "Pos.High.Y":
					result = attributeValidater.AttributesMapPositionHighY(attributeInformation[0], RenderObject);
					break;
				case "Pos.Low.X":
					result = attributeValidater.AttributesMapPositionLowX(attributeInformation[0], RenderObject);
					break;
				case "Pos.Low.Y":
					result = attributeValidater.AttributesMapPositionLowY(attributeInformation[0], RenderObject);
					break;
				default:
					result = null;
					break;
			}
			return result;
		}

		public string GetVariableName(string variable)
		{
			string result = variable.Split('(', ')')[1];
			return result;
		}

		public void IntPraser(string code)
		{
			string variable = code.Split('=').First();
			string equation = code.Split('=').ElementAt(1);

			string name = GetVariableName(variable);
			int result = IntReturnPraser(equation);
			Intergers.Add(name, result);
		}

		public void VarPraser(string code)
		{
			string variable = code.Split('=').First();
			string equation = code.Split('=').ElementAt(1);

			string name = GetVariableName(variable);
			//int result = VarReturnPraser(equation);
			//Intergers.Add(name, result);
		}

		public void DisposeResources()
		{

		}

		public void DrawFrame()
		{

		}

		public void LoadFrame()
		{

		}

	}
}
