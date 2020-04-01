using RSDKv5;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Windows.Forms;
using ManiacEditor.Entity_Renders;
using SystemColor = System.Drawing.Color;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using ImageMagick;
using SFML.Graphics;
using System.CodeDom;


namespace ManiacEditor.Methods.Entities
{
    public static class EntityDrawing
    {
        #region Definitions
        // Object Render List
        public static List<EntityRenderer> EntityRenderers { get; set; } = new List<EntityRenderer>();
        public static List<LinkedRenderer> LinkedEntityRenderers { get; set; } = new List<LinkedRenderer>();

        // Object List for initilizing the if statement
        public static Classes.General.EntityRenderingOptions RenderingSettings;
        public static List<string> RendersWithErrors = new List<string>();
        public static List<string> LinkedRendersWithErrors = new List<string>();

        public static Dictionary<string, EditorAnimation> AnimationCache = new Dictionary<string, EditorAnimation>();

        private static Controls.Editor.MainEditor Instance;
        #endregion

        #region Init
        public static void UpdateInstance(Controls.Editor.MainEditor instance)
        {
            Instance = instance;
        }
        #endregion

        #region Loading/Calling
        public static EditorAnimation LoadAnimation(DevicePanel d, string name, int AnimID = 0, int FrameID = 0)
        {
            if (AnimationCache.ContainsKey(name))
            {
                if (AnimationCache[name].Spritesheets != null)
                {
                    var animation = AnimationCache[name];
                    animation.RequestedAnimID = AnimID;
                    animation.RequestedFrameID = FrameID;
                    return animation;
                }
                else return new EditorAnimation(true);

            }
            else
            {
                LoadNextAnimation(d, name);
                return new EditorAnimation(true);
            }
        }
        public static void LoadNextAnimation(DevicePanel d, string name)
        {
            if (AnimationCache.ContainsKey(name)) return;
            else
            {
                var Animation = new EditorAnimation();
                var AssetInfo = GetAssetPath(name);
                Animation.SourcePath = AssetInfo.Item1;
                Animation.SourceDirectory = AssetInfo.Item2;
                if (AssetInfo.Item1 != string.Empty && AssetInfo.Item1 != null && File.Exists(AssetInfo.Item1))
                {
                    Animation RSDKAnim;
                    using (var stream = File.OpenRead(AssetInfo.Item1)) { RSDKAnim = new Animation(new RSDKv5.Reader(stream)); }
                    Animation.Animation = RSDKAnim;
                    Animation.Spritesheets = GetAnimationSpriteSheetTextures(d, name, RSDKAnim, Animation.SourcePath, Animation.SourceDirectory, true);
                }
                else
                {
                    Animation.Animation = null;
                    Animation.Spritesheets = null;
                }
                AnimationCache.Add(name, Animation);
            }
        }
        #endregion

        #region Texture Collection
        public static Dictionary<string, Texture> GetAnimationSpriteSheetTextures(DevicePanel d, string Name, Animation Animation, string SourcePath, string SourceDirectory, bool NoEncoreColors)
        {
            Dictionary<string, Texture> SpriteSheetTextures = new Dictionary<string, Texture>();

            foreach (var spriteSheetName in Animation.SpriteSheets)
            {
                Bitmap SpriteSheetBMP;
                string TargetFile;

                if (Methods.Entities.EntityDrawing.RenderingSettings.SpecialObjectRenders.Contains(Name)) TargetFile = GetEditorStaticBitmapPath(Name);
                else TargetFile = Path.Combine(SourceDirectory, "Sprites", spriteSheetName.Replace('/', '\\'));


                if (!File.Exists(TargetFile)) SpriteSheetBMP = null;
                else
                {
                    try
                    {
                        using (Stream stream = File.OpenRead(TargetFile))
                        {
                            Bitmap disposable = (Bitmap)System.Drawing.Bitmap.FromStream(stream);
                            var colour = disposable.Palette.Entries[0];
                            SpriteSheetBMP = disposable.Clone(new Rectangle(0, 0, disposable.Width, disposable.Height), PixelFormat.Format8bppIndexed);
                            SpriteSheetBMP = SetEncoreColors(SpriteSheetBMP, NoEncoreColors);
                            SpriteSheetBMP = RemoveColourImage(SpriteSheetBMP, colour);
                            disposable.Dispose();
                        }
                    }
                    catch
                    {
                        SpriteSheetBMP = null;
                    }
                }

                if (SpriteSheetBMP != null)
                {
                    SpriteSheetTextures.Add(spriteSheetName.Replace('/', '\\'), Methods.Draw.TextureHelper.FromBitmap(SpriteSheetBMP));
                }
                else
                {
                    SpriteSheetTextures.Add(spriteSheetName.Replace('/', '\\'), null);
                }
            }

            return SpriteSheetTextures;
        }
        public static Bitmap RemoveColourImage(Bitmap source, System.Drawing.Color colour)
        {
            source.MakeTransparent(colour);
            return source;
        }
        public static Bitmap SetEncoreColors(Bitmap map, bool NoEncoreColors)
        {
            if (Methods.Editor.SolutionState.UseEncoreColors && NoEncoreColors == false) return SetColors((Bitmap)map.Clone(), ManiacEditor.Methods.Editor.SolutionPaths.EncorePalette[0]);
            else return map;

            Bitmap SetColors(Bitmap _bitmap, string encoreColors)
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
        }
        #endregion

