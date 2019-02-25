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
using Microsoft.Scripting.Utils;

namespace ManiacEditor
{
    public class EditorEntity_ini
    {
        // Object Render List
        public List<EntityRenderer> EntityRenderers = new List<EntityRenderer>();
		//public ClearScript render = new ClearScript();

		// Object List for initilizing the if statement
		public List<string> entityRenderingObjects;
        public List<string> renderOnScreenExlusions;
        public List<string> rendersWithErrors = new List<string>();
        public readonly static List<string> EditorStaticObjects = new List<string> { "EditorAssets", "EditorText", "SuperSpecialRing", "EditorIcons2", "TransportTubes", "EditorUIRender" };

        public List<EditorEntity_ini.LoadAnimationData> AnimsToLoad = new List<EditorEntity_ini.LoadAnimationData>();

        public Dictionary<string, EditorEntity_ini.EditorAnimation> Animations = new Dictionary<string, EditorEntity_ini.EditorAnimation>();
        public Dictionary<string, EditorEntity_ini.EditorTilePlatforms> TilePlatforms = new Dictionary<string, EditorEntity_ini.EditorTilePlatforms>();
        public Dictionary<string, Bitmap> Sheets = new Dictionary<string, Bitmap>();
        public bool Working = false;

        public Editor EditorInstance;

        public enum Flag : int
        {
            DefaultBehavior = 0,
            FullEngineRotation = 1,
            PartialEngineRotation = 2,
            StaticRotationUsingExtraFrames = 3,
            Unknown = 4    
        }

        public EditorEntity_ini(Editor instance)
        {
            EditorInstance = instance;
            entityRenderingObjects = EditorInstance.entityRenderingObjects;
            renderOnScreenExlusions = EditorInstance.renderOnScreenExlusions;
        }

        public static List<string> GetEntityInternalList(int type)
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
        public static List<string> GetEntityExternalList(int type)
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

