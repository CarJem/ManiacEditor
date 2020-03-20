using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RSDKv5;
using ManiacEditor.Actions;
using System.Drawing;
using ManiacEditor.Classes.Rendering;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Scene = RSDKv5.Scene;
using ManiacEditor.Enums;
using ManiacEditor.Extensions;
using SFML.System;
using SFML.Graphics;

namespace ManiacEditor.Classes.Scene
{
    public class EditorLayer : IDrawable
    {
        #region Definitions

        #region General

        private ManiacEditor.Controls.Editor.MainEditor Instance { get; set; }
        public int GlobalSelectedTiles { get; set; }
        public List<IAction> Actions { get; set; } = new List<IAction>();
        private bool CanDrawForSFML { get; set; } = true;

        #endregion

        #region Chunk Map

        private ChunkVBO[][] ChunkMap;
        public int ChunksWidth { get; set; }
        public int ChunksHeight { get; set; }

        #endregion

        #region Original Layer
        private SceneLayer _layer { get; set; }
        internal SceneLayer Layer { get => _layer; }
        #endregion

        #region EditLayer Controls
        private bool TempSelectionDeselect { get; set; }
        private bool FirstDrag { get; set; }
        private bool isDragOver { get; set; }
        public bool ShowLayerScrollLines { get; set; } = false;
        public bool HasHorizontalLayerScrollInitilized { get; set; } = false;
        public bool AllowLayerToAnimateParallax { get; set; } = false;
        #endregion

        #region Point Maps

        public PointsMap SelectedTiles;
        public PointsMap TempSelectionTiles;
        public PointsMap TempSelectionDeselectTiles;

        #endregion

        #region Layer Rules
        public IList<HorizontalLayerScroll> HorizontalLayerRules { get => _horizontalLayerRules; }
        private IList<HorizontalLayerScroll> _horizontalLayerRules;

        #endregion

        #region Layer Properties

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
        public int HeightPixels { get => _layer.Height * Methods.Editor.EditorConstants.TILE_SIZE; }
        public int WidthPixels { get => _layer.Width * Methods.Editor.EditorConstants.TILE_SIZE; }
        public ushort[][] Tiles { get => Layer.Tiles; }

        #endregion

        #region Classes

        public class ChunkVBO
        {
            public bool IsReady = false;
            public SFML.Graphics.Texture Texture;
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

        /// <summary>
        /// Defines the horizontal scrolling behaviour of a set of potentially non-contiguous lines.
        /// </summary>
        public class HorizontalLayerScroll
        {
            private byte _id;
            private ScrollInfo _scrollInfo;
            private IList<ScrollInfoLines> _linesMapList;

            /// <summary>
            /// Creates a new scrolling behaviour rule, but not yet applied to any lines.
            /// </summary>
            /// <param name="id">Internal identifer to use for display purposes</param>
            /// <param name="info">The rules governing the scrolling behaviour</param>
            public HorizontalLayerScroll(byte id, ScrollInfo info)
                : this(id, info, new List<ScrollInfoLines>())
            {
            }

            /// <summary>
            /// Creates a new scrolling behaviour rule, applied to the given map of lines.
            /// </summary>
            /// <param name="id">Internal identifer to use for display purposes</param>
            /// <param name="info">The rules governing the scrolling behaviour</param>
            /// <param name="linesMap">Set of line level mappings which define the lines the rules apply to</param>
            public HorizontalLayerScroll(byte id, ScrollInfo info, IList<ScrollInfoLines> linesMap)
            {
                _id = id;
                _scrollInfo = info;
                _linesMapList = linesMap;
            }

            /// <summary>
            /// Internal identifier.
            /// </summary>
            /// <remarks>This is NOT persisted to any RSDKv5 backing field!</remarks>
            public byte Id { get => _id; set => _id = value; }
            public byte Behavior { get => _scrollInfo.Behaviour; set => _scrollInfo.Behaviour = value; }
            public byte DrawOrder { get => _scrollInfo.DrawOrder; set => _scrollInfo.DrawOrder = value; }
            public short RelativeSpeed { get => _scrollInfo.RelativeSpeed; set => _scrollInfo.RelativeSpeed = value; }
            public short ConstantSpeed { get => _scrollInfo.ConstantSpeed; set => _scrollInfo.ConstantSpeed = value; }

