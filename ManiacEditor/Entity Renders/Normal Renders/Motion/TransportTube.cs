using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class TransportTube : EntityRenderer
    {

		public override void Draw(Structures.EntityRenderProp Properties)
		{
			DevicePanel d = Properties.Graphics;
			Classes.Scene.EditorEntity entity = Properties.EditorObject;

			int x = Properties.DrawX;
			int y = Properties.DrawY;
			int Transparency = Properties.Transparency;

			bool hideFrame = false;
			int type = (int)entity.attributesMap["type"].ValueUInt8;
			int dirMask = (int)entity.attributesMap["dirMask"].ValueUInt8;

			TubeCombo combo = GetCombos(type, dirMask);

			bool isUnsafe = isDangerousCombonation(dirMask, type);
			if (type > 6) combo.showInvalid = true;

			var editorAnim = LoadAnimation("CPZ/TransportTube.bin", d, 0, 0);
			if (!hideFrame) DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency);

			var upAnim = LoadAnimation("TransportTubes", d, 0, 0);
			if (combo.showUp == true) DrawTexturePivotNormal(d, upAnim, upAnim.RequestedAnimID, upAnim.RequestedFrameID, x, y, Transparency);

			var downAnim = LoadAnimation("TransportTubes", d, 0, 1);
			if (combo.showDown == true) DrawTexturePivotNormal(d, downAnim, downAnim.RequestedAnimID, downAnim.RequestedFrameID, x, y, Transparency);

			var rightAnim = LoadAnimation("TransportTubes", d, 0, 2);
			if (combo.showRight == true) DrawTexturePivotNormal(d, rightAnim, rightAnim.RequestedAnimID, rightAnim.RequestedFrameID, x, y, Transparency);

			var leftAnim = LoadAnimation("TransportTubes", d, 0, 3);
			if (combo.showLeft == true) DrawTexturePivotNormal(d, leftAnim, leftAnim.RequestedAnimID, leftAnim.RequestedFrameID, x, y, Transparency);

			var uprightAnim = LoadAnimation("TransportTubes", d, 0, 6);
			if (combo.showUpRight == true) DrawTexturePivotNormal(d, uprightAnim, uprightAnim.RequestedAnimID, uprightAnim.RequestedFrameID, x, y, Transparency);

			var downrightAnim = LoadAnimation("TransportTubes", d, 0, 7);
			if (combo.showDownRight == true) DrawTexturePivotNormal(d, downrightAnim, downrightAnim.RequestedAnimID, downrightAnim.RequestedFrameID, x, y, Transparency);

			var upleftAnim = LoadAnimation("TransportTubes", d, 0, 4);
			if (combo.showUpLeft == true) DrawTexturePivotNormal(d, upleftAnim, upleftAnim.RequestedAnimID, upleftAnim.RequestedFrameID, x, y, Transparency);

			var downleftAnim = LoadAnimation("TransportTubes", d, 0, 5);
			if (combo.showDownLeft == true) DrawTexturePivotNormal(d, downleftAnim, downleftAnim.RequestedAnimID, downleftAnim.RequestedFrameID, x, y, Transparency);

			var centerAnim = LoadAnimation("TransportTubes", d, 1, 1);
			if (combo.showCenter == true) DrawTexturePivotNormal(d, centerAnim, centerAnim.RequestedAnimID, centerAnim.RequestedFrameID, x, y, Transparency);

			var A_Anim = LoadAnimation("TransportTubes", d, 1, 2);
			if (combo.showA == true) DrawTexturePivotNormal(d, A_Anim, A_Anim.RequestedAnimID, A_Anim.RequestedFrameID, x, y, Transparency);

			var B_Anim = LoadAnimation("TransportTubes", d, 1, 3);
			if (combo.showB == true) DrawTexturePivotNormal(d, B_Anim, B_Anim.RequestedAnimID, B_Anim.RequestedFrameID, x, y, Transparency);

			var C_Anim = LoadAnimation("TransportTubes", d, 1, 4);
			if (combo.showC == true) DrawTexturePivotNormal(d, C_Anim, C_Anim.RequestedAnimID, C_Anim.RequestedFrameID, x, y, Transparency);

			var inOutAnim = LoadAnimation("TransportTubes", d, 1, 5);
			if (combo.showInOut == true) DrawTexturePivotNormal(d, inOutAnim, inOutAnim.RequestedAnimID, inOutAnim.RequestedFrameID, x, y, Transparency);

			var runAnim = LoadAnimation("TransportTubes", d, 1, 6);
			if (combo.showRun == true) DrawTexturePivotNormal(d, runAnim, runAnim.RequestedAnimID, runAnim.RequestedFrameID, x, y, Transparency);

			var junctionAnim = LoadAnimation("TransportTubes", d, 1, 7);
			if (combo.showJunction == true) DrawTexturePivotNormal(d, junctionAnim, junctionAnim.RequestedAnimID, junctionAnim.RequestedFrameID, x, y, Transparency);

			var unsafeAnim = LoadAnimation("TransportTubes", d, 1, 0);
			if (isUnsafe == true) DrawTexturePivotNormal(d, unsafeAnim, unsafeAnim.RequestedAnimID, unsafeAnim.RequestedFrameID, x, y, Transparency);

			var unknownAnim = LoadAnimation("TransportTubes", d, 1, 8);
			if (combo.showUnkown == true) DrawTexturePivotNormal(d, unknownAnim, unknownAnim.RequestedAnimID, unknownAnim.RequestedFrameID, x, y, Transparency);

			var notValidAnim = LoadAnimation("TransportTubes", d, 1, 9);
			if (combo.showInvalid == true) DrawTexturePivotNormal(d, notValidAnim, notValidAnim.RequestedAnimID, notValidAnim.RequestedFrameID, x, y, Transparency);

		}

		public struct TubeCombo
		{
			public bool showUp;
			public bool showDown;
			public bool showLeft;
			public bool showRight;
			public bool showUpLeft;
			public bool showDownLeft;
			public bool showUpRight;
			public bool showDownRight;
			public bool showCenter;
			public bool showA;
			public bool showB;
			public bool showC;
			public bool showInOut;
			public bool showJunction;
			public bool showRun;
			public bool showUnkown;
			public bool showInvalid;
			public bool hideFrame;

			public TubeCombo(bool defaultValue)
			{
				showUp = defaultValue;
				showDown = defaultValue;
				showLeft = defaultValue;
				showRight = defaultValue;
				showUpLeft = defaultValue;
				showDownLeft = defaultValue;
				showUpRight = defaultValue;
				showDownRight = defaultValue;
				showCenter = defaultValue;
				showA = defaultValue;
				showB = defaultValue;
				showC = defaultValue;
				showInOut = defaultValue;
				showJunction = defaultValue;
				showRun = defaultValue;
				showUnkown = defaultValue;
				showInvalid = defaultValue;
				hideFrame = defaultValue;
			}

		}
		public TubeCombo GetCombos(int type, int dirMask)
		{
			TubeCombo comboSet = new TubeCombo(false);
			/* Types:
			 * 0 - Normal 
			 * 1 - Entry Tubes
			 * 2, 3, 4 - Path Tubes
			 * 5 - Directional Tubes
			 * 6 - "Run" Tubes (Keep Momentum)
			 */
			if (type == 5 || type == 1 || type == 6 || type == 0)
			{
				if (type == 5)
				{
					comboSet.showCenter = true;
					comboSet.showJunction = true;
				}
				else if (type == 1)
				{
					comboSet.showInOut = true;
				}
				else if (type == 6)
				{
					comboSet.showRun = true;
				}
				switch (dirMask)
				{
					case 136:
						comboSet.showRight = true;
						comboSet.showDownLeft = true;
						break;
					case 129:
						comboSet.showUp = true;
						comboSet.showDownLeft = true;
						break;
					case 68:
						comboSet.showLeft = true;
						comboSet.showDownRight = true;
						break;
					case 65:
						comboSet.showUp = true;
						comboSet.showDownRight = true;
						break;
					case 40:
						comboSet.showUpLeft = true;
						comboSet.showRight = true;
						break;
					case 20:
						comboSet.showLeft = true;
						comboSet.showUpRight = true;
						break;
					case 18:
						comboSet.showDown = true;
						comboSet.showUpRight = true;
						break;
					case 15:
						comboSet.showDown = true;
						comboSet.showLeft = true;
						comboSet.showRight = true;
						comboSet.showUp = true;
						break;
					case 14:
						comboSet.showDown = true;
						comboSet.showLeft = true;
						comboSet.showRight = true;
						break;
					case 13:
						comboSet.showUp = true;
						comboSet.showDown = false;
						comboSet.showLeft = true;
						comboSet.showRight = true;
						break;
					case 12:
						comboSet.showUp = false;
						comboSet.showDown = false;
						comboSet.showLeft = true;
						comboSet.showRight = true;
						break;
					case 11:
						comboSet.showUp = true;
						comboSet.showDown = true;
						comboSet.showLeft = false;
						comboSet.showRight = true;
						break;
					case 10:
						comboSet.showUp = false;
						comboSet.showDown = true;
						comboSet.showLeft = false;
						comboSet.showRight = true;
						break;
					case 9:
						comboSet.showUp = true;
						comboSet.showDown = false;
						comboSet.showLeft = false;
						comboSet.showRight = true;
						break;
					case 8:
						comboSet.showUp = false;
						comboSet.showDown = false;
						comboSet.showLeft = false;
						comboSet.showRight = true;
						break;
					case 7:
						comboSet.showUp = true;
						comboSet.showDown = true;
						comboSet.showLeft = true;
						comboSet.showRight = false;
						break;
					case 6:
						comboSet.showUp = false;
						comboSet.showDown = true;
						comboSet.showLeft = true;
						comboSet.showRight = false;
						break;
					case 5:
						comboSet.showUp = true;
						comboSet.showDown = false;
						comboSet.showLeft = true;
						comboSet.showRight = false;
						break;
					case 4:
						comboSet.showUp = false;
						comboSet.showDown = false;
						comboSet.showLeft = true;
						comboSet.showRight = false;
						break;
					case 3:
						comboSet.showUp = true;
						comboSet.showDown = true;
						comboSet.showLeft = false;
						comboSet.showRight = false;
						break;
					case 2:
						comboSet.showUp = false;
						comboSet.showDown = true;
						comboSet.showLeft = false;
						comboSet.showRight = false;
						break;
					case 1:
						comboSet.showUp = true;
						comboSet.showDown = false;
						comboSet.showLeft = false;
						comboSet.showRight = false;
						break;
					case 0:
						comboSet.showInvalid = true;
						break;
					default:
						comboSet.showUnkown = true;
						break;
				}
			}
			if (type == 2 || type == 3 || type == 4)
			{
				switch (type)
				{
					case 2:
						comboSet.showA = true;
						break;
					case 3:
						comboSet.showB = true;
						break;
					case 4:
						comboSet.showC = true;
						break;
				}
				comboSet.hideFrame = true;
			}

			return comboSet;
		}
		public bool isDangerousCombonation(int dirMask, int type)
		{
			/* Types:
			* 0 - Normal 
			* 1 - Entry Tubes
			* 2, 3, 4 - Path Tubes
			* 5 - Directional Tubes
			* 6 - "Run" Tubes (Keep Momentum)
			*/
			if (type == 0 || type == 1)
			{
				switch (dirMask)
				{
					case 0:
						return true;
					case 1:
						return true;
					case 2:
						return true;
					case 4:
						return true;
					case 8:
						return true;
				}
			}
			if (type == 5 && dirMask > 15) return true;
			if (type > 6) return true;
			return false;
		}
 
		public override string GetObjectName()
        {
            return "TransportTube";
        }
    }
}
