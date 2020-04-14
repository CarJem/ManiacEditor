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
        public IList<HorizontalLayerScroll> HorizontalLayerRules { get => _horizontalLayerRules; set => _horizontalLayerRules = value; }
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

        private ushort TempWidth { get; set; }
        private ushort TempHeight { get; set; }

        public ushort WorkingHeight
        {
            get
            {
                return TempHeight;
            }
            set
            {
                TempHeight = value;
            }
        }
        public ushort WorkingWidth
        {
            get
            {
                return TempWidth;
            }
            set
            {
                TempWidth = value;
            }
        }
        public int HeightPixels { get => _layer.Height * Methods.Editor.EditorConstants.TILE_SIZE; }
        public int WidthPixels { get => _layer.Width * Methods.Editor.EditorConstants.TILE_SIZE; }
        public ushort[][] Tiles { get => Layer.Tiles; }

        #endregion

        #region Classes

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

            public IList<ScrollInfoLines> LinesMapList { get => _linesMapList; set => _linesMapList = value; }
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
        public bool RequireRefresh { get; set; } = false;

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

            WorkingHeight = _layer.Height;
            WorkingWidth = _layer.Width;
        }




        private void InitiallizeChunkMap()
        {
            ChunksWidth = DivideRoundUp(Width, Methods.Editor.EditorConstants.TILES_CHUNK_SIZE);
            ChunksWidth += ModulusRoundUp(Width, Methods.Editor.EditorConstants.TILES_CHUNK_SIZE);
            ChunksHeight = DivideRoundUp(Height, Methods.Editor.EditorConstants.TILES_CHUNK_SIZE);
            ChunksHeight += ModulusRoundUp(Height, Methods.Editor.EditorConstants.TILES_CHUNK_SIZE);
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
            return GetChunkCoordinates((int)x, (int)y);
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
            
        }
        public void SelectChunk(Point point, bool addSelection = false, bool deselectIfSelected = false)
        {
            var ChunkPoint = GetChunkCoordinatesTopEdge(point.X, point.Y);
            Select(GetChunkArea(ChunkPoint.X, ChunkPoint.Y), addSelection, deselectIfSelected);
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
        }
        public void EndTempSelection()
        {
            TempSelectionTiles.Clear();
            TempSelectionDeselectTiles.Clear();
            RefreshTileCount();
            
        }
        public void StartDrag()
        {
            FirstDrag = true;
            RefreshTileCount();
        }
        public void StartDragOver(Point point, ushort value)
        {
            Deselect();
            isDragOver = true;
            DragOver(point, value);
            RefreshTileCount();
            
        }
        public void DragOver(Point point, ushort value)
        {
            SelectedTiles.Clear();
            SelectedTiles.Values.Clear();
            point = new Point(point.X / Methods.Editor.EditorConstants.TILE_SIZE, point.Y / Methods.Editor.EditorConstants.TILE_SIZE);
            SelectedTiles.Add(point);
            SelectedTiles.Values[point] = value;
            RefreshTileCount();
            
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
            

        }
        public void MoveSelectedQuonta(Point change)
        {
            MoveSelected(Point.Empty, new Point(change.X * Methods.Editor.EditorConstants.TILE_SIZE, change.Y * Methods.Editor.EditorConstants.TILE_SIZE), false);
        }
        public void MoveSelected(Point oldPos, Point newPos, bool duplicate)
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
        }
        private void RemoveTile(Point point)
        {
            SetTile(point, 0xffff);
            RefreshTileCount();
        }
        public void Resize(ushort width, ushort height)
        {
            // first resize the underlying SceneLayer
            _layer.Resize(width, height);

            SelectedTiles = new PointsMap(Width, Height);
            TempSelectionTiles = new PointsMap(Width, Height);

            WorkingHeight = Height;
            WorkingWidth = Width;
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
        public void FlipPropertySelected(FlipDirection direction, bool FlipIndividually = false)
        {
            DetachSelected();
            List<Point> points = new List<Point>(SelectedTiles.Values.Keys);

            if (points.Count == 0) return;

            if (points.Count == 1 || FlipIndividually)
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
            
        }
        private void FlipIndividualTiles(FlipDirection direction, IEnumerable<Point> points)
        {
            foreach (Point point in points)
            {
                SelectedTiles.Values[point] ^= (ushort)direction;
            }
            
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
                        GraphicsTileDrawing.DrawTile(g, _layer.Tiles[y][x], x, y);
                    }
                }
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
            RenderSection(RenderingProvider.MapRenderSelected);


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
            RenderingProvider.MapRenderSelected = new LayerRenderer(Methods.Editor.Solution.CurrentTiles.SelectionImage.GetTexture(), RenderingProvider.TileSelectedProvider, 16, 1);
            if (Methods.Editor.Solution.CurrentTiles.CollisionMaskA != null) RenderingProvider.MapRenderCollisionMapA = new LayerRenderer(Methods.Editor.Solution.CurrentTiles.CollisionMaskA.GetTexture(), RenderingProvider.TileCollisionProviderA, 16, 1);
            if (Methods.Editor.Solution.CurrentTiles.CollisionMaskB != null) RenderingProvider.MapRenderCollisionMapB = new LayerRenderer(Methods.Editor.Solution.CurrentTiles.CollisionMaskB.GetTexture(), RenderingProvider.TileCollisionProviderB, 16, 1);

            RequireRefresh = false;
            isMapRenderInitalized = true;
        }

        #endregion

        #region Disposing/Invalidating (SFML.Graphics)

        public void Dispose()
        {

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
