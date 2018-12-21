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
using ManiacEditor.Entity_Renders;
using SystemColor = System.Drawing.Color;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using ImageMagick;

namespace ManiacEditor
{
    public class EditorEntity_ini
    {
        // Object Render List
        public static List<EntityRenderer> EntityRenderers = new List<EntityRenderer>();

        // Object List for initilizing the if statement
        public static List<string> entityRenderingObjects = Editor.Instance.entityRenderingObjects;
        public static List<string> renderOnScreenExlusions = Editor.Instance.renderOnScreenExlusions;

        public static List<EditorEntity_ini.LoadAnimationData> AnimsToLoad = new List<EditorEntity_ini.LoadAnimationData>();

        public static Dictionary<string, EditorEntity_ini.EditorAnimation> Animations = new Dictionary<string, EditorEntity_ini.EditorAnimation>();
        public static Dictionary<string, EditorEntity_ini.EditorTilePlatforms> TilePlatforms = new Dictionary<string, EditorEntity_ini.EditorTilePlatforms>();
        public static Dictionary<string, Bitmap> Sheets = new Dictionary<string, Bitmap>();
        public static bool Working = false;

        public static List<string> getEntityInternalList(int type)
        {
            if (type == 1) //For the list of objects with renders
            {
                var resourceName = "ManiacEditor.Resources.objectRenderList.ini";
                var assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    return EnumerateLines(reader).ToList<string>();
                }
            }
            else //For On Screen Render Exlusions (So we can make our own rules in the object's render file)
            {
                var resourceName = "ManiacEditor.Resources.onScreenRenderExclusions.ini";
                var assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    return EnumerateLines(reader).ToList<string>();
                }
            }

        }
        public static List<string> getEntityExternalList(int type)
        {
            if (type == 1) //For the list of objects with renders
            {
                var path = Path.Combine(Environment.CurrentDirectory, "Resources\\objectRenderList.ini");
                using (StreamReader reader = new StreamReader(path))
                {
                    return EnumerateLines(reader).ToList<string>();
                }
            }
            else //For On Screen Render Exlusions (So we can make our own rules in the object's render file)
            {
                var path = Path.Combine(Environment.CurrentDirectory, "Resources\\onScreenRenderExclusions.ini");
                using (StreamReader reader = new StreamReader(path))
                {
                    return EnumerateLines(reader).ToList<string>();
                }
            }

        }

        public static List<string> getSpecialRenderList(int type)
        {
            if (type == 1) //For the list of objects with renders
            {
                List<string> entityRenderListInternal = getEntityInternalList(1); // Get the list embeded in the editor
                List<string> entityRenderListExternal = getEntityExternalList(1); // Get the list the user is allowed to edit
                if (entityRenderListExternal != entityRenderListInternal)
                {
                    return entityRenderListExternal;
                }
                else
                {
                    return entityRenderListInternal;
                }
            }
            else //For On Screen Render Exlusions (So we can make our own rules in the object's render file)
            {
                List<string> entityRenderListInternal = getEntityInternalList(0); // Get the list embeded in the editor
                List<string> entityRenderListExternal = getEntityExternalList(0); // Get the list the user is allowed to edit
                if (entityRenderListExternal != entityRenderListInternal)
                {
                    return entityRenderListExternal;
                }
                else
                {
                    return entityRenderListInternal;
                }
            }



        }

        public static IEnumerable<string> EnumerateLines(TextReader reader)
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }


        public static Animation rsdkAnim;

        //For Drawing/Saving Tile Platforms


        public static string[] DataDirectoryList = null;

        public static void LoadNextAnimation(EditorEntity entity)
        {
            if (AnimsToLoad.Count == 0)
                return;
            if (AnimsToLoad.Count < 1)
            {
                //Break Here
            }
            var val = AnimsToLoad[0];
            if (val.anim == null)
            {
                string key = $"{val.name}-{val.AnimId}-{val.frameId}-{val.fliph}-{val.flipv}-{val.rotate}-{val.rotateImg}-{val.legacyRotation}";
                if (!Animations.ContainsKey(key))
                {
                    if (!Working)
                    {
                        try
                        {                           
                            LoadAnimation(val.name, val.d, val.AnimId, val.frameId, val.fliph, val.flipv, val.rotate, val.rotateImg, false, val.legacyRotation);
                            entity.uniqueKey = $"{val.name}-{val.AnimId}-{val.frameId}-{val.fliph}-{val.flipv}-{val.rotate}-{val.rotateImg}-{val.legacyRotation}";
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
            {
                return;
            }
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
                val.anim.Frames[val.anim.loadedFrames]._Bitmap = null;
                ++val.anim.loadedFrames;
                if (val.anim.loadedFrames == val.anim.Frames.Count)
                {
                    val.anim.Ready = true;
                    AnimsToLoad.RemoveAt(0);
                }

            }
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
        public static EditorAnimation LoadAnimation2(string name, DevicePanel d, int AnimId, int frameId, bool fliph, bool flipv, bool rotate, int rotateImg = 0, bool legacyRotation = false)
        {
            string key = $"{name}-{AnimId}-{frameId}-{fliph}-{flipv}-{rotate}-{rotateImg}-{legacyRotation}";
            if (EditorEntity_ini.Animations.ContainsKey(key))
            {
                if (EditorEntity_ini.Animations[key].Ready)
                {
                    // Use the already loaded Amination
                    return EditorEntity_ini.Animations[key];
                }
                else
                    return null;
            }
            var entry = new EditorEntity_ini.LoadAnimationData()
            {
                name = name,
                d = d,
                AnimId = AnimId,
                frameId = frameId,
                fliph = fliph,
                flipv = flipv,
                rotate = rotate,
                rotateImg = rotateImg,
                legacyRotation = legacyRotation
            };
            EditorEntity_ini.AnimsToLoad.Add(entry);
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
        public static EditorAnimation LoadAnimation(string name, DevicePanel d, int AnimId, int frameId, bool fliph, bool flipv, bool rotate, int rotateImg, bool loadImageToDX = true, bool legacyRotate = true)
        {
            string key = $"{name}-{AnimId}-{frameId}-{fliph}-{flipv}-{rotate}-{rotateImg}-{legacyRotate}";
            var anim = new EditorEntity_ini.EditorAnimation();
            if (EditorEntity_ini.Animations.ContainsKey(key))
            {   
                if (EditorEntity_ini.Animations[key].Ready) return EditorEntity_ini.Animations[key]; // Use the already loaded Amination
                else return null;
            }

            EditorEntity_ini.Animations.Add(key, anim);

            // Get the path of the object's textures
            string assetName = (Editor.Instance.userDefinedEntityRenderSwaps.Keys.Contains(name) ? Editor.Instance.userDefinedEntityRenderSwaps[name] : name);
            var assetInfo = GetAssetPath(assetName, (Editor.ModDataDirectory != "" ? Editor.ModDataDirectory : ""), false, (Editor.ModDataDirectory != "" ? true : false));

            string path2 = assetInfo.Item1;
            string dataFolderLocation = assetInfo.Item2;         
            if (!File.Exists(path2) || path2 == null) return null;

            using (var stream = File.OpenRead(path2))
            {
                rsdkAnim = new Animation();
                rsdkAnim.Load(new BinaryReader(stream));
            }
            if (AnimId == -1)
            {
                if (rsdkAnim.Animations.Any(t => t.AnimName.Contains("Normal"))) AnimId = rsdkAnim.Animations.FindIndex(t => t.AnimName.Contains("Normal"));
                else AnimId = 0;
                // Use Vertical Amination if one exists
                if (rotate && rsdkAnim.Animations.Any(t => t.AnimName.EndsWith(" V"))) AnimId = rsdkAnim.Animations.FindIndex(t => t.AnimName.EndsWith(" V"));
            }
            if (AnimId == -2)
            {
                if (rsdkAnim.Animations.Any(t => t.AnimName.Contains("Swing"))) AnimId = rsdkAnim.Animations.FindIndex(t => t.AnimName.Contains("Swing"));
                else AnimId = 0;
            }
            if (AnimId >= rsdkAnim.Animations.Count) AnimId = rsdkAnim.Animations.Count - 1;
            for (int i = 0; i < rsdkAnim.Animations[AnimId].Frames.Count; ++i)
            {
                // check we don't stray outside our loaded animations/frames
                // if user enters a value that would take us there, just show
                // a valid frame instead
                var animiation = rsdkAnim.Animations[AnimId];
                var frame = animiation.Frames[i];
                if (frameId >= 0 && frameId < animiation.Frames.Count) frame = animiation.Frames[frameId];
                Bitmap map;
                bool noEncoreColors = false;
                if (assetName == "EditorAssets" || assetName == "SuperSpecialRing" || assetName == "EditorIcons2" || assetName == "TransportTubes") noEncoreColors = true;

                if (!EditorEntity_ini.Sheets.ContainsKey(rsdkAnim.SpriteSheets[frame.SpriteSheet]))
                {
                    string targetFile;

                    if (assetName == "EditorAssets" || assetName == "SuperSpecialRing" || assetName == "EditorIcons2" || assetName == "TransportTubes")
                    {
                        if (assetName == "EditorAssets") targetFile = Path.Combine(Environment.CurrentDirectory, "Global\\", "EditorAssets.gif");
                        else if (assetName == "EditorIcons2") targetFile = Path.Combine(Environment.CurrentDirectory, "Global\\", "EditorIcons2.gif");
                        else if (assetName == "TransportTubes") targetFile = Path.Combine(Environment.CurrentDirectory, "Global\\", "TransportTubes.gif");
                        else targetFile = Path.Combine(Environment.CurrentDirectory, "Global\\", "SuperSpecialRing.gif");
                    }
                    else targetFile = Path.Combine(dataFolderLocation, "Sprites", rsdkAnim.SpriteSheets[frame.SpriteSheet].Replace('/', '\\'));
                    if (!File.Exists(targetFile))
                    {
                        map = null;
                        // add a Null to our lookup, so we can avoid looking again in the future
                        EditorEntity_ini.Sheets.Add(rsdkAnim.SpriteSheets[frame.SpriteSheet], map);
                    }
                    else
                    {

                        map = new Bitmap(targetFile);
                        //Encore Colors
                        if (Editor.Instance.useEncoreColors && noEncoreColors == false && (frame.Width != 0 || frame.Height != 0)) map = SetEncoreColors(map, Editor.EncorePalette[0]);
                        EditorEntity_ini.Sheets.Add(rsdkAnim.SpriteSheets[frame.SpriteSheet], map);
                    }
                }
                else
                {
                    map = EditorEntity_ini.Sheets[rsdkAnim.SpriteSheets[frame.SpriteSheet]];
                    //Encore Colors
                    if (Editor.Instance.useEncoreColors && noEncoreColors == false && (frame.Width != 0 || frame.Height != 0)) map = SetEncoreColors(map, Editor.EncorePalette[0]);
                }


                if (frame.Width == 0 || frame.Height == 0) continue;

                // can't load the animation, it probably doesn't exist in the User's Sprites folder
                if (map == null) return null;

                // We are storing the first colour from the palette so we can use it to make sprites transparent
                var colour = map.Palette.Entries[0];
                // Slow


                map = CropImage(map, new Rectangle(frame.X, frame.Y, frame.Width, frame.Height), fliph, flipv, colour, rotateImg, rotate, legacyRotate);
                if (rotateImg != 0 && legacyRotate)
                {
                    map = RotateImageLegacy(map, rotateImg, colour);
                    frame.Height = frame.Width + frame.Height + 64;
                    frame.Width = frame.Height + frame.Width + 32;
                }



                RemoveColourImage(map, colour, map.Width, map.Height);

                Texture texture = null;
                if (loadImageToDX)
                {
                    texture = TextureCreator.FromBitmap(d._device, map);
                }
                
                var editorFrame = new EditorEntity_ini.EditorAnimation.EditorFrame()
                {
                    Texture = texture,
                    Frame = frame,
                    Entry = rsdkAnim.Animations[AnimId],
                    ImageWidth = map.Size.Width,
                    ImageHeight = map.Size.Height
                    
                    
                };
                if (loadImageToDX == false) editorFrame._Bitmap = map;
                anim.Frames.Add(editorFrame);
                if (frameId != -1) break;
            }
            anim.ImageLoaded = true;
            if (loadImageToDX) anim.Ready = true;
            Working = false;
            return anim;

        }

        public static EditorTilePlatforms LoadTilePlatform(DevicePanel d, int x2, int y2, int width, int height)
        {

            SceneLayer _layer = Editor.Instance.EditorScene?.Move.Layer;
            string key = $"{x2}-{y2}-{width}-{height}";
            var anim = new EditorEntity_ini.EditorTilePlatforms();
            if (EditorEntity_ini.TilePlatforms.ContainsKey(key))
            {
                if (EditorEntity_ini.TilePlatforms[key].Ready)
                {
                    // Use the already loaded Amination
                    return EditorEntity_ini.TilePlatforms[key];
                }
                else
                    return null;
            }
            EditorEntity_ini.TilePlatforms.Add(key, anim);
            Texture GroupTexture;
            Rectangle rect = GetTilePlatformArea(x2 * 16, y2 * 16, height * 16, width * 16);
            try
            {

                using (Bitmap bmp = new Bitmap(_layer.Width * 16, _layer.Height * 16, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        for (int tx = 0; tx <= x2 + width; ++tx)
                        {
                            for (int ty = 0; ty <= y2 + height; ++ty)
                            {
                                // We will draw those later
                                if (_layer.Tiles?[ty][tx] != 0xffff)
                                {
                                    DrawObjectTile(g, _layer.Tiles[ty][tx], tx - x2 + width / 2, ty - y2 + height / 2);
                                }
                            }
                        }
                    }
                    GroupTexture = TextureCreator.FromBitmap(d._device, bmp);
                    anim.Texture = GroupTexture;
                    if (GroupTexture != null)
                    {
                        anim.Ready = true;
                    }
                    anim.Height = bmp.Height;
                    anim.Width = bmp.Width;
                    return anim;
                }
            }
            catch
            {
                return null;
            }
        }

        public static void DrawObjectTile(Graphics g, ushort tile, int x, int y)
        {
            ushort TileIndex = (ushort)(tile & 0x3ff);
            int TileIndexInt = (int)TileIndex;
            bool flipX = ((tile >> 10) & 1) == 1;
            bool flipY = ((tile >> 11) & 1) == 1;
            bool SolidTopA = ((tile >> 12) & 1) == 1;
            bool SolidLrbA = ((tile >> 13) & 1) == 1;
            bool SolidTopB = ((tile >> 14) & 1) == 1;
            bool SolidLrbB = ((tile >> 15) & 1) == 1;

            g.DrawImage(Editor.Instance.StageTiles.Image.GetBitmap(new Rectangle(0, TileIndex * 16, 16, 16), flipX, flipY),
                new Rectangle(x * 16, y * 16, 16, 16));
        }

        private static Rectangle GetTilePlatformArea(int x, int y, int width, int height)
        {
            return new Rectangle(x, y, width, height);
        }

        public static Tuple<String, String> GetAssetPath(string name, string CustomDataDirectoryLocation = "", bool dontSeachCustom = false, bool isModLoaded = false)
        {
            string path, path2;
            string dataDirectory = (CustomDataDirectoryLocation != "" ? CustomDataDirectoryLocation : Editor.DataDirectory);
            if (name == "EditorAssets" || name == "SuperSpecialRing" || name == "EditorIcons2" || name == "TransportTubes")
            {
                switch (name)
                {
                    case "EditorAssets":
                        path2 = Path.Combine(Environment.CurrentDirectory, "Global\\", "EditorAssets.bin");
                        if (!File.Exists(path2)) return null;
                        break;
                    case "EditorIcons2":
                        path2 = Path.Combine(Environment.CurrentDirectory, "Global\\", "EditorIcons2.bin");
                        if (!File.Exists(path2)) return null;
                        break;
                    case "TransportTubes":
                        path2 = Path.Combine(Environment.CurrentDirectory, "Global\\", "TransportTubes.bin");
                        if (!File.Exists(path2)) return null;
                        break;
                    case "SuperSpecialRing":
                        path2 = Path.Combine(Environment.CurrentDirectory, "Global\\", "SuperSpecialRing.bin");
                        if (!File.Exists(path2)) return null;
                        break;
                    default:
                        return null;
                }
            }
            else
            {
                if (DataDirectoryList == null) DataDirectoryList = Directory.GetFiles(Path.Combine(dataDirectory, "Sprites"), $"*.bin", SearchOption.AllDirectories);


                // Checks the Stage Folder First

                path = Editor.Instance.SelectedZone + "\\" + name + ".bin";
                path2 = Path.Combine(dataDirectory, "Sprites") + '\\' + path;
                if (Editor.Instance.userDefinedSpritePaths != null && Editor.Instance.userDefinedSpritePaths.Count != 0)
                {
                    foreach (string userDefinedPath in Editor.Instance.userDefinedSpritePaths)
                    {
                        path = userDefinedPath + "\\" + name + ".bin";
                        path2 = Path.Combine(dataDirectory, "Sprites") + '\\' + path;
                        Debug.Print(path2);
                        if (File.Exists(path2))
                        {
                            break;
                        }
                    }
                    if (!File.Exists(path2))
                    {
                        path = Editor.Instance.SelectedZone + "\\" + name + ".bin";
                        path2 = Path.Combine(dataDirectory, "Sprites") + '\\' + path;
                    }
                }


                if (!File.Exists(path2))
                {

                    // Checks without last character
                    path = path = Editor.Instance.SelectedZone.Substring(0, Editor.Instance.SelectedZone.Length - 1) + "\\" + name + ".bin";
                    path2 = Path.Combine(dataDirectory, "Sprites") + '\\' + path;
                    if (!File.Exists(path2))
                    {
                        // Checks for name without the last character and without the numbers in the entity name
                        string adjustedName = new String(name.Where(c => c != '-' && (c < '0' || c > '9')).ToArray());
                        path = path = Editor.Instance.SelectedZone.Substring(0, Editor.Instance.SelectedZone.Length - 1) + "\\" + adjustedName + ".bin";
                        path2 = Path.Combine(dataDirectory, "Sprites") + '\\' + path;
                        if (!File.Exists(path2))
                        {
                            // Checks for name without any numbers in the Zone name
                            string adjustedZone = Regex.Replace(Editor.Instance.SelectedZone, @"[\d-]", string.Empty);
                            path = path = adjustedZone + "\\" + name + ".bin";
                            path2 = Path.Combine(dataDirectory, "Sprites") + '\\' + path;
                            if (!File.Exists(path2))
                            {
                                // Checks for name without any numbers in the Zone name, then add a 1 back
                                adjustedZone = adjustedZone + "1";
                                path = path = adjustedZone + "\\" + name + ".bin";
                                path2 = Path.Combine(dataDirectory, "Sprites") + '\\' + path;
                                if (!File.Exists(path2))
                                {
                                    // Checks Global
                                    path = "Global\\" + name + ".bin";
                                    path2 = Path.Combine(dataDirectory, "Sprites") + '\\' + path;

                                    if (isModLoaded && !File.Exists(path2) && !dontSeachCustom)
                                    {
                                        var masterDataDetails = GetAssetPath(name, Editor.DataDirectory, true, true);
                                        if (File.Exists(masterDataDetails.Item1))
                                        {
                                            path2 = masterDataDetails.Item1;
                                            dataDirectory = masterDataDetails.Item2;
                                        }
                                    }
                                    else if (!File.Exists(path2) && !dontSeachCustom)
                                    {
                                        var masterDataDetails = GetAssetPath(name, Editor.MasterDataDirectory, true);
                                        if (File.Exists(masterDataDetails.Item1))
                                        {
                                            path2 = masterDataDetails.Item1;
                                            dataDirectory = masterDataDetails.Item2;
                                        }
                                    }
                                    else if (!File.Exists(path2))
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
                                        if (!File.Exists(path2))
                                        {
                                            // No animation found
                                            path2 = null;
                                            dataDirectory = null;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }                           
            }

            return Tuple.Create(path2, dataDirectory);
        }

        public static Bitmap CropImage(Bitmap source, Rectangle section, bool fliph, bool flipv, SystemColor colour, int rotateImg = 0, bool rotate = false, bool legacyRotate = false)
        {
            Bitmap bmp2 = new Bitmap(section.Size.Width, section.Size.Height);
            using (Graphics g = Graphics.FromImage(bmp2)) g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
            if (fliph && flipv) bmp2.RotateFlip(RotateFlipType.RotateNoneFlipXY);
            else if (fliph) bmp2.RotateFlip(RotateFlipType.RotateNoneFlipX);
            else if (flipv) bmp2.RotateFlip(RotateFlipType.RotateNoneFlipY);
            if (rotate && !legacyRotate) bmp2 = RotateImage(bmp2, rotateImg, colour);




            // AH-HA! The Memory Issue lies here, the larger the bitmap, the more unused memory we have. (UPDATE: Inital Fix to the Problem)
            var squareSize = (bmp2.Width > bmp2.Height ? bmp2.Width : bmp2.Height);
            int factor = 32;
            int newSize = (int)Math.Round((squareSize / (double)factor), MidpointRounding.AwayFromZero) * factor;
            if (newSize == 0) newSize = factor;
            while (newSize < squareSize) newSize += factor;

            Bitmap bmp = new Bitmap(newSize, newSize);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                if (rotate && !legacyRotate) g.DrawImage(bmp2, bmp.Width/2 - bmp2.Width/2, bmp.Height/2 - bmp2.Height / 2, new Rectangle(0, 0, bmp2.Width, bmp2.Height), GraphicsUnit.Pixel);
                else g.DrawImage(bmp2, 0, 0, new Rectangle(0, 0, bmp2.Width, bmp2.Height), GraphicsUnit.Pixel);

            }
            bmp2.Dispose();
            //bmp.Save(Environment.CurrentDirectory + "//Images" + "//" + name + (rotateImg != 0 ? "_" + rotateImg : "") + (frameID != -1 ? "_" + frameID : "") + (animID != -1 ? "_" + animID : "") + ".gif");
            return bmp;

        }

        public static Bitmap RotateImage(Bitmap img, double rotationAngle, SystemColor colour)
        {
            // I don't know who though it was a good idea to disable this, but it is essential for rotating textures
            img.MakeTransparent(colour);
            MagickImage image = new MagickImage(img);
            image.RePage();
            image.BackgroundColor =  SystemColor.Transparent;
            image.Interpolate = PixelInterpolateMethod.Nearest;
            image.Rotate(rotationAngle);
            image.RePage();               
            Bitmap bmp = image.ToBitmap();
            image.Dispose();            
            return bmp;
            
        }

        public static Bitmap RotateImageLegacy(Bitmap img, double rotationAngle, SystemColor colour)
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
                gfx.RotateTransform((float)rotationAngle);

                //move the point system origin back to 0,0
                gfx.TranslateTransform(-pointX, -pointY);

                //set the InterpolationMode to HighQualityBicubic so to ensure a high
                //quality image once it is transformed to the specified size
                gfx.InterpolationMode = InterpolationMode.NearestNeighbor;

                //draw our new image onto the graphics object with its center on the center of rotation
                gfx.DrawImage(img, new PointF(pointX, pointY));
            }
            return bmp;
        }

        public static Bitmap RemoveColourImage(Bitmap source, System.Drawing.Color colour, int width, int height)
        {
            source.MakeTransparent(colour);
            return source;
        }

        private static Bitmap SetEncoreColors(Bitmap _bitmap, string encoreColors = null)
        {
            Bitmap _bitmapEditMemory;
            _bitmapEditMemory = _bitmap.Clone(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height), PixelFormat.Format8bppIndexed);

            //Encore Palettes (WIP Potentially Improvable)
            RSDKv5.Color[] readableColors = new RSDKv5.Color[256];
            bool loadSpecialColors = false;
            if (encoreColors != null && File.Exists(encoreColors))
            {
                using (var stream = File.OpenRead(encoreColors))
                {
                    for (int y = 0; y < 255; ++y)
                    {
                        readableColors[y].R = (byte)stream.ReadByte();
                        readableColors[y].G = (byte)stream.ReadByte();
                        readableColors[y].B = (byte)stream.ReadByte();
                    }
                }
                loadSpecialColors = true;
            }

            if (loadSpecialColors == true)
            {
                ColorPalette pal = _bitmapEditMemory.Palette;
                if (_bitmapEditMemory.Palette.Entries.Length == 256)
                {
                    for (int y = 0; y < 255; ++y)
                    {
                        if (readableColors[y].R != 255 && readableColors[y].G != 0 && readableColors[y].B != 255)
                        {
                            pal.Entries[y] = SystemColor.FromArgb(readableColors[y].R, readableColors[y].G, readableColors[y].B);
                        }
                    }
                    _bitmapEditMemory.Palette = pal;
                }

            }
            _bitmap = _bitmapEditMemory;
            return _bitmap;
        }

        // These are special
        public static void DrawOthers(DevicePanel d, SceneEntity entity, EditorEntity e, int childX, int childY, int index, int previousChildCount, int platformAngle, EditorAnimations EditorAnimations, bool Selected, AttributeValidater AttributeValidater, bool childDrawAddMode)
        {
            int x = entity.Position.X.High + childX;
            int y = entity.Position.Y.High + childY;
            if (childDrawAddMode == false)
            {
                x = childX;
                y = childY;
            }
            int Transparency = (Editor.Instance.EditLayer == null) ? 0xff : 0x32;
            if (entity.Object.Name.Name.Contains("Setup"))
            {
                EntityRenderer renderer = EditorEntity_ini.EntityRenderers.Where(t => t.GetObjectName() == "ZoneSetup").FirstOrDefault();
                if (renderer != null)
                    renderer.Draw(d, entity, null, x, y, Transparency, index, previousChildCount, platformAngle, EditorAnimations, Selected, AttributeValidater);
            }
            else if (entity.Object.Name.Name.Contains("Intro") || entity.Object.Name.Name.Contains("Outro"))
            {
                EntityRenderer renderer = EditorEntity_ini.EntityRenderers.Where(t => t.GetObjectName() == "Outro_Intro_Object").FirstOrDefault();
                if (renderer != null)
                    renderer.Draw(d, entity, null, x, y, Transparency, index, previousChildCount, platformAngle, EditorAnimations, Selected, AttributeValidater);
            }
            else if (entity.Object.Name.Name.Contains("TornadoPath") || entity.Object.Name.Name.Contains("AIZTornadoPath"))
            {
                EntityRenderer renderer = EditorEntity_ini.EntityRenderers.Where(t => t.GetObjectName() == "TornadoPath").FirstOrDefault();
                if (renderer != null)
                    renderer.Draw(d, entity, null, x, y, Transparency, index, previousChildCount, platformAngle, EditorAnimations, Selected, AttributeValidater);
            }
            else
            {
                EntityRenderer renderer = EditorEntity_ini.EntityRenderers.Where(t => t.GetObjectName() == entity.Object.Name.Name).FirstOrDefault();
                if (renderer != null)
                    renderer.Draw(d, entity, null, x, y, Transparency, index, previousChildCount, platformAngle, EditorAnimations, Selected, AttributeValidater);
            }

        }

        [Serializable]
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
                public int ImageWidth;
                public int ImageHeight;
            }
        }

        [Serializable]
        public class EditorTilePlatforms
        {
            public bool Ready = false;
            public int Width = 0;
            public int Height = 0;
            public Texture Texture;
        }

        [Serializable]
        public class LoadAnimationData
        {
            public string name;
            public DevicePanel d;
            public int AnimId, frameId;
            public bool fliph, flipv, rotate, legacyRotation;
            public int rotateImg;
            public EditorAnimation anim;
        }


        public static void ReleaseResources()
        {

            foreach (var pair in Sheets)
                pair.Value?.Dispose();
            Sheets.Clear();

            TilePlatforms.Clear();


            foreach (var pair in EditorEntity_ini.Animations)
                foreach (var pair2 in pair.Value.Frames)
                    pair2.Texture?.Dispose();

            Animations.Clear();
            TilePlatforms.Clear();
        }

    }
}