        #region Asset Retrival
        public static string GetEditorStaticBitmapPath(string assetName)
        {
            string targetFile = "";
            if (assetName == "EditorAssets") targetFile = Path.Combine(ManiacEditor.Methods.ProgramBase.GetExecutingDirectoryName(), "Resources\\Objects\\", "EditorAssets.gif");
            else if (assetName == "HUDEditorText") targetFile = Path.Combine(ManiacEditor.Methods.ProgramBase.GetExecutingDirectoryName(), "Resources\\Objects\\", "EditorText.gif");
            else if (assetName == "EditorIcons2") targetFile = Path.Combine(ManiacEditor.Methods.ProgramBase.GetExecutingDirectoryName(), "Resources\\Objects\\", "EditorIcons2.gif");
            else if (assetName == "TransportTubes") targetFile = Path.Combine(ManiacEditor.Methods.ProgramBase.GetExecutingDirectoryName(), "Resources\\Objects\\", "TransportTubes.gif");
            else if (assetName == "EditorUIRender") targetFile = Path.Combine(ManiacEditor.Methods.ProgramBase.GetExecutingDirectoryName(), "Resources\\Objects\\", "MenuRenders.gif");
            else targetFile = Path.Combine(ManiacEditor.Methods.ProgramBase.GetExecutingDirectoryName(), "Resources\\Objects\\", "SuperSpecialRing.gif");

            return targetFile;
        }
        public static string GetEditorStaticAssetPath(string name)
        {
            string path;
            switch (name)
            {
                case "EditorAssets":
                    path = Path.Combine(ManiacEditor.Methods.ProgramBase.GetExecutingDirectoryName(), "Resources\\Objects\\", "EditorAssets.bin");
                    break;
                case "HUDEditorText":
                    path = Path.Combine(ManiacEditor.Methods.ProgramBase.GetExecutingDirectoryName(), "Resources\\Objects\\", "EditorText.bin");
                    break;
                case "EditorIcons2":
                    path = Path.Combine(ManiacEditor.Methods.ProgramBase.GetExecutingDirectoryName(), "Resources\\Objects\\", "EditorIcons2.bin");
                    break;
                case "TransportTubes":
                    path = Path.Combine(ManiacEditor.Methods.ProgramBase.GetExecutingDirectoryName(), "Resources\\Objects\\", "TransportTubes.bin");
                    break;
                case "EditorUIRender":
                    path = Path.Combine(ManiacEditor.Methods.ProgramBase.GetExecutingDirectoryName(), "Resources\\Objects\\", "EditorUIRender.bin");
                    break;
                case "SuperSpecialRing":
                    path = Path.Combine(ManiacEditor.Methods.ProgramBase.GetExecutingDirectoryName(), "Resources\\Objects\\", "SuperSpecialRing.bin");
                    break;
                default:
                    path = null;
                    break;
            }
            return path;
        }
        public static Tuple<String, String> GetAssetPath(string name)
        {
			string path = "";
			string dataDirectory = "";
            if (name == "EditorAssets" || name == "HUDEditorText" || name == "SuperSpecialRing" || name == "EditorIcons2" || name == "TransportTubes" || name == "EditorUIRender")
            {
                path = GetEditorStaticAssetPath(name);
                dataDirectory = Path.Combine(ManiacEditor.Methods.ProgramBase.GetExecutingDirectoryName(), "Resources\\Objects");
                if (!File.Exists(path)) return null;
            }
            else
            {
				bool AssetFound = false;
				foreach (string dataDir in ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.ExtraDataDirectories)
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
					Tuple<string, string> Findings = GetAssetSourcePath(ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.MasterDataDirectory, name);
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
		public static Tuple<string, string> GetAssetSourcePath(string dataFolder, string name)
		{
			string path, path2;
			string dataDirectory = dataFolder;
			// Checks the Stage Folder First
			path = ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.Zone + "\\" + name + ".bin";
			path2 = Path.Combine(dataDirectory, "Sprites") + "\\" + path;
			if (Instance.userDefinedSpritePaths != null && Instance.userDefinedSpritePaths.Count != 0)
			{
				foreach (string userDefinedPath in Instance.userDefinedSpritePaths)
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
					path = ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.Zone + "\\" + name + ".bin";
					path2 = Path.Combine(dataDirectory, "\\Sprites") + "\\" + path;
				}
			}


			if (!File.Exists(path2))
			{
				// Checks using Setup Object (Removed Until Further Notice)
				//path = Extensions.ReplaceLastOccurrence(Classes.Edit.Scene.EditorSolution.Entities.SetupObject, "Setup", "") + "\\" + name + ".bin";
				//path2 = Path.Combine(dataDirectory, "Sprites") + "\\" + path;
				if (!File.Exists(path2))
				{
					// Checks without last character
					path = ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.Zone.Substring(0, ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.Zone.Length - 1) + "\\" + name + ".bin";
					path2 = Path.Combine(dataDirectory, "Sprites") + "\\" + path;
					if (!File.Exists(path2))
					{
						// Checks for name without the last character and without the numbers in the entity name
						string adjustedName = new String(name.Where(c => c != '-' && (c < '0' || c > '9')).ToArray());
						path = path = ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.Zone.Substring(0, ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.Zone.Length - 1) + "\\" + adjustedName + ".bin";
						path2 = Path.Combine(dataDirectory, "Sprites") + "\\" + path;
						if (!File.Exists(path2))
						{
							// Checks for name without any numbers in the Zone name
							string adjustedZone = Regex.Replace(ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.Zone, @"[\d-]", string.Empty);
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
                                                        if (Directory.Exists(Path.Combine(dataDirectory, "Sprites")))
                                                        {
                                                            foreach (string dir in Directory.GetDirectories(spriteFolder, $"*", SearchOption.TopDirectoryOnly))
                                                            {
                                                                path = Path.GetFileName(dir) + "\\" + name + ".bin";
                                                                path2 = Path.Combine(dataDirectory, "Sprites") + "\\" + path;
                                                                if (File.Exists(path2)) break;

                                                            }
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
        #endregion

        #region Disposal
        public static void ReleaseResources()
        {
            foreach (var pair in AnimationCache)
            {
                if (pair.Value.Spritesheets != null)
                {
                    foreach (var anim in pair.Value.Spritesheets)
                    {
                        anim.Value?.Dispose();
                    }
                }
                pair.Value.Spritesheets = null;

            }

            AnimationCache.Clear();
        }

        #endregion

        #region Drawing
        public static void DrawSelectionBox(DevicePanel d, Classes.Scene.EditorEntity _entity)
        {
            int X = _entity.Position.X.High;
            int Y = _entity.Position.Y.High;
            string Name = _entity.Object.Name.Name;
            int Transparency = GetTransparencyLevel();
            System.Drawing.Color BoxInsideColor = GetBoxBackgroundColor(_entity);
            System.Drawing.Color BoxFilterColor = GetBoxBorderColor(_entity);

            DrawSelectionBox(d, X, Y, Transparency, BoxInsideColor, BoxFilterColor, _entity);
        }
        public static void DrawNormal(DevicePanel d, Classes.Scene.EditorEntity _entity)
        {
            if (!IsObjectOnScreen(d, _entity)) return;

            LoadNextAnimation(d, _entity.Name);

            int X = _entity.Position.X.High;
            int Y = _entity.Position.Y.High;
            string Name = _entity.Object.Name.Name;
            int Transparency = GetTransparencyLevel();
            System.Drawing.Color BoxInsideColor = GetBoxBackgroundColor(_entity);
            System.Drawing.Color BoxFilterColor = GetBoxBorderColor(_entity);

            if (!ManiacEditor.Properties.Settings.MyPerformance.NeverLoadEntityTextures)
            {
                if (CanDraw(Name)) DrawDedicatedRender(d, _entity);
                else FallbackDraw(d, _entity.Name, X, Y, Transparency);
            }
        }
        public static void DrawDedicatedRender(DevicePanel d, Classes.Scene.EditorEntity e)
        {
            int x = e.Position.X.High;
            int y = e.Position.Y.High;
            int Transparency = (Methods.Editor.Solution.EditLayerA == null) ? 0xff : 0x32;

            Structures.EntityRenderProp properties = new Structures.EntityRenderProp(d, e, x, y, Transparency);

            if (!RendersWithErrors.Contains(e.Object.Name.Name))
            {
                if (e.CurrentRender == null)
                {
                    var RenderDrawing = EntityRenderers.Where(t => t.GetObjectName() == e.Object.Name.Name).FirstOrDefault();
                    e.CurrentRender = RenderDrawing;
                }
                if (e.CurrentRender != null) e.CurrentRender.Draw(properties);
            }


        }
        public static void FallbackDraw(DevicePanel d, string Name, int x, int y, int Transparency)
        {
            int FrameID = 0;
            int AnimID = 0;

            var animation = LoadAnimation(d, Name, AnimID, FrameID);
            Entity_Renders.EntityRenderer.DrawTexturePivotNormal(d, animation, animation.RequestedAnimID, animation.RequestedFrameID, x, y, Transparency);

        }
        public static void DrawLinked(DevicePanel d, Classes.Scene.EditorEntity _entity)
        {
            var structure = new Structures.LinkedEntityRenderProp(d, _entity);
            if (_entity.CurrentLinkedRender == null)
            {
                LinkedRenderer renderer = LinkedEntityRenderers.Where(t => t.GetObjectName() == _entity.Object.Name.Name.ToString()).FirstOrDefault();
                _entity.CurrentLinkedRender = renderer;
            }
            if (_entity.CurrentLinkedRender != null) _entity.CurrentLinkedRender.Draw(structure);
        }
        public static void DrawInternal(DevicePanel d, Classes.Scene.EditorEntity _entity)
        {
            int Transparency = GetTransparencyLevel();

            int x = _entity.Position.X.High;
            int y = _entity.Position.Y.High;
            DrawSelectionBox(d, x, y, Transparency, System.Drawing.Color.Transparent, System.Drawing.Color.Red, _entity);
        }
        public static void DrawSelectionBox(DevicePanel d, int x, int y, int Transparency, System.Drawing.Color BackgroundBoxColor, System.Drawing.Color BorderBoxColor, Classes.Scene.EditorEntity e)
        {
            if (Methods.Editor.SolutionState.ShowEntitySelectionBoxes && IsObjectOnScreen(d, e))
            {
                if (e.RenderNotFound)
                {
                    d.DrawRectangle(x, y, x + Methods.Editor.EditorConstants.ENTITY_NAME_BOX_WIDTH, y + Methods.Editor.EditorConstants.ENTITY_NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, BackgroundBoxColor));
                }
                else
                {
                    d.DrawRectangle(x, y, x + Methods.Editor.EditorConstants.ENTITY_NAME_BOX_WIDTH, y + Methods.Editor.EditorConstants.ENTITY_NAME_BOX_HEIGHT, GetSelectedColor(BorderBoxColor, e));
                }
                d.DrawLine(x, y, x + Methods.Editor.EditorConstants.ENTITY_NAME_BOX_WIDTH, y, System.Drawing.Color.FromArgb(Transparency, BorderBoxColor));
                d.DrawLine(x, y, x, y + Methods.Editor.EditorConstants.ENTITY_NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, BorderBoxColor));
                d.DrawLine(x, y + Methods.Editor.EditorConstants.ENTITY_NAME_BOX_HEIGHT, x + Methods.Editor.EditorConstants.ENTITY_NAME_BOX_WIDTH, y + Methods.Editor.EditorConstants.ENTITY_NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, BorderBoxColor));
                d.DrawLine(x + Methods.Editor.EditorConstants.ENTITY_NAME_BOX_WIDTH, y, x + Methods.Editor.EditorConstants.ENTITY_NAME_BOX_WIDTH, y + Methods.Editor.EditorConstants.ENTITY_NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, BorderBoxColor));

                if (Methods.Editor.SolutionState.Zoom >= 2)
                {
                    d.DrawText(string.Format("{0}", e.Object.Name), x + 2, y + 2, System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black), true, 4, System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.White));
                    d.DrawText(string.Format("(ID: {0})", e.SlotID), x + 2, y + 10, System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black), true, 4, System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.White));
                }

                if (e.SelectedIndex != -1)
                {
                    d.DrawText(string.Format("{0}", e.SelectedIndex + 1), x + 1, y + 1, System.Drawing.Color.Black, true, 6, System.Drawing.Color.Red);
                }
            }
        }

