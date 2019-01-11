﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ManiacEditor;
using Microsoft.Xna.Framework;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIText : EntityRenderer
    {
        string HUDLevelSelectCharS = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ*+,-./: \'\"_^]\\[";
        public char[] HUDLevelSelectChar;

        public UIText()
        {
            HUDLevelSelectChar = HUDLevelSelectCharS.ToCharArray();
        }

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            
            string text = entity.attributesMap["text"].ValueString;
            bool selectable = entity.attributesMap["selectable"].ValueBool;
            bool highlighted = entity.attributesMap["highlighted"].ValueBool;
            int spacingAmount = 0;
            foreach(char symb in text)
            {
                int frameID = GetFrameID(symb, e.EditorInstance.LevelSelectChar);
                int listID = (highlighted ? 1 : 0);
                var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation("Text", d, listID, frameID, false, false, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    var frame = editorAnim.Frames[Animation.index];
                    //Animation.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);
                    d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX + spacingAmount, y + frame.Frame.CenterY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    spacingAmount = spacingAmount + frame.Frame.Width;
                }
            }

            
        }

        public void DrawEditorHUDText(Editor instance, DevicePanel d, int x, int y, string text, bool highlighted, int Transparency = 0xff, int highlightDistance = -1)
        {
            int spacingAmount = 0;
            foreach (char symb in text)
            {
                bool fliph = false;
                bool flipv = false;
                int frameID = GetFrameIDHUD(symb, HUDLevelSelectChar);
                int listID = (highlighted ? 1 : 0);
                if (highlightDistance != -1)
                {
                    if (highlightDistance > 0) highlightDistance--;
                    else highlighted = false;
                }
                var editorAnim = instance.EditorEntity_ini.LoadAnimation("HUDEditorText", d, listID, frameID, fliph, flipv, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    var frame = editorAnim.Frames[0];
                    d.DrawHUDBitmap(frame.Texture, x + frame.Frame.CenterX + spacingAmount, y + frame.Frame.CenterY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    spacingAmount = spacingAmount + frame.Frame.Width;
                }
            }
        }

        public int GetFrameIDHUD(char letter, char[] arry)
        {
            char[] symArray = arry;
            int position = 0;
            foreach (char sym in symArray)
            {
                //MessageBox.Show(String.Format("Sym: {0} Letter: {1} Pos: {2}", sym, letter, position));
                if (sym.ToString().Equals(letter.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    return position;
                }
                position++;
            }
            return position;
        }

        public int GetFrameID(char letter, char[] arry)
        {
            char[] symArray = arry;
            int position = 0;
            foreach (char sym in symArray)
            {
                if (sym.ToString().Equals(letter.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    //MessageBox.Show(String.Format("Sym: {0} Letter: {1} Pos: {2}", sym, letter, position));
                    return position;
                }
                position++;
            }
            return position;
        }

        public override string GetObjectName()
        {
            return "UIText";
        }
    }
}