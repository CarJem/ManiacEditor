using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RSDKv5;
using ManiacEditor.Actions;
using SharpDX.Direct3D9;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ManiacEditor
{
    public static class EditorSolution
    {
        public static Tileconfig TileConfig;
        public static EditorTiles CurrentTiles;
        public static EditorScene CurrentScene;
        public static EditorEntities Entities;
        public static StageConfig StageConfig;
        public static Gameconfig GameConfig;

        public static void UnloadScene()
        {
            EditorSolution.CurrentScene?.Dispose();
            EditorSolution.CurrentScene = null;
            EditorSolution.StageConfig = null;
            Editor.Instance.EditorStatusBar._levelIDLabel.Content = "Level ID: NULL";
            Editor.Instance.Options.LevelID = -1;
            Editor.Instance.Options.EncorePaletteExists = false;
            Editor.Instance.Options.EncoreSetupType = 0;
            Editor.Instance.ManiacINI.ClearSettings();
            Editor.Instance.userDefinedEntityRenderSwaps = new Dictionary<string, string>();
            Editor.Instance.userDefinedSpritePaths = new List<string>();
            Editor.Instance.EditorToolbar.EncorePaletteButton.IsChecked = false;
            Editor.Instance.Paths.UnloadScene();
            Editor.Instance.Options.QuitWithoutSavingWarningRequired = false;

            if (EditorSolution.CurrentTiles != null) EditorSolution.CurrentTiles.Dispose();
            EditorSolution.CurrentTiles = null;

            Editor.Instance.TearDownExtraLayerButtons();

            Editor.Instance.Background = null;

            Editor.Instance.Chunks = null;

            EditorAnimations.AnimationTiming.Clear();


            /*if (entitiesClipboard != null)
            {
                foreach (EditorEntity entity in entitiesClipboard)
                    entity.PrepareForExternalCopy();
            }*/


            // Clear local clipboards
            //TilesClipboard = null;
            Editor.Instance.entitiesClipboard = null;

            EditorSolution.Entities = null;

            EditorStateModel.Zoom = 1;
            EditorStateModel.ZoomLevel = 0;

            Editor.Instance.UndoStack.Clear();
            Editor.Instance.RedoStack.Clear();

            Editor.Instance.EditorToolbar.EditFGLow.ClearCheckedItems();
            Editor.Instance.EditorToolbar.EditFGHigh.ClearCheckedItems();
            Editor.Instance.EditorToolbar.EditFGLower.ClearCheckedItems();
            Editor.Instance.EditorToolbar.EditFGHigher.ClearCheckedItems();
            Editor.Instance.EditorToolbar.EditEntities.ClearCheckedItems();

            Editor.Instance.ZoomModel.SetViewSize();

            Editor.Instance.UI.UpdateControls();

            // clear memory a little more aggressively 
            Editor.Instance.EntityDrawing.ReleaseResources();
            GC.Collect();
            EditorSolution.TileConfig = null;

            Editor.Instance.Options.MenuChar = Editor.Instance.Options.MenuCharS.ToCharArray();
            Editor.Instance.Options.MenuChar_Small = Editor.Instance.Options.MenuCharS_Small.ToCharArray();
            Editor.Instance.Options.LevelSelectChar = Editor.Instance.Options.LevelSelectCharS.ToCharArray();

            Editor.Instance.UpdateStartScreen(true);
        }


        public class EditorScene : Scene, IDisposable
        {
            public IList<EditorLayer> _editorLayers;
            public Editor EditorInstance;

            #region Layers
            public EditorLayer LowDetails
            {
                get => _editorLayers.FirstOrDefault(el => el.Name.Equals(EditorInstance.ManiacINI.ManiacINIData.ForegroundLower) || el.Name.Equals(Settings.MyDefaults.CustomFGLower) || el.Name.Equals("FG Lower") || el.Name.Equals("FG Supa Low"));
            }
            public EditorLayer ForegroundLow
            {
                get => _editorLayers.LastOrDefault(el => el.Name.Equals("FG Low") || el.Name.Equals("Playfield"));
            }
            public EditorLayer Scratch
            {
                get => _editorLayers.LastOrDefault(el => el.Name.Equals("Scratch"));
            }
            public EditorLayer Move
            {
                get => _editorLayers.LastOrDefault(el => el.Name.Equals("Move"));
            }
            public EditorLayer HighDetails
            {
                get => _editorLayers.FirstOrDefault(el => el.Name.Equals(EditorInstance.ManiacINI.ManiacINIData.ForegroundHigher) || el.Name.Equals(Settings.MyDefaults.CustomFGHigher) || el.Name.Equals("FG Higher") || el.Name.Equals("FG Overlay") || el.Name.Equals("FG Supa High"));
            }
            public EditorLayer ForegroundHigh
            {
                get => _editorLayers.LastOrDefault(el => el.Name.Equals("FG High") || el.Name.Equals("Ring Count"));
            }
            #endregion

            #region List of Layers
            public IEnumerable<EditorLayer> AllLayers
            {
                get { return _editorLayers; }
            }
            public IList<EditorLayer> AllLayersList
            {
                get { return _editorLayers; }
            }
            public IEnumerable<EditorLayer> OtherLayers
            {
                get
                {
                    return _editorLayers.Where(el => el != ForegroundLow && el != ForegroundHigh && el != HighDetails && el != LowDetails);
                }
            }
            public IEnumerable<EditorLayer> LayerByDrawingOrder
            {
                get
                {
                    return _editorLayers.Where(el => el.Layer.DrawingOrder.Equals(1));
                }
            }
            #endregion

            public EditorScene(string filename, DevicePanel d, Editor instance) : base(filename)
            {
                EditorInstance = instance;
                _editorLayers = new List<EditorLayer>(Layers.Count);
                foreach (SceneLayer layer in Layers)
                {
                    _editorLayers.Add(new EditorLayer(layer, EditorInstance));
                }
            }

            public EditorScene(DevicePanel d, int width, int height, int BGWidth, int BGHeight, Editor instance)
            {
                EditorInstance = instance;
                Layers = new List<SceneLayer>(3);
                Layers.Add(new SceneLayer("FG Low", (ushort)width, (ushort)height));
                Layers.Add(new SceneLayer("FG High", (ushort)width, (ushort)height));
                Layers.Add(new SceneLayer("Background", (ushort)BGWidth, (ushort)BGHeight));

                _editorLayers = new List<EditorLayer>(Layers.Count);
                foreach (SceneLayer layer in Layers)
                {
                    _editorLayers.Add(new EditorLayer(layer, EditorInstance));
                }
            }

            public EditorLayer ProduceLayer()
            {
                // lets just pick some reasonably safe defaults
                var sceneLayer = new SceneLayer("New Layer", 128, 128);
                var editorLayer = new EditorLayer(sceneLayer, EditorInstance);
                return editorLayer;
            }

            public void DeleteLayer(int byIndex)
            {
                _editorLayers.RemoveAt(byIndex);
            }

            public void DeleteLayer(EditorLayer thisLayer)
            {
                _editorLayers.Remove(thisLayer);
            }

            public String[] GetEncorePalette(string SelectedZone, string DataDirectory, string SelectedScene, string Result, int searchType, string userLoad = "")
            {
                string EncorePallete1 = ""; //Base Pallete
                string EncorePallete2 = "";
                string EncorePallete3 = "";
                string EncorePallete4 = "";
                string EncorePallete5 = "";
                string EncorePallete6 = "";
                //Encore Palette File Loading
                string ZoneName = SelectedZone.Replace("\\", "");
                if (ZoneName == "GHZ")
                {
                    EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreGHZ.act");
                    EncorePallete2 = Path.Combine(DataDirectory, "Palettes", "EncoreGHZ2.act");
                }
                else if (ZoneName == "CPZ")
                {
                    EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreCPZ.act");
                    EncorePallete2 = Path.Combine(DataDirectory, "Palettes", "EncoreCPZw.act");
                }
                else if (ZoneName == "SPZ1")
                {
                    EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreSPZ1.act");
                    EncorePallete2 = Path.Combine(DataDirectory, "Palettes", "EncoreSPZ1b1.act");
                    EncorePallete3 = Path.Combine(DataDirectory, "Palettes", "EncoreSPZ1b2.act");
                    EncorePallete4 = Path.Combine(DataDirectory, "Palettes", "EncoreSPZ1b3.act");
                }
                else if (ZoneName == "SPZ2")
                {
                    EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreSPZ2.act");
                }
                else if (ZoneName == "FBZ")
                {
                    EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreFBZ.act");
                    EncorePallete2 = Path.Combine(DataDirectory, "Palettes", "EncoreFBZf.act");
                    EncorePallete3 = Path.Combine(DataDirectory, "Palettes", "EncoreFBZi.act");
                    EncorePallete4 = Path.Combine(DataDirectory, "Palettes", "EncoreFBZs.act");
                }
                else if (ZoneName == "PSZ1")
                {
                    EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncorePSZ1.act");
                }
                else if (ZoneName == "PSZ2")
                {
                    EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncorePSZ2.act");
                }
                else if (ZoneName == "SSZ1")
                {
                    EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreSSZ1.act");
                }
                else if (ZoneName == "SSZ2")
                {
                    EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreSSZ2.act");
                }
                else if (ZoneName == "HCZ")
                {
                    EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreHCZ.act");
                    EncorePallete2 = Path.Combine(DataDirectory, "Palettes", "EncoreHCZw.act");
                }
                else if (ZoneName == "MSZ")
                {
                    if (SelectedScene.Contains("1"))
                    {
                        EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreMSZ1.act");
                    }
                    if (SelectedScene.Contains("2"))
                    {
                        EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreMSZ2.act");
                    }
                }
                else if (ZoneName == "OOZ1")
                {
                    EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreOOZ1.act");
                }
                else if (ZoneName == "OOZ2")
                {
                    EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreOOZ2.act");
                    EncorePallete2 = Path.Combine(DataDirectory, "Palettes", "EncoreOOZSmog.act");
                }
                else if (ZoneName == "LRZ1")
                {
                    EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreLRZ1.act");
                }
                else if (ZoneName == "LRZ2")
                {
                    EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreLRZ2.act");
                }
                else if (ZoneName == "LRZ3")
                {
                    EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreLRZ3.act");
                }
                else if (ZoneName == "MMZ")
                {
                    EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreMMZ.act");
                    EncorePallete2 = Path.Combine(DataDirectory, "Palettes", "EncoreMMZf.act");
                    EncorePallete3 = Path.Combine(DataDirectory, "Palettes", "EncoreMMZfp.act");
                }
                else if (ZoneName == "TMZ1")
                {
                    EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreTMZ1.act");
                    EncorePallete2 = Path.Combine(DataDirectory, "Palettes", "EncoreTMZ1d.act");
                    EncorePallete3 = Path.Combine(DataDirectory, "Palettes", "EncoreTMZ1l.act");
                }
                else if (ZoneName == "TMZ2")
                {
                    EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreTMZ2.act");
                }
                else if (ZoneName == "TMZ3")
                {
                    EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreTMZ3.act");
                }
                else
                {
                    if (searchType == -1)
                    {
                        if (File.Exists(userLoad))
                        {
                            EncorePallete1 = userLoad;
                        }
                    }
                    else
                    {
                        // For Custom Encore Palette Finding
                        String CustomEncore;
                        CustomEncore = locateEncorePalettes(ZoneName, SelectedScene, Result, DataDirectory, SelectedScene, SelectedScene);

                        if (File.Exists(CustomEncore))
                        {
                            EncorePallete1 = CustomEncore;
                        }
                    }

                }
                string[] encorePalletes = new string[6] { EncorePallete1, EncorePallete2, EncorePallete3, EncorePallete4, EncorePallete5, EncorePallete6 };
                return encorePalletes;
            }

            public int GetEncoreSetupType(string SelectedZone, string DataDirectory, string SelectedScene, string Result)
            {
                //Encore Palette File Loading
                string ZoneName = SelectedZone.Replace("\\", "");
                int encoreType = 0;
                /* Encore Types
                 * 0: Singular Palette
                 * 1: Singular with Objects (GHZ)
                 * 2: Singular with Water (HCZ/CPZ)
                 * <3: Zone Specific;
                 */

                switch (ZoneName)
                {
                    case "GHZ":
                        encoreType = 1;
                        break;
                    case "CPZ":
                        encoreType = 2;
                        break;
                    case "SPZ1":
                        encoreType = 3;
                        break;
                    case "SPZ2":
                        encoreType = 0;
                        break;
                    case "FBZ":
                        encoreType = 4;
                        break;
                    case "PSZ1":
                        encoreType = 0;
                        break;
                    case "PSZ2":
                        encoreType = 0;
                        break;
                    case "SSZ1":
                        encoreType = 0;
                        break;
                    case "SSZ2":
                        encoreType = 0;
                        break;
                    case "HCZ":
                        encoreType = 2;
                        break;
                    case "MSZ1":
                        encoreType = 0;
                        break;
                    case "OOZ1":
                        encoreType = 0;
                        break;
                    case "OOZ2":
                        encoreType = 5;
                        break;
                    case "LRZ1":
                        encoreType = 0;
                        break;
                    case "LRZ2":
                        encoreType = 0;
                        break;
                    case "LRZ3":
                        encoreType = 0;
                        break;
                    case "MMZ":
                        encoreType = 6;
                        break;
                    case "TMZ1":
                        encoreType = 7;
                        break;
                    case "TMZ2":
                        encoreType = 0;
                        break;
                    case "TMZ3":
                        encoreType = 0;
                        break;
                    default:
                        encoreType = 0;
                        break;
                }
                return encoreType;
            }

            private string locateEncorePalettes(string Zone, string Scene, string FullPath, string DataDirectory, string SelectedZone, string SelectedScene)
            {
                string palettesFolder = Path.Combine(DataDirectory, "Palettes");

                string modifiedZone;
                string modifiedScene;
                string actFile;
                string newPal;

                //First Check (intended for MSZ1e && MSZ1k)
                modifiedZone = Zone.Replace("e", "");
                modifiedZone = modifiedZone.Replace("k", "");
                modifiedScene = Scene.Replace("Scene", "");
                modifiedScene = modifiedScene.Replace(".bin", "");
                modifiedScene = modifiedScene.Replace("1e", "1");
                modifiedScene = modifiedScene.Replace("1k", "1");
                actFile = "Encore" + modifiedZone + modifiedScene + ".act";
                newPal = Path.Combine(DataDirectory, "Palettes", actFile);

                //Debug.Print(newPal);
                if (File.Exists(newPal))
                {
                    //Debug.Print("First Check Passed");
                    return newPal;
                }
                else
                {
                    //Second Check (intended for external data folders)
                    actFile = "Encore" + Zone + ".act";
                    newPal = Path.Combine(DataDirectory, "Palettes", actFile);
                    //Debug.Print(newPal);
                    if (File.Exists(newPal))
                    {
                        //Debug.Print("Second Check Passed");
                        return newPal;
                    }
                    else
                    {
                        //Third Check (intended for MSZ1e && MSZ1k (in mods))
                        modifiedZone = Zone.Replace("e", "");
                        modifiedZone = modifiedZone.Replace("k", "");
                        modifiedScene = Scene.Replace("Scene", "");
                        modifiedScene = modifiedScene.Replace(".bin", "");
                        modifiedScene = modifiedScene.Replace("1e", "1");
                        modifiedScene = modifiedScene.Replace("1k", "1");
                        actFile = "Encore" + modifiedZone + modifiedScene + ".act";
                        newPal = Path.Combine(DataDirectory, "Palettes", actFile);

                        //Debug.Print(newPal);
                        if (File.Exists(newPal))
                        {
                            //Debug.Print("Third Check Passed");
                            return newPal;
                        }

                        //Debug.Print("Did not find a Work Around");
                        return "";
                    }


                }


            }

            public void Save(string filename)
            {
                // save any changes made to the scrolling horizontal rules
                foreach (var el in _editorLayers)
                {
                    el.WriteHorizontalLineRules();
                }
                Layers = _editorLayers.Select(el => el.Layer).ToList();
                Write(filename);
            }

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // dispose managed state (managed objects).
                        foreach (var el in _editorLayers)
                        {
                            el.Dispose();
                        }
                    }

                    // free unmanaged resources (unmanaged objects) and override a finalizer below if we need to.
                    // then set large fields to null.

                    disposedValue = true;
                }
            }

            // Override this finalizer only if Dispose(bool disposing) ever gains code to free unmanaged resources.
            // ~EditorScene() {
            //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            //   Dispose(false);
            // }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                // uncomment the following line if the finalizer is overridden above.
                // GC.SuppressFinalize(this);
            }
            #endregion
        }
        public class EditorTiles
        {
            private Editor Instance;
            public StageTiles StageTiles;

            public EditorTiles(Editor instance)
            {
                Instance = instance;
            }

            public void Dispose()
            {
                if (StageTiles != null) StageTiles.Dispose();
            }

            public void DisposeTextures()
            {
                if (StageTiles != null) StageTiles.DisposeTextures();
            }
        }
        public class EditorLayer : IDrawable
        {
            private SceneLayer _layer;
            internal SceneLayer Layer { get => _layer; }

            public DevicePanel GraphicsPanel;

            public Editor EditorInstance;

            private ChunkVBO[][] ChunkMap;

            public PointsMap SelectedTiles;
            public PointsMap TempSelectionTiles;
            public PointsMap TempSelectionDeselectTiles;



            bool TempSelectionDeselect;


            public int GlobalSelectedTiles;
            public bool ShowLayerScrollLines { get; set; } = false;
            public bool HasHorizontalLayerScrollInitilized { get; set; } = false;
            public bool AllowLayerToAnimateParallax { get; set; } = false;

            public int ChunksWidth { get; set; }
            public int ChunksHeight { get; set; }


            bool FirstDrag;
            bool isDragOver;

            public List<IAction> Actions = new List<IAction>();

            public string Name
            {
                get
                {

                    string internalName = _layer.Name;
                    return internalName?.TrimEnd('\0');
                }
                set
                {
                    string name = value;
                    if (name == null) name = "\0";
                    if (!name.EndsWith("\0")) name += "\0";
                    _layer.Name = name;
                }
            }

            public byte Behaviour
            {
                get => _layer.Behaviour;
                set => _layer.Behaviour = value;
            }

            public byte DrawingOrder
            {
                get => _layer.DrawingOrder;
                set => _layer.DrawingOrder = value;
            }

            public short RelativeSpeed
            {
                get => _layer.RelativeSpeed;
                set => _layer.RelativeSpeed = value;
            }

            public short ConstantSpeed
            {
                get => _layer.ConstantSpeed;
                set => _layer.ConstantSpeed = value;
            }

            public ushort Height { get => _layer.Height; }
            public ushort Width { get => _layer.Width; }
            public int HeightPixels { get => _layer.Height * EditorConstants.TILE_SIZE; }
            public int WidthPixels { get => _layer.Width * EditorConstants.TILE_SIZE; }

            /// <summary>
            /// Collection of rules and mappings representing the horizontal scrolling info
            /// and other rules affecting lines of pixels in this layer
            /// </summary>
            public IList<HorizontalLayerScroll> HorizontalLayerScroll { get => _horizontalLayerRules; }
            private IList<HorizontalLayerScroll> _horizontalLayerRules;

            static int DivideRoundUp(int number, int by)
            {
                return (number + by - 1) / by;
            }



            public class PointsMap
            {
                public Dictionary<Point, ushort> Values = new Dictionary<Point, ushort>();
                HashSet<Point>[][] PointsChunks;
                HashSet<Point> OutOfBoundsPoints = new HashSet<Point>();
                public int Count = 0;

                public PointsMap(int width, int height)
                {
                    PointsChunks = new HashSet<Point>[DivideRoundUp(height, EditorConstants.TILES_CHUNK_SIZE)][];
                    for (int i = 0; i < PointsChunks.Length; ++i)
                    {
                        PointsChunks[i] = new HashSet<Point>[DivideRoundUp(width, EditorConstants.TILES_CHUNK_SIZE)];
                        for (int j = 0; j < PointsChunks[i].Length; ++j)
                            PointsChunks[i][j] = new HashSet<Point>();
                    }
                }

                public void Add(Point point)
                {

                    HashSet<Point> h;
                    if (point.Y < 0 || point.X < 0 || point.Y / EditorConstants.TILES_CHUNK_SIZE >= PointsChunks.Length || point.X / EditorConstants.TILES_CHUNK_SIZE >= PointsChunks[0].Length)
                        h = OutOfBoundsPoints;
                    else
                        h = PointsChunks[point.Y / EditorConstants.TILES_CHUNK_SIZE][point.X / EditorConstants.TILES_CHUNK_SIZE];
                    Count -= h.Count;
                    h.Add(point);
                    Count += h.Count;
                }

                public void Remove(Point point)
                {
                    HashSet<Point> h;
                    if (point.Y < 0 || point.X < 0 || point.Y / EditorConstants.TILES_CHUNK_SIZE >= PointsChunks.Length || point.X / EditorConstants.TILES_CHUNK_SIZE >= PointsChunks[0].Length)
                        h = OutOfBoundsPoints;
                    else
                        h = PointsChunks[point.Y / EditorConstants.TILES_CHUNK_SIZE][point.X / EditorConstants.TILES_CHUNK_SIZE];
                    Count -= h.Count;
                    h.Remove(point);
                    Count += h.Count;
                }

                public bool Contains(Point point)
                {
                    if (point.Y < 0 || point.X < 0 || point.Y / EditorConstants.TILES_CHUNK_SIZE >= PointsChunks.Length || point.X / EditorConstants.TILES_CHUNK_SIZE >= PointsChunks[0].Length)
                        return OutOfBoundsPoints.Contains(point);
                    else
                        return PointsChunks[point.Y / EditorConstants.TILES_CHUNK_SIZE][point.X / EditorConstants.TILES_CHUNK_SIZE].Contains(point);
                }

                public bool IsChunkUsed(int x, int y)
                {
                    return PointsChunks[y][x].Count > 0;
                }

                public void Clear()
                {
                    for (int i = 0; i < PointsChunks.Length; ++i)
                        for (int j = 0; j < PointsChunks[i].Length; ++j)
                            PointsChunks[i][j].Clear();
                    OutOfBoundsPoints.Clear();
                    Count = 0;
                }

                public HashSet<Point> GetChunkPoint(int x, int y)
                {
                    return PointsChunks[y][x];
                }

                public List<Point> PopAll()
                {
                    List<Point> points = GetAll();
                    Clear();
                    return points;
                }

                public List<Point> GetAll()
                {
                    List<Point> points = new List<Point>(Count);
                    for (int i = 0; i < PointsChunks.Length; ++i)
                        for (int j = 0; j < PointsChunks[i].Length; ++j)
                            points.AddRange(PointsChunks[i][j]);
                    points.AddRange(OutOfBoundsPoints);
                    return points;
                }

                public void AddPoints(List<Point> points)
                {
                    points.ForEach(point => Add(point));
                }


            }

            public EditorLayer(SceneLayer layer, Editor instance)
            {
                EditorInstance = instance;
                _layer = layer;

                SelectedTiles = new PointsMap(Width, Height);
                TempSelectionTiles = new PointsMap(Width, Height);
                TempSelectionDeselectTiles = new PointsMap(Width, Height);

                _horizontalLayerRules = ReadHorizontalLineRules();
                InitiallizeChunkMap();
            }

            private void InitiallizeChunkMap()
            {
                ChunksWidth = DivideRoundUp(Width, EditorConstants.TILES_CHUNK_SIZE);
                ChunksHeight = DivideRoundUp(Height, EditorConstants.TILES_CHUNK_SIZE);

                ChunkMap = new ChunkVBO[ChunksHeight][];
                for (int i = 0; i < ChunkMap.Length; ++i)
                {
                    ChunkMap[i] = new ChunkVBO[ChunksWidth];
                }

            }

            /// <summary>
            /// Interpret the layer's set of horizontal scroll rules (ScrollInfo),
            /// and the line level map (ScrollIndexes) into that set of rules.
            /// </summary>
            /// <returns>List of HorizontalLayerScroll objects containing the scrolling rules</returns>
            private IList<HorizontalLayerScroll> ReadHorizontalLineRules()
            {
                var tempList = new List<HorizontalLayerScroll>();
                byte generatedId = 0;
                foreach (var scrollInfo in _layer.ScrollingInfo)
                {
                    tempList.Add(new HorizontalLayerScroll(generatedId, scrollInfo));
                    ++generatedId;
                }

                var ruleMapCount = _layer.ScrollIndexes.Count();
                int i = 0;
                while (i < ruleMapCount)
                {
                    var currentValue = _layer.ScrollIndexes[i];
                    var currentRule = _layer.ScrollingInfo[currentValue];
                    var currentCount = 0;
                    var start = i;
                    while (i < ruleMapCount
                           && currentValue == _layer.ScrollIndexes[i])
                    {
                        ++currentCount;
                        ++i;
                    }

                    tempList.First(hlr => hlr.Id == currentValue).AddMapping(start, currentCount);
                }

                return tempList;
            }

            /// <summary>
            /// Persist the contents of the HorizontalLayerRules collection,
            /// to the Layer's RSDKv5 backing objects and arrays.
            /// </summary>
            public void WriteHorizontalLineRules()
            {
                var newIndexes = new byte[_layer.ScrollIndexes.Length];
                _layer.ScrollingInfo = _horizontalLayerRules.Select(hlr => hlr.ScrollInfo).ToList();

                // the internal ID may now be inaccurate
                // we were only using it for display purposes anyway
                // generate some correct ones, and use those!
                byte newIndex = 0;
                foreach (var hlr in _horizontalLayerRules)
                {
                    foreach (var lml in hlr.LinesMapList)
                    {
                        var count = lml.LineCount;
                        int index = lml.StartIndex;
                        for (int i = 0; i < count; i++)
                        {
                            newIndexes[index + i] = newIndex;
                        }
                    }
                    ++newIndex;
                }

                _layer.ScrollIndexes = newIndexes;
            }

            /// <summary>
            /// Creates a new HorizontalLayerScroll object with backing ScrollInfo object.
            /// Adding it to the HorizontalLayerRules collection.
            /// </summary>
            public void ProduceHorizontalLayerScroll()
            {
                var id = (byte)(_horizontalLayerRules.Select(hlr => hlr.Id).Max() + 1);
                var info = new ScrollInfo();

                _layer.ScrollingInfo.Add(info);
                var hls = new HorizontalLayerScroll(id, info);
                _horizontalLayerRules.Add(hls);
            }

            public void StartDrag()
            {
                FirstDrag = true;
                RefreshTileCount();
                InvalidateChunks();
            }

            public void StartDragOver(Point point, ushort value)
            {
                Deselect();
                isDragOver = true;
                DragOver(point, value);
                RefreshTileCount();
                InvalidateChunks();
            }

            public void DragOver(Point point, ushort value)
            {
                SelectedTiles.Clear();
                SelectedTiles.Values.Clear();
                point = new Point(point.X / EditorConstants.TILE_SIZE, point.Y / EditorConstants.TILE_SIZE);
                SelectedTiles.Add(point);
                SelectedTiles.Values[point] = value;
                RefreshTileCount();
                InvalidateChunks();
            }

            public void EndDragOver(bool remove)
            {
                if (isDragOver)
                {
                    if (remove)
                    {
                        SelectedTiles.Clear();
                        SelectedTiles.Values.Clear();
                        RefreshTileCount();
                    }
                    isDragOver = false;
                    RefreshTileCount();
                }
                InvalidateChunks();
            }

            private void DetachSelected()
            {
                foreach (Point point in SelectedTiles.GetAll())
                {
                    if (!SelectedTiles.Values.ContainsKey(point))
                    {
                        // Not moved yet
                        SelectedTiles.Values[point] = _layer.Tiles[point.Y][point.X];
                        RemoveTile(point);
                        RefreshTileCount();
                    }
                }
                InvalidateChunks();
            }

            public void MoveSelected(Point oldPos, Point newPos, bool duplicate, bool chunkAlign = false)
            {
                oldPos = new Point(oldPos.X / EditorConstants.TILE_SIZE, oldPos.Y / EditorConstants.TILE_SIZE);
                newPos = new Point(newPos.X / EditorConstants.TILE_SIZE, newPos.Y / EditorConstants.TILE_SIZE);
                if (oldPos != newPos)
                {
                    duplicate &= FirstDrag;
                    FirstDrag = false;
                    Dictionary<Point, ushort> newDict = new Dictionary<Point, ushort>();
                    List<Point> newPoints = new List<Point>(SelectedTiles.Count);
                    foreach (Point point in SelectedTiles.PopAll())
                    {
                        Point newPoint = new Point(point.X + (newPos.X - oldPos.X), point.Y + (newPos.Y - oldPos.Y));
                        newPoints.Add(newPoint);
                        if (SelectedTiles.Values.ContainsKey(point))
                        {
                            newDict[newPoint] = SelectedTiles.Values[point];

                        }
                        else
                        {
                            // Not moved yet
                            newDict[newPoint] = _layer.Tiles[point.Y][point.X];
                            if (!duplicate) RemoveTile(point);

                        }
                        InvalidateChunkFromPixelPosition(point);
                        InvalidateChunkFromPixelPosition(newPoint);
                    }
                    if (duplicate)
                    {
                        Deselect();
                        // Create new actions group
                        Actions.Add(new ActionDummy());
                        RefreshTileCount();
                    }
                    SelectedTiles.Values = newDict;
                    SelectedTiles.AddPoints(newPoints);
                    RefreshTileCount();
                }
            }

            public static Point GetChunkCoordinatesTopEdge(int x, int y)
            {
                Point ChunkCoordinate = new Point();
                if (x != 0) ChunkCoordinate.X = x / 128;
                else ChunkCoordinate.X = 0;
                if (y != 0) ChunkCoordinate.Y = y / 128;
                else ChunkCoordinate.Y = 0;

                return new Point(ChunkCoordinate.X * 128, ChunkCoordinate.Y * 128);
            }

            public static Point GetChunkCoordinatesBottomEdge(int x, int y)
            {
                Point ChunkCoordinate = new Point();
                if (x != 0) ChunkCoordinate.X = x / 128;
                else ChunkCoordinate.X = 0;
                if (y != 0) ChunkCoordinate.Y = y / 128;
                else ChunkCoordinate.Y = 0;

                return new Point((ChunkCoordinate.X * 128) + 16 * 8, (ChunkCoordinate.Y * 128) + 16 * 8);
            }

            public static Point GetDrawingChunkCoordinates(int x, int y)
            {
                Point ChunkCoordinate = new Point();
                if (x != 0) ChunkCoordinate.X = x / 16;
                else ChunkCoordinate.X = 0;
                if (y != 0) ChunkCoordinate.Y = y / 16;
                else ChunkCoordinate.Y = 0;

                return ChunkCoordinate;
            }

            public static Point GetChunkCoordinates(int x, int y)
            {
                Point ChunkCoordinate = new Point();
                if (x != 0) ChunkCoordinate.X = x / 128;
                else ChunkCoordinate.X = 0;
                if (y != 0) ChunkCoordinate.Y = y / 128;
                else ChunkCoordinate.Y = 0;

                return ChunkCoordinate;
            }

            public static Point GetChunkCoordinates(double x, double y)
            {
                Point ChunkCoordinate = new Point();
                if (x != 0) ChunkCoordinate.X = (int)(x / 128);
                else ChunkCoordinate.X = 0;
                if (y != 0) ChunkCoordinate.Y = (int)(y / 128);
                else ChunkCoordinate.Y = 0;

                return ChunkCoordinate;
            }

            public void MoveSelectedQuonta(Point change)
            {
                MoveSelected(Point.Empty, new Point(change.X * EditorConstants.TILE_SIZE, change.Y * EditorConstants.TILE_SIZE), false);
            }

            public void DeleteSelected()
            {
                bool removedSomething = SelectedTiles.Count > 0;
                foreach (Point p in SelectedTiles.PopAll())
                {
                    // Remove only tiles that not moved, because we already removed the moved tiles
                    if (!SelectedTiles.Values.ContainsKey(p))
                    {
                        RemoveTile(p);
                    }
                }
                if (removedSomething)
                {
                    Actions.Add(new ActionsGroupCloseMarker());
                }

                SelectedTiles.Values.Clear();
                RefreshTileCount();
                InvalidateChunks();

            }

            public void FlipPropertySelected(FlipDirection direction, bool flipIndividually = false)
            {
                DetachSelected();
                List<Point> points = new List<Point>(SelectedTiles.Values.Keys);

                if (points.Count == 0) return;

                if (points.Count == 1 || flipIndividually)
                {
                    FlipIndividualTiles(direction, points);
                    return;
                }

                IEnumerable<int> monoCoordinates;

                if (direction == FlipDirection.Horizontal)
                {
                    monoCoordinates = points.Select(p => p.X);
                }
                else
                {
                    monoCoordinates = points.Select(p => p.Y);
                }

                int min = monoCoordinates.Min();
                int max = monoCoordinates.Max();
                int diff = max - min;

                if (diff == 0)
                {
                    FlipIndividualTiles(direction, points);
                }
                else
                {
                    FlipGroupTiles(direction, points, min, max);
                }
                InvalidateChunks();
            }

            private void FlipIndividualTiles(FlipDirection direction, IEnumerable<Point> points)
            {
                foreach (Point point in points)
                {
                    SelectedTiles.Values[point] ^= (ushort)direction;
                }
                InvalidateChunks();
            }

            private void FlipGroupTiles(FlipDirection direction, IEnumerable<Point> points, int min, int max)
            {
                Dictionary<Point, ushort> workingTiles = new Dictionary<Point, ushort>();
                foreach (Point point in points)
                {
                    ushort tileValue = SelectedTiles.Values[point];
                    Point newPoint;

                    if (direction == FlipDirection.Horizontal)
                    {
                        int fromLeft = point.X - min;
                        int fromRight = max - point.X;

                        int newX = fromLeft < fromRight ? max - fromLeft : min + fromRight;
                        newPoint = new Point(newX, point.Y);
                    }
                    else
                    {
                        int fromBottom = point.Y - min;
                        int fromTop = max - point.Y;

                        int newY = fromBottom < fromTop ? max - fromBottom : min + fromTop;
                        newPoint = new Point(point.X, newY);
                    }

                    workingTiles.Add(newPoint, tileValue ^= (ushort)direction);
                }

                SelectedTiles.Clear();
                SelectedTiles.Values.Clear();
                SelectedTiles.AddPoints(workingTiles.Select(wt => wt.Key).ToList());
                SelectedTiles.Values = workingTiles;
                InvalidateChunks();
            }

            public void SetPropertySelected(int bit, bool state)
            {
                DetachSelected();

                List<Point> points = new List<Point>(SelectedTiles.Values.Keys);
                foreach (Point point in points)
                {
                    if (state)
                        SelectedTiles.Values[point] |= (ushort)(1 << bit);
                    else
                        SelectedTiles.Values[point] &= (ushort)(~(1 << bit));
                }
            }

            public List<ushort> GetSelectedValues()
            {
                // Including temp selection
                List<ushort> selectedValues = new List<ushort>();
                foreach (Point point in SelectedTiles.GetAll())
                {
                    if (TempSelectionDeselect && TempSelectionTiles.Contains(point)) continue;
                    if (SelectedTiles.Values.ContainsKey(point))
                    {
                        selectedValues.Add(SelectedTiles.Values[point]);
                    }
                    else
                    {
                        // Not moved yet
                        selectedValues.Add(_layer.Tiles[point.Y][point.X]);
                    }
                }
                foreach (Point point in TempSelectionTiles.GetAll())
                {
                    if (SelectedTiles.Contains(point)) continue;
                    selectedValues.Add(_layer.Tiles[point.Y][point.X]);
                }
                RefreshTileCount();
                return selectedValues;
            }

            public Dictionary<Point, ushort> CopyToClipboard(bool keepPosition = false)
            {
                if (SelectedTiles.Count == 0) return null;
                int minX = 0, minY = 0;

                Dictionary<Point, ushort> copiedTiles = new Dictionary<Point, ushort>(SelectedTiles.Values); ;
                foreach (Point point in SelectedTiles.GetAll())
                {
                    if (!copiedTiles.ContainsKey(point))
                    {
                        // Not moved yet
                        copiedTiles[point] = GetTile(point);
                    }
                }
                if (!keepPosition)
                {
                    minX = copiedTiles.Keys.Min(x => x.X);
                    minY = copiedTiles.Keys.Min(x => x.Y);
                }
                return copiedTiles.ToDictionary(x => new Point(x.Key.X - minX, x.Key.Y - minY), x => x.Value);
            }

            public static Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>> CopyMultiSelectionToClipboard(EditorLayer layer1, EditorLayer layer2, bool keepPosition = false)
            {
                if (layer1.SelectedTiles.Count == 0 && layer2.SelectedTiles.Count == 0) return null;
                int minX = 0, minY = 0;

                Dictionary<Point, ushort> copiedTilesA = new Dictionary<Point, ushort>(layer1.SelectedTiles.Values);
                Dictionary<Point, ushort> copiedTilesB = new Dictionary<Point, ushort>(layer2.SelectedTiles.Values);
                foreach (Point point in layer1.SelectedTiles.GetAll())
                {
                    if (!copiedTilesA.ContainsKey(point))
                    {
                        // Not moved yet
                        copiedTilesA[point] = layer1.GetTile(point);
                    }
                }
                foreach (Point point in layer2.SelectedTiles.GetAll())
                {
                    if (!copiedTilesB.ContainsKey(point))
                    {
                        // Not moved yet
                        copiedTilesB[point] = layer2.GetTile(point);
                    }
                }
                if (!keepPosition)
                {
                    int minX_A = (copiedTilesA.Count != 0 ? copiedTilesA.Keys.Min(x => x.X) : 0);
                    int minY_A = (copiedTilesA.Count != 0 ? copiedTilesA.Keys.Min(x => x.Y) : 0);
                    int minX_B = (copiedTilesB.Count != 0 ? copiedTilesB.Keys.Min(x => x.X) : 0);
                    int minY_B = (copiedTilesB.Count != 0 ? copiedTilesB.Keys.Min(x => x.Y) : 0);
                    minX = Math.Min(minX_A, minX_B);
                    minY = Math.Min(minY_A, minY_B);
                }
                copiedTilesA = copiedTilesA.ToDictionary(x => new Point(x.Key.X - minX, x.Key.Y - minY), x => x.Value);
                copiedTilesB = copiedTilesB.ToDictionary(x => new Point(x.Key.X - minX, x.Key.Y - minY), x => x.Value);
                return new Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>>(copiedTilesA, copiedTilesB);
            }

            public void DrawAsBrush(Point newPos, Dictionary<Point, ushort> points)
            {
                try
                {
                    bool updateActions = false;
                    newPos = new Point(newPos.X / EditorConstants.TILE_SIZE, newPos.Y / EditorConstants.TILE_SIZE);
                    Deselect();
                    foreach (KeyValuePair<Point, ushort> point in points)
                    {
                        Point tilePos = new Point(point.Key.X + newPos.X, point.Key.Y + newPos.Y);
                        if (point.Value != _layer.Tiles[tilePos.Y][tilePos.X])
                        {
                            SelectedTiles.Add(tilePos);
                            SelectedTiles.Values[tilePos] = point.Value;
                            updateActions = true;
                        }
                    }
                    // Create new actions group
                    if (updateActions) Actions.Add(new ActionDummy());
                    RefreshTileCount();
                }
                catch
                {
                    MessageBox.Show("Clipboard Content Problem!");
                }
            }

            public void PasteFromClipboard(Point newPos, Dictionary<Point, ushort> points, bool updateActions = true)
            {
                try
                {
                    newPos = new Point(newPos.X / EditorConstants.TILE_SIZE, newPos.Y / EditorConstants.TILE_SIZE);
                    Deselect();
                    foreach (KeyValuePair<Point, ushort> point in points)
                    {
                        Point tilePos = new Point(point.Key.X + newPos.X, point.Key.Y + newPos.Y);
                        SelectedTiles.Add(tilePos);
                        SelectedTiles.Values[tilePos] = point.Value;
                    }
                    // Create new actions group
                    if (updateActions) Actions.Add(new ActionDummy());
                    RefreshTileCount();
                }
                catch
                {
                    MessageBox.Show("Clipboard Content Problem!");
                }

            }

            public void SelectAll()
            {
                for (int y = 0; y < _layer.Tiles.Length; y += 1)
                {
                    for (int x = 0; x < _layer.Tiles[y].Length; x += 1)
                    {
                        if (_layer.Tiles[y][x] != 0xffff)
                        {
                            SelectedTiles.Add(new Point(x, y));
                            RefreshTileCount();
                        }
                        else if (_layer.Tiles[y][x] == 0xffff && EditorInstance.Options.CopyAir)
                        {
                            SelectedTiles.Add(new Point(x, y));
                            RefreshTileCount();
                        }

                    }
                }
                RefreshTileCount();
                InvalidateChunks();
            }

            public void Select(Rectangle area, bool addSelection = false, bool deselectIfSelected = false)
            {
                if (!addSelection) Deselect();
                for (int y = Math.Max(area.Y / EditorConstants.TILE_SIZE, 0); y < Math.Min(DivideRoundUp(area.Y + area.Height, EditorConstants.TILE_SIZE), _layer.Height); ++y)
                {
                    for (int x = Math.Max(area.X / EditorConstants.TILE_SIZE, 0); x < Math.Min(DivideRoundUp(area.X + area.Width, EditorConstants.TILE_SIZE), _layer.Width); ++x)
                    {
                        if (addSelection || deselectIfSelected)
                        {
                            Point p = new Point(x, y);
                            if (SelectedTiles.Contains(p))
                            {
                                if (deselectIfSelected)
                                {
                                    // Deselect
                                    DeselectPoint(p);
                                    RefreshTileCount();
                                }
                                // Don't add already selected tile, or if it was just deslected
                                continue;
                            }
                        }
                        if (_layer.Tiles[y][x] != 0xffff)
                        {
                            SelectedTiles.Add(new Point(x, y));
                            RefreshTileCount();
                        }
                        else if (_layer.Tiles[y][x] == 0xffff && EditorInstance.Options.CopyAir)
                        {
                            SelectedTiles.Add(new Point(x, y));
                            RefreshTileCount();
                        }
                    }
                }
                InvalidateChunks();
            }

            public void Select(Point point, bool addSelection = false, bool deselectIfSelected = false)
            {
                if (!addSelection) Deselect();
                point = new Point(point.X / EditorConstants.TILE_SIZE, point.Y / EditorConstants.TILE_SIZE);
                EditorStateModel.SelectedTileX = point.X;
                EditorStateModel.SelectedTileY = point.Y;
                if (point.X >= 0 && point.Y >= 0 && point.X < this._layer.Tiles[0].Length && point.Y < this._layer.Tiles.Length)
                {
                    if (deselectIfSelected && SelectedTiles.Contains(point))
                    {
                        // Deselect
                        DeselectPoint(point);
                        RefreshTileCount();
                    }
                    else if (this._layer.Tiles[point.Y][point.X] != 0xffff || EditorInstance.Options.CopyAir)
                    {
                        // Just add the point
                        SelectedTiles.Add(point);
                        RefreshTileCount();
                    }
                }
                InvalidateChunks();
            }

            public void TempSelection(Rectangle area, bool deselectIfSelected)
            {
                TempSelectionTiles.Clear();
                TempSelectionDeselectTiles.Clear();
                TempSelectionDeselect = deselectIfSelected;
                for (int y = Math.Max(area.Y / EditorConstants.TILE_SIZE, 0); y < Math.Min(DivideRoundUp(area.Y + area.Height, EditorConstants.TILE_SIZE), _layer.Height); ++y)
                {
                    for (int x = Math.Max(area.X / EditorConstants.TILE_SIZE, 0); x < Math.Min(DivideRoundUp(area.X + area.Width, EditorConstants.TILE_SIZE), _layer.Width); ++x)
                    {
                        if (SelectedTiles.Contains(new Point(x, y)) || (_layer.Tiles[y][x] != 0xffff || EditorInstance.Options.CopyAir))
                        {
                            TempSelectionTiles.Add(new Point(x, y));
                            if (SelectedTiles.Contains(new Point(x, y)) && TempSelectionTiles.Contains(new Point(x, y)))
                            {
                                TempSelectionDeselectTiles.Add(new Point(x, y));
                            }
                            RefreshTileCount();


                        }
                    }
                }
                InvalidateChunks();
            }

            public void EndTempSelection()
            {
                TempSelectionTiles.Clear();
                TempSelectionDeselectTiles.Clear();
                RefreshTileCount();
                InvalidateChunks();
            }

            private ushort GetTile(Point point)
            {
                return _layer.Tiles[point.Y][point.X];

            }

            private void SetTile(Point point, ushort value, bool addAction = true)
            {
                if (addAction)
                    Actions.Add(new ActionChangeTile((x, y) => SetTile(x, y, false), point, _layer.Tiles[point.Y][point.X], value));
                _layer.Tiles[point.Y][point.X] = value;
                InvalidateChunk(point.X / EditorConstants.TILES_CHUNK_SIZE, point.Y / EditorConstants.TILES_CHUNK_SIZE);
            }

            private void RemoveTile(Point point)
            {
                SetTile(point, 0xffff);
                RefreshTileCount();
            }

            private void DeselectPoint(Point p)
            {
                if (SelectedTiles.Values.ContainsKey(p))
                {
                    // Or else it wasn't moved at all
                    SetTile(p, SelectedTiles.Values[p]);
                    SelectedTiles.Values.Remove(p);
                }
                SelectedTiles.Remove(p);
            }

            public void Deselect()
            {
                bool hasTiles = SelectedTiles.Values.Count > 0;
                foreach (KeyValuePair<Point, ushort> point in SelectedTiles.Values)
                {
                    // ignore out of bounds
                    if (point.Key.X < 0 || point.Key.Y < 0 || point.Key.Y >= _layer.Height || point.Key.X >= _layer.Width) continue;
                    SetTile(point.Key, point.Value);
                }
                if (hasTiles)
                    Actions.Add(new ActionsGroupCloseMarker());

                SelectedTiles.Clear();
                SelectedTiles.Values.Clear();
                EditorStateModel.SelectedTilesCount = 0;
            }

            public bool IsPointSelected(Point point)
            {
                return SelectedTiles.Contains(new Point(point.X / EditorConstants.TILE_SIZE, point.Y / EditorConstants.TILE_SIZE));
            }

            public bool DoesChunkContainASelectedTile(Point point)
            {
                Point startingPoint = new Point(point.X / EditorConstants.TILE_SIZE, point.Y / EditorConstants.TILE_SIZE);
                List<Point> chunkPoints = new List<Point>();
                for (int x = 0; x < (EditorConstants.x128_CHUNK_SIZE / EditorConstants.TILE_SIZE); x++)
                {
                    for (int y = 0; y < (EditorConstants.x128_CHUNK_SIZE / EditorConstants.TILE_SIZE); y++)
                    {
                        Point p = new Point(startingPoint.X + x, startingPoint.Y + y);
                        if (SelectedTiles.Contains(p)) return true;
                        else continue;
                    }
                }
                return false;
            }

            public bool HasTileAt(Point point)
            {
                point = new Point(point.X / EditorConstants.TILE_SIZE, point.Y / EditorConstants.TILE_SIZE);
                if (point.X >= 0 && point.Y >= 0 && point.X < this._layer.Tiles[0].Length && point.Y < this._layer.Tiles.Length)
                {
                    return (_layer.Tiles[point.Y][point.X] != 0xffff || EditorInstance.Options.CopyAir);
                }
                return false;
            }

            public bool OnlyHasTileIn(Rectangle area, ushort tile)
            {
                area = new Rectangle(area.X / EditorConstants.TILE_SIZE, area.Y / EditorConstants.TILE_SIZE, area.Width / EditorConstants.TILE_SIZE, area.Height / EditorConstants.TILE_SIZE);
                for (int x = area.X; x < area.X + area.Width; x++)
                {
                    for (int y = area.Y; y < area.Y + area.Height; y++)
                    {
                        Point point = new Point(x, y);

                        if (SelectedTiles.Values.ContainsKey(point))
                        {
                            if (SelectedTiles.Values[point] != tile) return false;
                        }
                        else
                        {
                            if (_layer.Tiles[point.Y][point.X] != tile) return false;
                        }
                    }
                }
                return true;
            }

            public ushort GetTileAt(Point point)
            {
                point = new Point(point.X / EditorConstants.TILE_SIZE, point.Y / EditorConstants.TILE_SIZE);
                if (point.X >= 0 && point.Y >= 0 && point.X < this._layer.Tiles[0].Length && point.Y < this._layer.Tiles.Length)
                {
                    if (SelectedTiles.Values.ContainsKey(point)) return SelectedTiles.Values[point];
                    else return _layer.Tiles[point.Y][point.X];
                }
                return 0xffff;
            }

            public ushort GetTileAt(int x, int y)
            {
                Point point = new Point(x, y);
                if (point.X >= 0 && point.Y >= 0 && point.X < this._layer.Tiles[0].Length && point.Y < this._layer.Tiles.Length)
                {
                    if (SelectedTiles.Values.ContainsKey(point)) return SelectedTiles.Values[point];
                    else return _layer.Tiles[point.Y][point.X];
                }
                return 0xffff;
            }



            private Rectangle GetChunkArea(int x, int y)
            {
                return new Rectangle(x, y, 128, 128);
            }

            private Rectangle GetTilesChunkArea(int x, int y)
            {
                int y_start = y * EditorConstants.TILES_CHUNK_SIZE;
                int y_end = Math.Min((y + 1) * EditorConstants.TILES_CHUNK_SIZE, _layer.Height);

                int x_start = x * EditorConstants.TILES_CHUNK_SIZE;
                int x_end = Math.Min((x + 1) * EditorConstants.TILES_CHUNK_SIZE, _layer.Width);


                return new Rectangle(x_start, y_start, x_end - x_start, y_end - y_start);
            }







            public void Draw(Graphics g)
            {
                for (int y = 0; y < _layer.Height; ++y)
                {
                    for (int x = 0; x < _layer.Width; ++x)
                    {
                        if (this._layer.Tiles[y][x] != 0xffff)
                        {
                            OldDrawTile(g, _layer.Tiles[y][x], x, y);
                        }
                    }
                }
            }

            public void OldDrawTile(Graphics g, ushort tile, int x, int y)
            {
                ushort TileIndex = (ushort)(tile & 0x3ff);
                int TileIndexInt = (int)TileIndex;
                bool flipX = ((tile >> 10) & 1) == 1;
                bool flipY = ((tile >> 11) & 1) == 1;
                bool SolidTopA = ((tile >> 12) & 1) == 1;
                bool SolidLrbA = ((tile >> 13) & 1) == 1;
                bool SolidTopB = ((tile >> 14) & 1) == 1;
                bool SolidLrbB = ((tile >> 15) & 1) == 1;

                System.Drawing.Color AllSolid = System.Drawing.Color.FromArgb((int)EditorInstance.EditorToolbar.collisionOpacitySlider.Value, EditorInstance.CollisionAllSolid.R, EditorInstance.CollisionAllSolid.G, EditorInstance.CollisionAllSolid.B);
                System.Drawing.Color LRDSolid = System.Drawing.Color.FromArgb((int)EditorInstance.EditorToolbar.collisionOpacitySlider.Value, EditorInstance.CollisionLRDSolid.R, EditorInstance.CollisionLRDSolid.G, EditorInstance.CollisionLRDSolid.B);
                System.Drawing.Color TopOnlySolid = System.Drawing.Color.FromArgb((int)EditorInstance.EditorToolbar.collisionOpacitySlider.Value, EditorInstance.CollisionTopOnlySolid.R, EditorInstance.CollisionTopOnlySolid.G, EditorInstance.CollisionTopOnlySolid.B);

                g.DrawImage(EditorSolution.CurrentTiles.StageTiles.Image.GetBitmap(new Rectangle(0, TileIndex * EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE), flipX, flipY),
                    new Rectangle(x * EditorConstants.TILE_SIZE, y * EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE));

                if (Editor.Instance.Options.ShowCollisionA)
                {
                    if (SolidLrbA || SolidTopA)
                    {
                        if (SolidTopA && SolidLrbA) DrawCollision(true, AllSolid);
                        if (SolidTopA && !SolidLrbA) DrawCollision(true, TopOnlySolid);
                        if (SolidLrbA && !SolidTopA) DrawCollision(true, LRDSolid);
                    }
                }
                if (Editor.Instance.Options.ShowCollisionB)
                {
                    if (SolidLrbB || SolidTopB)
                    {
                        if (SolidTopB && SolidLrbB) DrawCollision(false, AllSolid);
                        if (SolidTopB && !SolidLrbB) DrawCollision(false, TopOnlySolid);
                        if (SolidLrbB && !SolidTopB) DrawCollision(false, LRDSolid);
                    }
                }

                if (EditorInstance.Options.ShowFlippedTileHelper == true)
                {
                    g.DrawImage(EditorSolution.CurrentTiles.StageTiles.EditorImage.GetBitmap(new Rectangle(0, 3 * EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE), false, false),
                                new Rectangle(x * EditorConstants.TILE_SIZE, y * EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE));
                }
                if (EditorInstance.Options.ShowTileID == true)
                {
                    g.DrawImage(EditorSolution.CurrentTiles.StageTiles.IDImage.GetBitmap(new Rectangle(0, TileIndex * EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE), false, false),
                                new Rectangle(x * EditorConstants.TILE_SIZE, y * EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE));
                }

                void DrawCollision(bool drawA, System.Drawing.Color colur)
                {
                    Bitmap Map;
                    if (drawA) Map = EditorSolution.CurrentTiles.StageTiles.CollisionMaskA.GetBitmap(new Rectangle(0, (tile & 0x3ff) * EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE), flipX, flipY);
                    else Map = EditorSolution.CurrentTiles.StageTiles.CollisionMaskB.GetBitmap(new Rectangle(0, (tile & 0x3ff) * EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE), flipX, flipY);

                    Map = Extensions.ChangeImageColor(Map, System.Drawing.Color.White, colur);

                    g.DrawImage(Map, x * EditorConstants.TILE_SIZE, y * EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE);
                }


            }






            #region New Draw Method
            public void Dispose()
            {
                for (int y = 0; y < ChunksHeight; y++)
                {
                    for (int x = 0; x < ChunksWidth; x++)
                    {
                        if (ChunkMap[y][x] != null)
                        {
                            ChunkMap[y][x].Dispose();
                            ChunkMap[y][x] = null;
                        }
                    }
                }
                ChunkMap = null;
            }

            public void InvalidateChunks()
            {
                for (int y = 0; y < ChunksHeight; y++)
                {
                    for (int x = 0; x < ChunksWidth; x++)
                    {
                        if (ChunkMap[y][x] != null)
                        {
                            ChunkMap[y][x].Dispose();
                            ChunkMap[y][x] = null;
                        }
                    }
                }
            }

            public void DisposeTextures()
            {
                InvalidateChunks();
            }

            public void Draw(DevicePanel d)
            {
                int Transperncy;


                if (EditorInstance.EditLayerA != null && (EditorInstance.EditLayerA != this && EditorInstance.EditLayerB != this))
                    Transperncy = 0x32;
                else if (EditorInstance.EditorToolbar.EditEntities.IsCheckedAll && EditorInstance.EditLayerA == null && EditorInstance.Options.ApplyEditEntitiesTransparency)
                    Transperncy = 0x32;
                else
                    Transperncy = 0xFF;

                Rectangle screen = d.GetScreen();
                int pos_x = screen.X;
                int pos_y = screen.Y;
                int width = screen.Width;
                int height = screen.Height;

                if (pos_x >= 0 && pos_y >= 0 && width >= 0 && height >= 0)
                {
                    int start_x = pos_x / (EditorConstants.TILES_CHUNK_SIZE * EditorConstants.TILE_SIZE);
                    int end_x = Math.Min(DivideRoundUp(pos_x + width, EditorConstants.TILES_CHUNK_SIZE * EditorConstants.TILE_SIZE), ChunkMap[0].Length);
                    int start_y = pos_y / (EditorConstants.TILES_CHUNK_SIZE * EditorConstants.TILE_SIZE);
                    int end_y = Math.Min(DivideRoundUp(pos_y + height, EditorConstants.TILES_CHUNK_SIZE * EditorConstants.TILE_SIZE), ChunkMap.Length);

                    for (int y = start_y; y < end_y; ++y)
                    {
                        for (int x = start_x; x < end_x; ++x)
                        {
                            if (d.IsObjectOnScreen(x * 256, y * 256, 256, 256))
                            {
                                Rectangle rect = GetTilesChunkArea(x, y);
                                d.DrawBitmap(GetChunk(d, x, y), rect.X * EditorConstants.TILE_SIZE, rect.Y * EditorConstants.TILE_SIZE, rect.Width * EditorConstants.TILE_SIZE, rect.Height * EditorConstants.TILE_SIZE, false, Transperncy);
                            }
                            else InvalidateChunk(x, y);
                        }
                    }
                    DisposeUnusedChunks();
                }
            }

            private void DisposeUnusedChunks()
            {
                for (int y = 0; y < ChunksHeight; y++)
                {
                    for (int x = 0; x < ChunksWidth; x++)
                    {
                        if (ChunkMap[y][x] != null && ChunkMap[y][x].HasBeenRendered)
                        {
                            ChunkMap[y][x].HasBeenRendered = false;
                        }
                        else if (ChunkMap[y][x] != null)
                        {
                            ChunkMap[y][x].Dispose();
                            ChunkMap[y][x] = null;
                        }
                    }
                }
            }

            private void InvalidateChunk(int x, int y)
            {
                if (ChunkMap[y][x] != null)
                {
                    ChunkMap[y][x].Dispose();
                    ChunkMap[y][x] = null;
                }
            }

            private void InvalidateChunkFromPixelPosition(Point point)
            {
                var chunkPoint = GetDrawingChunkCoordinates(point.X, point.Y);
                if (!(chunkPoint.X >= ChunksWidth || chunkPoint.Y >= ChunksHeight || chunkPoint.Y < 0 || chunkPoint.X < 0))
                {
                    if (ChunkMap[chunkPoint.Y][chunkPoint.X] != null) ChunkMap[chunkPoint.Y][chunkPoint.X].HasBeenSelectedPrior = true;
                }

                Point GetDrawingChunkCoordinates(int x, int y)
                {
                    Point ChunkCoordinate = new Point();
                    if (x != 0) ChunkCoordinate.X = x / 16;
                    else ChunkCoordinate.X = 0;
                    if (y != 0) ChunkCoordinate.Y = y / 16;
                    else ChunkCoordinate.Y = 0;

                    return ChunkCoordinate;
                }

            }
            private bool isChunkSelected(int _x, int _y)
            {
                Rectangle rect = GetTilesChunkArea(_x, _y);

                int x = rect.X * EditorConstants.TILE_SIZE;
                int y = rect.Y * EditorConstants.TILE_SIZE;
                int x2 = rect.Right * EditorConstants.TILE_SIZE;
                int y2 = rect.Bottom * EditorConstants.TILE_SIZE;

                int mouse_x = (int)EditorStateModel.LastX;
                int mouse_y = (int)EditorStateModel.LastY;

                if (mouse_x >= x && mouse_x <= x2 && mouse_y >= y && mouse_y <= y2)
                {
                    //System.Diagnostics.Debug.Print(string.Format("Chunk {0},{1} Selected", _x, _y));
                    return true;
                }
                else return false;

            }

            public Texture GetChunk(DevicePanel d, int x, int y)
            {
                bool isSelected = isChunkSelected(x, y);
                if (ChunkMap[y][x] != null && ChunkMap[y][x].IsReady && !ChunkMap[y][x].HasBeenSelectedPrior && !isSelected)
                {
                    ChunkMap[y][x].HasBeenRendered = true;
                    return ChunkMap[y][x].Texture;
                }
                else
                {
                    if (ChunkMap[y][x] != null && ChunkMap[y][x].HasBeenSelectedPrior) InvalidateChunk(x, y);
                    else if (ChunkMap[y][x] != null) InvalidateChunk(x, y);
                    Rectangle rect = GetTilesChunkArea(x, y);

                    Bitmap bmp2 = new Bitmap(rect.Width * EditorConstants.TILE_SIZE, rect.Height * EditorConstants.TILE_SIZE, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    var squareSize = (bmp2.Width > bmp2.Height ? bmp2.Width : bmp2.Height);
                    int factor = 32;
                    int newSize = (int)Math.Round((squareSize / (double)factor), MidpointRounding.AwayFromZero) * factor;
                    if (newSize == 0) newSize = factor;
                    while (newSize < squareSize) newSize += factor;

                    Bitmap bmp = new Bitmap(newSize, newSize, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    bool hasBeenSelected = false;

                    using (bmp)
                    {
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            for (int ty = rect.Y; ty < rect.Y + rect.Height; ++ty)
                            {
                                for (int tx = rect.X; tx < rect.X + rect.Width; ++tx)
                                {
                                    Point point = new Point(tx, ty);
                                    bool tileSelected = SelectedTiles.Contains(point) || TempSelectionTiles.Contains(point) && !TempSelectionDeselectTiles.Contains(point);
                                    DrawTile(g, GetTileToDraw(point), tx - rect.X, ty - rect.Y, tileSelected);
                                    if (tileSelected) hasBeenSelected = true;

                                }
                            }
                        }
                        ChunkMap[y][x] = new ChunkVBO();
                        ChunkMap[y][x].Texture = TextureCreator.FromBitmap(d._device, bmp);
                        ChunkMap[y][x].IsReady = true;
                        ChunkMap[y][x].HasBeenRendered = true;
                        ChunkMap[y][x].HasBeenSelectedPrior = hasBeenSelected;
                    }

                    bmp.Dispose();
                    bmp2.Dispose();
                    bmp = null;
                    bmp2 = null;

                    return ChunkMap[y][x].Texture;
                }




            }

            private ushort GetTileToDraw(Point source)
            {
                if (SelectedTiles.Values.ContainsKey(source)) return SelectedTiles.Values[source];
                else return Layer.Tiles[source.Y][source.X];
            }

            public void DrawTile(Graphics g, ushort tile, int x, int y, bool isSelected = false)
            {
                if (tile != 0xffff)
                {
                    ushort TileIndex = (ushort)(tile & 0x3ff);
                    int TileIndexInt = (int)TileIndex;
                    bool flipX = ((tile >> 10) & 1) == 1;
                    bool flipY = ((tile >> 11) & 1) == 1;
                    bool SolidTopA = ((tile >> 12) & 1) == 1;
                    bool SolidLrbA = ((tile >> 13) & 1) == 1;
                    bool SolidTopB = ((tile >> 14) & 1) == 1;
                    bool SolidLrbB = ((tile >> 15) & 1) == 1;

                    System.Drawing.Color AllSolid = Editor.Instance.CollisionAllSolid;
                    System.Drawing.Color LRDSolid = Editor.Instance.CollisionLRDSolid;
                    System.Drawing.Color TopOnlySolid = Editor.Instance.CollisionTopOnlySolid;

                    g.DrawImage(EditorSolution.CurrentTiles.StageTiles.Image.GetBitmap(new Rectangle(0, TileIndex * EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE), flipX, flipY, isSelected), new Rectangle(x * EditorConstants.TILE_SIZE, y * EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE));

                    if (Editor.Instance.Options.ShowCollisionA)
                    {
                        if (SolidLrbA || SolidTopA)
                        {
                            if (SolidTopA && SolidLrbA) DrawCollision(true, AllSolid, flipX, flipY);
                            if (SolidTopA && !SolidLrbA) DrawCollision(true, TopOnlySolid, flipX, flipY);
                            if (SolidLrbA && !SolidTopA) DrawCollision(true, LRDSolid, flipX, flipY);
                        }
                    }
                    if (Editor.Instance.Options.ShowCollisionB)
                    {
                        if (SolidLrbB || SolidTopB)
                        {
                            if (SolidTopB && SolidLrbB) DrawCollision(false, AllSolid, flipX, flipY);
                            if (SolidTopB && !SolidLrbB) DrawCollision(false, TopOnlySolid, flipX, flipY);
                            if (SolidLrbB && !SolidTopB) DrawCollision(false, LRDSolid, flipX, flipY);
                        }
                    }
                }

                if (isSelected)
                {
                    g.DrawRectangle(Pens.Red, new Rectangle(x * EditorConstants.TILE_SIZE, y * EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE - 1, EditorConstants.TILE_SIZE - 1));
                }

                void DrawCollision(bool drawA, System.Drawing.Color colour, bool flipX, bool flipY)
                {
                    //create some image attributes
                    ImageAttributes attributes = new ImageAttributes();

                    //TODO : Collision Opacity
                    int opacity = (int)Editor.Instance.EditorToolbar.collisionOpacitySlider.Value;

                    float[][] colourMatrixElements =
                    {
                    new float[] { colour.R / 255.0f, 0, 0, 0, 0 },
                    new float[] { 0, colour.G / 255.0f, 0, 0, 0 },
                    new float[] { 0, 0, colour.B / 255.0f, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 0, 1 }
                };

                    var matrix = new ColorMatrix(colourMatrixElements);
                    matrix.Matrix33  = opacity;
                    //set the color matrix attribute
                    attributes.SetColorMatrix(matrix);


                    int _x = 0;
                    int _y = 0;
                    int _width = EditorConstants.TILE_SIZE;
                    int _height = EditorConstants.TILE_SIZE;

                    Rectangle dest = new Rectangle(x * EditorConstants.TILE_SIZE, y * EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE);

                    Bitmap collisionMap;

                    if (drawA) collisionMap = EditorSolution.CurrentTiles.StageTiles.CollisionMaskA.GetBitmap(new Rectangle(0, (tile & 0x3ff) * EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE), flipX, flipY);
                    else collisionMap = EditorSolution.CurrentTiles.StageTiles.CollisionMaskB.GetBitmap(new Rectangle(0, (tile & 0x3ff) * EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE, EditorConstants.TILE_SIZE), flipX, flipY);


                    g.DrawImage(collisionMap, dest, _x, _y, _width, _height, GraphicsUnit.Pixel, attributes);

                    attributes.Dispose();
                    attributes = null;

                    colourMatrixElements = null;
                }


            }

            #endregion

            #region Horizontal Layer Scroll Rendering

            Dictionary<string, EditorEntityDrawing.EditorAnimation.EditorFrame> HorizontalLayerScrollAnimationList = new Dictionary<string, EditorEntityDrawing.EditorAnimation.EditorFrame>();

            private static Bitmap cropImage(Bitmap img, Rectangle cropArea)
            {
                Bitmap bmpImage = new Bitmap(img);
                return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
            }

            public void DrawLayerForScrollRender(int startIndex, int lineCount, int HRI, int HRMI, string name, DevicePanel d = null)
            {
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(WidthPixels, HeightPixels);
                System.Drawing.Bitmap section = new System.Drawing.Bitmap(WidthPixels, lineCount);
                string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ManiacEditor Config", "Test", string.Format("{0}{1}{2}.png", name, HRI, HRMI));

                using (bitmap)
                {
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
                    {
                        Draw(g);
                    }
                    section = cropImage(bitmap, new Rectangle(0, startIndex, WidthPixels, lineCount));
                }

                List<EditorEntityDrawing.EditorAnimation.EditorFrame> LayerFrames = new List<EditorEntityDrawing.EditorAnimation.EditorFrame>();
                System.Drawing.Bitmap parallax = new System.Drawing.Bitmap(WidthPixels, lineCount);
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(parallax))
                {
                    int section1CropWidth = WidthPixels;
                    Bitmap section1 = cropImage(section, new Rectangle(0, 0, section1CropWidth, lineCount));
                    g.DrawImage(section1, 0, 0);
                }

                Texture texture = null;
                texture = TextureCreator.FromBitmap(d._device, parallax);
                var animFrame = new Animation.AnimationEntry.Frame()
                {
                    X = 0,
                    Y = (short)startIndex,
                    Width = (short)parallax.Size.Width,
                    Height = (short)lineCount
                };
                var frame = new EditorEntityDrawing.EditorAnimation.EditorFrame()
                {
                    Texture = texture,
                    Frame = animFrame,
                    Entry = new Animation.AnimationEntry(),
                    ImageWidth = parallax.Size.Width,
                    ImageHeight = parallax.Size.Height
                };

                string key = string.Format("{0}{1}.png", HRI, HRMI);
                HorizontalLayerScrollAnimationList.Add(key, frame);
            }

            public void RenderParallaxToFramesForAnimation()
            {
                foreach (var frame in HorizontalLayerScrollAnimationList.Values)
                {
                    frame.Texture?.Dispose();
                }
                HorizontalLayerScrollAnimationList.Clear();

                int HorizontalRuleIndex = 0;
                int HorizontalRuleMapIndex = 0;

                foreach (var layer in HorizontalLayerScroll)
                {
                    foreach (var lines in layer.LinesMapList)
                    {
                        DrawLayerForScrollRender(lines.StartIndex, lines.LineCount, HorizontalRuleIndex, HorizontalRuleMapIndex, "BGLayer", Editor.Instance.FormsModel.GraphicPanel);
                        HorizontalRuleMapIndex++;
                    }
                    HorizontalRuleMapIndex = 0;
                    HorizontalRuleIndex++;
                }

                HasHorizontalLayerScrollInitilized = true;
            }


            //Parallax Animating

            public int ProcessParallax(ManiacEditor.HorizontalLayerScroll layer, int maxWidth, int speed = 1, int duration = 0)
            {
                if (speed <= 0) speed = 1;
                string group = string.Format("{0},{1}", speed, maxWidth);
                if (!ManiacEditor.EditorAnimations.AnimationTiming.ContainsKey(group)) ManiacEditor.EditorAnimations.AnimationTiming.Add(group, new ManiacEditor.EditorAnimations.Timing());
                // Playback
                if (Editor.Instance.Options.ParallaxAnimationChecked && Editor.Instance.Options.AllowAnimations)
                {
                    if ((DateTime.Now - ManiacEditor.EditorAnimations.AnimationTiming[group].LastParallaxTime).TotalMilliseconds > 1024 / speed)
                    {
                        ManiacEditor.EditorAnimations.AnimationTiming[group].LastParallaxTime = DateTime.Now;
                        ManiacEditor.EditorAnimations.AnimationTiming[group].FrameIndex++;
                    }
                }
                else ManiacEditor.EditorAnimations.AnimationTiming[group].FrameIndex = 0;
                if (ManiacEditor.EditorAnimations.AnimationTiming[group].FrameIndex >= maxWidth)
                    ManiacEditor.EditorAnimations.AnimationTiming[group].FrameIndex = 0;


                return ManiacEditor.EditorAnimations.AnimationTiming[group].FrameIndex;


            }

            public void DrawLayerScroll(DevicePanel d)
            {

                if (!HasHorizontalLayerScrollInitilized) RenderParallaxToFramesForAnimation();

                if (HasHorizontalLayerScrollInitilized && AllowLayerToAnimateParallax)
                {
                    foreach (var layer in HorizontalLayerScroll)
                    {
                        foreach (var lines in layer.LinesMapList)
                        {
                            int speed = (layer.RelativeSpeed == 0 ? 1 : layer.RelativeSpeed);
                            string groupKey = string.Format("{0},{1}", speed, WidthPixels);
                            int index = HorizontalLayerScroll.IndexOf(layer);
                            string key = string.Format("{0}{1}.png", index, HorizontalLayerScroll[index].LinesMapList.IndexOf(lines));
                            int scrollPoint = ManiacEditor.EditorAnimations.AnimationTiming[groupKey].FrameIndex;
                            int section1CropWidth = WidthPixels - scrollPoint;
                            int section2CropWidth = scrollPoint;

                            var frame = HorizontalLayerScrollAnimationList[key];

                            if (frame != null)
                            {
                                d.DrawBitmap(frame.Texture, frame.Frame.X + WidthPixels - scrollPoint, frame.Frame.Y, scrollPoint, frame.Frame.Height, false, 0xFF);
                                d.DrawBitmap(frame.Texture, frame.Frame.X - scrollPoint, frame.Frame.Y, frame.Frame.Width, frame.Frame.Height, false, 0xFF);
                            }
                        }
                    }

                }
            }

            public void DrawScrollLines(DevicePanel d)
            {
                if (ShowLayerScrollLines)
                {
                    int Transperncy = 0xFF;

                    if (EditorInstance.EditLayerA != null && (EditorInstance.EditLayerA != this && EditorInstance.EditLayerB != this))
                        Transperncy = 0x32;
                    else if (EditorInstance.EditorToolbar.EditEntities.IsCheckedAll && EditorInstance.EditLayerA == null && EditorInstance.Options.ApplyEditEntitiesTransparency)
                        Transperncy = 0x32;
                    else
                        Transperncy = 0xFF;

                    foreach (var layer in HorizontalLayerScroll)
                    {
                        foreach (var lines in layer.LinesMapList)
                        {

                            d.DrawLine(0, lines.StartIndex, _layer.Width * EditorConstants.TILE_SIZE, lines.StartIndex, System.Drawing.Color.FromArgb(Transperncy, System.Drawing.Color.Red));
                            d.DrawLine(0, lines.StartIndex + lines.LineCount, _layer.Width * EditorConstants.TILE_SIZE, lines.StartIndex + lines.LineCount, System.Drawing.Color.FromArgb(Transperncy, System.Drawing.Color.Red));
                        }
                    }
                }

            }

            public void UpdateLayerScrollIndex()
            {
                foreach (var layer in HorizontalLayerScroll)
                {
                    foreach (var lines in layer.LinesMapList)
                    {
                        ProcessParallax(layer, WidthPixels, layer.RelativeSpeed);
                    }
                }
            }

            #endregion

            /// <summary>
            /// Resizes both this EditorLayer, and the underlying SceneLayer
            /// </summary>
            /// <param name="width">The new width of the layer</param>
            /// <param name="height">The new height of the layer</param>
            public void Resize(ushort width, ushort height)
            {
                ushort oldWidth = Width;
                ushort oldHeight = Height;

                // first resize the underlying SceneLayer
                _layer.Resize(width, height);

                int oldWidthChunkSize = DivideRoundUp(oldWidth, EditorConstants.TILES_CHUNK_SIZE);
                int newWidthChunkSize = DivideRoundUp(Width, EditorConstants.TILES_CHUNK_SIZE);


                SelectedTiles = new PointsMap(Width, Height);
                TempSelectionTiles = new PointsMap(Width, Height);
            }



            public void RefreshTileCount()
            {
                GlobalSelectedTiles = SelectedTiles.Count + TempSelectionTiles.Count;
                EditorStateModel.DeselectTilesCount = TempSelectionDeselectTiles.Count;
                EditorStateModel.SelectedTilesCount = GlobalSelectedTiles - EditorStateModel.DeselectTilesCount;
            }


            public class ChunkVBO
            {
                public bool IsReady = false;
                public SharpDX.Direct3D9.Texture Texture;
                public bool HasBeenRendered = false;
                public bool HasBeenSelectedPrior = false;

                public void Dispose()
                {
                    if (this.Texture != null)
                    {
                        this.Texture.Dispose();
                        this.Texture = null;
                    }
                    this.IsReady = false;
                    this.HasBeenSelectedPrior = false;
                }
            }
        }
    }
}