        #endregion

        #region Drawing Helpers

        public static System.Drawing.Color GetSelectedColor(System.Drawing.Color color, Classes.Scene.EditorEntity e)
        {
            if (e.InTempSelection)
            {
                return System.Drawing.Color.FromArgb(e.TempSelected && ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit() ? 0x60 : 0x00, color);
            }
            else
            {
                return System.Drawing.Color.FromArgb(e.Selected && ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit() ? 0x60 : 0x00, color);
            }
        }
        public static bool IsObjectOnScreen(DevicePanel d, Classes.Scene.EditorEntity _entity)
        {
            int x = _entity.Position.X.High;
            int y = _entity.Position.Y.High;
            int Transparency = (Methods.Editor.Solution.EditLayerA == null) ? 0xff : 0x32;

            if (!_entity.FilteredOut)
            {
                if (_entity.CurrentRender == null)
                {
                    var RenderDrawing = EntityRenderers.Where(t => t.GetObjectName() == _entity.Object.Name.Name).FirstOrDefault();
                    _entity.CurrentRender = RenderDrawing;
                }
                if (_entity.CurrentRender != null) return _entity.CurrentRender.isObjectOnScreen(d, _entity, x, y, Transparency);
                else return d.IsObjectOnScreen(x, y, 20, 20);
            }
            else return false;
        }
        public static System.Drawing.Color GetBoxBorderColor(Classes.Scene.EditorEntity e)
        {
            System.Drawing.Color color = System.Drawing.Color.DarkBlue;
            if (e.HasSpecificFilter(1) || e.HasSpecificFilter(5))
            {
                color = System.Drawing.Color.DarkBlue;
            }
            else if (e.HasSpecificFilter(2))
            {
                color = System.Drawing.Color.DarkRed;
            }
            else if (e.HasSpecificFilter(4))
            {
                color = System.Drawing.Color.DarkGreen;
            }
            else if (e.HasSpecificFilter(255))
            {
                color = System.Drawing.Color.Purple;
            }
            else if (e.HasFilterOther())
            {
                color = System.Drawing.Color.Yellow;
            }
            else if (!e.HasFilter())
            {
                color = System.Drawing.Color.White;
            }
            return color;

        }
        public static System.Drawing.Color GetBoxBackgroundColor(Classes.Scene.EditorEntity e)
        {
            if (e.InTempSelection)
            {
                return (e.TempSelected && ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit()) ? System.Drawing.Color.MediumPurple : System.Drawing.Color.MediumTurquoise;
            }
            else
            {
                return (e.Selected && ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit()) ? System.Drawing.Color.MediumPurple : System.Drawing.Color.MediumTurquoise;
            }
        }
        public static int GetTransparencyLevel()
        {
            return (Methods.Editor.Solution.EditLayerA == null) ? 0xff : 0x32;
        }
        public static bool CanDraw(string Name)
        {
            return Methods.Entities.EntityDrawing.RenderingSettings.ObjectToRender.Contains(Name);
        }
        public static bool CanDrawLinked(string Name)
        {
            return Methods.Entities.EntityDrawing.RenderingSettings.LinkedObjectsToRender.Contains(Name);
        }
        #endregion

