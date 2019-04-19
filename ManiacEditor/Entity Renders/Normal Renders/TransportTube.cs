using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class TransportTube : EntityRenderer
    {

		public override void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
		{
			bool hideFrame = false;
			int type = (int)entity.attributesMap["type"].ValueUInt8;
			int dirMask = (int)entity.attributesMap["dirMask"].ValueUInt8;
			var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("TransportTube", d.DevicePanel, 0, 0, false, false, false);
			var upAnim = Editor.Instance.EntityDrawing.LoadAnimation2("TransportTubes", d.DevicePanel, 0, 0, false, false, false);
			var downAnim = Editor.Instance.EntityDrawing.LoadAnimation2("TransportTubes", d.DevicePanel, 0, 1, false, false, false);
			var rightAnim = Editor.Instance.EntityDrawing.LoadAnimation2("TransportTubes", d.DevicePanel, 0, 2, false, false, false);
			var leftAnim = Editor.Instance.EntityDrawing.LoadAnimation2("TransportTubes", d.DevicePanel, 0, 3, false, false, false);
			var upleftAnim = Editor.Instance.EntityDrawing.LoadAnimation2("TransportTubes", d.DevicePanel, 0, 4, false, false, false);
			var downleftAnim = Editor.Instance.EntityDrawing.LoadAnimation2("TransportTubes", d.DevicePanel, 0, 5, false, false, false);
			var uprightAnim = Editor.Instance.EntityDrawing.LoadAnimation2("TransportTubes", d.DevicePanel, 0, 6, false, false, false);
			var downrightAnim = Editor.Instance.EntityDrawing.LoadAnimation2("TransportTubes", d.DevicePanel, 0, 7, false, false, false);
			var centerAnim = Editor.Instance.EntityDrawing.LoadAnimation2("TransportTubes", d.DevicePanel, 1, 1, false, false, false);
			var A_Anim = Editor.Instance.EntityDrawing.LoadAnimation2("TransportTubes", d.DevicePanel, 1, 2, false, false, false);
			var B_Anim = Editor.Instance.EntityDrawing.LoadAnimation2("TransportTubes", d.DevicePanel, 1, 3, false, false, false);
			var C_Anim = Editor.Instance.EntityDrawing.LoadAnimation2("TransportTubes", d.DevicePanel, 1, 4, false, false, false);
			var inOutAnim = Editor.Instance.EntityDrawing.LoadAnimation2("TransportTubes", d.DevicePanel, 1, 5, false, false, false);
			var junctionAnim = Editor.Instance.EntityDrawing.LoadAnimation2("TransportTubes", d.DevicePanel, 1, 7, false, false, false);
			var runAnim = Editor.Instance.EntityDrawing.LoadAnimation2("TransportTubes", d.DevicePanel, 1, 6, false, false, false);
			var unknownAnim = Editor.Instance.EntityDrawing.LoadAnimation2("TransportTubes", d.DevicePanel, 1, 8, false, false, false);
			var unsafeAnim = Editor.Instance.EntityDrawing.LoadAnimation2("TransportTubes", d.DevicePanel, 1, 0, false, false, false);
			var notValidAnim = Editor.Instance.EntityDrawing.LoadAnimation2("TransportTubes", d.DevicePanel, 1, 9, false, false, false);

			bool showUp = false, showDown = false, showLeft = false, showRight = false, showUpLeft = false, showDownLeft = false, showUpRight = false, showDownRight = false, showCenter = false, showA = false, showB = false, showC = false, showInOut = false, showJunction = false, showRun = false, showUnkown = false, showInvalid = false;
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
					showCenter = true;
					showJunction = true;
				}
				else if (type == 1)
				{
					showInOut = true;
				}
				else if (type == 6)
				{
					showRun = true;
				}
				switch (dirMask)
				{
					case 136:
						showRight = true;
						showDownLeft = true;
						break;
					case 129:
						showUp = true;
						showDownLeft = true;
						break;
					case 68:
						showLeft = true;
						showDownRight = true;
						break;
					case 65:
						showUp = true;
						showDownRight = true;
						break;
					case 40:
						showUpLeft = true;
						showRight = true;
						break;
					case 20:
						showLeft = true;
						showUpRight = true;
						break;
					case 18:
						showDown = true;
						showUpRight = true;
						break;
					case 15:
						showDown = true;
						showLeft = true;
						showRight = true;
						showUp = true;
						break;
					case 14:
						showDown = true;
						showLeft = true;
						showRight = true;
						break;
					case 13:
						showUp = true;
						showDown = false;
						showLeft = true;
						showRight = true;
						break;
					case 12:
						showUp = false;
						showDown = false;
						showLeft = true;
						showRight = true;
						break;
					case 11:
						showUp = true;
						showDown = true;
						showLeft = false;
						showRight = true;
						break;
					case 10:
						showUp = false;
						showDown = true;
						showLeft = false;
						showRight = true;
						break;
					case 9:
						showUp = true;
						showDown = false;
						showLeft = false;
						showRight = true;
						break;
					case 8:
						showUp = false;
						showDown = false;
						showLeft = false;
						showRight = true;
						break;
					case 7:
						showUp = true;
						showDown = true;
						showLeft = true;
						showRight = false;
						break;
					case 6:
						showUp = false;
						showDown = true;
						showLeft = true;
						showRight = false;
						break;
					case 5:
						showUp = true;
						showDown = false;
						showLeft = true;
						showRight = false;
						break;
					case 4:
						showUp = false;
						showDown = false;
						showLeft = true;
						showRight = false;
						break;
					case 3:
						showUp = true;
						showDown = true;
						showLeft = false;
						showRight = false;
						break;
					case 2:
						showUp = false;
						showDown = true;
						showLeft = false;
						showRight = false;
						break;
					case 1:
						showUp = true;
						showDown = false;
						showLeft = false;
						showRight = false;
						break;
					case 0:
						showInvalid = true;
						break;
					default:
						showUnkown = true;
						break;
				}
			}
			if (type == 2 || type == 3 || type == 4)
			{
				e.drawSelectionBoxInFront = false;
				switch (type)
				{
					case 2:
						showA = true;
						break;
					case 3:
						showB = true;
						break;
					case 4:
						showC = true;
						break;
				}
				hideFrame = true;
			}
			else
			{
				e.drawSelectionBoxInFront = true;
			}

			bool isUnsafe = isDangerousCombonation(dirMask, type);
			if (type > 6)
			{
				showInvalid = true;
			}

			if (editorAnim != null && editorAnim.Frames.Count != 0 && upAnim != null && upAnim.Frames.Count != 0 && downAnim != null && downAnim.Frames.Count != 0 && rightAnim != null && rightAnim.Frames.Count != 0 && leftAnim != null && leftAnim.Frames.Count != 0 && uprightAnim != null && uprightAnim.Frames.Count != 0 && downrightAnim != null && downrightAnim.Frames.Count != 0 && upleftAnim != null && upleftAnim.Frames.Count != 0 && downleftAnim != null && downleftAnim.Frames.Count != 0)
				{
					var frame = editorAnim.Frames[Animation.index];
					var frame2 = upAnim.Frames[Animation.index];
					var frame3 = downAnim.Frames[Animation.index];
					var frame4 = rightAnim.Frames[Animation.index];
					var frame5 = leftAnim.Frames[Animation.index];
					var frame6 = uprightAnim.Frames[Animation.index];
					var frame7 = downrightAnim.Frames[Animation.index];
					var frame8 = upleftAnim.Frames[Animation.index];
					var frame9 = downleftAnim.Frames[Animation.index];

					if (!hideFrame)
					{
						d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
							x + frame.Frame.PivotX,
							y + frame.Frame.PivotY,
							frame.Frame.Width, frame.Frame.Height, false, Transparency);
					}

					if (showUp == true)
					{
						d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame2),
							x + frame2.Frame.PivotX,
							y + frame2.Frame.PivotY,
							frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
					}
					if (showDown == true)
					{
						d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame3),
							x + frame3.Frame.PivotX,
							y + frame3.Frame.PivotY,
							frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
					}
					if (showRight == true)
					{
						d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame4),
							x + frame4.Frame.PivotX,
							y + frame4.Frame.PivotY,
							frame4.Frame.Width, frame4.Frame.Height, false, Transparency);
					}
					if (showLeft == true)
					{
						d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame5),
							x + frame5.Frame.PivotX,
							y + frame5.Frame.PivotY,
							frame5.Frame.Width, frame5.Frame.Height, false, Transparency);
					}
					if (showUpRight == true)
					{
						d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame6),
							x + frame6.Frame.PivotX,
							y + frame6.Frame.PivotY,
							frame6.Frame.Width, frame6.Frame.Height, false, Transparency);
					}
					if (showDownRight == true)
					{
						d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame7),
							x + frame7.Frame.PivotX,
							y + frame7.Frame.PivotY,
							frame7.Frame.Width, frame7.Frame.Height, false, Transparency);
					}
					if (showUpLeft == true)
					{
						d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame8),
							x + frame8.Frame.PivotX,
							y + frame8.Frame.PivotY,
							frame8.Frame.Width, frame8.Frame.Height, false, Transparency);
					}
					if (showDownLeft == true)
					{
						d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame9),
							x + frame9.Frame.PivotX,
							y + frame9.Frame.PivotY,
							frame9.Frame.Width, frame9.Frame.Height, false, Transparency);
					}
				}

			if (centerAnim != null && centerAnim.Frames.Count != 0 && A_Anim != null && A_Anim.Frames.Count != 0 && B_Anim != null && B_Anim.Frames.Count != 0 && C_Anim != null && C_Anim.Frames.Count != 0 && inOutAnim != null && inOutAnim.Frames.Count != 0 && junctionAnim != null && junctionAnim.Frames.Count != 0 && runAnim != null && runAnim.Frames.Count != 0 && unknownAnim != null && unknownAnim.Frames.Count != 0 && unsafeAnim != null && unsafeAnim.Frames.Count != 0 && notValidAnim != null && notValidAnim.Frames.Count != 0)
			{
				var frame = centerAnim.Frames[Animation.index];
				var frame2 = A_Anim.Frames[Animation.index];
				var frame3 = B_Anim.Frames[Animation.index];
				var frame4 = C_Anim.Frames[Animation.index];
				var frame5 = inOutAnim.Frames[Animation.index];
				var frame6 = junctionAnim.Frames[Animation.index];
				var frame7 = runAnim.Frames[Animation.index];
				var frame8 = unknownAnim.Frames[Animation.index];
				var frame9 = unsafeAnim.Frames[Animation.index];
				var frame10 = notValidAnim.Frames[Animation.index];

				if (showCenter == true)
				{
					d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
						x + frame.Frame.PivotX,
						y + frame.Frame.PivotY,
						frame.Frame.Width, frame.Frame.Height, false, Transparency);
				}
				if (showA == true)
				{
					d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame2),
						x + frame2.Frame.PivotX,
						y + frame2.Frame.PivotY,
						frame2.Frame.Width, frame2.Frame.Height, e.Selected, Transparency);
				}
				if (showB == true)
				{
					d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame3),
						x + frame3.Frame.PivotX,
						y + frame3.Frame.PivotY,
						frame3.Frame.Width, frame3.Frame.Height, e.Selected, Transparency);
				}
				if (showC == true)
				{
					d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame4),
						x + frame4.Frame.PivotX,
						y + frame4.Frame.PivotY,
						frame4.Frame.Width, frame4.Frame.Height, e.Selected, Transparency);
				}
				if (showInOut == true)
				{
					d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame5),
						x + frame5.Frame.PivotX,
						y + frame5.Frame.PivotY,
						frame5.Frame.Width, frame5.Frame.Height, false, Transparency);
				}
				if (showJunction == true)
				{
					d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame6),
						x + frame6.Frame.PivotX,
						y + frame6.Frame.PivotY,
						frame6.Frame.Width, frame6.Frame.Height, false, Transparency);
				}
				if (showRun == true)
				{
					d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame7),
						x + frame7.Frame.PivotX,
						y + frame7.Frame.PivotY,
						frame7.Frame.Width, frame7.Frame.Height, false, Transparency);
				}

				if (isUnsafe == true)
				{
					d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame9),
						x + frame9.Frame.PivotX,
						y + frame9.Frame.PivotY,
						frame9.Frame.Width, frame9.Frame.Height, false, Transparency);
				}

				if (showUnkown == true)
				{
					d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame8),
						x + frame8.Frame.PivotX,
						y + frame8.Frame.PivotY,
						frame8.Frame.Width, frame8.Frame.Height, false, Transparency);
				}
				if (showInvalid == true)
				{
					d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame10),
						x + frame10.Frame.PivotX,
						y + frame10.Frame.PivotY,
						frame10.Frame.Width, frame10.Frame.Height, false, Transparency);
				}
			}

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
			if (type == 0 || type == 1 || type == 6)
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
