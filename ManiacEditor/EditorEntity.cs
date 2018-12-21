using ManiacEditor.Entity_Renders;
using ManiacEditor.Enums;
using RSDKv5;
using SharpDX.Direct3D9;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using System.Diagnostics;
using MonoGame.UI.Forms;
using MonoGame.Extended;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using ImageMagick;

namespace ManiacEditor
{
    [Serializable]
    public class EditorEntity : IDrawable
    {
        protected const int NAME_BOX_WIDTH  = 20;
        protected const int NAME_BOX_HEIGHT = 20;

        protected const int NAME_BOX_HALF_WIDTH  = NAME_BOX_WIDTH  / 2;
        protected const int NAME_BOX_HALF_HEIGHT = NAME_BOX_HEIGHT / 2;

        public bool Selected;

        public bool rotateImageLegacyMode = false;

        //public static EditorEntity Instance;
        public EditorAnimations EditorAnimations;
        public AttributeValidater AttributeValidater;

        private SceneEntity entity;
        public bool filteredOut;
        public string uniqueKey = "";




        //Rotating/Moving Platforms
        public int platformAngle = 0;
        public int platformpositionX = 0;
        public int platformpositionY = 0;
        //bool platformdisableX = false;
        //bool platformdisableY = false;
        //bool platformreverse = false;



        //For Drawing Extra Child Objects

        public bool childDraw = false;
        public int childX = 0;
        public int childY = 0;
        public bool childDrawAddMode = true;
        public int previousChildCount = 0; //For Reseting ChildCount States



        public static bool Working = false;
        public DateTime lastFrametime;
        public int index = 0;
        public int layerPriority = 0;
        public SceneEntity Entity { get { return entity; } }

        public int PositionX = 0;
        public int PositionY = 0;
        public string Name = "";

        public EditorEntity(SceneEntity entity)
        {
            this.entity = entity;
            PositionX = entity.Position.X.High;
            PositionY = entity.Position.Y.High;
            Name = entity.Object.Name.Name;
            lastFrametime = DateTime.Now;
            EditorAnimations = new EditorAnimations();
            AttributeValidater = new AttributeValidater();

            if (EditorEntity_ini.EntityRenderers.Count == 0)
            {
                var types = GetType().Assembly.GetTypes().Where(t => t.BaseType == typeof(EntityRenderer)).ToList();
                foreach (var type in types)
                    EditorEntity_ini.EntityRenderers.Add((EntityRenderer)Activator.CreateInstance(type));
            }
        }

        public void Draw(Graphics g)
        {

        }

        public bool ContainsPoint(Point point)
        {
            if (filteredOut) return false;

            return GetDimensions().Contains(point);
        }

        public bool IsInArea(Rectangle area)
        {
            if (filteredOut) return false;

            return GetDimensions().IntersectsWith(area);
        }

        public void Move(Point diff)
        {
                entity.Position.X.High += (short)diff.X;
                entity.Position.Y.High += (short)diff.Y;



                // Since the Editor can now update without the use of this render, I removed it
                //if (Properties.Settings.Default.AllowMoreRenderUpdates == true) Editor.Instance.UpdateRender();
                if (Editor.GameRunning && Properties.Settings.Default.EnableRealTimeObjectMovingInGame)
                {
                    int ObjectStart = 0x0086FFA0;
                    int ObjectSize = 0x458;

                    if (Properties.Settings.Default.UsePrePlusOffsets)
                        ObjectStart = 0x00A5DCC0;

                    // TODO: Find out if this is constent
                    int ObjectAddress = ObjectStart + (ObjectSize * entity.SlotID);
                    Editor.GameMemory.WriteInt16(ObjectAddress + 2, entity.Position.X.High);
                    Editor.GameMemory.WriteInt16(ObjectAddress + 6, entity.Position.Y.High);
                }


        }

        public Rectangle GetDimensions()
        {
            return new Rectangle(entity.Position.X.High, entity.Position.Y.High, NAME_BOX_WIDTH, NAME_BOX_HEIGHT);
        }




        