        #region Object Render Templates

        private static bool UseBuiltIn { get; set; } = true;
        private static bool CanCompile { get; set; } = false;
        public static void RefreshRenderLists()
        {

            if (Methods.Editor.Solution.Entities != null)
            {
                foreach (var entry in Methods.Editor.Solution.Entities.Entities)
                {
                    entry.CurrentRender = null;
                    entry.CurrentLinkedRender = null;
                }
            }

            Methods.Entities.EntityDrawing.EntityRenderers.Clear();
            Methods.Entities.EntityDrawing.LinkedEntityRenderers.Clear();

            if (Methods.Entities.EntityDrawing.EntityRenderers.Count == 0)
            {
                if (UseBuiltIn)
                {
                    var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.BaseType == typeof(EntityRenderer)).ToList();
                    foreach (var type in types)
                        Methods.Entities.EntityDrawing.EntityRenderers.Add((EntityRenderer)Activator.CreateInstance(type));
                }

                if (CanCompile)
                {
                    var list = Directory.EnumerateFiles(Methods.ProgramPaths.EntityRendersDirectory, "*.cs", SearchOption.AllDirectories).ToList();
                    if (list.Count != 0)
                    {
                        var render = ScriptLoader.LoadRenderers(list);
                        Methods.Entities.EntityDrawing.EntityRenderers.AddRange(render);
                    }
                }
            }