            public IList<ScrollInfoLines> LinesMapList { get => _linesMapList; }
            public ScrollInfo ScrollInfo { get => _scrollInfo; }

            /// <summary>
            /// Applies the set of rules to the given set of lines.
            /// This may be called multiple times to set-up multiple mappings, 
            /// which need not be contiguous.
            /// </summary>
            /// <param name="startLine">The line at which these rules begin to apply (base 0)</param>
            /// <param name="lineCount">The number of contiguous lines to which the rules apply</param>
            public void AddMapping(int startLine, int lineCount)
            {
                _linesMapList.Add(new ScrollInfoLines(startLine, lineCount));
            }

            /// <summary>
            /// Creates an empty line level mapping which can be further manipulated
            /// </summary>
            public void AddMapping()
            {
                AddMapping(0, 0);
            }




        }

        /// <summary>
        /// Defines the lines to which a ScrollInfo set of horizontal scrolling rules, applies.
        /// </summary>
        public class ScrollInfoLines
        {
            private int _startIndex;
            private int _lineCount;

            /// <summary>
            /// Creates a new mapping with the given values.
            /// </summary>
            /// <param name="startIndex">The line at which any rules start to apply.</param>
            /// <param name="lineCount">The number of lines to which this applies.</param>
            public ScrollInfoLines(int startIndex, int lineCount)
            {
                _startIndex = startIndex;
                _lineCount = lineCount;
            }

            /// <summary>
            /// The line at which these rules begin to apply
            /// </summary>
            public int StartIndex
            {
                get => _startIndex;
                set => _startIndex = value;
            }

            /// <summary>
            /// The number of lines to which this set of rules applies.
            /// </summary>
            public int LineCount
            {
                get => _lineCount;
                set => _lineCount = value;
            }

            public override string ToString()
            {
                return $"Start: {_startIndex}, for {_lineCount} lines";
            }
        }

        public class PointsMap
        {
            public Dictionary<Point, ushort> Values = new Dictionary<Point, ushort>();
            HashSet<Point>[][] PointsChunks;
            HashSet<Point> OutOfBoundsPoints = new HashSet<Point>();
            public int Count = 0;

            public PointsMap(int width, int height)
            {
                PointsChunks = new HashSet<Point>[DivideRoundUp(height, Methods.Editor.EditorConstants.TILES_CHUNK_SIZE)][];
                for (int i = 0; i < PointsChunks.Length; ++i)
                {
                    PointsChunks[i] = new HashSet<Point>[DivideRoundUp(width, Methods.Editor.EditorConstants.TILES_CHUNK_SIZE)];
                    for (int j = 0; j < PointsChunks[i].Length; ++j)
                        PointsChunks[i][j] = new HashSet<Point>();
                }
            }

            public void Add(Point point)
            {

                HashSet<Point> h;
                if (point.Y < 0 || point.X < 0 || point.Y / Methods.Editor.EditorConstants.TILES_CHUNK_SIZE >= PointsChunks.Length || point.X / Methods.Editor.EditorConstants.TILES_CHUNK_SIZE >= PointsChunks[0].Length)
                    h = OutOfBoundsPoints;
                else
                    h = PointsChunks[point.Y / Methods.Editor.EditorConstants.TILES_CHUNK_SIZE][point.X / Methods.Editor.EditorConstants.TILES_CHUNK_SIZE];
                Count -= h.Count;
                h.Add(point);
                Count += h.Count;
            }

            public void Remove(Point point)
            {
                HashSet<Point> h;
                if (point.Y < 0 || point.X < 0 || point.Y / Methods.Editor.EditorConstants.TILES_CHUNK_SIZE >= PointsChunks.Length || point.X / Methods.Editor.EditorConstants.TILES_CHUNK_SIZE >= PointsChunks[0].Length)
                    h = OutOfBoundsPoints;
                else
                    h = PointsChunks[point.Y / Methods.Editor.EditorConstants.TILES_CHUNK_SIZE][point.X / Methods.Editor.EditorConstants.TILES_CHUNK_SIZE];
                Count -= h.Count;
                h.Remove(point);
                Count += h.Count;
            }

