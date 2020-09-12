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
        public List<IAction> Actions { get; set; } = new List<IAction>();
        private SceneLayer _layer { get; set; }
        internal SceneLayer Layer { get => _layer; }

        #endregion

        #region Specific Definitions
        public int LastSelectedPositionX
        {
            get
            {
                var list = SelectedTiles.GetAll();
                if (list.Count != 1) return -1;
                else if (list.Count != 0) return list[0].X;
                else return -1;
            }
        }
        public int LastSelectedPositionY
        {
            get
            {
                var list = SelectedTiles.GetAll();
                if (list.Count != 1) return -1;
                else if (list.Count != 0) return list[0].Y;
                else return -1;
            }
        }
        private bool TempSelectionDeselect { get; set; }
        private bool FirstDrag { get; set; }
        private bool isDragOver { get; set; }
        private bool IsDrawing { get; set; }
        private bool FirstDraw { get; set; }
        public PointsMap SelectedTiles { get; set; }
        public PointsMap TempSelectionTiles { get; set; }
        public PointsMap TempSelectionDeselectTiles { get; set; }

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
        public int ChunksWidth { get; set; }
        public int ChunksHeight { get; set; }
        public ushort WorkingHeight { get; set; }
        public ushort WorkingWidth { get; set; }
        public int PixelHeight { get => _layer.Height * Methods.Solution.SolutionConstants.TILE_SIZE; }
        public int PixelWidth { get => _layer.Width * Methods.Solution.SolutionConstants.TILE_SIZE; }
        public ushort[][] Tiles { get => Layer.Tiles; }
        public IList<HorizontalLayerScroll> HorizontalLayerRules { get; set; }

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


            public override string ToString()
            {
                return $"ID: {Behavior}{DrawOrder}{RelativeSpeed}{ConstantSpeed}, with {LinesMapList.Count} Maps";
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
            public Dictionary<Point, ushort> Values { get; set; } = new Dictionary<Point, ushort>();
            HashSet<Point>[][] PointsChunks;
            public HashSet<Point> OutOfBoundsPoints { get; private set; } = new HashSet<Point>();
            public int Count = 0;

            public PointsMap(int width, int height)
            {
                PointsChunks = new HashSet<Point>[DivideRoundUp(height, Methods.Solution.SolutionConstants.TILES_CHUNK_SIZE)][];
                for (int i = 0; i < PointsChunks.Length; ++i)
                {
                    PointsChunks[i] = new HashSet<Point>[DivideRoundUp(width, Methods.Solution.SolutionConstants.TILES_CHUNK_SIZE)];
                    for (int j = 0; j < PointsChunks[i].Length; ++j)
                        PointsChunks[i][j] = new HashSet<Point>();
                }
            }

            public void Add(Point point)
            {

                HashSet<Point> h;
                if (point.Y < 0 || point.X < 0 || point.Y / Methods.Solution.SolutionConstants.TILES_CHUNK_SIZE >= PointsChunks.Length || point.X / Methods.Solution.SolutionConstants.TILES_CHUNK_SIZE >= PointsChunks[0].Length)
                    h = OutOfBoundsPoints;
                else
                    h = PointsChunks[point.Y / Methods.Solution.SolutionConstants.TILES_CHUNK_SIZE][point.X / Methods.Solution.SolutionConstants.TILES_CHUNK_SIZE];
                Count -= h.Count;
                h.Add(point);
                Count += h.Count;
            }

            public void Remove(Point point)
            {
                HashSet<Point> h;
                if (point.Y < 0 || point.X < 0 || point.Y / Methods.Solution.SolutionConstants.TILES_CHUNK_SIZE >= PointsChunks.Length || point.X / Methods.Solution.SolutionConstants.TILES_CHUNK_SIZE >= PointsChunks[0].Length)
                    h = OutOfBoundsPoints;
                else
                    h = PointsChunks[point.Y / Methods.Solution.SolutionConstants.TILES_CHUNK_SIZE][point.X / Methods.Solution.SolutionConstants.TILES_CHUNK_SIZE];
                Count -= h.Count;
                h.Remove(point);
                Count += h.Count;
            }

            public bool Contains(Point point)
            {
                if (point.Y < 0 || point.X < 0 || point.Y / Methods.Solution.SolutionConstants.TILES_CHUNK_SIZE >= PointsChunks.Length || point.X / Methods.Solution.SolutionConstants.TILES_CHUNK_SIZE >= PointsChunks[0].Length)
                    return OutOfBoundsPoints.Contains(point);
                else
                    return PointsChunks[point.Y / Methods.Solution.SolutionConstants.TILES_CHUNK_SIZE][point.X / Methods.Solution.SolutionConstants.TILES_CHUNK_SIZE].Contains(point);
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

        private ChunkVBO[][] ChunkMap;
        public int RenderingTransparency { get; set; }
        public bool Visible { get; set; }

        #endregion

        #endregion

        #region Init

        public EditorLayer(SceneLayer layer, ManiacEditor.Controls.Editor.MainEditor instance)
        {
            Instance = instance;
            _layer = layer;

            SelectedTiles = new PointsMap(Width, Height);
            TempSelectionTiles = new PointsMap(Width, Height);
            TempSelectionDeselectTiles = new PointsMap(Width, Height);

            HorizontalLayerRules = ReadHorizontalLineRules();

            WorkingHeight = _layer.Height;
            WorkingWidth = _layer.Width;
            InitiallizeChunkMap();

        }

        private void InitiallizeChunkMap()
        {
            ChunksWidth = DivideRoundUp(Width, Methods.Solution.SolutionConstants.TILES_CHUNK_SIZE);
            ChunksWidth += ModulusRoundUp(Width, Methods.Solution.SolutionConstants.TILES_CHUNK_SIZE);
            ChunksHeight = DivideRoundUp(Height, Methods.Solution.SolutionConstants.TILES_CHUNK_SIZE);
            ChunksHeight += ModulusRoundUp(Height, Methods.Solution.SolutionConstants.TILES_CHUNK_SIZE);

            ChunkMap = new ChunkVBO[ChunksHeight][];
            for (int i = 0; i < ChunkMap.Length; ++i)
            {
                ChunkMap[i] = new ChunkVBO[ChunksWidth];
            }

        }

        #endregion

        #region Horizontal Layer Rules
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
            _layer.ScrollingInfo = HorizontalLayerRules.Select(hlr => hlr.ScrollInfo).ToList();

            // the internal ID may now be inaccurate
            // we were only using it for display purposes anyway
            // generate some correct ones, and use those!
            byte newIndex = 0;
            foreach (var hlr in HorizontalLayerRules)
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
            var id = (byte)(HorizontalLayerRules.Select(hlr => hlr.Id).Max() + 1);
            var info = new ScrollInfo();

            _layer.ScrollingInfo.Add(info);
            var hls = new HorizontalLayerScroll(id, info);
            HorizontalLayerRules.Add(hls);
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

        #region Get Tile/Chunk Details
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
        private ushort GetTile(Point point)
        {
            return _layer.Tiles[point.Y][point.X];

        }
        public bool IsPointSelected(Point point)
        {
            return SelectedTiles.Contains(new Point(point.X / Methods.Solution.SolutionConstants.TILE_SIZE, point.Y / Methods.Solution.SolutionConstants.TILE_SIZE));
        }
        public bool DoesChunkContainASelectedTile(Point point)
        {
            Point startingPoint = new Point(point.X / Methods.Solution.SolutionConstants.TILE_SIZE, point.Y / Methods.Solution.SolutionConstants.TILE_SIZE);
            List<Point> chunkPoints = new List<Point>();
            for (int x = 0; x < (Methods.Solution.SolutionConstants.x128_CHUNK_SIZE / Methods.Solution.SolutionConstants.TILE_SIZE); x++)
            {
                for (int y = 0; y < (Methods.Solution.SolutionConstants.x128_CHUNK_SIZE / Methods.Solution.SolutionConstants.TILE_SIZE); y++)
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
            point = new Point(point.X / Methods.Solution.SolutionConstants.TILE_SIZE, point.Y / Methods.Solution.SolutionConstants.TILE_SIZE);
            if (point.X >= 0 && point.Y >= 0 && point.X < this._layer.Tiles[0].Length && point.Y < this._layer.Tiles.Length)
            {
                return (_layer.Tiles[point.Y][point.X] != 0xffff || Methods.Solution.SolutionState.Main.CopyAir);
            }
            return false;
        }
        public ushort GetTileAt(Point point)
        {
            point = new Point(point.X / Methods.Solution.SolutionConstants.TILE_SIZE, point.Y / Methods.Solution.SolutionConstants.TILE_SIZE);
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
            int y_start = y * Methods.Solution.SolutionConstants.TILES_CHUNK_SIZE;
            int y_end = Math.Min((y + 1) * Methods.Solution.SolutionConstants.TILES_CHUNK_SIZE, _layer.Height);

            int x_start = x * Methods.Solution.SolutionConstants.TILES_CHUNK_SIZE;
            int x_end = Math.Min((x + 1) * Methods.Solution.SolutionConstants.TILES_CHUNK_SIZE, _layer.Width);


            return new Rectangle(x_start, y_start, x_end - x_start, y_end - y_start);
        }

        public bool IsPointOutOfBounds(int x, int y)
        {
            bool OutOfBoundsPositive = (Height - 1 < y || Width - 1 < x);
            bool OutOfBoundsNegative = (0 > y || 0 > x);
            return (OutOfBoundsPositive || OutOfBoundsNegative);
        }
        public bool IsPixelOutOfBounds(int x, int y)
        {
            bool OutOfBoundsPositive = (PixelHeight - 1 < y || PixelWidth - 1 < x);
            bool OutOfBoundsNegative = (0 > y || 0 > x);
            return (OutOfBoundsPositive || OutOfBoundsNegative);
        }

        public bool IsPointOutOfBounds(Point p)
        {
            return IsPointOutOfBounds(p.X, p.Y);
        }
        public bool IsPixelOutOfBounds(Point p)
        {
            return IsPixelOutOfBounds(p.X, p.Y);
        }

        #endregion

        #region Selection Manipulation

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
            if (removedSomething && !IsDrawing)
            {
                Actions.Add(new ActionsGroupCloseMarker());
            }

            SelectedTiles.Values.Clear();
            InvalidateChunks();
        }
        public void EndTempSelection()
        {
            TempSelectionTiles.Clear();
            TempSelectionDeselectTiles.Clear();
            InvalidateChunks();
        }
        private void DetachSelectedFromLayer()
        {
            foreach (Point point in SelectedTiles.GetAll())
            {
                if (!SelectedTiles.Values.ContainsKey(point))
                {
                    // Not moved yet
                    SelectedTiles.Values[point] = _layer.Tiles[point.Y][point.X];
                    RemoveTile(point);
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
            InvalidateChunks();
        }
        public void DeselectAll()
        {
            bool hasTiles = SelectedTiles.Values.Count > 0;
            foreach (KeyValuePair<Point, ushort> point in SelectedTiles.Values)
            {
                // ignore out of bounds
                if (point.Key.X < 0 || point.Key.Y < 0 || point.Key.Y >= _layer.Height || point.Key.X >= _layer.Width) continue;
                SetTile(point.Key, point.Value);
            }
            if (hasTiles && !IsDrawing) Actions.Add(new ActionsGroupCloseMarker());

            SelectedTiles.Clear();
            SelectedTiles.Values.Clear();
            InvalidateChunks();
        }
        private void SelectEverything()
        {
            for (int y = 0; y < _layer.Tiles.Length; y += 1)
            {
                for (int x = 0; x < _layer.Tiles[y].Length; x += 1)
                {
                    if (CanSelectTile(_layer.Tiles[y][x])) SelectedTiles.Add(new Point(x, y));
                }
            }
            InvalidateChunks();
        }
        public void Select(Rectangle area, bool addSelection = false, bool deselectIfSelected = false)
        {
            if (!addSelection) DeselectAll();
            for (int y = Math.Max(area.Y / Methods.Solution.SolutionConstants.TILE_SIZE, 0); y < Math.Min(DivideRoundUp(area.Y + area.Height, Methods.Solution.SolutionConstants.TILE_SIZE), _layer.Height); ++y)
            {
                for (int x = Math.Max(area.X / Methods.Solution.SolutionConstants.TILE_SIZE, 0); x < Math.Min(DivideRoundUp(area.X + area.Width, Methods.Solution.SolutionConstants.TILE_SIZE), _layer.Width); ++x)
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

                            }
                            // Don't add already selected tile, or if it was just deslected
                            continue;
                        }
                    }

                    if (CanSelectTile(_layer.Tiles[y][x])) SelectedTiles.Add(new Point(x, y));

                }
            }
            InvalidateChunks();
        }
        public void SelectChunk(Point point, bool addSelection = false, bool deselectIfSelected = false)
        {
            var ChunkPoint = GetChunkCoordinatesTopEdge(point.X, point.Y);
            Select(GetChunkArea(ChunkPoint.X, ChunkPoint.Y), addSelection, deselectIfSelected);
        }
        public void Select(Point point, bool addSelection = false, bool deselectIfSelected = false)
        {
            if (!addSelection) DeselectAll();
            point = new Point(point.X / Methods.Solution.SolutionConstants.TILE_SIZE, point.Y / Methods.Solution.SolutionConstants.TILE_SIZE);
            if (point.X >= 0 && point.Y >= 0 && point.X < this._layer.Tiles[0].Length && point.Y < this._layer.Tiles.Length)
            {
                if (deselectIfSelected && SelectedTiles.Contains(point))
                {
                    // Deselect
                    DeselectPoint(point);

                }
                else if (CanSelectTile(this._layer.Tiles[point.Y][point.X]))
                {
                    // Just add the point
                    SelectedTiles.Add(point);

                }
            }
            InvalidateChunks();
        }
        public void TempSelection(Rectangle area, bool deselectIfSelected)
        {
            TempSelectionTiles.Clear();
            TempSelectionDeselectTiles.Clear();
            TempSelectionDeselect = deselectIfSelected;
            for (int y = Math.Max(area.Y / Methods.Solution.SolutionConstants.TILE_SIZE, 0); y < Math.Min(DivideRoundUp(area.Y + area.Height, Methods.Solution.SolutionConstants.TILE_SIZE), _layer.Height); ++y)
            {
                for (int x = Math.Max(area.X / Methods.Solution.SolutionConstants.TILE_SIZE, 0); x < Math.Min(DivideRoundUp(area.X + area.Width, Methods.Solution.SolutionConstants.TILE_SIZE), _layer.Width); ++x)
                {
                    if (SelectedTiles.Contains(new Point(x, y)) || CanSelectTile(_layer.Tiles[y][x]))
                    {
                        TempSelectionTiles.Add(new Point(x, y));
                        if (SelectedTiles.Contains(new Point(x, y)) && TempSelectionTiles.Contains(new Point(x, y)))
                        {
                            TempSelectionDeselectTiles.Add(new Point(x, y));
                        }
                    }
                }
            }
            InvalidateChunks();
        }

        private bool CanSelectTile(ushort tile)
        {
            if (Methods.Solution.SolutionState.Main.UseMagicSelectWand)
            {
                Structures.TileSelectSpecifics tileSelect = Methods.Solution.SolutionState.Main.GetMagicWandSelectSpecifics();
                bool isMatch = tileSelect.IsMatch(tile);
                if (isMatch) return true;
                else return false;
            }
            else
            {
                if (tile != 0xffff) return true;
                else if (tile == 0xffff && Methods.Solution.SolutionState.Main.CopyAir) return true;
                else return false;
            }

        }

        #endregion

        #region Drag Methods
        public void StartDrag()
        {
            FirstDrag = true;
            InvalidateChunks();
        }
        public void StartDragOver(Point point, ushort value)
        {
            DeselectAll();
            isDragOver = true;
            DragOver(point, value);
            InvalidateChunks();
        }
        public void DragOver(Point point, ushort value)
        {
            SelectedTiles.Clear();
            SelectedTiles.Values.Clear();
            point = new Point(point.X / Methods.Solution.SolutionConstants.TILE_SIZE, point.Y / Methods.Solution.SolutionConstants.TILE_SIZE);
            SelectedTiles.Add(point);
            SelectedTiles.Values[point] = value;
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
                    
                }
                isDragOver = false;
                
            }
            InvalidateChunks();
        }

        #endregion

        #region Movement Methods

        public void MoveSelectedQuonta(Point change)
        {
            MoveSelected(Point.Empty, new Point(change.X * Methods.Solution.SolutionConstants.TILE_SIZE, change.Y * Methods.Solution.SolutionConstants.TILE_SIZE), false);
        }
        public void MoveSelected(Point oldPos, Point newPos, bool duplicate)
        {
            oldPos = new Point(oldPos.X / Methods.Solution.SolutionConstants.TILE_SIZE, oldPos.Y / Methods.Solution.SolutionConstants.TILE_SIZE);
            newPos = new Point(newPos.X / Methods.Solution.SolutionConstants.TILE_SIZE, newPos.Y / Methods.Solution.SolutionConstants.TILE_SIZE);
            if (oldPos != newPos)
            {
                duplicate &= FirstDrag;
                FirstDrag = false;
                Dictionary<Point, ushort> newDict = new Dictionary<Point, ushort>();
                List<Point> newPoints = new List<Point>(SelectedTiles.Count);
                var points = SelectedTiles.PopAll();
                for (int i = 0; i < points.Count; i++)
                {
                    var point = points[i];
                    Point newPoint = new Point(point.X + (newPos.X - oldPos.X), point.Y + (newPos.Y - oldPos.Y));
                    newPoints.Add(newPoint);
                    if (SelectedTiles.Values.TryGetValue(point, out ushort selectedTile))
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
                    DeselectAll();
                    // Create new actions group
                    Actions.Add(new ActionDummy());
                }
                SelectedTiles.Values = newDict;
                SelectedTiles.AddPoints(newPoints);
                InvalidateChunks();
            }
        }

        #endregion

        #region Internal Set/Remove Tile

        private void SetTile(Point point, ushort value, bool addAction = true)
        {
            if (addAction) Actions.Add(new ActionChangeTile((x, y) => SetTile(x, y, false), point, _layer.Tiles[point.Y][point.X], value));
            _layer.Tiles[point.Y][point.X] = value;
            InvalidateChunks();
        }
        private void RemoveTile(Point point)
        {
            SetTile(point, 0xffff);
        }

        #endregion

        #region Tile Draw and Place
        public void StartDraw()
        {
            if (!IsDrawing)
            {
                IsDrawing = true;
                FirstDraw = true;
            }
        }
        public void EndDraw()
        {
            if (IsDrawing)
            {
                IsDrawing = false;
                FirstDraw = false;
                Actions.Add(new ActionsGroupCloseMarker());
            }
        }
        public void EraseTiles(Rectangle p)
        {
            Select(p);
            DeleteSelected();
        }
        public void EraseTiles(Point p)
        {
            Select(p);
            DeleteSelected();
        }
        private Point GetTilePlacementPoint()
        {
            return new Point(0, 0);
        }
        public void DrawTiles(Point position, ushort tile)
        {
            double offset = (Methods.Solution.SolutionState.Main.DrawBrushSize / 2) * Methods.Solution.SolutionConstants.TILE_SIZE;
            Point finalPosition = new Point((int)(position.X - offset), (int)(position.Y - offset));
            Dictionary<Point, ushort> tiles = new Dictionary<Point, ushort>();
            for (int x = 0; x < Methods.Solution.SolutionState.Main.DrawBrushSize; x++)
            {
                for (int y = 0; y < Methods.Solution.SolutionState.Main.DrawBrushSize; y++)
                {
                    if (!tiles.ContainsKey(new Point(x, y))) tiles.Add(new Point(x, y), (ushort)tile);
                }
            }
            DrawTilesBrush(finalPosition, tiles);
        }
        public void DrawTilesBrush(Point newPos, Dictionary<Point, ushort> points)
        {
            try
            {
                bool updateActions = false;
                newPos = new Point(newPos.X / Methods.Solution.SolutionConstants.TILE_SIZE, newPos.Y / Methods.Solution.SolutionConstants.TILE_SIZE);
                DeselectAll();
                foreach (KeyValuePair<Point, ushort> point in points)
                {

                    Point tilePos = new Point(point.Key.X + newPos.X, point.Key.Y + newPos.Y);
                    if (IsPointOutOfBounds(tilePos)) continue;
                    if (point.Value != _layer.Tiles[tilePos.Y][tilePos.X])
                    {
                        SelectedTiles.Add(tilePos);
                        SelectedTiles.Values[tilePos] = point.Value;
                        updateActions = true;
                    }
                }
                // Create new actions group
                if (updateActions && FirstDraw)
                {
                    Actions.Add(new ActionDummy());
                    FirstDraw = false;
                }
            }
            catch
            {
                MessageBox.Show("Clipboard Content Problem!");
            }
        }
        public void PlaceTile(Point position, ushort tile, bool isDrawing = false)
        {
            if (isDrawing) DrawTiles(position, tile);
            else PlaceTiles(position, new List<ushort>() { tile }, 2, 0);
        }
        public void PlaceTiles(Point position, List<ushort> placeTiles, int cutOfPoint, int startingPoint)
        {
            Dictionary<Point, ushort> tiles = new Dictionary<Point, ushort>();

            int count = placeTiles.Count;

            var spawn = GetTilePlacementPoint();

            int x = spawn.X + startingPoint;
            int y = spawn.Y;

            foreach (var tile in placeTiles)
            {
                tiles.Add(new Point(x, y), tile);
                if (x >= spawn.X + cutOfPoint - 1)
                {
                    x = spawn.X;
                    y = y + 1;
                }
                else x = x + 1;
            }

            this.PasteClipboardData(position, new Methods.Solution.SolutionClipboard.TilesClipboardEntry(tiles));
        }

        #endregion

        #region Flipping/Properties
        public void FlipPropertySelected(FlipDirection direction, bool FlipIndividually = false)
        {
            DetachSelectedFromLayer();
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
            DetachSelectedFromLayer();

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
            
            return selectedValues;
        }
        #endregion

        #region Methods
        public void Resize(ushort width, ushort height)
        {
            // first resize the underlying SceneLayer
            _layer.Resize(width, height);

            SelectedTiles = new PointsMap(Width, Height);
            TempSelectionTiles = new PointsMap(Width, Height);

            WorkingHeight = Height;
            WorkingWidth = Width;
        }

        public EditorLayer Clone()
        {
            var cloneLayer = new EditorLayer(_layer, Instance);
            return cloneLayer;
        }

        #endregion

        #region Actions

        public static void Copy()
        {
            Methods.Solution.SolutionMultiLayer.Copy();
        }
        public static void Paste()
        {
            Methods.Solution.SolutionMultiLayer.Paste();
        }
        public static void Cut()
        {
            Copy();
            Methods.Solution.CurrentSolution.EditLayerA?.DeleteSelected();
            Methods.Solution.CurrentSolution.EditLayerB?.DeleteSelected();
            Methods.Solution.CurrentSolution.EditLayerC?.DeleteSelected();
            Methods.Solution.CurrentSolution.EditLayerD?.DeleteSelected();
            ManiacEditor.Actions.UndoRedoModel.UpdateEditLayersActions();
        }
        public static void Duplicate()
        {
            var copyDataA = Methods.Solution.CurrentSolution.EditLayerA?.GetClipboardData();
            var copyDataB = Methods.Solution.CurrentSolution.EditLayerB?.GetClipboardData();
            var copyDataC = Methods.Solution.CurrentSolution.EditLayerC?.GetClipboardData();
            var copyDataD = Methods.Solution.CurrentSolution.EditLayerD?.GetClipboardData();
            Methods.Solution.CurrentSolution.EditLayerA?.PasteClipboardData(new Point(16, 16), copyDataA);
            Methods.Solution.CurrentSolution.EditLayerB?.PasteClipboardData(new Point(16, 16), copyDataB);
            Methods.Solution.CurrentSolution.EditLayerC?.PasteClipboardData(new Point(16, 16), copyDataC);
            Methods.Solution.CurrentSolution.EditLayerD?.PasteClipboardData(new Point(16, 16), copyDataD);
            ManiacEditor.Actions.UndoRedoModel.UpdateEditLayersActions();
        }
        public static void Delete()
        {
            Methods.Solution.CurrentSolution.EditLayerA?.DeleteSelected();
            Methods.Solution.CurrentSolution.EditLayerB?.DeleteSelected();
            Methods.Solution.CurrentSolution.EditLayerC?.DeleteSelected();
            Methods.Solution.CurrentSolution.EditLayerD?.DeleteSelected();
            ManiacEditor.Actions.UndoRedoModel.UpdateEditLayersActions();
        }
        public static void Deselect()
        {
            Methods.Solution.CurrentSolution.EditLayerA?.DeselectAll();
            Methods.Solution.CurrentSolution.EditLayerB?.DeselectAll();
            Methods.Solution.CurrentSolution.EditLayerC?.DeselectAll();
            Methods.Solution.CurrentSolution.EditLayerD?.DeselectAll();
            ManiacEditor.Actions.UndoRedoModel.UpdateEditLayersActions();
        }
        public static void SelectAll()
        {
            if (Methods.Solution.CurrentSolution.EditLayerA != null) Methods.Solution.CurrentSolution.EditLayerA?.SelectEverything();
            if (Methods.Solution.CurrentSolution.EditLayerB != null) Methods.Solution.CurrentSolution.EditLayerB?.SelectEverything();
            if (Methods.Solution.CurrentSolution.EditLayerC != null) Methods.Solution.CurrentSolution.EditLayerC?.SelectEverything();
            if (Methods.Solution.CurrentSolution.EditLayerD != null) Methods.Solution.CurrentSolution.EditLayerD?.SelectEverything();
            ManiacEditor.Actions.UndoRedoModel.UpdateEditLayersActions();
        }
        public static void FlipTiles(bool Individual, FlipDirection Direction)
        {
            Methods.Solution.CurrentSolution.EditLayerA?.FlipPropertySelected(Direction, Individual);
            Methods.Solution.CurrentSolution.EditLayerB?.FlipPropertySelected(Direction, Individual);
            Methods.Solution.CurrentSolution.EditLayerC?.FlipPropertySelected(Direction, Individual);
            Methods.Solution.CurrentSolution.EditLayerD?.FlipPropertySelected(Direction, Individual);
            ManiacEditor.Actions.UndoRedoModel.UpdateEditLayersActions();
        }
        public static void MoveTiles(System.Windows.Forms.KeyEventArgs e)
        {
            int x = 0, y = 0;
            int modifier = (ManiacEditor.Methods.Solution.SolutionState.Main.IsChunksEdit() ? 8 : 1);
            if (Methods.Solution.SolutionState.Main.UseMagnetMode)
            {
                switch (e.KeyData)
                {
                    case Keys.Up: y = (Methods.Solution.SolutionState.Main.UseMagnetYAxis ? -Methods.Solution.SolutionState.Main.MagnetSize : -1); break;
                    case Keys.Down: y = (Methods.Solution.SolutionState.Main.UseMagnetYAxis ? Methods.Solution.SolutionState.Main.MagnetSize : 1); break;
                    case Keys.Left: x = (Methods.Solution.SolutionState.Main.UseMagnetXAxis ? -Methods.Solution.SolutionState.Main.MagnetSize : -1); break;
                    case Keys.Right: x = (Methods.Solution.SolutionState.Main.UseMagnetXAxis ? Methods.Solution.SolutionState.Main.MagnetSize : 1); break;
                }
            }
            if (Methods.Solution.SolutionState.Main.EnableFasterNudge)
            {
                if (Methods.Solution.SolutionState.Main.UseMagnetMode)
                {
                    switch (e.KeyData)
                    {
                        case Keys.Up: y = (Methods.Solution.SolutionState.Main.UseMagnetYAxis ? -Methods.Solution.SolutionState.Main.MagnetSize * Methods.Solution.SolutionState.Main.FasterNudgeAmount : -1 - Methods.Solution.SolutionState.Main.FasterNudgeAmount); break;
                        case Keys.Down: y = (Methods.Solution.SolutionState.Main.UseMagnetYAxis ? Methods.Solution.SolutionState.Main.MagnetSize * Methods.Solution.SolutionState.Main.FasterNudgeAmount : 1 + Methods.Solution.SolutionState.Main.FasterNudgeAmount); break;
                        case Keys.Left: x = (Methods.Solution.SolutionState.Main.UseMagnetXAxis ? -Methods.Solution.SolutionState.Main.MagnetSize * Methods.Solution.SolutionState.Main.FasterNudgeAmount : -1 - Methods.Solution.SolutionState.Main.FasterNudgeAmount); break;
                        case Keys.Right: x = (Methods.Solution.SolutionState.Main.UseMagnetXAxis ? Methods.Solution.SolutionState.Main.MagnetSize * Methods.Solution.SolutionState.Main.FasterNudgeAmount : 1 + Methods.Solution.SolutionState.Main.FasterNudgeAmount); break;
                    }
                }
                else
                {
                    if (ManiacEditor.Methods.Solution.SolutionState.Main.IsChunksEdit())
                    {
                        switch (e.KeyData)
                        {
                            case Keys.Up: y = -1 * modifier; break;
                            case Keys.Down: y = 1 * modifier; break;
                            case Keys.Left: x = -1 * modifier; break;
                            case Keys.Right: x = 1 * modifier; break;
                        }
                    }
                    else
                    {
                        switch (e.KeyData)
                        {
                            case Keys.Up: y = (-1 - Methods.Solution.SolutionState.Main.FasterNudgeAmount) * modifier; break;
                            case Keys.Down: y = (1 + Methods.Solution.SolutionState.Main.FasterNudgeAmount) * modifier; break;
                            case Keys.Left: x = (-1 - Methods.Solution.SolutionState.Main.FasterNudgeAmount) * modifier; break;
                            case Keys.Right: x = (1 + Methods.Solution.SolutionState.Main.FasterNudgeAmount) * modifier; break;
                        }
                    }

                }

            }
            if (Methods.Solution.SolutionState.Main.UseMagnetMode == false && Methods.Solution.SolutionState.Main.EnableFasterNudge == false)
            {
                switch (e.KeyData)
                {
                    case Keys.Up: y = -1 * modifier; break;
                    case Keys.Down: y = 1 * modifier; break;
                    case Keys.Left: x = -1 * modifier; break;
                    case Keys.Right: x = 1 * modifier; break;
                }

            }
            Methods.Solution.CurrentSolution.EditLayerA?.MoveSelectedQuonta(new Point(x, y));
            Methods.Solution.CurrentSolution.EditLayerB?.MoveSelectedQuonta(new Point(x, y));
            Methods.Solution.CurrentSolution.EditLayerC?.MoveSelectedQuonta(new Point(x, y));
            Methods.Solution.CurrentSolution.EditLayerD?.MoveSelectedQuonta(new Point(x, y));

            ManiacEditor.Actions.UndoRedoModel.UpdateEditLayersActions();
        }

        #endregion

        #region Clipboard Data Retrival

        private static bool AvoidWindowsClipboard = false;
        public Dictionary<Point, ushort> GetClipboardDataRaw(bool KeepPosition = false)
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
            if (!KeepPosition)
            {
                minX = copiedTiles.Keys.Min(x => x.X);
                minY = copiedTiles.Keys.Min(x => x.Y);
            }
            return copiedTiles.ToDictionary(x => new Point(x.Key.X - minX, x.Key.Y - minY), x => x.Value);
        }
        public Methods.Solution.SolutionClipboard.TilesClipboardEntry GetClipboardData(bool KeepPosition = false)
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
            if (!KeepPosition)
            {
                minX = copiedTiles.Keys.Min(x => x.X);
                minY = copiedTiles.Keys.Min(x => x.Y);
            }
            return new Methods.Solution.SolutionClipboard.TilesClipboardEntry(copiedTiles.ToDictionary(x => new Point(x.Key.X - minX, x.Key.Y - minY), x => x.Value));
        }
        public static Methods.Solution.SolutionClipboard.MultiTilesClipboardEntry GetMultiClipboardData(bool KeepPosition = false)
        {
            int minX = 0, minY = 0;

            var copyDataA = Methods.Solution.CurrentSolution.EditLayerA?.GetClipboardDataRaw(true) ?? new Dictionary<Point, ushort>();
            var copyDataB = Methods.Solution.CurrentSolution.EditLayerB?.GetClipboardDataRaw(true) ?? new Dictionary<Point, ushort>();
            var copyDataC = Methods.Solution.CurrentSolution.EditLayerC?.GetClipboardDataRaw(true) ?? new Dictionary<Point, ushort>();
            var copyDataD = Methods.Solution.CurrentSolution.EditLayerD?.GetClipboardDataRaw(true) ?? new Dictionary<Point, ushort>();

            if (!KeepPosition)
            {
                int minX_A = (copyDataA.Keys.Count != 0 ? copyDataA.Keys.Min(x => x.X) : -1);
                int minY_A = (copyDataA.Keys.Count != 0 ? copyDataA.Keys.Min(x => x.Y) : -1);

                int minX_B = (copyDataB.Keys.Count != 0 ? copyDataB.Keys.Min(x => x.X) : -1);
                int minY_B = (copyDataB.Keys.Count != 0 ? copyDataB.Keys.Min(x => x.Y) : -1);

                int minX_C = (copyDataC.Keys.Count != 0 ? copyDataC.Keys.Min(x => x.X) : -1);
                int minY_C = (copyDataC.Keys.Count != 0 ? copyDataC.Keys.Min(x => x.Y) : -1);

                int minX_D = (copyDataD.Keys.Count != 0 ? copyDataD.Keys.Min(x => x.X) : -1);
                int minY_D = (copyDataD.Keys.Count != 0 ? copyDataD.Keys.Min(x => x.Y) : -1);

                List<int> minX_array = new List<int>();
                if (minX_A != -1) minX_array.Add(minX_A);
                if (minX_B != -1) minX_array.Add(minX_B);
                if (minX_C != -1) minX_array.Add(minX_C);
                if (minX_D != -1) minX_array.Add(minX_D);

                List<int> minY_array = new List<int>();
                if (minY_A != -1) minY_array.Add(minY_A);
                if (minY_B != -1) minY_array.Add(minY_B);
                if (minY_C != -1) minY_array.Add(minY_C);
                if (minY_D != -1) minY_array.Add(minY_D);


                if (minX_array.Count != 0) minX = minX_array.Min();
                if (minY_array.Count != 0) minY = minY_array.Min();
            }

            var resultA = new Methods.Solution.SolutionClipboard.TilesClipboardEntry(copyDataA.ToDictionary(x => new Point(x.Key.X - minX, x.Key.Y - minY), x => x.Value));
            var resultB = new Methods.Solution.SolutionClipboard.TilesClipboardEntry(copyDataB.ToDictionary(x => new Point(x.Key.X - minX, x.Key.Y - minY), x => x.Value));
            var resultC = new Methods.Solution.SolutionClipboard.TilesClipboardEntry(copyDataC.ToDictionary(x => new Point(x.Key.X - minX, x.Key.Y - minY), x => x.Value));
            var resultD = new Methods.Solution.SolutionClipboard.TilesClipboardEntry(copyDataD.ToDictionary(x => new Point(x.Key.X - minX, x.Key.Y - minY), x => x.Value));


            return new Methods.Solution.SolutionClipboard.MultiTilesClipboardEntry(resultA, resultB, resultC, resultD);
        }
        public void PasteClipboardData(Point newPos, Methods.Solution.SolutionClipboard.TilesClipboardEntry data, bool UpdateActions = true)
        {
            try
            {
                var points = data.GetData();
                newPos = new Point(newPos.X / Methods.Solution.SolutionConstants.TILE_SIZE, newPos.Y / Methods.Solution.SolutionConstants.TILE_SIZE);
                DeselectAll();
                foreach (KeyValuePair<Point, ushort> point in points)
                {
                    Point tilePos = new Point(point.Key.X + newPos.X, point.Key.Y + newPos.Y);
                    SelectedTiles.Add(tilePos);
                    SelectedTiles.Values[tilePos] = point.Value;
                }
                // Create new actions group
                if (UpdateActions) Actions.Add(new ActionDummy());
                
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("There was a problem with pasting the content provided: " + Environment.NewLine + ex.Message);
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
                        Methods.Drawing.CommonDrawing.DrawTile(g, _layer.Tiles[y][x], x, y);
                    }
                }
            }
        }

        #endregion

        #region New Draw Method
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

        public void Draw(DevicePanel d)
        {
            DrawLayer(d);
        }

        private bool IsSemiTransparent()
        {
            if (Methods.Solution.CurrentSolution.EditLayerA != null && Methods.Solution.CurrentSolution.EditLayerA != this) return true;
            else if (Methods.Solution.CurrentSolution.EditLayerB != null && Methods.Solution.CurrentSolution.EditLayerB != this) return true;
            else if (Methods.Solution.CurrentSolution.EditLayerC != null && Methods.Solution.CurrentSolution.EditLayerC != this) return true;
            else if (Methods.Solution.CurrentSolution.EditLayerD != null && Methods.Solution.CurrentSolution.EditLayerD != this) return true;
            else if (Instance.EditorToolbar.EditEntities.IsCheckedAll && Methods.Solution.SolutionState.Main.ApplyEditEntitiesTransparency) return true;
            else return false;
        }

        public void DrawLayer(DevicePanel d)
        {
            int Transperncy;


            if (IsSemiTransparent()) Transperncy = 0x32;
            else Transperncy = 0xFF;

            Rectangle screen = d.GetScreen();
            int pos_x = (Methods.Solution.SolutionState.Main.UnlockCamera ? 0 : screen.X);
            int pos_y = (Methods.Solution.SolutionState.Main.UnlockCamera ? 0 : screen.Y);
            int width = (Methods.Solution.SolutionState.Main.UnlockCamera ? this.PixelWidth : screen.Width);
            int height = (Methods.Solution.SolutionState.Main.UnlockCamera ? this.PixelHeight : screen.Height);

            if (pos_x >= 0 && pos_y >= 0 && width >= 0 && height >= 0)
            {
                int start_x = pos_x / (Methods.Solution.SolutionConstants.TILES_CHUNK_SIZE * Methods.Solution.SolutionConstants.TILE_SIZE);
                int end_x = Math.Min(DivideRoundUp(pos_x + width, Methods.Solution.SolutionConstants.TILES_CHUNK_SIZE * Methods.Solution.SolutionConstants.TILE_SIZE), ChunkMap[0].Length);
                int start_y = pos_y / (Methods.Solution.SolutionConstants.TILES_CHUNK_SIZE * Methods.Solution.SolutionConstants.TILE_SIZE);
                int end_y = Math.Min(DivideRoundUp(pos_y + height, Methods.Solution.SolutionConstants.TILES_CHUNK_SIZE * Methods.Solution.SolutionConstants.TILE_SIZE), ChunkMap.Length);

                for (int y = start_y; y < end_y; ++y)
                {
                    for (int x = start_x; x < end_x; ++x)
                    {
                        if (d.IsObjectOnScreen(x * 256, y * 256, 256, 256))
                        {
                            Rectangle rect = GetTilesChunkArea(x, y);
                            var texture = GetChunk(d, x, y);
                            if (texture != null) d.DrawBitmap(texture, rect.X * Methods.Solution.SolutionConstants.TILE_SIZE, rect.Y * Methods.Solution.SolutionConstants.TILE_SIZE, rect.Width * Methods.Solution.SolutionConstants.TILE_SIZE, rect.Height * Methods.Solution.SolutionConstants.TILE_SIZE, false, Transperncy);
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

            int x = rect.X * Methods.Solution.SolutionConstants.TILE_SIZE;
            int y = rect.Y * Methods.Solution.SolutionConstants.TILE_SIZE;
            int x2 = rect.Right * Methods.Solution.SolutionConstants.TILE_SIZE;
            int y2 = rect.Bottom * Methods.Solution.SolutionConstants.TILE_SIZE;

            int mouse_x = (int)Methods.Solution.SolutionState.Main.LastX;
            int mouse_y = (int)Methods.Solution.SolutionState.Main.LastY;

            if (mouse_x >= x && mouse_x <= x2 && mouse_y >= y && mouse_y <= y2)
            {
                //System.Diagnostics.Debug.Print(string.Format("Chunk {0},{1} Selected", _x, _y));
                return true;
            }
            else return false;

        }

        public Classes.Rendering.TextureExt GetChunk(DevicePanel d, int x, int y)
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

                if (rect.Width <= 0 || rect.Height <= 0) return null;

                Bitmap bmp2 = new Bitmap(rect.Width * Methods.Solution.SolutionConstants.TILE_SIZE, rect.Height * Methods.Solution.SolutionConstants.TILE_SIZE, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
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
                    ChunkMap[y][x].Texture = Methods.Drawing.TextureCreator.FromBitmap(d._device, bmp);
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

                System.Drawing.Color AllSolid = Methods.Solution.SolutionState.Main.CollisionAllSolid_Color;
                System.Drawing.Color LRDSolid = Methods.Solution.SolutionState.Main.CollisionLRDSolid_Color;
                System.Drawing.Color TopOnlySolid = Methods.Solution.SolutionState.Main.CollisionTopOnlySolid_Color;

                g.DrawImage(Methods.Solution.CurrentSolution.CurrentTiles.BaseImage.GetBitmap(new Rectangle(0, TileIndex * Methods.Solution.SolutionConstants.TILE_SIZE, Methods.Solution.SolutionConstants.TILE_SIZE, Methods.Solution.SolutionConstants.TILE_SIZE), flipX, flipY, isSelected), new Rectangle(x * Methods.Solution.SolutionConstants.TILE_SIZE, y * Methods.Solution.SolutionConstants.TILE_SIZE, Methods.Solution.SolutionConstants.TILE_SIZE, Methods.Solution.SolutionConstants.TILE_SIZE));

                if (Methods.Solution.SolutionState.Main.ShowCollisionA)
                {
                    if (SolidLrbA || SolidTopA)
                    {
                        if (SolidTopA && SolidLrbA) DrawCollision(true, AllSolid, flipX, flipY);
                        if (SolidTopA && !SolidLrbA) DrawCollision(true, TopOnlySolid, flipX, flipY);
                        if (SolidLrbA && !SolidTopA) DrawCollision(true, LRDSolid, flipX, flipY);
                    }
                }
                if (Methods.Solution.SolutionState.Main.ShowCollisionB)
                {
                    if (SolidLrbB || SolidTopB)
                    {
                        if (SolidTopB && SolidLrbB) DrawCollision(false, AllSolid, flipX, flipY);
                        if (SolidTopB && !SolidLrbB) DrawCollision(false, TopOnlySolid, flipX, flipY);
                        if (SolidLrbB && !SolidTopB) DrawCollision(false, LRDSolid, flipX, flipY);
                    }
                }

                if (Methods.Solution.SolutionState.Main.ShowFlippedTileHelper == true)
                {
                    g.DrawImage(Methods.Solution.CurrentSolution.CurrentTiles.InternalImage.GetBitmap(new Rectangle(0, 3 * Methods.Solution.SolutionConstants.TILE_SIZE, Methods.Solution.SolutionConstants.TILE_SIZE, Methods.Solution.SolutionConstants.TILE_SIZE), false, false),
                                new Rectangle(x * Methods.Solution.SolutionConstants.TILE_SIZE, y * Methods.Solution.SolutionConstants.TILE_SIZE, Methods.Solution.SolutionConstants.TILE_SIZE, Methods.Solution.SolutionConstants.TILE_SIZE));
                }
                if (Methods.Solution.SolutionState.Main.ShowTileID == true)
                {
                    g.DrawImage(Methods.Solution.CurrentSolution.CurrentTiles.IDImage.GetBitmap(new Rectangle(0, TileIndex * Methods.Solution.SolutionConstants.TILE_SIZE, Methods.Solution.SolutionConstants.TILE_SIZE, Methods.Solution.SolutionConstants.TILE_SIZE), false, false),
                                new Rectangle(x * Methods.Solution.SolutionConstants.TILE_SIZE, y * Methods.Solution.SolutionConstants.TILE_SIZE, Methods.Solution.SolutionConstants.TILE_SIZE, Methods.Solution.SolutionConstants.TILE_SIZE));
                }
            }



            if (isSelected)
            {
                g.DrawRectangle(Pens.Red, new Rectangle(x * Methods.Solution.SolutionConstants.TILE_SIZE, y * Methods.Solution.SolutionConstants.TILE_SIZE, Methods.Solution.SolutionConstants.TILE_SIZE - 1, Methods.Solution.SolutionConstants.TILE_SIZE - 1));
            }

            void DrawCollision(bool drawA, System.Drawing.Color colour, bool flipX, bool flipY)
            {
                //create some image attributes
                ImageAttributes attributes = new ImageAttributes();

                //TODO : Collision Opacity
                int opacity = (int)ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.CollisionOpacitySlider.Value;

                float[][] colourMatrixElements =
                {
                    new float[] { colour.R / 255.0f, 0, 0, 0, 0 },
                    new float[] { 0, colour.G / 255.0f, 0, 0, 0 },
                    new float[] { 0, 0, colour.B / 255.0f, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 0, 1 }
                };

                var matrix = new ColorMatrix(colourMatrixElements);
                matrix.Matrix33 = opacity;
                //set the color matrix attribute
                attributes.SetColorMatrix(matrix);


                int _x = 0;
                int _y = 0;
                int _width = Methods.Solution.SolutionConstants.TILE_SIZE;
                int _height = Methods.Solution.SolutionConstants.TILE_SIZE;

                Rectangle dest = new Rectangle(x * Methods.Solution.SolutionConstants.TILE_SIZE, y * Methods.Solution.SolutionConstants.TILE_SIZE, Methods.Solution.SolutionConstants.TILE_SIZE, Methods.Solution.SolutionConstants.TILE_SIZE);

                Bitmap collisionMap;

                if (drawA) collisionMap = Methods.Solution.CurrentSolution.CurrentTiles.CollisionMaskA.GetBitmap(new Rectangle(0, (tile & 0x3ff) * Methods.Solution.SolutionConstants.TILE_SIZE, Methods.Solution.SolutionConstants.TILE_SIZE, Methods.Solution.SolutionConstants.TILE_SIZE), flipX, flipY);
                else collisionMap = Methods.Solution.CurrentSolution.CurrentTiles.CollisionMaskB.GetBitmap(new Rectangle(0, (tile & 0x3ff) * Methods.Solution.SolutionConstants.TILE_SIZE, Methods.Solution.SolutionConstants.TILE_SIZE, Methods.Solution.SolutionConstants.TILE_SIZE), flipX, flipY);


                g.DrawImage(collisionMap, dest, _x, _y, _width, _height, GraphicsUnit.Pixel, attributes);

                attributes.Dispose();
                attributes = null;

                colourMatrixElements = null;
            }


        }

        #endregion


        public class ChunkVBO
        {
            public bool IsReady = false;
            public Classes.Rendering.TextureExt Texture;
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