        public bool SetFilter()
        {
            if (HasFilter())
            {
                int filter = entity.GetAttribute("filter").ValueUInt8;

                /**
                 * 1 or 5 = Both
                 * 2 = Mania
                 * 4 = Encore
                 * 
                 * 0b0000
                 *   ||||
                 *   |||- Common
                 *   ||-- Mania
                 *   |--- Encore
                 *   ---- Unknown
                 */
                if (Properties.Settings.Default.useBitOperators)
                {
                    filteredOut =
                        ((filter & 0b0001) != 0 && !Properties.Settings.Default.showBothEntities) ||
                        ((filter & 0b0010) != 0 && !Properties.Settings.Default.showManiaEntities) ||
                        ((filter & 0b0100) != 0 && !Properties.Settings.Default.showEncoreEntities) ||
                        ((filter & 0b1000) != 0 && !Properties.Settings.Default.showOtherEntities);
                }
                else
                {
                    filteredOut =
                        ((filter == 1 || filter == 5) && !Properties.Settings.Default.showBothEntities) ||
                        (filter == 2 && !Properties.Settings.Default.showManiaEntities) ||
                        (filter == 4 && !Properties.Settings.Default.showEncoreEntities) ||
                        ((filter < 1 || filter == 3 || filter > 5) && !Properties.Settings.Default.showOtherEntities);
                }


            }
            else
                filteredOut = !Properties.Settings.Default.showBothEntities;

            if (Editor.Instance.entitiesTextFilter != "" && !entity.Object.Name.Name.Contains(Editor.Instance.entitiesTextFilter))
            {
                filteredOut = true;
            }

            return filteredOut;
        }

        public void Draw(DevicePanel d, List<EditorEntity> editorEntities = null, EditorEntity entity = null)
        {
            Draw(d);
        }

        // allow derived types to override the draw
        public virtual void Draw(DevicePanel d)
        {
            bool validPlane = false;

            if (Properties.Settings.Default.PrioritizedObjectRendering)
            {
                if (layerPriority != 0)
                {
                    validPlane = AttributeValidater.PlaneFilterCheck(entity, layerPriority);
                }
                else
                {
                    validPlane = true;
                }
                if (validPlane == false && !Editor.Instance.IsEntitiesEdit()) return;
            }

            bool skipRenderforx86 = false;
            if (entity.Object.Name.Name == "Player" && !Editor.Instance.playerObjectPosition.Contains(entity))
            {
                Editor.Instance.playerObjectPosition.Add(entity);
            }

            List<string> entityRenderList = EditorEntity_ini.entityRenderingObjects;
            List<string> onScreenExlusionList = EditorEntity_ini.renderOnScreenExlusions;
            if (Properties.Settings.Default.DisableRenderExlusions)
            {
                onScreenExlusionList = new List<string>();
            }

            if (filteredOut && !Editor.isPreRending) return;


            if (Properties.Settings.Default.AlwaysRenderObjects == false && !onScreenExlusionList.Contains(entity.Object.Name.Name))
            {
                //This causes some objects not to load ever, which is problamatic, so I made a toggle(and a list as of recently). It can also have some performance benifits
                if (!Editor.isPreRending)
                {
                    if (!this.IsObjectOnScreen(d)) return;
                }
            }
            System.Drawing.Color color = Selected ? System.Drawing.Color.MediumPurple : System.Drawing.Color.MediumTurquoise;
            System.Drawing.Color color2 = System.Drawing.Color.DarkBlue;
            if (HasSpecificFilter(1) || HasSpecificFilter(5))
            {
                 color2 = System.Drawing.Color.DarkBlue;
            }
            else if (HasSpecificFilter(2))
            {
                 color2 = System.Drawing.Color.DarkRed;
            }
            else if (HasSpecificFilter(4))
            {
                color2 = System.Drawing.Color.DarkGreen;
            }
            else if (HasFilterOther())
            {
                 color2 = System.Drawing.Color.Yellow;
            }

            int Transparency = (Editor.Instance.EditLayer == null) ? 0xff : 0x32;
            if (!Properties.Settings.Default.NeverLoadEntityTextures)
            {
                if (!Environment.Is64BitProcess && entity.Object.Name.Name == "SpecialRing") skipRenderforx86 = true;
                else EditorEntity_ini.LoadNextAnimation(this);
            }
            int x = entity.Position.X.High;
            int y = entity.Position.Y.High;
            int _ChildX = entity.Position.X.High + (childDraw ? childX : 0);
            int _ChildY = entity.Position.Y.High + (childDraw ? childY : 0);
            if (childDrawAddMode == false)
            {
                _ChildX = childX;
                _ChildY = childY;
            }
            bool fliph = false;
            bool flipv = false;
            bool rotate = false;
            var offset = GetRotationFromAttributes(ref fliph, ref flipv, ref rotate);
            string name = entity.Object.Name.Name;





            if (entityRenderList.Contains(name) && !skipRenderforx86)
            {
                if (!Properties.Settings.Default.NeverLoadEntityTextures)
                {
                    if ((this.IsObjectOnScreen(d) || onScreenExlusionList.Contains(entity.Object.Name.Name)) && Properties.Settings.Default.UseAltEntityRenderMode)
                    {
                        EditorEntity_ini.DrawOthers(d, entity, null, childX, childY, index, previousChildCount, platformAngle, EditorAnimations, Selected, AttributeValidater, childDrawAddMode);
                    }
                    else if (!Properties.Settings.Default.UseAltEntityRenderMode) {
                        EditorEntity_ini.DrawOthers(d, entity, null, childX, childY, index, previousChildCount, platformAngle, EditorAnimations, Selected, AttributeValidater, childDrawAddMode);
                    }

                }

            }
            else {
                var editorAnim = EditorEntity_ini.LoadAnimation2(name, d, -1, -1, fliph, flipv, rotate);
                if (editorAnim != null && editorAnim.Frames.Count > 0)
                {

                    // Special cases that always display a set frame(?)
                    if (Editor.Instance.ShowAnimations.Enabled == true)
                    {
                        if (entity.Object.Name.Name == "StarPost")
                            index = 1;
                    }



                    // Just incase
                    if (index >= editorAnim.Frames.Count)
                        index = 0;
                    var frame = editorAnim.Frames[index];

                    if (entity.attributesMap.ContainsKey("frameID"))
                        frame = GetFrameFromAttribute(editorAnim, entity.attributesMap["frameID"]);

                    if (frame != null)
                    {
                        EditorAnimations.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);
                        // Draw the normal filled Rectangle but Its visible if you have the entity selected
                        d.DrawBitmap(frame.Texture, _ChildX + frame.Frame.CenterX + ((int)offset.X * frame.Frame.Width), _ChildY + frame.Frame.CenterY + ((int)offset.Y * frame.Frame.Height),
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                    else
                    { // No frame to render
                        if (Properties.EditorState.Default.ShowEntitySelectionBoxes) d.DrawRectangle(x, y, x + NAME_BOX_WIDTH, y + NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color));
                    }
                    //Failsafe?
                    //DrawOthers(d);

                }
                else
                {
                    if (this.IsObjectOnScreen(d) && Properties.EditorState.Default.ShowEntitySelectionBoxes)
                    {
                        d.DrawRectangle(x, y, x + NAME_BOX_WIDTH, y + NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color));
                    }

                }
            }