        public static List<string> GetSpecialRenderList(int type)
        {
            if (type == 1) //For the list of objects with renders
            {
                List<string> entityRenderListInternal = GetEntityInternalList(1); // Get the list embeded in the editor
                List<string> entityRenderListExternal = GetEntityExternalList(1); // Get the list the user is allowed to edit
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
                List<string> entityRenderListInternal = GetEntityInternalList(0); // Get the list embeded in the editor
                List<string> entityRenderListExternal = GetEntityExternalList(0); // Get the list the user is allowed to edit
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

        public void LoadNextAnimation(EditorEntity entity)
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
        public EditorAnimation LoadAnimation2(string name, DevicePanel d, int AnimId, int frameId, bool fliph, bool flipv, bool rotate, int rotateImg = 0, bool legacyRotation = true)
        {
            string key = $"{name}-{AnimId}-{frameId}-{fliph}-{flipv}-{rotate}-{rotateImg}-{legacyRotation}";
            if (EditorInstance.EditorEntity_ini.Animations.ContainsKey(key))
            {
                if (EditorInstance.EditorEntity_ini.Animations[key].Ready)
                {
                    // Use the already loaded Amination
                    return EditorInstance.EditorEntity_ini.Animations[key];
                }
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
            EditorInstance.EditorEntity_ini.AnimsToLoad.Add(entry);
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
        public EditorAnimation LoadAnimation(string name, DevicePanel d, int AnimId, int frameId, bool fliph, bool flipv, bool rotate, int rotateImg = 0, bool loadImageToDX = true, bool legacyRotate = true, bool PartialEngineRotation = false, bool FullEngineRotation = false, bool stacker = false)
        {

            string key = $"{name}-{AnimId}-{frameId}-{fliph}-{flipv}-{rotate}-{rotateImg}-{legacyRotate}";
            var anim = new EditorEntity_ini.EditorAnimation();
            if (EditorInstance.EditorEntity_ini.Animations.ContainsKey(key))
            {   
                if (Animations[key].Ready) return Animations[key]; // Use the already loaded Amination
                else return null;
            }

            Animations.Add(key, anim);

            // Get the path of the object's textures
            string assetName = (EditorInstance.userDefinedEntityRenderSwaps.Keys.Contains(name) ? EditorInstance.userDefinedEntityRenderSwaps[name] : name);
            var assetInfo = GetAssetPath(name);

            string path2 = assetInfo.Item1;
            string dataFolderLocation = assetInfo.Item2;
            if (!File.Exists(path2) || path2 == null) return null;


            using (var stream = File.OpenRead(path2))
            {
                rsdkAnim = new Animation(new RSDKv5.Reader(stream));
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
				Bitmap map = null;
				bool noEncoreColors = false;
                if (assetName == "EditorAssets" || assetName == "EditorText" || assetName == "SuperSpecialRing" || assetName == "EditorIcons2" || assetName == "TransportTubes" || name == "EditorUIRender") noEncoreColors = true;

                if (frame.SpriteSheet > rsdkAnim.SpriteSheets.Count) frame.SpriteSheet = (byte)(rsdkAnim.SpriteSheets.Count - 1);
                if (!Sheets.ContainsKey(rsdkAnim.SpriteSheets[frame.SpriteSheet]))
                {
                    string targetFile;

                    if (assetName == "EditorAssets" || assetName == "HUDEditorText" || assetName == "SuperSpecialRing" || assetName == "EditorIcons2" || assetName == "TransportTubes" || name == "EditorUIRender")
                    {
                        if (assetName == "EditorAssets") targetFile = Path.Combine(Environment.CurrentDirectory, "Resources\\Global\\", "EditorAssets.gif");
                        else if (assetName == "HUDEditorText") targetFile = Path.Combine(Environment.CurrentDirectory, "Resources\\Global\\", "EditorText.gif");
                        else if (assetName == "EditorIcons2") targetFile = Path.Combine(Environment.CurrentDirectory, "Resources\\Global\\", "EditorIcons2.gif");
                        else if (assetName == "TransportTubes") targetFile = Path.Combine(Environment.CurrentDirectory, "Resources\\Global\\", "TransportTubes.gif");
                        else if (assetName == "EditorUIRender") targetFile = Path.Combine(Environment.CurrentDirectory, "Resources\\Global\\", "MenuRenders.gif");
                        else targetFile = Path.Combine(Environment.CurrentDirectory, "Resources\\Global\\", "SuperSpecialRing.gif");
                    }
                    else targetFile = Path.Combine(dataFolderLocation, "Sprites", rsdkAnim.SpriteSheets[frame.SpriteSheet].Replace('/', '\\'));
                    if (!File.Exists(targetFile))
                    {
                        map = null;
                        // add a Null to our lookup, so we can avoid looking again in the future
                        Sheets.Add(rsdkAnim.SpriteSheets[frame.SpriteSheet], map);
                    }
                    else
                    {
						using (Stream stream = File.OpenRead(targetFile))
						{
							Bitmap disposable = (Bitmap)System.Drawing.Bitmap.FromStream(stream);
							map = disposable.Clone(new Rectangle(0, 0, disposable.Width, disposable.Height), PixelFormat.Format8bppIndexed);
							//Encore Colors
							if (EditorInstance.useEncoreColors && noEncoreColors == false && (frame.Width != 0 || frame.Height != 0)) map = SetEncoreColors((Bitmap)map.Clone(), EditorInstance.EncorePalette[0]);
							Sheets.Add(rsdkAnim.SpriteSheets[frame.SpriteSheet], map);
							disposable.Dispose();
						}


                    }
                }
                else if (Sheets[rsdkAnim.SpriteSheets[frame.SpriteSheet]] != null)
                {
						map = Sheets[rsdkAnim.SpriteSheets[frame.SpriteSheet]];
						//Encore Colors
						if (EditorInstance.useEncoreColors && noEncoreColors == false && (frame.Width != 0 || frame.Height != 0)) map = SetEncoreColors(map, EditorInstance.EncorePalette[0]);		
				}


                if (frame.Width == 0 || frame.Height == 0) continue;

                // can't load the animation, it probably doesn't exist in the User's Sprites folder
                if (map == null) return null;

				// We are storing the first colour from the palette so we can use it to make sprites transparent
				var colour = map.Palette.Entries[0];

				// Slow
				if (PartialEngineRotation || FullEngineRotation)
				{
					map = SimplyCropImage(map, new Rectangle(frame.X, frame.Y, frame.Width, frame.Height), fliph, flipv, colour);
					map = RotateImage(map, rotateImg, colour);
					map = FitForSharpDXTexture(map);
				}
				else 
				{
					map = CropImage(map, new Rectangle(frame.X, frame.Y, frame.Width, frame.Height), fliph, flipv, colour, rotateImg, rotate, legacyRotate);
					if (rotateImg != 0 && legacyRotate)
					{
						map = RotateImageLegacy(map, rotateImg, colour);
						frame.Height = (short)(frame.Width + frame.Height + 64);
						frame.Width = (short)(frame.Height + frame.Width + 32);
					}
				}



                map = RemoveColourImage(map, colour, map.Width, map.Height);

				Bitmap finalMap = map.Clone(new Rectangle(0, 0, map.Width, map.Height), map.PixelFormat);
				map.Dispose();

				Texture texture = null;
                if (loadImageToDX)
                {
                    texture = TextureCreator.FromBitmap(d._device, finalMap);
                }
                
                var editorFrame = new EditorEntity_ini.EditorAnimation.EditorFrame()
                {
                    Texture = texture,
                    Frame = frame,
                    Entry = rsdkAnim.Animations[AnimId],
                    ImageWidth = finalMap.Size.Width,
                    ImageHeight = finalMap.Size.Height
                    
                    
                };
                if (loadImageToDX == false) editorFrame._Bitmap = finalMap;
                anim.Frames.Add(editorFrame);
                if (frameId != -1) break;
            }
            anim.ImageLoaded = true;
            if (loadImageToDX) anim.Ready = true;
            Working = false;
			
			return anim;

        }

        #region New Animation System (Untested)
        public EditorAnimation LoadAnimation3(string name, DevicePanel d, int AnimID, int FrameID, bool FlipH, bool FlipV, bool StackFrames = false, bool LoadImageToDX = true, Flag FlagAttributes = Flag.DefaultBehavior, bool Rotate = false, int TextureRotation = 0, bool LegacyRotate = false, int StackStart = 0, int StackEnd = 0)
        {
            string key = $"{name}-{AnimID}-{FrameID}-{FlipH}-{FlipV}-{FlagAttributes}-{TextureRotation}-{Rotate}";
            var anim = new EditorEntity_ini.EditorAnimation();
            if (EditorInstance.EditorEntity_ini.Animations.ContainsKey(key))
            {
                if (Animations[key].Ready) return Animations[key]; // Use the already loaded Amination
                else return null;
            }

            Animations.Add(key, anim);

            // Get the path of the object's textures
            string assetName = (EditorInstance.userDefinedEntityRenderSwaps.Keys.Contains(name) ? EditorInstance.userDefinedEntityRenderSwaps[name] : name);
            Tuple<string, string> AssetInfo = GetAssetPath(name);
            string path2 = AssetInfo.Item1;
            string dataFolderLocation = AssetInfo.Item2;
            if (!File.Exists(path2) || path2 == null) return null;
            using (var stream = File.OpenRead(path2)) { rsdkAnim = new Animation(new RSDKv5.Reader(stream)); }
            if (AnimID >= rsdkAnim.Animations.Count) AnimID = rsdkAnim.Animations.Count - 1;
            
            if (StackFrames) anim = ProcessAnimationFramesAsStack(name, d, AnimID, FrameID, StackStart, StackEnd, FlipH, FlipV, assetName, dataFolderLocation, LoadImageToDX, anim, LegacyRotate, TextureRotation, FlagAttributes, Rotate);
            else anim = ProcessAnimationFrames(name, d, AnimID, FrameID, FlipH, FlipV, assetName, dataFolderLocation, LoadImageToDX, anim, LegacyRotate, TextureRotation, FlagAttributes, Rotate);

            anim.ImageLoaded = true;
            if (LoadImageToDX) anim.Ready = true;
            Working = false;
            return anim;
        }
        public EditorAnimation ProcessAnimationFrames(string name, DevicePanel d, int AnimID, int FrameID, bool FlipH, bool FlipV, string assetName, string dataFolderLocation, bool LoadImageToDX, EditorAnimation anim, bool LegacyRotate, int TextureRotation, Flag FlagAttributes = Flag.DefaultBehavior, bool Rotate = false)
        {
            for (int i = 0; i < rsdkAnim.Animations[AnimID].Frames.Count; ++i)
            {
                var animiation = rsdkAnim.Animations[AnimID];
                var frame = animiation.Frames[i];
                if (FrameID >= 0 && FrameID < animiation.Frames.Count) frame = animiation.Frames[FrameID];
                Bitmap map = null;
                bool noEncoreColors = false;
                map = GetAnimationBitmap(name, AnimID, FrameID, assetName, dataFolderLocation, i, map, frame, noEncoreColors);


                // We are storing the first colour from the palette so we can use it to make sprites transparent
                var colour = map.Palette.Entries[0];

                // Slow
                if (FlagAttributes == Flag.FullEngineRotation || FlagAttributes == Flag.PartialEngineRotation)
                {
                    map = SimplyCropImage(map, new Rectangle(frame.X, frame.Y, frame.Width, frame.Height), FlipH, FlipV, colour);
                    map = RotateImage(map, TextureRotation, colour);
                    map = FitForSharpDXTexture(map);
                }
                else
                {
                    map = CropImage(map, new Rectangle(frame.X, frame.Y, frame.Width, frame.Height), FlipH, FlipV, colour, TextureRotation, Rotate, LegacyRotate);
                    if (TextureRotation != 0 && LegacyRotate)
                    {
                        map = RotateImageLegacy(map, TextureRotation, colour);
                        frame.Height = (short)(frame.Width + frame.Height + 64);
                        frame.Width = (short)(frame.Height + frame.Width + 32);
                    }
                }
                map = RemoveColourImage(map, colour, map.Width, map.Height);
                Bitmap finalMap = map.Clone(new Rectangle(0, 0, map.Width, map.Height), map.PixelFormat);
                map.Dispose();

                var editorFrame = GenerateNewFrame(frame, d, AnimID, finalMap, LoadImageToDX);
                if (LoadImageToDX == false) editorFrame._Bitmap = finalMap;
                anim.Frames.Add(editorFrame);
                if (FrameID != -1) break;
            }
            return anim;

        }
        public EditorAnimation ProcessAnimationFramesAsStack(string name, DevicePanel d, int AnimID, int FrameID, int StartID, int EndID, bool FlipH, bool FlipV, string assetName, string dataFolderLocation, bool LoadImageToDX, EditorAnimation anim, bool LegacyRotate, int TextureRotation, Flag FlagAttributes = Flag.DefaultBehavior, bool Rotate = false)
        {
            //Copied Unmodified Code from ProcessAnimationFrames, still need to implement this section correctly.

            for (int i = 0; i < rsdkAnim.Animations[AnimID].Frames.Count; ++i)
            {
                var animiation = rsdkAnim.Animations[AnimID];
                var frame = animiation.Frames[i];
                if (FrameID >= 0 && FrameID < animiation.Frames.Count) frame = animiation.Frames[FrameID];
                Bitmap map = null;
                bool noEncoreColors = false;
                map = GetAnimationBitmap(name, AnimID, FrameID, assetName, dataFolderLocation, i, map, frame, noEncoreColors);


                // We are storing the first colour from the palette so we can use it to make sprites transparent
                var colour = map.Palette.Entries[0];

                // Slow
                if (FlagAttributes == Flag.FullEngineRotation || FlagAttributes == Flag.PartialEngineRotation)
                {
                    map = SimplyCropImage(map, new Rectangle(frame.X, frame.Y, frame.Width, frame.Height), FlipH, FlipV, colour);
                    map = RotateImage(map, TextureRotation, colour);
                    map = FitForSharpDXTexture(map);
                    map = RemoveColourImage(map, colour, map.Width, map.Height);
                }
                else
                {
                    map = CropImage(map, new Rectangle(frame.X, frame.Y, frame.Width, frame.Height), FlipH, FlipV, colour, TextureRotation, Rotate, LegacyRotate);
                    if (TextureRotation != 0 && LegacyRotate)
                    {
                        map = RotateImageLegacy(map, TextureRotation, colour);
                        frame.Height = (short)(frame.Width + frame.Height + 64);
                        frame.Width = (short)(frame.Height + frame.Width + 32);
                    }
                }

                Bitmap finalMap = map.Clone(new Rectangle(0, 0, map.Width, map.Height), map.PixelFormat);
                map.Dispose();

                var editorFrame = GenerateNewFrame(frame, d, AnimID, finalMap, LoadImageToDX);
                if (LoadImageToDX == false) editorFrame._Bitmap = finalMap;
                anim.Frames.Add(editorFrame);
                if (FrameID != -1) break;
            }
            return anim;

        }
        public EditorEntity_ini.EditorAnimation.EditorFrame GenerateNewFrame(RSDKv5.Animation.AnimationEntry.Frame frame, DevicePanel d, int AnimID, Bitmap finalMap, bool LoadImageToDX)
        {
            Texture texture = null;
            if (LoadImageToDX) texture = TextureCreator.FromBitmap(d._device, finalMap);
            return new EditorEntity_ini.EditorAnimation.EditorFrame()
            {
                Texture = texture,
                Frame = frame,
                Entry = rsdkAnim.Animations[AnimID],
                ImageWidth = finalMap.Size.Width,
                ImageHeight = finalMap.Size.Height
            };
        }
        public Bitmap GetAnimationBitmap(string name, int AnimID, int FrameID, string assetName, string dataFolderLocation, int index, Bitmap map, RSDKv5.Animation.AnimationEntry.Frame frame, bool noEncoreColors)
        {
            if (EditorStaticObjects.Contains(assetName)) noEncoreColors = true;
            if (frame.SpriteSheet > rsdkAnim.SpriteSheets.Count) frame.SpriteSheet = (byte)(rsdkAnim.SpriteSheets.Count - 1);
            if (!Sheets.ContainsKey(rsdkAnim.SpriteSheets[frame.SpriteSheet]))
            {
                string targetFile;
                if (EditorStaticObjects.Contains(assetName)) targetFile = GetEditorStaticBitmapPath(assetName);
                else targetFile = Path.Combine(dataFolderLocation, "Sprites", rsdkAnim.SpriteSheets[frame.SpriteSheet].Replace('/', '\\'));
                if (!File.Exists(targetFile)) map = AddNullLookup(map, frame);
                else map = OpenBitmapTargetFile(map, frame, targetFile, noEncoreColors);
            }
            else if (Sheets[rsdkAnim.SpriteSheets[frame.SpriteSheet]] != null)
            {
                map = Sheets[rsdkAnim.SpriteSheets[frame.SpriteSheet]];
                map = TestForEncoreColors(map, noEncoreColors, frame);
            }

            return map;
        }
        public Bitmap AddNullLookup(Bitmap map, RSDKv5.Animation.AnimationEntry.Frame frame)
        {
            // Add a Null to our lookup, so we can avoid looking again in the future
            Sheets.Add(rsdkAnim.SpriteSheets[frame.SpriteSheet], map);
            return null;
        }
        public Bitmap OpenBitmapTargetFile(Bitmap map, RSDKv5.Animation.AnimationEntry.Frame frame, string targetFile, bool noEncoreColors)
        {
            using (Stream stream = File.OpenRead(targetFile))
            {
                Bitmap disposable = (Bitmap)System.Drawing.Bitmap.FromStream(stream);
                map = disposable.Clone(new Rectangle(0, 0, disposable.Width, disposable.Height), PixelFormat.Format8bppIndexed);
                //Encore Colors
                map = TestForEncoreColors(map, noEncoreColors, frame);
                Sheets.Add(rsdkAnim.SpriteSheets[frame.SpriteSheet], map);
                disposable.Dispose();
            }
            return map;
        }
        public Bitmap TestForEncoreColors(Bitmap map, bool NoEncoreColors, RSDKv5.Animation.AnimationEntry.Frame frame)
        {
            if (EditorInstance.useEncoreColors && NoEncoreColors == false && (frame.Width != 0 || frame.Height != 0)) return SetEncoreColors((Bitmap)map.Clone(), EditorInstance.EncorePalette[0]);
            else return map;
        }
        public string GetEditorStaticBitmapPath(string assetName)
        {
            string targetFile = "";
            if (assetName == "EditorAssets") targetFile = Path.Combine(Environment.CurrentDirectory, "Resources\\Global\\", "EditorAssets.gif");
            else if (assetName == "HUDEditorText") targetFile = Path.Combine(Environment.CurrentDirectory, "Resources\\Global\\", "EditorText.gif");
            else if (assetName == "EditorIcons2") targetFile = Path.Combine(Environment.CurrentDirectory, "Resources\\Global\\", "EditorIcons2.gif");
            else if (assetName == "TransportTubes") targetFile = Path.Combine(Environment.CurrentDirectory, "Resources\\Global\\", "TransportTubes.gif");
            else if (assetName == "EditorUIRender") targetFile = Path.Combine(Environment.CurrentDirectory, "Resources\\Global\\", "MenuRenders.gif");
            else targetFile = Path.Combine(Environment.CurrentDirectory, "Resources\\Global\\", "SuperSpecialRing.gif");

            return targetFile;
        }
        #endregion

        public EditorTilePlatforms LoadTilePlatform(DevicePanel d, int x2, int y2, int width, int height)
        {

            SceneLayer _layer = EditorInstance.EditorScene?.Move.Layer;
            string key = $"{x2}-{y2}-{width}-{height}";
            var anim = new EditorTilePlatforms();
            if (TilePlatforms.ContainsKey(key))
            {
                if (TilePlatforms[key].Ready)
                {
                    // Use the already loaded Amination
                    return TilePlatforms[key];
                }
                else
                    return null;
            }
            TilePlatforms.Add(key, anim);
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

        public void DrawObjectTile(Graphics g, ushort tile, int x, int y)
        {
            ushort TileIndex = (ushort)(tile & 0x3ff);
            int TileIndexInt = (int)TileIndex;
            bool flipX = ((tile >> 10) & 1) == 1;
            bool flipY = ((tile >> 11) & 1) == 1;
            bool SolidTopA = ((tile >> 12) & 1) == 1;
            bool SolidLrbA = ((tile >> 13) & 1) == 1;
            bool SolidTopB = ((tile >> 14) & 1) == 1;
            bool SolidLrbB = ((tile >> 15) & 1) == 1;

            g.DrawImage(EditorInstance.StageTiles.Image.GetBitmap(new Rectangle(0, TileIndex * 16, 16, 16), flipX, flipY),
                new Rectangle(x * 16, y * 16, 16, 16));
        }

        private Rectangle GetTilePlatformArea(int x, int y, int width, int height)
        {
            return new Rectangle(x, y, width, height);
        }

        public Tuple<String, String> GetAssetPath(string name)
        {
			string path = "";
			string dataDirectory = "";
            if (name == "EditorAssets" || name == "HUDEditorText" || name == "SuperSpecialRing" || name == "EditorIcons2" || name == "TransportTubes" || name == "EditorUIRender")
            {
                switch (name)
                {
                    case "EditorAssets":
                        path = Path.Combine(Environment.CurrentDirectory, "Resources\\Global\\", "EditorAssets.bin");
                        if (!File.Exists(path)) return null;
                        break;
                    case "HUDEditorText":
                        path = Path.Combine(Environment.CurrentDirectory, "Resources\\Global\\", "EditorText.bin");
                        if (!File.Exists(path)) return null;
                        break;
                    case "EditorIcons2":
                        path = Path.Combine(Environment.CurrentDirectory, "Resources\\Global\\", "EditorIcons2.bin");
                        if (!File.Exists(path)) return null;
                        break;
                    case "TransportTubes":
                        path = Path.Combine(Environment.CurrentDirectory, "Resources\\Global\\", "TransportTubes.bin");
                        if (!File.Exists(path)) return null;
                        break;
                    case "EditorUIRender":
                        path = Path.Combine(Environment.CurrentDirectory, "Resources\\Global\\", "EditorUIRender.bin");
                        if (!File.Exists(path)) return null;
                        break;
                    case "SuperSpecialRing":
                        path = Path.Combine(Environment.CurrentDirectory, "Resources\\Global\\", "SuperSpecialRing.bin");
                        if (!File.Exists(path)) return null;
                        break;
                    default:
                        return null;
                }
            }
            else
            {
				bool AssetFound = false;
				foreach (string dataDir in EditorInstance.ResourcePackList)
				{
					Tuple<string, string> Findings = GetAssetSourcePath(dataDir, name);
					if (Findings.Item1 != null && Findings.Item2 != null)
					{
						AssetFound = true;
						path = Findings.Item1;
						dataDirectory = Findings.Item2;
						break;
					}
				}

				if (!AssetFound)
				{
					Tuple<string, string> Findings = GetAssetSourcePath(EditorInstance.DataDirectory, name);
					if (Findings.Item1 != null && Findings.Item2 != null)
					{
						AssetFound = true;
						path = Findings.Item1;
						dataDirectory = Findings.Item2;
					}
				}

            }

            return Tuple.Create(path, dataDirectory);
        }

		public Tuple<string, string> GetAssetSourcePath(string dataFolder, string name)
		{
			string path, path2;
			string dataDirectory = dataFolder;
			// Checks the Stage Folder First
			path = EditorInstance.EditorPath.CurrentZone + "\\" + name + ".bin";
			path2 = Path.Combine(dataDirectory, "Sprites") + "\\" + path;
			if (EditorInstance.userDefinedSpritePaths != null && EditorInstance.userDefinedSpritePaths.Count != 0)
			{
				foreach (string userDefinedPath in EditorInstance.userDefinedSpritePaths)
				{
					path = userDefinedPath + "\\" + name + ".bin";
					path2 = Path.Combine(dataDirectory, "Sprites") + "\\" + path;
					//Debug.Print(path2);
					if (File.Exists(path2))
					{
						break;
					}
				}
				if (!File.Exists(path2))
				{
					path = EditorInstance.EditorPath.CurrentZone + "\\" + name + ".bin";
					path2 = Path.Combine(dataDirectory, "\\Sprites") + "\\" + path;
				}
			}


			if (!File.Exists(path2))
			{
				// Checks using Setup Object (Removed Until Further Notice)
				//path = Extensions.ReplaceLastOccurrence(EditorInstance.entities.SetupObject, "Setup", "") + "\\" + name + ".bin";
				//path2 = Path.Combine(dataDirectory, "Sprites") + "\\" + path;
				if (!File.Exists(path2))
				{
					// Checks without last character
					path = EditorInstance.EditorPath.CurrentZone.Substring(0, EditorInstance.EditorPath.CurrentZone.Length - 1) + "\\" + name + ".bin";
					path2 = Path.Combine(dataDirectory, "Sprites") + "\\" + path;
					if (!File.Exists(path2))
					{
						// Checks for name without the last character and without the numbers in the entity name
						string adjustedName = new String(name.Where(c => c != '-' && (c < '0' || c > '9')).ToArray());
						path = path = EditorInstance.EditorPath.CurrentZone.Substring(0, EditorInstance.EditorPath.CurrentZone.Length - 1) + "\\" + adjustedName + ".bin";
						path2 = Path.Combine(dataDirectory, "Sprites") + "\\" + path;
						if (!File.Exists(path2))
						{
							// Checks for name without any numbers in the Zone name
							string adjustedZone = Regex.Replace(EditorInstance.EditorPath.CurrentZone, @"[\d-]", string.Empty);
							path = path = adjustedZone + "\\" + name + ".bin";
							path2 = Path.Combine(dataDirectory, "Sprites") + "\\" + path;
							if (!File.Exists(path2))
							{
								// Checks for name without any numbers in the Zone name, then add a 1 back
								adjustedZone = adjustedZone + "1";
								path = path = adjustedZone + "\\" + name + ".bin";
								path2 = Path.Combine(dataDirectory, "Sprites") + "\\" + path;
								if (!File.Exists(path2))
								{
									// Checks Global
									path = "Global\\" + name + ".bin";
									path2 = Path.Combine(dataDirectory, "Sprites") + "\\" + path;
									if (!File.Exists(path2))
									{
										//Checks Editor
										path = "Editor\\" + name + ".bin";
										path2 = Path.Combine(dataDirectory, "Sprites") + "\\" + path;
										if (!File.Exists(path2))
										{
											//Checks Cutscene
											path = "Cutscene\\" + name + ".bin";
											path2 = Path.Combine(dataDirectory, "Sprites") + "\\" + path;
											if (!File.Exists(path2))
											{
												//Checks MSZ
												path = "MSZ\\" + name + ".bin";
												path2 = Path.Combine(dataDirectory, "Sprites") + "\\" + path;
												if (!File.Exists(path2))
												{
													//Checks Base without a Path
													path = name + ".bin";
													path2 = Path.Combine(dataDirectory, "Sprites") + "\\" + path;


													if (!File.Exists(path2))
													{
														string spriteFolder = Path.Combine(dataDirectory, "Sprites");
														// Checks the Entire Sprite folder 
														foreach (string dir in Directory.GetDirectories(spriteFolder, $"*", SearchOption.TopDirectoryOnly))
														{
															path = Path.GetFileName(dir) + "\\" + name + ".bin";
															path2 = Path.Combine(dataDirectory, "Sprites") + "\\" + path;
															if (File.Exists(path2)) break;

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
						}
					}
				}
			}
			return Tuple.Create(path2, dataDirectory);
		}

        public Bitmap CropImage(Bitmap source, Rectangle section, bool fliph, bool flipv, SystemColor colour, int rotateImg = 0, bool rotate = false, bool legacyRotate = false)
        {
            Bitmap bmp2 = new Bitmap(section.Size.Width, section.Size.Height);
            using (Graphics g = Graphics.FromImage(bmp2)) g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
            if (fliph && flipv) bmp2.RotateFlip(RotateFlipType.RotateNoneFlipXY);
            else if (fliph) bmp2.RotateFlip(RotateFlipType.RotateNoneFlipX);
            else if (flipv) bmp2.RotateFlip(RotateFlipType.RotateNoneFlipY);
            if (rotate && !legacyRotate) bmp2 = RotateImage(bmp2, rotateImg, colour);



         //   if (!legacyRotate)
         //   {
                // AH-HA! The Memory Issue lies here, the larger the bitmap, the more unused memory we have. (UPDATE: Inital Fix to the Problem)
                var squareSize = (bmp2.Width > bmp2.Height ? bmp2.Width : bmp2.Height);
                int factor = 32;
                int newSize = (int)Math.Round((squareSize / (double)factor), MidpointRounding.AwayFromZero) * factor;
                if (newSize == 0) newSize = factor;
                while (newSize < squareSize) newSize += factor;

                Bitmap bmp = new Bitmap(newSize, newSize);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    if (rotate && !legacyRotate) g.DrawImage(bmp2, bmp.Width / 2 - bmp2.Width / 2, bmp.Height / 2 - bmp2.Height / 2, new Rectangle(0, 0, bmp2.Width, bmp2.Height), GraphicsUnit.Pixel);
                    else g.DrawImage(bmp2, 0, 0, new Rectangle(0, 0, bmp2.Width, bmp2.Height), GraphicsUnit.Pixel);

                }
                bmp2.Dispose();
                //bmp.Save(Environment.CurrentDirectory + "//Images" + "//" + name + (rotateImg != 0 ? "_" + rotateImg : "") + (frameID != -1 ? "_" + frameID : "") + (animID != -1 ? "_" + animID : "") + ".gif");
                return bmp;
          //  }
           // else
           // {
           //     Bitmap bmp = new Bitmap(1024, 1024);
           //     using (Graphics g = Graphics.FromImage(bmp))
           //         g.DrawImage(bmp2, 0, 0, new Rectangle(0, 0, bmp2.Width, bmp2.Height), GraphicsUnit.Pixel);
           //     return bmp;
            //}


        }

		public Bitmap SimplyCropImage(Bitmap source, Rectangle section, bool fliph, bool flipv, SystemColor colour)
		{
			Bitmap bmp2 = new Bitmap(section.Size.Width, section.Size.Height);
			using (Graphics g = Graphics.FromImage(bmp2)) g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
			if (fliph && flipv) bmp2.RotateFlip(RotateFlipType.RotateNoneFlipXY);
			else if (fliph) bmp2.RotateFlip(RotateFlipType.RotateNoneFlipX);
			else if (flipv) bmp2.RotateFlip(RotateFlipType.RotateNoneFlipY);

			return bmp2;

		}

		public Bitmap FitForSharpDXTexture(Bitmap source)
		{	
			var squareSize = (source.Width > source.Height ? source.Width : source.Height);
			int factor = 32;
			int newSize = (int)Math.Round((squareSize / (double)factor), MidpointRounding.AwayFromZero) * factor;
			if (newSize == 0) newSize = factor;
			while (newSize < squareSize) newSize += factor;

			Bitmap bmp = new Bitmap(newSize, newSize);
			using (Graphics g = Graphics.FromImage(bmp))
			{
				g.DrawImage(source, bmp.Width / 2 - source.Width / 2, bmp.Height / 2 - source.Height / 2, new Rectangle(0, 0, source.Width, source.Height), GraphicsUnit.Pixel);
			}
			source.Dispose();
			return bmp;

		}

		public Bitmap RotateImage(Bitmap img, double rotationAngle, SystemColor colour)
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

        public Bitmap RotateImageLegacy(Bitmap img, double rotationAngle, SystemColor colour)
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

        public Bitmap RemoveColourImage(Bitmap source, System.Drawing.Color colour, int width, int height)
        {
            source.MakeTransparent(colour);
            return source;
        }

        private Bitmap SetEncoreColors(Bitmap _bitmap, string encoreColors = null)
        {
            if (encoreColors == "") return _bitmap;
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
			_bitmap = (Bitmap)_bitmapEditMemory.Clone();
			_bitmapEditMemory.Dispose();
            return _bitmap;
        }

        public ColorPalette[] GetStageConfigColors()
        {
            var stgCfg = EditorInstance.StageConfig;
            ColorPalette[] stageConfigColors = new ColorPalette[8];
            for (int i = 0; i < 8; i++)
            {
                stageConfigColors[i] = EditorInstance.StageTiles.Image.GetBitmap(new Rectangle(0, 0, 1024, 1024)).Palette;
            }
            for (int i = 0; i < 8; i++)
            {
                for (int x = 0; x < 16; x++)
                {
                    for (int y = 0; y < 16; y++)
                    {
                        try
                        {
                            stageConfigColors[i].Entries[16 * x + y] = System.Drawing.Color.FromArgb(stgCfg.Palettes[i].Colors[x][y].R, stgCfg.Palettes[i].Colors[x][y].G, stgCfg.Palettes[i].Colors[x][y].B);
                        }
                        catch
                        {
                            stageConfigColors[i].Entries[16 * x + y] = System.Drawing.Color.Black;
                        }
                    }
                }
            }
            return stageConfigColors;
        }

        // These are special
        public void DrawOthers(DevicePanel d, SceneEntity entity, EditorEntity e, int childX, int childY, int index, int previousChildCount, int platformAngle, EditorAnimations EditorAnimations, bool Selected, AttributeValidater AttributeValidater, bool childDrawAddMode, bool graphicsMode = false)
        {
            int x = entity.Position.X.High + childX;
            int y = entity.Position.Y.High + childY;
            if (childDrawAddMode == false)
            {
                x = childX;
                y = childY;
            }
            int Transparency = (EditorInstance.EditLayer == null) ? 0xff : 0x32;
            try
			{
				
				if (!rendersWithErrors.Contains(entity.Object.Name.Name))
                {
					/*if (entity.Object.Name.Name.Equals("Ring"))
					{
						render.Draw(d, entity, e, x, y, Transparency, index, previousChildCount, platformAngle, EditorAnimations, Selected, AttributeValidater);
					}*/
					
					if (entity.Object.Name.Name.Contains("Setup"))
                    {
                        EntityRenderer renderer = EntityRenderers.Where(t => t.GetObjectName() == "ZoneSetup").FirstOrDefault();
                        if (renderer != null)
                            renderer.Draw(d, entity, e, x, y, Transparency, index, previousChildCount, platformAngle, EditorAnimations, Selected, AttributeValidater);
                    }
					else if (entity.Object.Name.Name.Contains("Intro") || entity.Object.Name.Name.Contains("Outro"))
                    {
                        EntityRenderer renderer = EntityRenderers.Where(t => t.GetObjectName() == "Outro_Intro_Object").FirstOrDefault();
                        if (renderer != null)
                            renderer.Draw(d, entity, e, x, y, Transparency, index, previousChildCount, platformAngle, EditorAnimations, Selected, AttributeValidater);
                    }
                    else if (entity.Object.Name.Name.Contains("TornadoPath") || entity.Object.Name.Name.Contains("AIZTornadoPath"))
                    {
                        EntityRenderer renderer = EntityRenderers.Where(t => t.GetObjectName() == "TornadoPath").FirstOrDefault();
                        if (renderer != null)
                            renderer.Draw(d, entity, e, x, y, Transparency, index, previousChildCount, platformAngle, EditorAnimations, Selected, AttributeValidater);
                    }
                    else
                    {
                        EntityRenderer renderer = EntityRenderers.Where(t => t.GetObjectName() == entity.Object.Name.Name).FirstOrDefault();
                        if (renderer != null)
                            renderer.Draw(d, entity, e, x, y, Transparency, index, previousChildCount, platformAngle, EditorAnimations, Selected, AttributeValidater);
                    }
				}

			}
            catch (Exception ex)
            {
				if (!EditorInstance.isPreRending)
				{
					MessageBox.Show("Unable to load the render for " + entity.Object.Name.Name + "! " + ex.ToString());
					rendersWithErrors.Add(entity.Object.Name.Name);
				}

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
                public Animation.AnimationEntry.Frame Frame;
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


        public void ReleaseResources()
        {

            foreach (var pair in Sheets)
                pair.Value?.Dispose();
            Sheets.Clear();

            TilePlatforms.Clear();


            foreach (var pair in Animations)
                foreach (var pair2 in pair.Value.Frames)
                    pair2.Texture?.Dispose();

            Animations.Clear();
            TilePlatforms.Clear();
        }

    }
}
