using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor
{
    class LinkedEditorEntity : EditorEntity
    {
        private uint goProperty;
        private uint destinationTag;
        private byte tag;
        private ushort slotID;
        private ushort targetSlotID;
        private RSDKv5.SceneEntity currentEntity;
        private Editor EditorInstance2;
		private byte TransportTubeType;

        private uint NodeCount;
        private uint ChildCount;
        private uint ControlTag;

        private uint ButtonTag;



        public LinkedEditorEntity(RSDKv5.SceneEntity entity, Editor instance) : base(entity, instance)
        {
            EditorInstance2 = instance;
            if (entity.Object.Name.Name == "WarpDoor")
            {
                goProperty = Entity.GetAttribute("go").ValueVar;
                destinationTag = Entity.GetAttribute("destinationTag").ValueVar;
                tag = Entity.GetAttribute("tag").ValueUInt8;
                currentEntity = Entity;
            }
            else if (entity.Object.Name.Name == "TornadoPath")
            {
                slotID = Entity.SlotID;
                targetSlotID = (ushort)(Entity.SlotID + 1);
                currentEntity = Entity;
            }
			else if (entity.Object.Name.Name == "TransportTube")
			{
				TransportTubeType = Entity.GetAttribute("type").ValueUInt8;
				slotID = Entity.SlotID;
				targetSlotID = (ushort)(Entity.SlotID + 1);
				currentEntity = Entity;
			}
            else if (entity.Object.Name.Name == "PlatformControl")
            {
                slotID = Entity.SlotID;
                targetSlotID = (ushort)(Entity.SlotID + 1);
                currentEntity = Entity;
                NodeCount = Entity.GetAttribute("nodeCount").ValueVar;
                ControlTag = Entity.GetAttribute("buttonTag").ValueVar;
                ChildCount = Entity.GetAttribute("childCount").ValueVar;
            }
            else if (entity.Object.Name.Name == "Button")
            {
                slotID = Entity.SlotID;
                targetSlotID = (ushort)(Entity.SlotID + 1);
                currentEntity = Entity;
                ButtonTag = Entity.GetAttribute("tag").ValueUInt8;
            }
            else if (entity.Object.Name.Name == "PlatformNode")
            {
                slotID = Entity.SlotID;
                targetSlotID = (ushort)(Entity.SlotID + 1);
                currentEntity = Entity;
            }
            else if (entity.Object.Name.Name == "AIZTornadoPath")
			{
				slotID = Entity.SlotID;
				targetSlotID = (ushort)(Entity.SlotID + 1);
				currentEntity = Entity;
			}

		}

        public override void Draw(DevicePanel d)
        {
			if (filteredOut || currentEntity == null) return;
			if (EditorInstance.showEntityPathArrows)
            {
                if (currentEntity.Object.Name.Name == "WarpDoor")
                {

                    base.Draw(d);
                    if (goProperty == 1 && destinationTag == 0) return; // probably just a destination

                    // this is the start of a WarpDoor, find its partner(s)
                    var warpDoors = Entity.Object.Entities.Where(e => e.GetAttribute("tag").ValueUInt8 ==
                                                                        destinationTag);

                    if (warpDoors != null
                        && warpDoors.Any())
                    {
                        // some destinations seem to be duplicated, so we must loop
                        foreach (var wd in warpDoors)
                        {
                            DrawLinkArrow(d, Entity, wd);
                        }
                    }
                }
				else if (currentEntity.Object.Name.Name == "TornadoPath")
                {
                    base.Draw(d);

                    //if (goProperty == 1 && destinationTag == 0) return; // probably just a destination

                    // this is the start of a WarpDoor, find its partner(s)
                    var tornadoPaths = Entity.Object.Entities.Where(e => e.SlotID == targetSlotID);

                    if (tornadoPaths != null
                        && tornadoPaths.Any())
                    {
                        // some destinations seem to be duplicated, so we must loop
                        foreach (var tp in tornadoPaths)
                        {
                            DrawLinkArrow(d, Entity, tp);
                        }
                    }
                }
                else if (currentEntity.Object.Name.Name == "AIZTornadoPath")
                {
                    base.Draw(d);

                    //if (goProperty == 1 && destinationTag == 0) return; // probably just a destination

                    // this is the start of a WarpDoor, find its partner(s)
                    var tornadoPaths = Entity.Object.Entities.Where(e => e.SlotID == targetSlotID);

                    if (tornadoPaths != null
                        && tornadoPaths.Any())
                    {
                        // some destinations seem to be duplicated, so we must loop
                        foreach (var tp in tornadoPaths)
                        {
                            DrawLinkArrow(d, Entity, tp);
                        }
                    }
                }
				else if (currentEntity.Object.Name.Name == "PlatformNode")
				{


					//if (goProperty == 1 && destinationTag == 0) return; // probably just a destination

					// this is the start of a WarpDoor, find its partner(s)
					var nodePaths = Entity.Object.Entities.Where(e => e.SlotID == targetSlotID);

					if (nodePaths != null
						&& nodePaths.Any())
					{
						base.Draw(d);
						// some destinations seem to be duplicated, so we must loop
						foreach (var tp in nodePaths)
						{
							DrawCenteredLinkArrow(d, Entity, tp);
						}
					}
					else
					{
                        base.Draw(d);
                    }

				}
                else if (currentEntity.Object.Name.Name == "PlatformControl")
				{
                    base.Draw(d);
                    ushort minimumSlot = targetSlotID;
                    ushort maximumSlot = (ushort)(targetSlotID + NodeCount);

                    List<Tuple<EditorEntity, EditorEntity>> NodePoints = new List<Tuple<EditorEntity, EditorEntity>>();
                    List<EditorEntity> ChildPoints = new List<EditorEntity>();

                    int currentTargetSlotID = targetSlotID;
                    int remainder = (NodeCount % 2 == 1 ? 1 : 0);

                    for (int i = 0; i < NodeCount; i++)
                    {
                        int targetID1 = currentTargetSlotID;
                        int targetID2 = currentTargetSlotID + 1;
                        if (targetID2 >= maximumSlot)
                        {
                            targetID2 = minimumSlot;
                            var target1 = EditorInstance.entities.entities.Where(e => e.Entity.SlotID == targetID1).First();
                            var target2 = EditorInstance.entities.entities.Where(e => e.Entity.SlotID == targetID2).First();

                            NodePoints.Add(new Tuple<EditorEntity, EditorEntity>(target1, target2));
                        }
                        currentTargetSlotID = currentTargetSlotID + 1;
                    }

                    if (NodePoints != null && NodePoints.Any())
                    {
                        foreach(var pairs in NodePoints)
                        {
                            DrawCenteredLinkArrow(d, pairs.Item1.Entity, pairs.Item2.Entity);
                        }
                    }

                    
                    for (int i = 0; i < ChildCount; i++)
                    {
                        int targetID = currentTargetSlotID;


                        var target1 = EditorInstance.entities.entities.Where(e => e.Entity.SlotID == targetID).First();

                        ChildPoints.Add(target1);
                        currentTargetSlotID = currentTargetSlotID + 1;
                    }

                    if (NodePoints != null && NodePoints.Any())
                    {
                        foreach (var children in ChildPoints)
                        {
                            DrawCenteredLinkArrow(d, currentEntity, children.Entity, Color.Red);
                        }
                    }

                    var tagged = EditorInstance.entities.entities.Where(e => e.Entity.AttributeExists("tag", RSDKv5.AttributeTypes.UINT8));
                    var triggers = tagged.Where(e => e.Entity.GetAttribute("tag").ValueUInt8 == ControlTag);

                    if (triggers != null && triggers.Any())
                    {
                        foreach (var t in triggers)
                        {
                            DrawLinkArrow(d, Entity, t.Entity);
                        }
                    }

                }
                else if (currentEntity.Object.Name.Name == "Button")
                {

                    var tagged = EditorInstance.entities.entities.Where(e => e.Entity.AttributeExists("buttonTag", RSDKv5.AttributeTypes.VAR));
                    var triggers = tagged.Where(e => e.Entity.GetAttribute("buttonTag").ValueVar == ButtonTag);

                    if (triggers != null && triggers.Any())
                    {
                        foreach (var t in triggers)
                        {
                            DrawLinkArrow(d, Entity, t.Entity);
                        }
                    }
                    base.Draw(d);

                }
            }


           if (currentEntity.Object.Name.Name == "TransportTube")
		    {
			    if (EditorInstance.showEntityPathArrows)
			    {
				    if ((TransportTubeType == 2 || TransportTubeType == 4))
				    {
					    var transportTubePaths = Entity.Object.Entities.Where(e => e.SlotID == targetSlotID);

					    if (transportTubePaths != null && transportTubePaths.Any())
					    {
						    foreach (var ttp in transportTubePaths)
						    {
							    int destinationType = ttp.GetAttribute("type").ValueUInt8;
							    if (destinationType == 3)
							    {
								    DrawLinkArrowTransportTubes(d, Entity, ttp, 3, TransportTubeType);
							    }
							    else if (destinationType == 4)
							    {
								    DrawLinkArrowTransportTubes(d, Entity, ttp, 4, TransportTubeType);
							    }
							    else if (destinationType == 2)
							    {
								    DrawLinkArrowTransportTubes(d, Entity, ttp, 2, TransportTubeType);
							    }
							    else
							    {
								    DrawLinkArrowTransportTubes(d, Entity, ttp, 1, TransportTubeType);
							    }

						    }
					    }
				    }
			    }

			    base.Draw(d);
		    }
		}

        private void DrawLinkArrow(DevicePanel d, RSDKv5.SceneEntity start, RSDKv5.SceneEntity end)
        {
            int startX = start.Position.X.High;
            int startY = start.Position.Y.High;
            int endX = end.Position.X.High;
            int endY = end.Position.Y.High;

            int dx = endX - startX;
            int dy = endY - startY;

            int offsetX = 0;
            int offsetY = 0;
            int offsetDestinationX = 0;
            int offsetDestinationY = 0;

            if (Math.Abs(dx) > Math.Abs(dy))
            {
                // horizontal difference greater than vertical difference
                offsetY = NAME_BOX_HALF_HEIGHT;
                offsetDestinationY = NAME_BOX_HALF_HEIGHT;

                if (dx > 0)
                {
                    offsetX = NAME_BOX_WIDTH;
                }
                else
                {
                    offsetDestinationX = NAME_BOX_WIDTH;
                }
            }
            else
            {
                // vertical difference greater than horizontal difference
                offsetX = NAME_BOX_HALF_WIDTH;
                offsetDestinationX = NAME_BOX_HALF_WIDTH;

                if (dy > 0)
                {
                    offsetY = NAME_BOX_HEIGHT;
                }
                else
                {
                    offsetDestinationY = NAME_BOX_HEIGHT;
                }
            }

            d.DrawArrow(startX + offsetX,
                        startY + offsetY,
                        end.Position.X.High + offsetDestinationX,
                        end.Position.Y.High + offsetDestinationY,
                        Color.GreenYellow);
        }

		private void DrawCenteredLinkArrow(DevicePanel d, RSDKv5.SceneEntity start, RSDKv5.SceneEntity end, Color? colur = null)
		{
            Color color = (colur != null ? colur.Value : Color.GreenYellow);
			int startX = start.Position.X.High;
			int startY = start.Position.Y.High;
			int endX = end.Position.X.High;
			int endY = end.Position.Y.High;

			int dx = endX - startX;
			int dy = endY - startY;

			int offsetX = 0;
			int offsetY = 0;
			int offsetDestinationX = 0;
			int offsetDestinationY = 0;

			d.DrawArrow(startX + offsetX,
						startY + offsetY,
						end.Position.X.High + offsetDestinationX,
						end.Position.Y.High + offsetDestinationY,
                        color);
		}

		private void DrawLinkArrowTransportTubes(DevicePanel d, RSDKv5.SceneEntity start, RSDKv5.SceneEntity end, int destType, int sourceType)
		{
			Color color = Color.Transparent;
			switch (destType)
			{
				case 4:
					color = Color.Yellow;
					break;
				case 3:
					color = Color.Red;
					break;
			}
			if (sourceType == 2)
			{
				switch (destType)
				{
					case 4:
						color = Color.Green;
						break;
					case 3:
						color = Color.Red;
						break;
				}
			}
			int startX = start.Position.X.High;
			int startY = start.Position.Y.High;
			int endX = end.Position.X.High;
			int endY = end.Position.Y.High;

			int dx = endX - startX;
			int dy = endY - startY;

			int offsetX = 0;
			int offsetY = 0;
			int offsetDestinationX = 0;
			int offsetDestinationY = 0;

			d.DrawArrow(startX + offsetX,
						startY + offsetY,
						end.Position.X.High + offsetDestinationX,
						end.Position.Y.High + offsetDestinationY,
						color);
		}
        public void DrawLinkedSelectionBox(DevicePanel d, int x, int y, int Transparency, System.Drawing.Color color, System.Drawing.Color color2, RSDKv5.SceneEntity entity)
        {
            if (renderNotFound)
            {
                d.DrawRectangle(x, y, x + NAME_BOX_WIDTH, y + NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color));
            }
            else
            {
                d.DrawRectangle(x, y, x + NAME_BOX_WIDTH, y + NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Selected ? 0x60 : 0x00, System.Drawing.Color.MediumPurple));
            }
            d.DrawLine(x, y, x + NAME_BOX_WIDTH, y, System.Drawing.Color.FromArgb(Transparency, color2));
            d.DrawLine(x, y, x, y + NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color2));
            d.DrawLine(x, y + NAME_BOX_HEIGHT, x + NAME_BOX_WIDTH, y + NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color2));
            d.DrawLine(x + NAME_BOX_WIDTH, y, x + NAME_BOX_WIDTH, y + NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color2));
            if (Properties.Settings.Default.UseObjectRenderingImprovements == false)
            {
                if (EditorInstance.GetZoom() >= 1) d.DrawTextSmall(String.Format("{0} (ID: {1})", entity.Object.Name, entity.SlotID), x + 2, y + 2, NAME_BOX_WIDTH - 4, System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black), true);
            }
        }

    }
}
