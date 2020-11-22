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
using ManiacEditor.Classes.Rendering;
using System.Windows.Data;
using ManiacEditor.Methods.Solution;

namespace ManiacEditor.Methods.Drawing
{
    public static class ObjectDrawing
    {
        #region Definitions

        private static int LastViewPositionX { get; set; } = -1;
        private static int LastViewPositionY { get; set; } = -1;

        public static List<string> SpecialObjectRenders { get; private set; } = new List<string>() 
        {
            "EditorAssets",
            "HUDEditorText",
            "EditorIcons2",
            "TransportTubes",
            "EditorUIRender"
        };

        // Object Render List
        public static List<EntityRenderer> EntityRenderers { get; set; } = new List<EntityRenderer>();
        public static List<LinkedRenderer> LinkedEntityRenderers { get; set; } = new List<LinkedRenderer>();

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
        public static EditorAnimation LoadAnimation(DevicePanel d, string name, int AnimID = 0, int FrameID = 0, bool FallBack = false)
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
                LoadNextAnimation(d, name, FallBack);
                return new EditorAnimation(true);
            }
        }
        public static void LoadNextAnimation(DevicePanel d, string name, bool FallBack = false)
        {
            if (AnimationCache.ContainsKey(name)) return;
            else
            {
                var Animation = new EditorAnimation();
                var AssetInfo = GetAnimationPath(name, FallBack);
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

        #region TextureExt Collection
        public static Dictionary<string, TextureExt> GetAnimationSpriteSheetTextures(DevicePanel d, string Name, Animation Animation, string SourcePath, string SourceDirectory, bool NoEncoreColors)
        {
            Dictionary<string, TextureExt> SpriteSheetTextures = new Dictionary<string, TextureExt>();

            foreach (var SpriteSheetPath in Animation.SpriteSheets)
            {
                Bitmap SpriteSheetBMP;
                string TargetFile = GetBitmapPath(Name, SpriteSheetPath);


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
                            SpriteSheetBMP = FixBitmapSize(SpriteSheetBMP);
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
                    SpriteSheetTextures.Add(SpriteSheetPath.Replace('/', '\\'), Methods.Drawing.TextureCreator.FromBitmap(d._device, SpriteSheetBMP));
                }
                else
                {
                    SpriteSheetTextures.Add(SpriteSheetPath.Replace('/', '\\'), null);
                }
            }

            return SpriteSheetTextures;
        }

        public static Bitmap FixBitmapSize(Bitmap bmp2)
        {
            var squareSize = (bmp2.Width > bmp2.Height ? bmp2.Width : bmp2.Height);
            int factor = 32;
            int newSize = (int)Math.Round((squareSize / (double)factor), MidpointRounding.AwayFromZero) * factor;
            if (newSize == 0) newSize = factor;
            while (newSize < squareSize) newSize += factor;

            Bitmap bmp = new Bitmap(newSize, newSize);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(bmp2, 0, 0, new Rectangle(0, 0, bmp2.Width, bmp2.Height), GraphicsUnit.Pixel);
            }
            bmp2 = (Bitmap)bmp.Clone();
            bmp.Dispose();
            return bmp2;
        }
        public static Bitmap RemoveColourImage(Bitmap source, System.Drawing.Color colour)
        {
            source.MakeTransparent(colour);
            return source;
        }
        public static Bitmap SetEncoreColors(Bitmap map, bool NoEncoreColors)
        {
            if (Methods.Solution.SolutionState.Main.UseEncoreColors && NoEncoreColors == false) return SetColors((Bitmap)map.Clone(), ManiacEditor.Methods.Solution.SolutionPaths.EncorePalette[0]);
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
        public static string GetEditorBitmapPath(string assetName)
        {
            string targetFile = "";
            if (assetName == "EditorAssets") targetFile = Path.Combine(ManiacEditor.Methods.ProgramBase.GetExecutingDirectoryName(), "Resources\\Objects\\", "EditorAssets.gif");
            else if (assetName == "HUDEditorText") targetFile = Path.Combine(ManiacEditor.Methods.ProgramBase.GetExecutingDirectoryName(), "Resources\\Objects\\", "EditorText.gif");
            else if (assetName == "EditorIcons2") targetFile = Path.Combine(ManiacEditor.Methods.ProgramBase.GetExecutingDirectoryName(), "Resources\\Objects\\", "EditorIcons2.gif");
            else if (assetName == "TransportTubes") targetFile = Path.Combine(ManiacEditor.Methods.ProgramBase.GetExecutingDirectoryName(), "Resources\\Objects\\", "TransportTubes.gif");
            else if (assetName == "EditorUIRender") targetFile = Path.Combine(ManiacEditor.Methods.ProgramBase.GetExecutingDirectoryName(), "Resources\\Objects\\", "MenuRenders.gif");

            return targetFile;
        }
        public static string GetZoneSetupBitmapPath(string AssetName)
        {
            string TargetFile = "";
            TargetFile = ManiacEditor.Methods.Solution.CurrentSolution.Entities.SetupObject.Replace("Setup", "") + "\\" + AssetName + ".bin";
            return TargetFile;
        }
        public static string GetEditorAnimationPath(string name)
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
                default:
                    path = null;
                    break;
            }
            return path;
        }
        public static Tuple<String, String> GetAnimationPath(string name, bool FallBack = false)
        {
			string path = "";
			string dataDirectory = "";
            if (name == "EditorAssets" || name == "HUDEditorText" || name == "EditorIcons2" || name == "TransportTubes" || name == "EditorUIRender")
            {
                path = GetEditorAnimationPath(name);
                dataDirectory = Path.Combine(ManiacEditor.Methods.ProgramBase.GetExecutingDirectoryName(), "Resources\\Objects");
                if (!File.Exists(path)) return null;
            }
            else
            {
				bool AssetFound = false;
				foreach (string dataDir in ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.ExtraDataDirectories)
				{
					Tuple<string, string> Findings = GetSpecificAnimationPath(dataDir, name, FallBack);
					if (Findings.Item1 != null && Findings.Item2 != null)
					{
                        AssetFound = true;
                        path = Findings.Item1;
                        dataDirectory = Findings.Item2;
                        break;
                    }
				}

				if (!AssetFound && ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.MasterDataDirectory != null)
				{
					Tuple<string, string> Findings = GetSpecificAnimationPath(ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.MasterDataDirectory, name, FallBack);
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
        public static string GetBitmapPath(string Name, string SpritePath)
        {
            string TargetFile = "";
            if (SpecialObjectRenders.Contains(Name)) TargetFile = GetEditorBitmapPath(Name);
            else
            {
                bool AssetFound = false;
                foreach (string dataDir in ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.ExtraDataDirectories)
                {
                    Tuple<string, string> Findings = GetSpecificBitmapPath(dataDir, SpritePath);
                    if (Findings.Item1 != null && Findings.Item2 != null)
                    {
                        AssetFound = true;
                        TargetFile = Findings.Item1;
                        break;
                    }
                }

                if (!AssetFound && ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.MasterDataDirectory != null)
                {
                    Tuple<string, string> Findings = GetSpecificBitmapPath(ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.MasterDataDirectory, SpritePath);
                    if (Findings.Item1 != null && Findings.Item2 != null)
                    {
                        AssetFound = true;
                        TargetFile = Findings.Item1;
                    }
                }

            }

            return TargetFile;
        }
        private static string[] GetAllPossibleDefaultSpriteFolders(string DataDirectory)
        {
            List<string> SpriteFolders = new List<string>();
            foreach (var folder in Directory.GetDirectories(System.IO.Path.Combine(ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.MasterDataDirectory, "Sprites"), $"*", SearchOption.TopDirectoryOnly))
            {
                var dirInfo = new DirectoryInfo(folder);
                SpriteFolders.Add(System.IO.Path.Combine(DataDirectory, "Sprites", dirInfo.Name));
            }
            return SpriteFolders.ToArray();
        }
        public static Tuple<string, string> GetSpecificAnimationPath(string DataDirectory, string Name, bool FallBack = false)
		{
            string AnimationPath = null;
            string FullPath = null;
            string RelativePath = null;

            if (Directory.Exists(Path.Combine(DataDirectory, "Sprites")))
            {
                if (FallBack)
                {
                    // Checks the Entire Data Directory
                    foreach (string SpriteDirectory in GetAllPossibleDefaultSpriteFolders(DataDirectory))
                    {
                        AnimationPath = Path.GetFileName(SpriteDirectory) + "\\" + Name + ".bin";
                        RelativePath = Path.Combine("Data", "Sprites").Replace("\\", "/") + "/" + AnimationPath.Replace("\\", "/");
                        if (DoesIZMatchExist(RelativePath))
                        {
                            string NewPath = GetIZPath(RelativePath);
                            RelativePath = NewPath;
                            FullPath = Directory.GetParent(DataDirectory).FullName + "\\" + RelativePath.Replace("/", "\\");
                        }
                        else
                        {
                            AnimationPath = Path.GetFileName(SpriteDirectory) + "\\" + Name + ".bin";
                            FullPath = DataDirectory + "\\Sprites\\" + AnimationPath.Replace("/", "\\");
                        }
                        if (File.Exists(FullPath)) break;
                    }
                }
                else
                {
                    // Try Just By Name
                    AnimationPath = Name;
                    RelativePath = Path.Combine("Data", "Sprites").Replace("\\", "/") + "/" + AnimationPath.Replace("\\", "/");
                    if (DoesIZMatchExist(RelativePath))
                    {
                        string NewPath = GetIZPath(RelativePath);
                        RelativePath = NewPath;
                        FullPath = Directory.GetParent(DataDirectory).FullName + "\\" + RelativePath.Replace("/", "\\");
                    }
                    else
                    {
                        AnimationPath = Name;
                        FullPath = DataDirectory + "\\Sprites\\" + AnimationPath.Replace("/", "\\");
                    }
                }
            }

            if (!File.Exists(FullPath)) 
            {
                AnimationPath = null;
                FullPath = null;
                DataDirectory = null;
            } 

            return Tuple.Create(FullPath, DataDirectory);
		}
        public static string GetIZPath(string RelativePath)
        {
            bool DirectPathExists = CurrentSolution.IZ_Stage.Assets.Exists(x => x.BasePath == RelativePath);
            if (DirectPathExists)
            {
                return CurrentSolution.IZ_Stage.Assets.Where(x => x.BasePath == RelativePath).FirstOrDefault().NewPath;
            }
            else
            {
                string Folder = Path.GetDirectoryName(RelativePath).Replace("\\", "/") + "/";
                bool FolderRedirectExists = CurrentSolution.IZ_Stage.Assets.Exists(x => x.BasePath == Folder);
                if (FolderRedirectExists)
                {
                    string NewFolder = CurrentSolution.IZ_Stage.Assets.Where(x => x.BasePath.Contains(Folder)).FirstOrDefault().NewPath;
                    RelativePath = RelativePath.Replace(Folder, NewFolder);
                    return RelativePath;
                }
                else return RelativePath;
            }
        }
        public static bool DoesIZMatchExist(string RelativePath)
        {
            if (CurrentSolution.IZ_Stage != null && CurrentSolution.IZ_Stage.Assets != null)
            {
                bool DirectPathExists = CurrentSolution.IZ_Stage.Assets.Exists(x => x.BasePath == RelativePath);
                if (DirectPathExists) return DirectPathExists;
                else
                {
                    string Folder = Path.GetDirectoryName(RelativePath).Replace("\\", "/") + "/";
                    bool FolderRedirectExists = CurrentSolution.IZ_Stage.Assets.Exists(x => x.BasePath == Folder);
                    if (FolderRedirectExists) return FolderRedirectExists;
                    else return false;
                }

            }
            else return false;

        }
        public static Tuple<string, string> GetSpecificBitmapPath(string DataDirectory, string Path)
        {
            string FullPath = null;
            string RelativePath = null;


            Path = Path.Replace("/", "\\");

            string SpriteFolder = System.IO.Path.Combine(DataDirectory, "Sprites");
            // Checks the Data Directories Sprite Folder
            if (Directory.Exists(SpriteFolder))
            {
                RelativePath = System.IO.Path.Combine("Data", "Sprites").Replace("\\", "/") + "/" + Path.Replace("\\", "/");
                if (DoesIZMatchExist(RelativePath))
                {
                    string NewPath = GetIZPath(RelativePath);
                    RelativePath = NewPath;
                    FullPath = Directory.GetParent(DataDirectory).FullName + "\\" + RelativePath.Replace("/", "\\");
                }
                else
                {
                    FullPath = SpriteFolder + "\\" + Path;
                }

                if (!File.Exists(FullPath))
                {
                    FullPath = null;
                }
            }


            return Tuple.Create(FullPath, DataDirectory);
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
            if (_entity.FilteredOut) return;
            if (_entity.ManuallyFilteredOut) return;

            int X = _entity.Position.X.High;
            int Y = _entity.Position.Y.High;
            string Name = _entity.Object.Name.Name;

            int Transparency = GetTransparencyLevel();
            System.Drawing.Color BoxInsideColor = GetBoxBackgroundColor(_entity);
            System.Drawing.Color BoxFilterColor = GetBoxBorderColor(_entity);

            int index = _entity.FilterSlotID;
            string boxText = String.Format("{0}{2}ID: {1}{2}IDX: {3}", GetShortenedName(Name, 8), _entity.SlotID, Environment.NewLine, index);

            DrawSelectionBox(d, X, Y, Transparency, BoxInsideColor, BoxFilterColor, _entity, boxText);

            string GetShortenedName(string value, int maxLength)
            {
                if (string.IsNullOrEmpty(value)) return value;
                return value.Length <= maxLength ? value : value.Substring(0, maxLength - 1) + ".";
            }
        }
        public static void DrawNormal(DevicePanel d, Classes.Scene.EditorEntity _entity)
        {
            if (!IsObjectOnScreen(d, _entity)) return;

            //LoadNextAnimation(d, _entity.Name);

            int X = _entity.Position.X.High;
            int Y = _entity.Position.Y.High;
            string Name = _entity.Object.Name.Name;
            int Transparency = GetTransparencyLevel();


            bool fliph = false;
            bool flipv = false;
            bool rotate = false;
            _entity.GetRotationFromAttributes(ref fliph, ref flipv, ref rotate);

            if (!ManiacEditor.Properties.Settings.MyPerformance.NeverLoadEntityTextures)
            {
                if (CanDraw(Name)) DrawDedicatedRender(d, _entity);
                else FallbackDraw(d, _entity.Name, X, Y, Transparency, fliph, flipv, rotate);
            }
        }
        public static void DrawDedicatedRender(DevicePanel d, Classes.Scene.EditorEntity e)
        {
            if (ProgramBase.IsDebug) Call();
            else
            {
                try
                {
                    Call();
                }
                catch (Exception ex)
                {
                    string note = "This object will no longer render until reloaded!";
                    string error = string.Format("Entity Rendering Error on Object {0}:{1}{2}{1}{3}{1}{1}{4})", e.Object.Name.Name, Environment.NewLine, ex.Message, ex.StackTrace, note);
                    MessageBox.Show(error);
                    e.DoesRenderHaveErrors = true;
                }
            }


            void Call()
            {
                int x = e.Position.X.High;
                int y = e.Position.Y.High;
                int Transparency = (Methods.Solution.CurrentSolution.EditLayerA == null) ? 0xff : 0x32;

                Structures.EntityRenderProp properties = new Structures.EntityRenderProp(d, e, x, y, Transparency);

                if (!e.DoesRenderHaveErrors)
                {
                    if (e.CurrentRender == null)
                    {
                        var RenderDrawing = EntityRenderers.Where(t => t.GetObjectName() == e.Object.Name.Name).FirstOrDefault();
                        e.CurrentRender = RenderDrawing;
                    }
                    if (e.CurrentRender != null) e.CurrentRender.Draw(properties);
                }
            }


        }
        public static void FallbackDraw(DevicePanel d, string Name, int x, int y, int Transparency, bool FlipH, bool FlipV, bool Rotate)
        {
            int FrameID = 0;
            int AnimID = 0;

            var animation = LoadAnimation(d, Name, AnimID, FrameID, true);
            Entity_Renders.EntityRenderer.DrawTexturePivotNormal(d, animation, animation.RequestedAnimID, animation.RequestedFrameID, x, y, Transparency, FlipH, FlipV);

        }
        public static void DrawLinked(DevicePanel d, Classes.Scene.EditorEntity _entity)
        {
            if (ProgramBase.IsDebug) Call();
            else
            {
                try
                {
                    Call();
                }
                catch (Exception ex)
                {
                    string note = "This object will no longer render it's linked render until reloaded!";
                    string error = string.Format("Linked Entity Rendering Error on Object {0}:{1}{2}{1}{3}{1}{1}{4})", _entity.Object.Name.Name, Environment.NewLine, ex.Message, ex.StackTrace, note);
                    MessageBox.Show(error);
                    _entity.DoesLinkedRenderHaveErrors = true;
                }
            }


            void Call()
            {
                if (!_entity.DoesLinkedRenderHaveErrors)
                {
                    var structure = new Structures.LinkedEntityRenderProp(d, _entity);
                    if (_entity.CurrentLinkedRender == null)
                    {
                        LinkedRenderer renderer = LinkedEntityRenderers.Where(t => t.GetObjectName() == _entity.Object.Name.Name.ToString()).FirstOrDefault();
                        _entity.CurrentLinkedRender = renderer;
                    }
                    if (_entity.CurrentLinkedRender != null) _entity.CurrentLinkedRender.Draw(structure);
                }
            }

        }
        public static void DrawInternal(DevicePanel d, Classes.Scene.EditorEntity _entity)
        {
            int Transparency = GetTransparencyLevel();

            int x = _entity.Position.X.High;
            int y = _entity.Position.Y.High;
            DrawSelectionBox(d, x, y, Transparency, System.Drawing.Color.Transparent, System.Drawing.Color.Red, _entity, "");
        }
        public static void DrawSelectionBox(DevicePanel d, int x, int y, int Transparency, System.Drawing.Color BackgroundBoxColor, System.Drawing.Color BorderBoxColor, Classes.Scene.EditorEntity e, string boxText)
        {
            if (Methods.Solution.SolutionState.Main.ShowEntitySelectionBoxes && IsObjectOnScreen(d, e))
            {
                if (e.RenderNotFound)
                {
                    d.DrawRectangle(x, y, x + Methods.Solution.SolutionConstants.ENTITY_NAME_BOX_WIDTH, y + Methods.Solution.SolutionConstants.ENTITY_NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, BackgroundBoxColor));
                }
                else
                {
                    d.DrawRectangle(x, y, x + Methods.Solution.SolutionConstants.ENTITY_NAME_BOX_WIDTH, y + Methods.Solution.SolutionConstants.ENTITY_NAME_BOX_HEIGHT, GetSelectedColor(BorderBoxColor, e));
                }
                d.DrawLine(x, y, x + Methods.Solution.SolutionConstants.ENTITY_NAME_BOX_WIDTH, y, System.Drawing.Color.FromArgb(Transparency, BorderBoxColor));
                d.DrawLine(x, y, x, y + Methods.Solution.SolutionConstants.ENTITY_NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, BorderBoxColor));
                d.DrawLine(x, y + Methods.Solution.SolutionConstants.ENTITY_NAME_BOX_HEIGHT, x + Methods.Solution.SolutionConstants.ENTITY_NAME_BOX_WIDTH, y + Methods.Solution.SolutionConstants.ENTITY_NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, BorderBoxColor));
                d.DrawLine(x + Methods.Solution.SolutionConstants.ENTITY_NAME_BOX_WIDTH, y, x + Methods.Solution.SolutionConstants.ENTITY_NAME_BOX_WIDTH, y + Methods.Solution.SolutionConstants.ENTITY_NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, BorderBoxColor));

                if (e.SelectedIndex != -1)
                {
                    d.DrawText(string.Format("{0}", e.SelectedIndex + 1), x + 2, y + 2, Methods.Solution.SolutionConstants.ENTITY_NAME_BOX_WIDTH, System.Drawing.Color.Black, true);
                }


                /*
                if (Instance.ViewPanel.SharpPanel.GetZoom() >= 1 && boxText != "")
                {
                    d.DrawTextSmall(boxText, x + 2, y + 1, Methods.Solution.SolutionConstants.ENTITY_NAME_BOX_WIDTH, System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black), true);
                }
                */
            }
        }

        #endregion

        #region Drawing Helpers

        public static System.Drawing.Color GetSelectedColor(System.Drawing.Color color, Classes.Scene.EditorEntity e)
        {
            if (e.InTempSelection)
            {
                return System.Drawing.Color.FromArgb(e.TempSelected && ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit() ? 0x60 : 0x30, color);
            }
            else
            {
                return System.Drawing.Color.FromArgb(e.Selected && ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit() ? 0x60 : 0x30, color);
            }
        }
        public static bool IsObjectOnScreen(DevicePanel d, Classes.Scene.EditorEntity _entity)
        {
            int x = _entity.Position.X.High;
            int y = _entity.Position.Y.High;
            int Transparency = (Methods.Solution.CurrentSolution.EditLayerA == null) ? 0xff : 0x32;

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
                color = System.Drawing.Color.Black;
            }
            return color;

        }
        public static System.Drawing.Color GetBoxBackgroundColor(Classes.Scene.EditorEntity e)
        {
            if (e.InTempSelection)
            {
                return (e.TempSelected && ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit()) ? System.Drawing.Color.MediumPurple : System.Drawing.Color.MediumTurquoise;
            }
            else
            {
                return (e.Selected && ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit()) ? System.Drawing.Color.MediumPurple : System.Drawing.Color.MediumTurquoise;
            }
        }
        public static int GetTransparencyLevel()
        {
            return (Methods.Solution.CurrentSolution.EditLayerA == null) ? 0xff : 0x32;
        }
        public static bool CanDraw(string Name)
        {
            return EntityRenderers.Exists(t => t.GetObjectName() == Name);
        }
        public static bool CanDrawLinked(string Name)
        {
            return LinkedEntityRenderers.Exists(t => t.GetObjectName() == Name);
        }
        #endregion

        #region Object Render Templates

        private static bool UseBuiltIn
        {
            get
            {
                return Properties.Settings.MyPerformance.UseEditableObjectRenders == false;
            }
        }
        private static bool CanCompile
        {
            get
            {
                return Properties.Settings.MyPerformance.UseEditableObjectRenders == true;
            }
        }

        public static void RefreshRenderLists(bool Promptable = false)
        {
            if (Promptable && CanCompile)
            {
                var result = MessageBox.Show("Recompile Object Renders?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes) return;
            }


            if (Methods.Solution.CurrentSolution.Entities != null)
            {
                foreach (var entry in Methods.Solution.CurrentSolution.Entities.Entities)
                {
                    entry.DoesRenderHaveErrors = false;
                    entry.DoesLinkedRenderHaveErrors = false;
                    entry.CurrentRender = null;
                    entry.CurrentLinkedRender = null;
                }
            }

            Methods.Drawing.ObjectDrawing.EntityRenderers.Clear();
            Methods.Drawing.ObjectDrawing.LinkedEntityRenderers.Clear();

            if (Methods.Drawing.ObjectDrawing.EntityRenderers.Count == 0)
            {
                if (UseBuiltIn)
                {
                    var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.BaseType == typeof(EntityRenderer)).ToList();
                    foreach (var type in types)
                        Methods.Drawing.ObjectDrawing.EntityRenderers.Add((EntityRenderer)Activator.CreateInstance(type));
                }

                if (CanCompile)
                {
                    var list = Directory.EnumerateFiles(Methods.ProgramPaths.EntityRendersDirectory, "*.cs", SearchOption.AllDirectories).ToList();
                    if (list.Count != 0)
                    {
                        var render = Methods.Runtime.ScriptLoader.LoadRenderers(list);
                        Methods.Drawing.ObjectDrawing.EntityRenderers.AddRange(render);
                    }
                }
            }

            if (Methods.Drawing.ObjectDrawing.LinkedEntityRenderers.Count == 0)
            {
                if (UseBuiltIn)
                {
                    var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.BaseType == typeof(LinkedRenderer)).ToList();
                    foreach (var type in types)
                        Methods.Drawing.ObjectDrawing.LinkedEntityRenderers.Add((LinkedRenderer)Activator.CreateInstance(type));
                }
                if (CanCompile)
                {
                    var list = Directory.EnumerateFiles(Methods.ProgramPaths.LinkedEntityRendersDirectory, "*.cs", SearchOption.AllDirectories).ToList();
                    if (list.Count != 0)
                    {
                        var render = Methods.Runtime.ScriptLoader.LoadLinkedRenderers(list);
                        Methods.Drawing.ObjectDrawing.LinkedEntityRenderers.AddRange(render);
                    }
                }
            }


            if (CanCompile)
            {
                var process = System.Diagnostics.Process.GetProcessesByName("VBCSCompiler").FirstOrDefault();
                if (process != null) process.Kill();
            }
        }
        #endregion

        #region Update Visibility
        public static void UpdateEntityTileMaps()
        {
            NewTilePlatform.TilesNeedUpdate = true;
            EncoreRoute.TilesNeedUpdate = true;
            Water.TilesNeedUpdate = true;
        }
        public static void UpdateVisibleEntities(List<Classes.Scene.EditorEntity> Entities)
        {
            foreach (var entity in Entities)
            {
                entity.IsVisible = IsObjectOnScreen(Instance.ViewPanel.SharpPanel.GraphicPanel, entity);
            }
        }
        public static void UpdateVisibleEntities(DevicePanel d, List<Classes.Scene.EditorEntity> Entities)
        {
            foreach (var entity in Entities)
            {
                entity.IsVisible = IsObjectOnScreen(d, entity);
            }
        }
        public static void RequestEntityVisiblityRefresh(bool force = false)
        {
            if (Methods.Solution.CurrentSolution.Entities != null)
            {
                if (Methods.Solution.SolutionState.Main.ViewPositionX != LastViewPositionX || Methods.Solution.SolutionState.Main.ViewPositionY != LastViewPositionY)
                {
                    LastViewPositionX = Methods.Solution.SolutionState.Main.ViewPositionX;
                    LastViewPositionY = Methods.Solution.SolutionState.Main.ViewPositionY;
                    Classes.Scene.EditorEntities.ObjectRefreshNeeded = true;
                }
                else if (force)
                {
                    Classes.Scene.EditorEntities.ObjectRefreshNeeded = true;
                }
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
            public Dictionary<string, TextureExt> Spritesheets { get; set; }
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
