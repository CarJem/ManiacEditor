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

        public static EditorEntity Instance;

        private SceneEntity entity;
        private bool filteredOut;

        // Object Render List
        public static List<EntityRenderer> EntityRenderers = new List<EntityRenderer>();
        // Object List for initilizing the if statement
        List<string> entityRenderingObjects = Editor.Instance.entityRenderingObjects;
        List<string> renderOnScreenExlusions = Editor.Instance.renderOnScreenExlusions;

        //Rotating/Moving Platforms
        public int platformAngle = 0;
        public int platformpositionX = 0;
        public int platformpositionY = 0;
        bool platformdisableX = false;
        bool platformdisableY = false;
        bool platformreverse = false;


        public static Dictionary<string, EditorAnimation> Animations = new Dictionary<string, EditorAnimation>();
        public static Dictionary<string, Bitmap> Sheets = new Dictionary<string, Bitmap>();
        public static string[] DataDirectoryList = null;
        public static bool Working = false;
        public Animation rsdkAnim;
        public DateTime lastFrametime;
        public int index = 0;
        public SceneEntity Entity { get { return entity; } }

        public EditorEntity(SceneEntity entity)
        {
            this.entity = entity;
            lastFrametime = DateTime.Now;

            if (EntityRenderers.Count == 0)
            {
                var types = GetType().Assembly.GetTypes().Where(t => t.BaseType == typeof(EntityRenderer)).ToList();
                foreach (var type in types)
                    EntityRenderers.Add((EntityRenderer)Activator.CreateInstance(type));
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
            if (Editor.GameRunning)
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

        public void SnapToGrid(Point diff)
        {
            entity.Position.X.High = (short)((diff.X + 8) / 16 * 16);
            entity.Position.Y.High = (short)((diff.Y + 8) / 16 * 16);
            if (Editor.GameRunning)
            {
                // TODO: Find out if this is constent
                int ObjectStart = 0x00A5DCC0;
                int ObjectSize = 0x458;
                int ObbjectAddress = ObjectStart + (ObjectSize * entity.SlotID);
                Editor.GameMemory.WriteInt16(ObbjectAddress + 2, entity.Position.X.High);
                Editor.GameMemory.WriteInt16(ObbjectAddress + 6, entity.Position.Y.High);
            }
        }

        public Rectangle GetDimensions()
        {
            return new Rectangle(entity.Position.X.High, entity.Position.Y.High, NAME_BOX_WIDTH, NAME_BOX_HEIGHT);
        }

        public Bitmap CropImage(Bitmap source, Rectangle section, bool fliph, bool flipv)
        {
            Bitmap bmp2 = new Bitmap(section.Size.Width, section.Size.Height);
            using (Graphics gg = Graphics.FromImage(bmp2))
                gg.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
            if (fliph && flipv)
                bmp2.RotateFlip(RotateFlipType.RotateNoneFlipXY);
            else if (fliph)
                bmp2.RotateFlip(RotateFlipType.RotateNoneFlipX);
            else if (flipv)
                bmp2.RotateFlip(RotateFlipType.RotateNoneFlipY);
            int size = ((section.Width > section.Height ? section.Width : section.Height) / 64) * 64;
            Bitmap bmp = new Bitmap(1024, 1024);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(bmp2, 0, 0, new Rectangle(0, 0, bmp2.Width, bmp2.Height), GraphicsUnit.Pixel);
            }

            return bmp;
        }

        public Bitmap RotateImage(Bitmap img, float rotationAngle)
        {

            // Get a reasonable size
            int width;
            int height;
            int xDiffrence = img.Width - img.Height;
            int yDiffrence = img.Height - img.Width;
            if (xDiffrence < 0)
            {
                xDiffrence = -xDiffrence;
            }
            if (yDiffrence < 0)
            {
                yDiffrence = -yDiffrence;
            }
            width = img.Width + xDiffrence;
            height = img.Height + yDiffrence;

            float pointX = img.Width / 16;
            float pointY = img.Height / 16;

            //create an empty Bitmap image 
            Bitmap bmp = new Bitmap(width, height);

            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                //set the point system origin to the center of our image
                gfx.TranslateTransform(pointX, pointY);

                //now rotate the image
                gfx.RotateTransform(rotationAngle);

                //move the point system origin back to 0,0
                gfx.TranslateTransform(-pointX, -pointY);

                //set the InterpolationMode to HighQualityBicubic so to ensure a high
                //quality image once it is transformed to the specified size
                gfx.InterpolationMode = InterpolationMode.NearestNeighbor;

                //draw our new image onto the graphics object with its center on the center of rotation
                gfx.DrawImage(img, new PointF(pointX, pointY));


            }

            //return the image
            return bmp;


        }

        public Bitmap RemoveColourImage(Bitmap source, System.Drawing.Color colour, int width, int height)
        {
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    if (source.GetPixel(x, y) == colour)
                        source.SetPixel(x, y, System.Drawing.Color.Transparent);
                }
            }
            return source;
        }

        public List<LoadAnimationData> AnimsToLoad = new List<LoadAnimationData>();
        
        public void LoadNextAnimation()
        {
            if (AnimsToLoad.Count == 0)
                return;
            var val = AnimsToLoad[0];
            if (val.anim == null)
            {
                string key = $"{val.name}-{val.AnimId}-{val.frameId}-{val.fliph}-{val.flipv}-{val.rotate}-{val.rotateImg}";
                if (!Animations.ContainsKey(key))
                {
                    if (!Working)
                    {
                        try
                        {
                            LoadAnimation(val.name, val.d, val.AnimId, val.frameId, val.fliph, val.flipv, val.rotate, val.rotateImg, false);
                        }
                        catch (Exception)
                        {
                            // lots of changes introduced by Plus, just hide errors for now (evil I know!)
                            //Console.WriteLine($"Pop loading next animiation. {val.name}, {val.AnimId}, {val.frameId}, {val.fliph}, {val.flipv}, {val.rotate}", e);
                        }
                    }
                }
                else
                {
                    val.anim = Animations[key];
                }

            }
            if (val.anim == null)
                return;
            if (val.anim.Ready)
                AnimsToLoad.RemoveAt(0);
            else
            {
                if (val.anim.Frames.Count == 0)
                {
                    val.anim.Ready = true;
                    AnimsToLoad.RemoveAt(0);
                    return;
                }
                val.anim.Frames[val.anim.loadedFrames].Texture = TextureCreator.FromBitmap(val.d._device, val.anim.Frames[val.anim.loadedFrames]._Bitmap);
                val.anim.Frames[val.anim.loadedFrames]._Bitmap.Dispose();
                ++val.anim.loadedFrames;
                if (val.anim.loadedFrames == val.anim.Frames.Count)
                    val.anim.Ready = true;
            }
        }

        [Serializable]
        public class LoadAnimationData
        {
            public string name;
            public DevicePanel d;
            public int AnimId, frameId;
            public bool fliph, flipv, rotate;
            public int rotateImg;
            public EditorAnimation anim;
        }

        /// <summary>
        /// Loads / Gets the Sprite Animation
        /// NOTE: 
        /// </summary>
        /// <param name="name">The Name of the object</param>
        /// <param name="d">The DevicePanel</param>
        /// <param name="AnimId">The Animation ID (-1 to Load Normal)</param>
        /// <param name="frameId">The Frame ID for the specified Animation (-1 to load all frames)</param>
        /// <param name="fliph">Flip the Texture Horizontally</param>
        /// <param name="flipv">Flip the Texture Vertically</param>
        /// <returns>The fully loaded Animation</returns>
        public EditorAnimation LoadAnimation2(string name, DevicePanel d, int AnimId, int frameId, bool fliph, bool flipv, bool rotate, int rotateImg = 0)
        {
            string key = $"{name}-{AnimId}-{frameId}-{fliph}-{flipv}-{rotate}-{rotateImg}";
            if (Animations.ContainsKey(key))
            {
                if (Animations[key].Ready)
                {
                    // Use the already loaded Amination
                    return Animations[key];
                }
                else
                    return null;
            }
            var entry = new LoadAnimationData()
            {
                name = name,
                d = d,
                AnimId = AnimId,
                frameId = frameId,
                fliph = fliph,
                flipv = flipv,
                rotate = rotate,
                rotateImg = rotateImg
            };
            AnimsToLoad.Add(entry);
            return null;
        }

        /// <summary>
        /// Loads / Gets the Sprite Animation
        /// NOTE: 
        /// </summary>
        /// <param name="name">The Name of the object</param>
        /// <param name="d">The DevicePanel</param>
        /// <param name="AnimId">The Animation ID (-1 to Load Normal)</param>
        /// <param name="frameId">The Frame ID for the specified Animation (-1 to load all frames)</param>
        /// <param name="fliph">Flip the Texture Horizontally</param>
        /// <param name="flipv">Flip the Texture Vertically</param>
        /// <returns>The fully loaded Animation</returns>
        public EditorAnimation LoadAnimation(string name, DevicePanel d, int AnimId, int frameId, bool fliph, bool flipv, bool rotate, int rotateImg, bool loadImageToDX = true)
        {
            string key = $"{name}-{AnimId}-{frameId}-{fliph}-{flipv}-{rotate}-{rotateImg}";
            var anim = new EditorAnimation();
            if (Animations.ContainsKey(key))
            {
                if (Animations[key].Ready)
                {
                    // Use the already loaded Amination
                    return Animations[key];
                }
                else
                {
                    return null;
                }
            }

            Animations.Add(key, anim);

            // Get the path of the object's textures
            string path2 = getPaths(name);
            if (path2 == null)
            {
                return null;
            }

            using (var stream = File.OpenRead(path2))
            {
                rsdkAnim = new Animation();
                rsdkAnim.Load(new BinaryReader(stream));
            }
            if (AnimId == -1)
            {
                if (rsdkAnim.Animations.Any(t => t.AnimName.Contains("Normal")))
                    AnimId = rsdkAnim.Animations.FindIndex(t => t.AnimName.Contains("Normal"));
                else AnimId = 0;
                // Use Vertical Amination if one exists
                if (rotate && rsdkAnim.Animations.Any(t => t.AnimName.EndsWith(" V")))
                    AnimId = rsdkAnim.Animations.FindIndex(t => t.AnimName.EndsWith(" V"));
            }
            if (AnimId == -2)
            {
                if (rsdkAnim.Animations.Any(t => t.AnimName.Contains("Swing")))
                    AnimId = rsdkAnim.Animations.FindIndex(t => t.AnimName.Contains("Swing"));
                else AnimId = 0;
            }
            if (AnimId >= rsdkAnim.Animations.Count)
                AnimId = rsdkAnim.Animations.Count - 1;
            for (int i = 0; i < rsdkAnim.Animations[AnimId].Frames.Count; ++i)
            {
                // check we don't stray outside our loaded animations/frames
                // if user enters a value that would take us there, just show
                // a valid frame instead
                var animiation = rsdkAnim.Animations[AnimId];
                var frame = animiation.Frames[i];
                if (frameId >= 0 && frameId < animiation.Frames.Count)
                    frame = animiation.Frames[frameId];
                Bitmap map;

                if (!Sheets.ContainsKey(rsdkAnim.SpriteSheets[frame.SpriteSheet]))
                {
                    string targetFile;
                    if (name == "EditorAssets" || name == "SuperSpecialRing" || name == "EditorIcons2" || name == "TransportTubes")
                    {
                        if (name == "EditorAssets")
                        {
                            targetFile = Path.Combine(Environment.CurrentDirectory, "EditorAssets.gif");
                        }
                        else if (name == "EditorIcons2")
                        {
                            targetFile = Path.Combine(Environment.CurrentDirectory, "EditorIcons2.gif");
                        }
                        else if (name == "TransportTubes")
                        {
                            targetFile = Path.Combine(Environment.CurrentDirectory, "Global\\", "TransportTubes.gif");
                        }
                        else
                        {
                            targetFile = Path.Combine(Environment.CurrentDirectory, "Global\\", "SuperSpecialRing.gif");
                        }
                    }
                    else
                        targetFile = Path.Combine(Editor.DataDirectory, "sprites", rsdkAnim.SpriteSheets[frame.SpriteSheet].Replace('/', '\\'));
                    if (!File.Exists(targetFile))
                    {
                        map = null;
                        
                        // add a Null to our lookup, so we can avoid looking again in the future
                        Sheets.Add(rsdkAnim.SpriteSheets[frame.SpriteSheet], map);
                    }
                    else
                    {
                        
                            map = new Bitmap(targetFile);
                            Sheets.Add(rsdkAnim.SpriteSheets[frame.SpriteSheet], map);
                    }
                }
                else
                {
                    map = Sheets[rsdkAnim.SpriteSheets[frame.SpriteSheet]];
                }


                if (frame.Width == 0 || frame.Height == 0)
                    continue;

                // can't load the animation, it probably doesn't exist in the User's Sprites folder
                if (map == null) return null;

                // We are storing the first colour from the palette so we can use it to make sprites transparent
                var colour = map.Palette.Entries[0];
                // Slow
                map = CropImage(map, new Rectangle(frame.X, frame.Y, frame.Width, frame.Height), fliph, flipv);
                if (rotateImg != 0)
                {
                    map = RotateImage(map, rotateImg);


                    // Get a reasonable size
                    int xDiffrence = map.Width - map.Height;
                    int yDiffrence = map.Height - map.Width;
                    if (xDiffrence < 0)
                    {
                        xDiffrence = -xDiffrence;
                    }
                    if (yDiffrence < 0)
                    {
                        yDiffrence = -yDiffrence;
                    }
                    frame.Height = map.Width + xDiffrence;
                    frame.Width = map.Height + yDiffrence;


                }

                RemoveColourImage(map, colour, frame.Width, frame.Height);

                Texture texture = null;
                if (loadImageToDX)
                {
                    texture = TextureCreator.FromBitmap(d._device, map);

                }
                var editorFrame = new EditorAnimation.EditorFrame()
                {
                    Texture = texture,
                    Frame = frame,
                    Entry = rsdkAnim.Animations[AnimId]
                };
                if (loadImageToDX == false)
                    editorFrame._Bitmap = map;
                anim.Frames.Add(editorFrame);
                if (frameId != -1)
                    break;
            }
            anim.ImageLoaded = true;
            if (loadImageToDX)
                anim.Ready = true;
            Working = false;
            return anim;

        }


        public String getPaths(string name)
        {
            string path, path2;
            if (name == "EditorAssets" || name == "SuperSpecialRing" || name == "EditorIcons2" || name == "TransportTubes")
            {
                if (name == "EditorAssets")
                {
                    path2 = Path.Combine(Environment.CurrentDirectory, "EditorAssets.bin");
                    if (!File.Exists(path2))
                        return null;
                }
                else if (name == "EditorIcons2")
                {
                    path2 = Path.Combine(Environment.CurrentDirectory, "EditorIcons2.bin");
                    if (!File.Exists(path2))
                        return null;
                }
                else if (name == "TransportTubes")
                {
                    path2 = Path.Combine(Environment.CurrentDirectory, "Global\\", "TransportTubes.bin");
                    if (!File.Exists(path2))
                        return null;
                }
                else
                {
                    path2 = Path.Combine(Environment.CurrentDirectory, "Global\\", "SuperSpecialRing.bin");
                    Debug.Print(path2);
                    if (!File.Exists(path2))
                        return null;
                }
            }
            else
            {
                if (DataDirectoryList == null)
                    DataDirectoryList = Directory.GetFiles(Path.Combine(Editor.DataDirectory, "Sprites"), $"*.bin", SearchOption.AllDirectories);


                // Checks Global frist
                path = Editor.Instance.SelectedZone + "\\" + name + ".bin";
                path2 = Path.Combine(Editor.DataDirectory, "sprites") + '\\' + path;
                if (!File.Exists(path2))
                {
                    // Checks without last character
                    path = path = Editor.Instance.SelectedZone.Substring(0, Editor.Instance.SelectedZone.Length - 1) + "\\" + name + ".bin";
                    path2 = Path.Combine(Editor.DataDirectory, "sprites") + '\\' + path;
                }
                /*if (!File.Exists(path2))
                {
                    // Checks Editor Global
                    path2 = Environment.CurrentDirectory + "\\Global\\" + name + ".bin";
                }*/
                if (!File.Exists(path2))
                {
                    // Checks Global
                    path = "Global\\" + name + ".bin";
                    path2 = Path.Combine(Editor.DataDirectory, "sprites") + '\\' + path;
                }
                if (!File.Exists(path2))
                {
                    // Checks the Stage folder 
                    foreach (string dir in Directory.GetDirectories(Path.Combine(Editor.DataDirectory, "Sprites"), $"*", SearchOption.TopDirectoryOnly))
                    {
                        path = Path.GetFileName(dir) + "\\" + name + ".bin";
                        path2 = Path.Combine(Editor.DataDirectory, "sprites") + '\\' + path;
                        if (File.Exists(path2))
                            break;
                    }
                }
                if (!File.Exists(path2))
                {
                    // Seaches all around the Data directory
                    var list = DataDirectoryList;
                    if (list.Any(t => Path.GetFileName(t.ToLower()).Contains(name.ToLower())))
                    {
                        list = list.Where(t => Path.GetFileName(t.ToLower()).Contains(name.ToLower())).ToArray();
                        if (list.Any(t => t.ToLower().Contains(Editor.Instance.SelectedZone)))
                            path2 = list.Where(t => t.ToLower().Contains(Editor.Instance.SelectedZone)).First();
                        else
                            path2 = list.First();
                    }
                }
                if (!File.Exists(path2))
                {
                    // No animation found
                    return null;
                }
            }

            return path2;
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
            return filteredOut;
        }

        // allow derived types to override the draw
        public virtual void Draw(DevicePanel d)
        {
            List<string> entityRenderList = entityRenderingObjects;
            List<string> onScreenExlusionList = renderOnScreenExlusions;
            if (Properties.Settings.Default.DisableRenderExlusions)
            {
                onScreenExlusionList = new List<string>();
            }


            if (filteredOut) return;


            if (Properties.Settings.Default.AlwaysRenderObjects == false && !onScreenExlusionList.Contains(entity.Object.Name.Name))
            {
                //This causes some objects not to load ever, which is problamatic, so I made a toggle(and a list as of recently). It can also have some performance benifits
                if (!d.IsObjectOnScreen(entity.Position.X.High, entity.Position.Y.High, NAME_BOX_WIDTH, NAME_BOX_HEIGHT)) return;
            }
            System.Drawing.Color color = Selected ? System.Drawing.Color.MediumPurple : System.Drawing.Color.MediumTurquoise;
            System.Drawing.Color color2 = System.Drawing.Color.DarkBlue;
            if (HasFilter(1) || HasFilter(5))
            {
                 color2 = System.Drawing.Color.DarkBlue;
            }
            else if (HasFilter(2))
            {
                 color2 = System.Drawing.Color.DarkRed;
            }
            else if (HasFilter(4))
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
                LoadNextAnimation();
            }
            int x = entity.Position.X.High;
            int y = entity.Position.Y.High;
            bool fliph = false;
            bool flipv = false;
            bool rotate = false;
            var offset = GetRotationFromAttributes(ref fliph, ref flipv, ref rotate);
            string name = entity.Object.Name.Name;
            var editorAnim = LoadAnimation2(name, d, -1, -1, fliph, flipv, rotate);
            if (entityRenderList.Contains(name))
            {
                if (!Properties.Settings.Default.NeverLoadEntityTextures)
                {
                    if ((d.IsObjectOnScreen(entity.Position.X.High, entity.Position.Y.High, NAME_BOX_WIDTH, NAME_BOX_HEIGHT) || onScreenExlusionList.Contains(entity.Object.Name.Name)) && Properties.Settings.Default.UseAltEntityRenderMode)
                    {
                        DrawOthers(d);
                    }
                    else if (!Properties.Settings.Default.UseAltEntityRenderMode) {
                        DrawOthers(d);
                    }

                }

            }
            else if (editorAnim != null && editorAnim.Frames.Count > 0)
            {
                // Special cases that always display a set frame(?)
                if (Editor.Instance.ShowAnimations.Enabled == true)
                {
                    if (entity.Object.Name.Name == "StarPost")
                        index = 1;
                    //else if (entity.Object.Name.Name == "SpecialRing")
                    //    index = 9;
                }



                // Just incase
                if (index >= editorAnim.Frames.Count)
                    index = 0;
                var frame = editorAnim.Frames[index];

                if (entity.attributesMap.ContainsKey("frameID"))
                    frame = GetFrameFromAttribute(editorAnim, entity.attributesMap["frameID"]);
                
                if (frame != null)
                {
                    ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);
                    // Draw the normal filled Rectangle but Its visible if you have the entity selected
                        d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX + ((int)offset.X * frame.Frame.Width), y + frame.Frame.CenterY + ((int)offset.Y * frame.Frame.Height),
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
                else
                { // No frame to render
                    d.DrawRectangle(x, y, x + NAME_BOX_WIDTH, y + NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color));
                }
                // Draws the Special Objects
            }
            else
            {
                if (d.IsObjectOnScreen(entity.Position.X.High, entity.Position.Y.High, NAME_BOX_WIDTH, NAME_BOX_HEIGHT))
                {
                    d.DrawRectangle(x, y, x + NAME_BOX_WIDTH, y + NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color));
                }
            }

            if (d.IsObjectOnScreen(entity.Position.X.High, entity.Position.Y.High, NAME_BOX_WIDTH, NAME_BOX_HEIGHT))
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

        public EditorAnimation.EditorFrame GetFrameFromAttribute(EditorAnimation anim, AttributeValue attribute, string key = "frameID")
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

        /// <summary>
        /// Handles animation timing
        /// </summary>
        /// <param name="speed">Speed</param>
        /// <param name="frameCount">The total amount of frames</param>
        public void ProcessAnimation(int speed, int frameCount, int duration, int startFrame = 0)
        {
            // Playback
            if (Editor.Instance.ShowAnimations.Checked && Properties.EditorState.Default.annimationsChecked)
            {
                if (speed > 0)
                {
                    int speed1 = speed * 64 / (duration == 0 ? 256 : duration);
                    if (speed1 == 0)
                        speed1 = 1;
                    if ((DateTime.Now - lastFrametime).TotalMilliseconds > 1024 / speed1)
                    {
                        index++;
                        lastFrametime = DateTime.Now;
                    }
                }
            }
            else index = 0 + startFrame;
            if (index >= frameCount)
                index = 0;

        }

        public void ProcessMovingPlatform(int angleDefault, int speed = 3)
        {
            int duration = 1;
            // Playback
            if (Editor.Instance.ShowAnimations.Checked && Properties.EditorState.Default.movingPlatformsChecked)
            {
                if (speed > 0)
                {
                    int speed1 = speed * 64 / (duration == 0 ? 256 : duration);
                    if (speed1 == 0)
                        speed1 = 1;
                    if ((DateTime.Now - lastFrametime).TotalMilliseconds > 1024 / speed1)
                    {
                        platformAngle++;
                        lastFrametime = DateTime.Now;
                    }
                }
            }
            else platformAngle = angleDefault;
            if (platformAngle >= 768)
                platformAngle = 0;

        }

        public void ProcessMovingPlatform2(int ampX, int ampY, int speed = 3)
        {
            int duration = 1;
            // Playback
            if (Editor.Instance.ShowAnimations.Checked && Properties.EditorState.Default.movingPlatformsChecked)
            {
                if (speed > 0)
                {
                    int speed1 = speed * 64 / (duration == 0 ? 256 : duration);
                    if (speed1 == 0)
                        speed1 = 1;
                    if ((DateTime.Now - lastFrametime).TotalMilliseconds > 1024 / speed1)
                    {
                        if (platformreverse) {
                            if (ampX != 0 && !platformdisableX) platformpositionX--;
                            if (ampY != 0 && !platformdisableY) platformpositionY--;
                            if (platformpositionX >= ampX / 2 || platformpositionX <= -(ampX / 2))
                            {
                                platformdisableX = true;
                            }
                            if (platformpositionY == ampY)
                            {
                                platformdisableY = true;
                            }
                            if (platformpositionY == ampY && platformpositionX == ampX)
                            {
                                platformreverse = true;
                                platformdisableX = false;
                                platformdisableY = false;
                            }
                        }
                        else
                        {
                            if (ampX != 0 && !platformdisableX) platformpositionX++;
                            if (ampY != 0 && !platformdisableY) platformpositionY++;
                            if (platformpositionX >= ampX / 2 || platformpositionX <= -(ampX / 2))
                            {
                                platformdisableX = true;
                            }
                            if (platformpositionY == ampY)
                            {
                                platformdisableY = true;
                            }
                            if (platformpositionY == ampY && platformpositionX == ampX)
                            {
                                platformreverse = true;
                                platformdisableX = false;
                                platformdisableY = false;
                            }
                        }


                        lastFrametime = DateTime.Now;
                    }
                }
            }
            else
            {
                platformpositionX = 0;
                platformpositionY = 0;
            }


        }

        // These are special
        public void DrawOthers(DevicePanel d)
        {
            int x = entity.Position.X.High;
            int y = entity.Position.Y.High;
            int Transparency = (Editor.Instance.EditLayer == null) ? 0xff : 0x32;
            if (entity.Object.Name.Name.Contains("Setup"))
            {
                EntityRenderer renderer = EntityRenderers.Where(t => t.GetObjectName() == "ZoneSetup").FirstOrDefault();
                if (renderer != null)
                    renderer.Draw(d, entity, this, x, y, Transparency);
            }
            else if (entity.Object.Name.Name.Contains("Intro") || entity.Object.Name.Name.Contains("Outro"))
            {
                EntityRenderer renderer = EntityRenderers.Where(t => t.GetObjectName() == "Outro_Intro_Object").FirstOrDefault();
                if (renderer != null)
                    renderer.Draw(d, entity, this, x, y, Transparency);
            }
            else
            {
                EntityRenderer renderer = EntityRenderers.Where(t => t.GetObjectName() == entity.Object.Name.Name).FirstOrDefault();
                if (renderer != null)
                    renderer.Draw(d, entity, this, x, y, Transparency);
            }


        }


        public bool HasFilter()
        {
            return entity.attributesMap.ContainsKey("filter") && entity.attributesMap["filter"].Type == AttributeTypes.UINT8;
        }

        public bool HasFilter(uint input, bool checkHigher = false)
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

        public static void ReleaseResources()
        {
            DataDirectoryList = null;

            foreach (var pair in Sheets)
                pair.Value?.Dispose();
            Sheets.Clear();

            foreach (var pair in Animations)
                foreach (var pair2 in pair.Value.Frames)
                    pair2.Texture?.Dispose();

            Animations.Clear();
        }

        public class EditorAnimation
        {
            public int loadedFrames = 0;
            public bool Ready = false;
            public bool ImageLoaded = false;
            public List<EditorFrame> Frames = new List<EditorFrame>();
            public class EditorFrame
            {
                public Texture Texture;
                public Animation.Frame Frame;
                public Animation.AnimationEntry Entry;
                public Bitmap _Bitmap;
            }
        }

    }
}