            if (Methods.Entities.EntityDrawing.LinkedEntityRenderers.Count == 0)
            {
                if (UseBuiltIn)
                {
                    var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.BaseType == typeof(LinkedRenderer)).ToList();
                    foreach (var type in types)
                        Methods.Entities.EntityDrawing.LinkedEntityRenderers.Add((LinkedRenderer)Activator.CreateInstance(type));
                }
                if (CanCompile)
                {
                    var list = Directory.EnumerateFiles(Methods.ProgramPaths.LinkedEntityRendersDirectory, "*.cs", SearchOption.AllDirectories).ToList();
                    if (list.Count != 0)
                    {
                        var render = ScriptLoader.LoadLinkedRenderers(list);
                        Methods.Entities.EntityDrawing.LinkedEntityRenderers.AddRange(render);
                    }
                }
            }
        }
        #endregion

        #region Update Visibility

        public static void UpdateVisibleEntities(DevicePanel d, List<Classes.Scene.EditorEntity> Entities)
        {
            foreach (var entity in Entities)
            {
                entity.IsVisible = IsObjectOnScreen(d, entity);
            }
        }

        #endregion

        #region Classes
        [Serializable]
        public class EditorAnimation
        {
            public EditorAnimation()
            {

            }
            public EditorAnimation(bool _isNull)
            {
                if (_isNull) isNull = _isNull;
            }

            public bool isNull { get; set; } = false;
            public string Name { get; set; }
            public string SourcePath { get; set; }
            public string SourceDirectory { get; set; }
            public Dictionary<string, Texture> Spritesheets { get; set; }
            public Animation Animation { get; set; }
            public int RequestedAnimID { get; set; }
            public int RequestedFrameID { get; set; }
            public Animation.AnimationEntry RequestedAnimation
            {
                get
                {
                    if (Animation != null && Animation.Animations.Count - 1 >= RequestedAnimID)
                    {
                        return Animation.Animations[RequestedAnimID];
                    }
                    return new Animation.AnimationEntry();
                }
            }
            public Animation.AnimationEntry.Frame RequestedFrame
            {
                get
                {
                    if (Animation != null && Animation.Animations.Count - 1 >= RequestedAnimID)
                    {
                        if (Animation.Animations[RequestedAnimID].Frames.Count - 1 >= RequestedFrameID)
                        {
                            return Animation.Animations[RequestedAnimID].Frames[RequestedFrameID];
                        }
                    }
                    return new Animation.AnimationEntry.Frame();
                }
            }
        }


        #endregion

    }
}