            if (this.IsObjectOnScreen(d) && Properties.EditorState.Default.ShowEntitySelectionBoxes)
            {
                d.DrawRectangle(x, y, x + NAME_BOX_WIDTH, y + NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Selected ? 0x60 : 0x00, System.Drawing.Color.MediumPurple));
                d.DrawLine(x, y, x + NAME_BOX_WIDTH, y, System.Drawing.Color.FromArgb(Transparency, color2));
                d.DrawLine(x, y, x, y + NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color2));
                d.DrawLine(x, y + NAME_BOX_HEIGHT, x + NAME_BOX_WIDTH, y + NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color2));
                d.DrawLine(x + NAME_BOX_WIDTH, y, x + NAME_BOX_WIDTH, y + NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color2));
                if (Properties.Settings.Default.UseObjectRenderingImprovements == false && entity.Object.Name.Name != "TransportTube")
                {
                    if (Editor.Instance.GetZoom() >= 1) d.DrawTextSmall(String.Format("{0} (ID: {1})", entity.Object.Name, entity.SlotID), x + 2, y + 2, NAME_BOX_WIDTH - 4, System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black), true);
                }
                if (entity.Object.Name.Name == "TransportTube")
                {
                    if (Editor.Instance.GetZoom() >= 1)
                    {
                        d.DrawText(String.Format(entity.attributesMap["dirMask"].ValueUInt8.ToString()), x + 2, y + 2, NAME_BOX_WIDTH - 4, System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Red), true);
                    }
                }

            }



        }

        public EditorEntity_ini.EditorAnimation.EditorFrame GetFrameFromAttribute(EditorEntity_ini.EditorAnimation anim, AttributeValue attribute, string key = "frameID")
        {
            int frameID = -1;
            switch (attribute.Type)
            {
                case AttributeTypes.UINT8:
                    frameID = entity.attributesMap[key].ValueUInt8;
                    break;
                case AttributeTypes.INT8:
                    frameID = entity.attributesMap[key].ValueInt8;
                    break;
                case AttributeTypes.VAR:
                    frameID = (int)entity.attributesMap[key].ValueVar;
                    break;
            }
            if (frameID >= anim.Frames.Count)
                frameID = (anim.Frames.Count - 1) - (frameID % anim.Frames.Count + 1);
            if (frameID < 0)
                frameID = 0;
            if (frameID >= 0 && frameID < int.MaxValue)
                return anim.Frames[frameID % anim.Frames.Count];
            else
                return null; // Don't Render the Animation
        }

        /// <summary>
        /// Guesses the rotation of an entity
        /// </summary>
        /// <param name="attributes">List of all Attributes</param>
        /// <param name="fliph">Reference to fliph</param>
        /// <param name="flipv">Reference to flipv</param>
        /// <returns>AnimationID Offset</returns>
        public SharpDX.Vector2 GetRotationFromAttributes(ref bool fliph, ref bool flipv, ref bool rotate)
        {
            AttributeValue attribute = null;
            var attributes = entity.attributesMap;
            int dir = 0;
            var offset = new SharpDX.Vector2();
            if (attributes.ContainsKey("orientation"))
            {
                attribute = attributes["orientation"];
                switch (attribute.Type)
                {
                    case AttributeTypes.UINT8:
                        dir = attribute.ValueUInt8;
                        break;
                    case AttributeTypes.INT8:
                        dir = attribute.ValueInt8;
                        break;
                    case AttributeTypes.VAR:
                        dir = (int) attribute.ValueVar;
                        break;
                }
                if (dir == 0) // Up
                {
                }
                else if (dir == 1) // Down
                {
                    fliph = true;
                    offset.X = 1;
                    flipv = true;
                    offset.Y = 1;
                }
                else if (dir == 2) // Right
                {
                    flipv = true;
                    rotate = true;
                    offset.Y = 1;
                    //animID = 1;
                }
                else if (dir == 3) // Left
                {
                    flipv = true;
                    rotate = true;
                    offset.Y = 1;
                    //animID = 1;
                }
            }
            if (attributes.ContainsKey("direction") && dir == 0)
            {
                attribute = attributes["direction"];
                switch (attribute.Type)
                {
                    case AttributeTypes.UINT8:
                        dir = attribute.ValueUInt8;
                        break;
                    case AttributeTypes.INT8:
                        dir = attribute.ValueInt8;
                        break;
                    case AttributeTypes.VAR:
                        dir = (int)attribute.ValueVar;
                        break;
                }
                if (dir == 0) // Right
                {
                }
                else if (dir == 1) // left
                {
                    fliph = true;
                    offset.X = 0;
                    rotate = true;
                }
                else if (dir == 2) // Up
                {
                    flipv = true;
                }
                else if (dir == 3) // Down
                {
                    flipv = true;
                    //offset.Y = 1;
                }
            }
            return offset;
        }

        public bool IsObjectOnScreen(DevicePanel d)
        {
            int x = entity.Position.X.High + childX;
            int y = entity.Position.Y.High + childY;
            if (childDrawAddMode == false)
            {
                x = childX;
                y = childY;
            }
            int Transparency = (Editor.Instance.EditLayer == null) ? 0xff : 0x32;

            bool isObjectVisibile = false;

            EntityRenderer renderer = EditorEntity_ini.EntityRenderers.Where(t => t.GetObjectName() == entity.Object.Name.Name).FirstOrDefault();
            if (renderer != null)
            {
                isObjectVisibile = renderer.isObjectOnScreen(d, entity, null, x, y, 0);
            }
            else
            {
                isObjectVisibile = d.IsObjectOnScreen(x, y, 20, 20);
            }

            return isObjectVisibile;


        }


        public bool HasFilter()
        {
            return entity.attributesMap.ContainsKey("filter") && entity.attributesMap["filter"].Type == AttributeTypes.UINT8;
        }

        public bool HasSpecificFilter(uint input, bool checkHigher = false)
        {
            if (entity.attributesMap.ContainsKey("filter") && entity.attributesMap["filter"].Type == AttributeTypes.UINT8)
            {
                if (entity.attributesMap["filter"].ValueUInt8 == input && checkHigher == false)
                {
                    return true;
                }
                else if (entity.attributesMap["filter"].ValueUInt8 >= input && checkHigher)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public bool HasFilterOther()
        {
            if (entity.attributesMap.ContainsKey("filter") && entity.attributesMap["filter"].Type == AttributeTypes.UINT8)
            {
                int filter = entity.attributesMap["filter"].ValueUInt8;
                if (filter < 1 || filter == 3 || filter > 5)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void PrepareForExternalCopy()
        {
            entity.PrepareForExternalCopy();
        }

        public bool IsExternal()
        {
            return entity.IsExternal();
        }

        internal void Flip(FlipDirection flipDirection)
        {
            if (entity.attributesMap.ContainsKey("flipFlag"))
            {
                if (flipDirection == FlipDirection.Horizontal)
                {
                    entity.attributesMap["flipFlag"].ValueVar ^= 0x01;
                }
                else
                {
                    entity.attributesMap["flipFlag"].ValueVar ^= 0x02;
                }
            }
        }

        public void Dispose()
        {

        }
    }
}