            public bool Contains(Point point)
            {
                if (point.Y < 0 || point.X < 0 || point.Y / Methods.Editor.EditorConstants.TILES_CHUNK_SIZE >= PointsChunks.Length || point.X / Methods.Editor.EditorConstants.TILES_CHUNK_SIZE >= PointsChunks[0].Length)
                    return OutOfBoundsPoints.Contains(point);
                else
                    return PointsChunks[point.Y / Methods.Editor.EditorConstants.TILES_CHUNK_SIZE][point.X / Methods.Editor.EditorConstants.TILES_CHUNK_SIZE].Contains(point);
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

        #endregion

        #region Rendering Values

        private Classes.Rendering.LayerTileProvider RenderingProvider { get; set; }
        private bool isMapRenderInitalized { get; set; } = false;
        public int RenderingTransparency { get; set; }
        public static bool RequireRefresh { get; set; } = false;

        #endregion



        #endregion

        #region Init

        public EditorLayer(SceneLayer layer, ManiacEditor.Controls.Editor.MainEditor instance)
        {
            Instance = instance;
            _layer = layer;
            RenderingProvider = new LayerTileProvider(this);

            SelectedTiles = new PointsMap(Width, Height);
            TempSelectionTiles = new PointsMap(Width, Height);
            TempSelectionDeselectTiles = new PointsMap(Width, Height);

            _horizontalLayerRules = ReadHorizontalLineRules();
            InitiallizeChunkMap();
        }




        private void InitiallizeChunkMap()
        {
            ChunksWidth = DivideRoundUp(Width, Methods.Editor.EditorConstants.TILES_CHUNK_SIZE);
            ChunksWidth += ModulusRoundUp(Width, Methods.Editor.EditorConstants.TILES_CHUNK_SIZE);
            ChunksHeight = DivideRoundUp(Height, Methods.Editor.EditorConstants.TILES_CHUNK_SIZE);
            ChunksHeight += ModulusRoundUp(Height, Methods.Editor.EditorConstants.TILES_CHUNK_SIZE);

            ChunkMap = new ChunkVBO[ChunksHeight][];
            for (int i = 0; i < ChunkMap.Length; ++i)
            {
                ChunkMap[i] = new ChunkVBO[ChunksWidth];
            }

        }
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
        public void ProduceHorizontalLayerScroll()
        {
            var id = (byte)(_horizontalLayerRules.Select(hlr => hlr.Id).Max() + 1);
            var info = new ScrollInfo();

            _layer.ScrollingInfo.Add(info);
            var hls = new HorizontalLayerScroll(id, info);
            _horizontalLayerRules.Add(hls);
        }

        #endregion

        #region Round Up Methods

        static int DivideRoundUp(int number, int by)
        {
            return (number + by - 1) / by;
        }
        static int ModulusRoundUp(int number, int by)
        {
            return (number + by - 1) % by;
        }

        #endregion

        #region Get Chunk Details

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
        private ushort GetTile(Point point)
        {
            return _layer.Tiles[point.Y][point.X];

        }
        public bool IsPointSelected(Point point)
        {
            return SelectedTiles.Contains(new Point(point.X / Methods.Editor.EditorConstants.TILE_SIZE, point.Y / Methods.Editor.EditorConstants.TILE_SIZE));
        }
        public bool DoesChunkContainASelectedTile(Point point)
        {
            Point startingPoint = new Point(point.X / Methods.Editor.EditorConstants.TILE_SIZE, point.Y / Methods.Editor.EditorConstants.TILE_SIZE);
            List<Point> chunkPoints = new List<Point>();
            for (int x = 0; x < (Methods.Editor.EditorConstants.x128_CHUNK_SIZE / Methods.Editor.EditorConstants.TILE_SIZE); x++)
            {
                for (int y = 0; y < (Methods.Editor.EditorConstants.x128_CHUNK_SIZE / Methods.Editor.EditorConstants.TILE_SIZE); y++)
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
            point = new Point(point.X / Methods.Editor.EditorConstants.TILE_SIZE, point.Y / Methods.Editor.EditorConstants.TILE_SIZE);
            if (point.X >= 0 && point.Y >= 0 && point.X < this._layer.Tiles[0].Length && point.Y < this._layer.Tiles.Length)
            {
                return (_layer.Tiles[point.Y][point.X] != 0xffff || Methods.Editor.SolutionState.CopyAir);
            }
            return false;
        }
        public bool OnlyHasTileIn(Rectangle area, ushort tile)
        {
            area = new Rectangle(area.X / Methods.Editor.EditorConstants.TILE_SIZE, area.Y / Methods.Editor.EditorConstants.TILE_SIZE, area.Width / Methods.Editor.EditorConstants.TILE_SIZE, area.Height / Methods.Editor.EditorConstants.TILE_SIZE);
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
            point = new Point(point.X / Methods.Editor.EditorConstants.TILE_SIZE, point.Y / Methods.Editor.EditorConstants.TILE_SIZE);
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
            int y_start = y * Methods.Editor.EditorConstants.TILES_CHUNK_SIZE;
            int y_end = Math.Min((y + 1) * Methods.Editor.EditorConstants.TILES_CHUNK_SIZE, _layer.Height);

            int x_start = x * Methods.Editor.EditorConstants.TILES_CHUNK_SIZE;
            int x_end = Math.Min((x + 1) * Methods.Editor.EditorConstants.TILES_CHUNK_SIZE, _layer.Width);


            return new Rectangle(x_start, y_start, x_end - x_start, y_end - y_start);
        }

        #endregion

        #region Adjust Selection/Drag
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
            Methods.Editor.SolutionState.SelectedTilesCount = 0;
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
                    else if (_layer.Tiles[y][x] == 0xffff && Methods.Editor.SolutionState.CopyAir)
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
            for (int y = Math.Max(area.Y / Methods.Editor.EditorConstants.TILE_SIZE, 0); y < Math.Min(DivideRoundUp(area.Y + area.Height, Methods.Editor.EditorConstants.TILE_SIZE), _layer.Height); ++y)
            {
                for (int x = Math.Max(area.X / Methods.Editor.EditorConstants.TILE_SIZE, 0); x < Math.Min(DivideRoundUp(area.X + area.Width, Methods.Editor.EditorConstants.TILE_SIZE), _layer.Width); ++x)
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
                    else if (_layer.Tiles[y][x] == 0xffff && Methods.Editor.SolutionState.CopyAir)
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
            point = new Point(point.X / Methods.Editor.EditorConstants.TILE_SIZE, point.Y / Methods.Editor.EditorConstants.TILE_SIZE);
            Methods.Editor.SolutionState.SelectedTileX = point.X;
            Methods.Editor.SolutionState.SelectedTileY = point.Y;
            if (point.X >= 0 && point.Y >= 0 && point.X < this._layer.Tiles[0].Length && point.Y < this._layer.Tiles.Length)
            {
                if (deselectIfSelected && SelectedTiles.Contains(point))
                {
                    // Deselect
                    DeselectPoint(point);
                    RefreshTileCount();
                }
                else if (this._layer.Tiles[point.Y][point.X] != 0xffff || Methods.Editor.SolutionState.CopyAir)
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
            for (int y = Math.Max(area.Y / Methods.Editor.EditorConstants.TILE_SIZE, 0); y < Math.Min(DivideRoundUp(area.Y + area.Height, Methods.Editor.EditorConstants.TILE_SIZE), _layer.Height); ++y)
            {
                for (int x = Math.Max(area.X / Methods.Editor.EditorConstants.TILE_SIZE, 0); x < Math.Min(DivideRoundUp(area.X + area.Width, Methods.Editor.EditorConstants.TILE_SIZE), _layer.Width); ++x)
                {
                    if (SelectedTiles.Contains(new Point(x, y)) || (_layer.Tiles[y][x] != 0xffff || Methods.Editor.SolutionState.CopyAir))
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
            point = new Point(point.X / Methods.Editor.EditorConstants.TILE_SIZE, point.Y / Methods.Editor.EditorConstants.TILE_SIZE);
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

        #endregion

        #region Common Manipulation
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
        public void MoveSelectedQuonta(Point change)
        {
            MoveSelected(Point.Empty, new Point(change.X * Methods.Editor.EditorConstants.TILE_SIZE, change.Y * Methods.Editor.EditorConstants.TILE_SIZE), false);
        }
        public void MoveSelected(Point oldPos, Point newPos, bool duplicate, bool chunkAlign = false)
        {
            oldPos = new Point(oldPos.X / Methods.Editor.EditorConstants.TILE_SIZE, oldPos.Y / Methods.Editor.EditorConstants.TILE_SIZE);
            newPos = new Point(newPos.X / Methods.Editor.EditorConstants.TILE_SIZE, newPos.Y / Methods.Editor.EditorConstants.TILE_SIZE);
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
        private void SetTile(Point point, ushort value, bool addAction = true)
        {
            if (addAction)
                Actions.Add(new ActionChangeTile((x, y) => SetTile(x, y, false), point, _layer.Tiles[point.Y][point.X], value));
            _layer.Tiles[point.Y][point.X] = value;
            InvalidateChunk(point.X / Methods.Editor.EditorConstants.TILES_CHUNK_SIZE, point.Y / Methods.Editor.EditorConstants.TILES_CHUNK_SIZE);
        }
        private void RemoveTile(Point point)
        {
            SetTile(point, 0xffff);
            RefreshTileCount();
        }
        public void Resize(ushort width, ushort height)
        {
            ushort oldWidth = Width;
            ushort oldHeight = Height;

            // first resize the underlying SceneLayer
            _layer.Resize(width, height);

            int oldWidthChunkSize = DivideRoundUp(oldWidth, Methods.Editor.EditorConstants.TILES_CHUNK_SIZE);
            int newWidthChunkSize = DivideRoundUp(Width, Methods.Editor.EditorConstants.TILES_CHUNK_SIZE);


            SelectedTiles = new PointsMap(Width, Height);
            TempSelectionTiles = new PointsMap(Width, Height);
        }
        public void DrawAsBrush(Point newPos, Dictionary<Point, ushort> points)
        {
            try
            {
                bool updateActions = false;
                newPos = new Point(newPos.X / Methods.Editor.EditorConstants.TILE_SIZE, newPos.Y / Methods.Editor.EditorConstants.TILE_SIZE);
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

        #endregion

        #region Flipping/Properties
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
        #endregion

        #region Clipboards
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
        public void PasteFromClipboard(Point newPos, Dictionary<Point, ushort> points, bool updateActions = true)
        {
            try
            {
                newPos = new Point(newPos.X / Methods.Editor.EditorConstants.TILE_SIZE, newPos.Y / Methods.Editor.EditorConstants.TILE_SIZE);
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
        #endregion

        #region Rendering (System.Drawing.Graphics)

        public void Draw(Graphics g)
        {
            for (int y = 0; y < _layer.Height; ++y)
            {
                for (int x = 0; x < _layer.Width; ++x)
                {
                    if (this._layer.Tiles[y][x] != 0xffff)
                    {
                        DrawTileGraphics(g, _layer.Tiles[y][x], x, y);
                    }
                }
            }
        }
        public void DrawTileGraphics(Graphics g, ushort tile, int x, int y)
        {
            ushort TileIndex = (ushort)(tile & 0x3ff);
            int TileIndexInt = (int)TileIndex;
            bool flipX = ((tile >> 10) & 1) == 1;
            bool flipY = ((tile >> 11) & 1) == 1;
            bool SolidTopA = ((tile >> 12) & 1) == 1;
            bool SolidLrbA = ((tile >> 13) & 1) == 1;
            bool SolidTopB = ((tile >> 14) & 1) == 1;
            bool SolidLrbB = ((tile >> 15) & 1) == 1;

            System.Drawing.Color AllSolid = System.Drawing.Color.FromArgb((int)Instance.EditorToolbar.collisionOpacitySlider.Value, Instance.CollisionAllSolid.R, Instance.CollisionAllSolid.G, Instance.CollisionAllSolid.B);
            System.Drawing.Color LRDSolid = System.Drawing.Color.FromArgb((int)Instance.EditorToolbar.collisionOpacitySlider.Value, Instance.CollisionLRDSolid.R, Instance.CollisionLRDSolid.G, Instance.CollisionLRDSolid.B);
            System.Drawing.Color TopOnlySolid = System.Drawing.Color.FromArgb((int)Instance.EditorToolbar.collisionOpacitySlider.Value, Instance.CollisionTopOnlySolid.R, Instance.CollisionTopOnlySolid.G, Instance.CollisionTopOnlySolid.B);

            g.DrawImage(Methods.Editor.Solution.CurrentTiles.Image.GetBitmap(new Rectangle(0, TileIndex * Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE), flipX, flipY),
                new Rectangle(x * Methods.Editor.EditorConstants.TILE_SIZE, y * Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE));

            if (Methods.Editor.SolutionState.ShowCollisionA)
            {
                if (SolidLrbA || SolidTopA)
                {
                    if (SolidTopA && SolidLrbA) DrawCollision(true, AllSolid);
                    if (SolidTopA && !SolidLrbA) DrawCollision(true, TopOnlySolid);
                    if (SolidLrbA && !SolidTopA) DrawCollision(true, LRDSolid);
                }
            }
            if (Methods.Editor.SolutionState.ShowCollisionB)
            {
                if (SolidLrbB || SolidTopB)
                {
                    if (SolidTopB && SolidLrbB) DrawCollision(false, AllSolid);
                    if (SolidTopB && !SolidLrbB) DrawCollision(false, TopOnlySolid);
                    if (SolidLrbB && !SolidTopB) DrawCollision(false, LRDSolid);
                }
            }

            if (Methods.Editor.SolutionState.ShowFlippedTileHelper == true)
            {
                g.DrawImage(Methods.Editor.Solution.CurrentTiles.EditorImage.GetBitmap(new Rectangle(0, 3 * Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE), false, false),
                            new Rectangle(x * Methods.Editor.EditorConstants.TILE_SIZE, y * Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE));
            }
            if (Methods.Editor.SolutionState.ShowTileID == true)
            {
                g.DrawImage(Methods.Editor.Solution.CurrentTiles.IDImage.GetBitmap(new Rectangle(0, TileIndex * Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE), false, false),
                            new Rectangle(x * Methods.Editor.EditorConstants.TILE_SIZE, y * Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE));
            }

            void DrawCollision(bool drawA, System.Drawing.Color colur)
            {
                Bitmap Map;
                if (drawA) Map = Methods.Editor.Solution.CurrentTiles.CollisionMaskA.GetBitmap(new Rectangle(0, (tile & 0x3ff) * Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE), flipX, flipY);
                else Map = Methods.Editor.Solution.CurrentTiles.CollisionMaskB.GetBitmap(new Rectangle(0, (tile & 0x3ff) * Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE), flipX, flipY);

                Map = Extensions.Extensions.ChangeImageColor(Map, System.Drawing.Color.White, colur);

                g.DrawImage(Map, x * Methods.Editor.EditorConstants.TILE_SIZE, y * Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE);
            }


        }

        #endregion

        #region Rendering (SFML.Graphics)
        public void Draw(DevicePanel d)
        {
            if (!isMapRenderInitalized || RequireRefresh)
            {
                InitalizeRender();
                return;
            }

            if (Methods.Editor.Solution.EditLayerA != null && (Methods.Editor.Solution.EditLayerA != this && Methods.Editor.Solution.EditLayerB != this))
                RenderingTransparency = 0x32;
            else if (Instance.EditorToolbar.EditEntities.IsCheckedAll && Methods.Editor.Solution.EditLayerA == null && Methods.Editor.SolutionState.ApplyEditEntitiesTransparency)
                RenderingTransparency = 0x32;
            else
                RenderingTransparency = 0xFF;

            RenderSection(RenderingProvider.MapRender);
            if (Methods.Editor.SolutionState.ShowTileID) RenderSection(RenderingProvider.MapRenderTileID);
            if (Methods.Editor.SolutionState.ShowFlippedTileHelper) RenderSection(RenderingProvider.MapRenderEditor);
            if (Methods.Editor.SolutionState.ShowCollisionA) RenderSection(RenderingProvider.MapRenderCollisionMapA);
            if (Methods.Editor.SolutionState.ShowCollisionB) RenderSection(RenderingProvider.MapRenderCollisionMapB);


            void RenderSection(LayerRenderer vbo)
            {
                if (vbo == null) return;
                vbo.Refresh();
                vbo.Zoom = Methods.Editor.SolutionState.Zoom;
                d.RenderWindow.Draw(vbo);
            }

        }

        private void InitalizeRender()
        {
            RenderingProvider.MapRender = new LayerRenderer(Methods.Editor.Solution.CurrentTiles.Image.GetTexture(), RenderingProvider.TileProvider, 16, 1);
            RenderingProvider.MapRenderTileID = new LayerRenderer(Methods.Editor.Solution.CurrentTiles.IDImage.GetTexture(), RenderingProvider.TileIDProvider, 16, 1);
            RenderingProvider.MapRenderEditor = new LayerRenderer(Methods.Editor.Solution.CurrentTiles.EditorImage.GetTexture(), RenderingProvider.FlippedTileProvider, 16, 1);
            if (Methods.Editor.Solution.CurrentTiles.CollisionMaskA != null) RenderingProvider.MapRenderCollisionMapA = new LayerRenderer(Methods.Editor.Solution.CurrentTiles.CollisionMaskA.GetTexture(), RenderingProvider.TileCollisionProviderA, 16, 1);
            if (Methods.Editor.Solution.CurrentTiles.CollisionMaskB != null) RenderingProvider.MapRenderCollisionMapB = new LayerRenderer(Methods.Editor.Solution.CurrentTiles.CollisionMaskB.GetTexture(), RenderingProvider.TileCollisionProviderB, 16, 1);

            RequireRefresh = false;
            isMapRenderInitalized = true;
        }

        #endregion

        #region Disposing/Invalidating (SFML.Graphics)

        public void Dispose()
        {
            if (ChunkMap != null)
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
            ChunkMap = null;
        }
        public void InvalidateChunks()
        {
            if (ChunkMap != null)
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
        }
        public void DisposeTextures()
        {
            InvalidateChunks();
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

        #endregion

        #region Horizontal Layer Scroll Rendering

        public class EditorFrame
        {
            public SFML.Graphics.Texture Texture;
            public Animation.AnimationEntry.Frame Frame;
            public Animation.AnimationEntry Entry;
            public Bitmap _Bitmap;
            public int ImageWidth;
            public int ImageHeight;
        }

        Dictionary<string, EditorFrame> HorizontalLayerScrollAnimationList = new Dictionary<string, EditorFrame>();

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

            List<EditorFrame> LayerFrames = new List<EditorFrame>();
            System.Drawing.Bitmap parallax = new System.Drawing.Bitmap(WidthPixels, lineCount);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(parallax))
            {
                int section1CropWidth = WidthPixels;
                Bitmap section1 = cropImage(section, new Rectangle(0, 0, section1CropWidth, lineCount));
                g.DrawImage(section1, 0, 0);
            }

            SFML.Graphics.Texture texture = null;
            texture = Methods.Draw.TextureHelper.FromBitmap(parallax);
            var animFrame = new Animation.AnimationEntry.Frame()
            {
                X = 0,
                Y = (short)startIndex,
                Width = (short)parallax.Size.Width,
                Height = (short)lineCount
            };
            var frame = new EditorFrame()
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

            foreach (var layer in HorizontalLayerRules)
            {
                foreach (var lines in layer.LinesMapList)
                {
                    DrawLayerForScrollRender(lines.StartIndex, lines.LineCount, HorizontalRuleIndex, HorizontalRuleMapIndex, "BGLayer", ManiacEditor.Controls.Editor.MainEditor.Instance.ViewPanel.SharpPanel.GraphicPanel);
                    HorizontalRuleMapIndex++;
                }
                HorizontalRuleMapIndex = 0;
                HorizontalRuleIndex++;
            }

            HasHorizontalLayerScrollInitilized = true;
        }


        //Parallax Animating

        public int ProcessParallax(HorizontalLayerScroll layer, int maxWidth, int speed = 1, int duration = 0)
        {
            if (speed <= 0) speed = 1;
            string group = string.Format("{0},{1}", speed, maxWidth);
            if (!ManiacEditor.Methods.Entities.EntityAnimator.AnimationTiming.ContainsKey(group)) ManiacEditor.Methods.Entities.EntityAnimator.AnimationTiming.Add(group, new ManiacEditor.Methods.Entities.EntityAnimator.Timing());
            // Playback
            if (Methods.Editor.SolutionState.ParallaxAnimationChecked && Methods.Editor.SolutionState.AllowAnimations)
            {
                if ((DateTime.Now - ManiacEditor.Methods.Entities.EntityAnimator.AnimationTiming[group].LastParallaxTime).TotalMilliseconds > 1024 / speed)
                {
                    ManiacEditor.Methods.Entities.EntityAnimator.AnimationTiming[group].LastParallaxTime = DateTime.Now;
                    ManiacEditor.Methods.Entities.EntityAnimator.AnimationTiming[group].FrameIndex++;
                }
            }
            else ManiacEditor.Methods.Entities.EntityAnimator.AnimationTiming[group].FrameIndex = 0;
            if (ManiacEditor.Methods.Entities.EntityAnimator.AnimationTiming[group].FrameIndex >= maxWidth)
                ManiacEditor.Methods.Entities.EntityAnimator.AnimationTiming[group].FrameIndex = 0;


            return ManiacEditor.Methods.Entities.EntityAnimator.AnimationTiming[group].FrameIndex;


        }

        public void DrawLayerScroll(DevicePanel d)
        {

            if (!HasHorizontalLayerScrollInitilized) RenderParallaxToFramesForAnimation();

            if (HasHorizontalLayerScrollInitilized && AllowLayerToAnimateParallax)
            {
                foreach (var layer in HorizontalLayerRules)
                {
                    foreach (var lines in layer.LinesMapList)
                    {
                        int speed = (layer.RelativeSpeed == 0 ? 1 : layer.RelativeSpeed);
                        string groupKey = string.Format("{0},{1}", speed, WidthPixels);
                        int index = HorizontalLayerRules.IndexOf(layer);
                        string key = string.Format("{0}{1}.png", index, HorizontalLayerRules[index].LinesMapList.IndexOf(lines));
                        int scrollPoint = ManiacEditor.Methods.Entities.EntityAnimator.AnimationTiming[groupKey].FrameIndex;
                        int section1CropWidth = WidthPixels - scrollPoint;
                        int section2CropWidth = scrollPoint;

                        var frame = HorizontalLayerScrollAnimationList[key];

                        if (frame != null)
                        {
                            d.DrawTexture(frame.Texture, frame.Frame.X + WidthPixels - scrollPoint, frame.Frame.Y, scrollPoint, frame.Frame.Height, false, 0xFF);
                            d.DrawTexture(frame.Texture, frame.Frame.X - scrollPoint, frame.Frame.Y, frame.Frame.Width, frame.Frame.Height, false, 0xFF);
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

                if (Methods.Editor.Solution.EditLayerA != null && (Methods.Editor.Solution.EditLayerA != this && Methods.Editor.Solution.EditLayerB != this))
                    Transperncy = 0x32;
                else if (Instance.EditorToolbar.EditEntities.IsCheckedAll && Methods.Editor.Solution.EditLayerA == null && Methods.Editor.SolutionState.ApplyEditEntitiesTransparency)
                    Transperncy = 0x32;
                else
                    Transperncy = 0xFF;

                foreach (var layer in HorizontalLayerRules)
                {
                    foreach (var lines in layer.LinesMapList)
                    {

                        d.DrawLine(0, lines.StartIndex, _layer.Width * Methods.Editor.EditorConstants.TILE_SIZE, lines.StartIndex, System.Drawing.Color.FromArgb(Transperncy, System.Drawing.Color.Red));
                        d.DrawLine(0, lines.StartIndex + lines.LineCount, _layer.Width * Methods.Editor.EditorConstants.TILE_SIZE, lines.StartIndex + lines.LineCount, System.Drawing.Color.FromArgb(Transperncy, System.Drawing.Color.Red));
                    }
                }
            }

        }

        public void UpdateLayerScrollIndex()
        {
            foreach (var layer in HorizontalLayerRules)
            {
                foreach (var lines in layer.LinesMapList)
                {
                    ProcessParallax(layer, WidthPixels, layer.RelativeSpeed);
                }
            }
        }

        #endregion

        #region Other
        public void RefreshTileCount()
        {
            GlobalSelectedTiles = SelectedTiles.Count + TempSelectionTiles.Count;
            Methods.Editor.SolutionState.DeselectTilesCount = TempSelectionDeselectTiles.Count;
            Methods.Editor.SolutionState.SelectedTilesCount = GlobalSelectedTiles - Methods.Editor.SolutionState.DeselectTilesCount;
        }

        #endregion

    }
}
