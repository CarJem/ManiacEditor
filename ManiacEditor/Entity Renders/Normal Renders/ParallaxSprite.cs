using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ManiacEditor;
using Microsoft.Xna.Framework;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class ParallaxSprite : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            int aniID = (int)entity.attributesMap["aniID"].ValueUInt8;
            if (Animation.parallaxSprite == "")
            {
                Animation.parallaxSprite = GetParallaxPath(e.EditorInstance);
            }

            var editorAnim = e.EditorInstance.EntityDrawing.LoadAnimation2("EditorIcons2", d, 0, 12, fliph, flipv, false);
            if (e.EditorInstance.UIModes.ShowParallaxSprites)
            {
                editorAnim = e.EditorInstance.EntityDrawing.LoadAnimation2(Animation.parallaxSprite, d, aniID, -1, fliph, flipv, false);
            }
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                if (e.EditorInstance.UIModes.ShowParallaxSprites)
                {
                    frame = editorAnim.Frames[Animation.index];
                }

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
        }

        private string GetParallaxPath (Editor EditorInstance)
        {
            string name = EditorInstance.Paths.CurrentZone.Replace("\\", "");
            string zoneName = "";
            string binPath = "";
            string parallaxName = "";
            // Normal Check First
            zoneName = name;
            parallaxName = name + "Parallax";
            binPath = Path.Combine(EditorInstance.DataDirectory, "Sprites") + '\\' + zoneName + '\\' + parallaxName + ".bin";
            if (!File.Exists(binPath))
            {
                //Stick with the Zone Name, but ditch the last char for parallax
                zoneName = name;
                parallaxName = name.Substring(0, name.Length - 1) + "Parallax";
                binPath = Path.Combine(EditorInstance.DataDirectory, "Sprites") + '\\' + zoneName + '\\' + parallaxName + ".bin";
                if (!File.Exists(binPath))
                {
                    //Remove the Last Char of the Zone Name and Parallax but use "1" for the Zone Name
                    zoneName = name.Substring(0, name.Length - 1) + "1";
                    parallaxName = name.Substring(0, name.Length - 1) + "Parallax";
                    binPath = Path.Combine(EditorInstance.DataDirectory, "Sprites") + '\\' + zoneName + '\\' + parallaxName + ".bin";
                    if (!File.Exists(binPath))
                    {
                        //Remove the Last Char of the Zone Name and Parallax but use "2" for the Zone Name
                        zoneName = name.Substring(0, name.Length - 1) + "2";
                        parallaxName = name.Substring(0, name.Length - 1) + "Parallax";
                        binPath = Path.Combine(EditorInstance.DataDirectory, "Sprites") + '\\' + zoneName + '\\' + parallaxName + ".bin";
                        if (!File.Exists(binPath))
                        {
                            //Remove the Last Char of the Zone Name and Parallax
                            zoneName = name.Substring(0, name.Length - 1);
                            parallaxName = name.Substring(0, name.Length - 1) + "Parallax";
                            binPath = Path.Combine(EditorInstance.DataDirectory, "Sprites") + '\\' + zoneName + '\\' + parallaxName + ".bin";
                        }
                    }
                }
            }
            //Debug.Print(zoneName);
            //Debug.Print(binPath);
            //Debug.Print(parallaxName);
            return parallaxName;
        }

        public override string GetObjectName()
        {
            return "ParallaxSprite";
        }
    }
}
