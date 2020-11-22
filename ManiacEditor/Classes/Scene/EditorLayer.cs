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
using ManiacEditor.Entity_Renders;
using Net.Sgoliver.NRtfTree.Util;
using ManiacEditor.Methods.Drawing;

namespace ManiacEditor.Classes.Scene
{
    public class EditorLayer : IDrawable
    {
        #region RSDKv5 Properties
        private SceneLayer _layer { get; set; }
        internal SceneLayer Layer { get => _layer; }

        #endregion

        #region Tile/Drawing Properties

        private GIF CollisionMaskA
        {
            get
            {
                return Methods.Solution.CurrentSolution.CurrentTiles.CollisionMaskA;
            }
        }
        private GIF CollisionMaskB
        {
            get
            {
                return Methods.Solution.CurrentSolution.CurrentTiles.CollisionMaskB;
            }
        }
        private GIF Tilesheet
        {
            get
            {
                return Methods.Solution.CurrentSolution.CurrentTiles.BaseImage;
            }
        }
        private GIF EditorTilesheet
        {
            get
            {
                return Methods.Solution.CurrentSolution.CurrentTiles.InternalImage;
            }
        }
        private GIF IdentificationTilesheet
        {
            get
            {
                return Methods.Solution.CurrentSolution.CurrentTiles.IDImage;
            }
        }
        private ChunkVBO[][] ChunkMap { get; set; }

        private int CollisionOpacity
        {
            get
            {
                return (int)ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.CollisionOpacitySlider.Value;
            }
        }
        private bool ShowCollisionPathA
        {
            get
            {
                return Methods.Solution.SolutionState.Main.ShowCollisionA;
            }
        }
        private bool ShowCollisionPathB
        {
            get
            {
                return Methods.Solution.SolutionState.Main.ShowCollisionB;
            }
        }
        private System.Drawing.Color CollisionAllSolidColor
        {
            get
            {
                return Methods.Solution.SolutionState.Main.CollisionAllSolid_Color;
            }
        }
        private System.Drawing.Color CollisionLRDSolidColor
        {
            get
            {
                return Methods.Solution.SolutionState.Main.CollisionLRDSolid_Color;
            }
        }
        private System.Drawing.Color CollisionTopOnlySolidColor
        {
            get
            {
                return Methods.Solution.SolutionState.Main.CollisionTopOnlySolid_Color;
            }
        }

        private bool IsCurrentEditLayer
        {
            get
            {
                if (Methods.Solution.CurrentSolution.EditLayerA == null) return true;
                else if (Methods.Solution.CurrentSolution.EditLayerA != null && Methods.Solution.CurrentSolution.EditLayerA == this) return true;
                else if (Methods.Solution.CurrentSolution.EditLayerB != null && Methods.Solution.CurrentSolution.EditLayerB == this) return true;
                else if (Methods.Solution.CurrentSolution.EditLayerC != null && Methods.Solution.CurrentSolution.EditLayerC == this) return true;
                else if (Methods.Solution.CurrentSolution.EditLayerD != null && Methods.Solution.CurrentSolution.EditLayerD == this) return true;
                else return false;
            }
        }
        private bool IsEditEntitiesTransparency
        {
            get
            {
                return Methods.Solution.CurrentSolution.UI_Instance.EditorToolbar.EditEntities.IsCheckedAll && Methods.Solution.SolutionState.Main.ApplyEditEntitiesTransparency;
            }
        }
        public int RenderingTransparency { get; set; }
        public bool Visible { get; set; }

        #endregion

        #region Editor Specific Properties

        public List<IAction> Actions { get; set; } = new List<IAction>();
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
        public int PixelHeight { get => _layer.Height * TILE_SIZE; }
        public int PixelWidth { get => _layer.Width * TILE_SIZE; }
        public ushort[][] Tiles { get => Layer.Tiles; }
        public IList<HorizontalLayerScroll> HorizontalLayerRules { get; set; }

        #endregion

        #region Events Definitions

        public static event EventHandler OnUpdate;

        #endregion

        #region Constant Variables

        const int TILE_SIZE = 16;

        const int TILES_CHUNK_SIZE = 16;

        #endregion

        #region Init Methods

        public EditorLayer(SceneLayer layer)
        {
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
            ChunksWidth = DivideRoundUp(Width, TILES_CHUNK_SIZE);
            ChunksWidth += ModulusRoundUp(Width, TILES_CHUNK_SIZE);
            ChunksHeight = DivideRoundUp(Height, TILES_CHUNK_SIZE);
            ChunksHeight += ModulusRoundUp(Height, TILES_CHUNK_SIZE);

            ChunkMap = new ChunkVBO[ChunksHeight][];
            for (int i = 0; i < ChunkMap.Length; ++i)
            {
                ChunkMap[i] = new ChunkVBO[ChunksWidth];
            }

        }

        #endregion

        #region Layer Rule Methods
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

        #region Get Tile/Chunk Info Methods
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
            return SelectedTiles.Contains(new Point(point.X / TILE_SIZE, point.Y / TILE_SIZE));
        }
        public bool DoesChunkContainASelectedTile(Point point)
        {
            Point startingPoint = new Point(point.X / TILE_SIZE, point.Y / TILE_SIZE);
            List<Point> chunkPoints = new List<Point>();
            for (int x = 0; x < (Methods.Solution.SolutionConstants.x128_CHUNK_SIZE / TILE_SIZE); x++)
            {
                for (int y = 0; y < (Methods.Solution.SolutionConstants.x128_CHUNK_SIZE / TILE_SIZE); y++)
                {
                    Point p = new Point(startingPoint.X + x, startingPoint.Y + y);
                    if (SelectedTiles.Contains(p)) return true;
                    else continue;
                }
            }
            return false;
        }
        public bool IsChunkEmpty(Point point)
        {
            Point startingPoint = new Point(point.X / TILE_SIZE, point.Y / TILE_SIZE);
            List<Point> chunkPoints = new List<Point>();
            bool HasTile = false;

            for (int x = 0; x < (Methods.Solution.SolutionConstants.x128_CHUNK_SIZE / TILE_SIZE); x++)
            {
                for (int y = 0; y < (Methods.Solution.SolutionConstants.x128_CHUNK_SIZE / TILE_SIZE); y++)
                {
                    Point p = new Point(startingPoint.X + x, startingPoint.Y + y);
                    HasTile = HasTileAt(p, false);
                    if (HasTile == true) return HasTile;
                }
            }
            return HasTile;
        }
        public bool HasTileAt(Point point, bool CopyAir)
        {
            point = new Point(point.X / TILE_SIZE, point.Y / TILE_SIZE);
            if (point.X >= 0 && point.Y >= 0 && point.X < this._layer.Tiles[0].Length && point.Y < this._layer.Tiles.Length)
            {
                if (CopyAir) return true;
                else return (_layer.Tiles[point.Y][point.X] != 0xffff);
            }
            return false;
        }
        public ushort GetTileAt(Point point)
        {
            point = new Point(point.X / TILE_SIZE, point.Y / TILE_SIZE);
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
            int y_start = y * TILES_CHUNK_SIZE;
            int y_end = Math.Min((y + 1) * TILES_CHUNK_SIZE, _layer.Height);

            int x_start = x * TILES_CHUNK_SIZE;
            int x_end = Math.Min((x + 1) * TILES_CHUNK_SIZE, _layer.Width);


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

        #region Selection Methods

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
            OnUpdate?.Invoke(null, null);
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
            OnUpdate?.Invoke(null, null);
        }
        public void SelectEverything()
        {
            for (int y = 0; y < _layer.Tiles.Length; y += 1)
            {
                for (int x = 0; x < _layer.Tiles[y].Length; x += 1)
                {
                    if (CanSelectTile(_layer.Tiles[y][x])) SelectedTiles.Add(new Point(x, y));
                }
            }
            InvalidateChunks();
            OnUpdate?.Invoke(null, null);
        }
        public void Select(Rectangle area, bool addSelection = false, bool deselectIfSelected = false)
        {
            if (!addSelection) DeselectAll();
            for (int y = Math.Max(area.Y / TILE_SIZE, 0); y < Math.Min(DivideRoundUp(area.Y + area.Height, TILE_SIZE), _layer.Height); ++y)
            {
                for (int x = Math.Max(area.X / TILE_SIZE, 0); x < Math.Min(DivideRoundUp(area.X + area.Width, TILE_SIZE), _layer.Width); ++x)
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
            point = new Point(point.X / TILE_SIZE, point.Y / TILE_SIZE);
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
            for (int y = Math.Max(area.Y / TILE_SIZE, 0); y < Math.Min(DivideRoundUp(area.Y + area.Height, TILE_SIZE), _layer.Height); ++y)
            {
                for (int x = Math.Max(area.X / TILE_SIZE, 0); x < Math.Min(DivideRoundUp(area.X + area.Width, TILE_SIZE), _layer.Width); ++x)
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
            point = new Point(point.X / TILE_SIZE, point.Y / TILE_SIZE);
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
            MoveSelected(Point.Empty, new Point(change.X * TILE_SIZE, change.Y * TILE_SIZE), false);
        }
        public void MoveSelected(Point oldPos, Point newPos, bool duplicate)
        {
            oldPos = new Point(oldPos.X / TILE_SIZE, oldPos.Y / TILE_SIZE);
            newPos = new Point(newPos.X / TILE_SIZE, newPos.Y / TILE_SIZE);
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
                OnUpdate?.Invoke(null, null);
            }
        }

        #endregion

        #region Set/Remove Tile Methods

        private void SetTile(Point point, ushort value, bool addAction = true)
        {
            if (addAction) Actions.Add(new ActionChangeTile((x, y) => SetTile(x, y, false), point, _layer.Tiles[point.Y][point.X], value));
            _layer.Tiles[point.Y][point.X] = value;
            InvalidateChunks();
            ObjectDrawing.UpdateEntityTileMaps();
        }
        private void RemoveTile(Point point)
        {
            SetTile(point, 0xffff);
        }

        #endregion

        #region Tile Placing Methods
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
            double offset = (Methods.Solution.SolutionState.Main.DrawBrushSize / 2) * TILE_SIZE;
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
                newPos = new Point(newPos.X / TILE_SIZE, newPos.Y / TILE_SIZE);
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

            this.PasteClipboardData(position, new Classes.Clipboard.TilesClipboardEntry(tiles));
        }

        #endregion

        #region Flipping/Property Setting Methods

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
                return;
            }
            else
            {
                FlipGroupTiles(direction, points, min, max);
                return;
            }

            void FlipIndividualTiles(FlipDirection _direction, IEnumerable<Point> _points)
            {
                foreach (Point point in _points)
                {
                    SelectedTiles.Values[point] ^= (ushort)_direction;
                }
                InvalidateChunks();
                OnUpdate?.Invoke(null, null);
            }

            void FlipGroupTiles(FlipDirection _direction, IEnumerable<Point> _points, int _min, int _max)
            {
                Dictionary<Point, ushort> workingTiles = new Dictionary<Point, ushort>();
                foreach (Point point in _points)
                {
                    ushort tileValue = SelectedTiles.Values[point];
                    Point newPoint;

                    if (_direction == FlipDirection.Horizontal)
                    {
                        int fromLeft = point.X - _min;
                        int fromRight = _max - point.X;

                        int newX = fromLeft < fromRight ? _max - fromLeft : _min + fromRight;
                        newPoint = new Point(newX, point.Y);
                    }
                    else
                    {
                        int fromBottom = point.Y - _min;
                        int fromTop = _max - point.Y;

                        int newY = fromBottom < fromTop ? _max - fromBottom : _min + fromTop;
                        newPoint = new Point(point.X, newY);
                    }

                    workingTiles.Add(newPoint, tileValue ^= (ushort)_direction);
                }

                SelectedTiles.Clear();
                SelectedTiles.Values.Clear();
                SelectedTiles.AddPoints(workingTiles.Select(wt => wt.Key).ToList());
                SelectedTiles.Values = workingTiles;
                InvalidateChunks();
                OnUpdate?.Invoke(null, null);
            }
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
            OnUpdate?.Invoke(null, null);
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

        #region Other Methods

        public void Resize(ushort width, ushort height)
        {
            // first resize the underlying SceneLayer
            _layer.Resize(width, height);

            SelectedTiles = new PointsMap(Width, Height);
            TempSelectionTiles = new PointsMap(Width, Height);
            InitiallizeChunkMap();

            WorkingHeight = Height;
            WorkingWidth = Width;
        }

        public EditorLayer Clone()
        {
            var cloneLayer = new RSDKv5.SceneLayer(_layer.Name, _layer.Width, _layer.Height);
            cloneLayer.DrawingOrder = _layer.DrawingOrder;
            cloneLayer.Behaviour = _layer.Behaviour;
            cloneLayer.ConstantSpeed = _layer.ConstantSpeed;
            cloneLayer.RelativeSpeed = _layer.RelativeSpeed;
            cloneLayer.ScrollIndexes = _layer.ScrollIndexes;
            cloneLayer.ScrollingInfo = _layer.ScrollingInfo;
            cloneLayer.Tiles = _layer.Tiles;
            return new EditorLayer(cloneLayer);
        }

        #endregion

        #region Clipboard Methods

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
        public Classes.Clipboard.TilesClipboardEntry GetClipboardData(bool KeepPosition = false)
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
            return new Classes.Clipboard.TilesClipboardEntry(copiedTiles.ToDictionary(x => new Point(x.Key.X - minX, x.Key.Y - minY), x => x.Value));
        }
        public void PasteClipboardData(Point newPos, Classes.Clipboard.TilesClipboardEntry data, bool UpdateActions = true)
        {
            try
            {
                var points = data.GetData();
                newPos = new Point(newPos.X / TILE_SIZE, newPos.Y / TILE_SIZE);
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

        #region Drawing Methods

        public void Draw(Graphics g, int _x, int _y, int _width, int _height, bool divide = true)
        {
            if (_x < 0) _x = 0;
            if (_y < 0) _y = 0;

            if (divide)
            {
                if (_x != 0) _x = _x / TILES_CHUNK_SIZE;
                if (_y != 0) _y = _y / TILES_CHUNK_SIZE;
                if (_width != 0) _width = _width / TILES_CHUNK_SIZE;
                if (_height != 0) _height = _height / TILES_CHUNK_SIZE;
            }

            int Width = (_layer.Width > _x + _width || _x + _width < 0 ? _layer.Width : _x + _width);
            int Height = (_layer.Height > _y + _height || _y + _height < 0 ? _layer.Height : _y + _height);


            for (int y = _y; y < Height; ++y)
            {
                for (int x = _x; x < Width; ++x)
                {
                    if (this._layer.Tiles[y][x] != 0xffff)
                    {
                        Methods.Drawing.CommonDrawing.DrawTile(g, _layer.Tiles[y][x], x, y);
                    }
                }
            }
        }
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
        public void Draw(DevicePanel d)
        {
            int Transparency;

            if (IsLayerTransparent()) Transparency = 0x32;
            else Transparency = 0xFF;

            Rectangle screen = d.GetScreen();
            int pos_x = screen.X;
            int pos_y = screen.Y;
            int width = screen.Width;
            int height = screen.Height;

            if (width >= 0 && height >= 0)
            {
                int start_x = pos_x / (TILES_CHUNK_SIZE * TILE_SIZE);
                int end_x = Math.Min(DivideRoundUp(pos_x + width, TILES_CHUNK_SIZE * TILE_SIZE), ChunkMap[0].Length);
                int start_y = pos_y / (TILES_CHUNK_SIZE * TILE_SIZE);
                int end_y = Math.Min(DivideRoundUp(pos_y + height, TILES_CHUNK_SIZE * TILE_SIZE), ChunkMap.Length);


                int CHUNK_SIZE = TILES_CHUNK_SIZE * TILE_SIZE;

                for (int y = start_y; y < end_y; ++y)
                {
                    for (int x = start_x; x < end_x; ++x)
                    {
                        if (x >= 0 && y >= 0)
                        {
                            if (d.IsObjectOnScreen(x * CHUNK_SIZE, y * CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE))
                            {
                                Rectangle rect = GetTilesChunkArea(x, y);
                                var texture = DrawChunk(d, x, y);
                                if (texture != null)
                                {
                                    d.DrawBitmap(texture, rect.X * TILE_SIZE, rect.Y * TILE_SIZE, 0, 0, rect.Width * TILE_SIZE, rect.Height * TILE_SIZE, false, Transparency, false, false, 0, null);
                                }

                            }
                            else InvalidateChunk(x, y);
                        }
                    }
                }
                DisposeUnusedChunks();
            }

            bool IsLayerTransparent()
            {
                if (!IsCurrentEditLayer) return true;
                else if (!IsEditEntitiesTransparency) return false;
                else return true;
            }
        }
        public void DrawTile(Graphics g, ushort tile, int x, int y, bool isSelected, System.Drawing.Color AllSolid, System.Drawing.Color LRDSolid, System.Drawing.Color TopOnlySolid)
        {
            if (tile != 0xffff)
            {
                ushort TileIndex = (ushort)(tile & 0x3ff);
                bool flipX = ((tile >> 10) & 1) == 1;
                bool flipY = ((tile >> 11) & 1) == 1;
                bool SolidTopA = ((tile >> 12) & 1) == 1;
                bool SolidLrbA = ((tile >> 13) & 1) == 1;
                bool SolidTopB = ((tile >> 14) & 1) == 1;
                bool SolidLrbB = ((tile >> 15) & 1) == 1;

                g.DrawImage(Tilesheet.GetBitmap(new Rectangle(0, TileIndex * TILE_SIZE, TILE_SIZE, TILE_SIZE), flipX, flipY, isSelected), new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE));

                if (ShowCollisionPathA)
                {
                    if (SolidLrbA || SolidTopA)
                    {
                        if (SolidTopA && SolidLrbA) DrawCollision(true, AllSolid, flipX, flipY);
                        if (SolidTopA && !SolidLrbA) DrawCollision(true, TopOnlySolid, flipX, flipY);
                        if (SolidLrbA && !SolidTopA) DrawCollision(true, LRDSolid, flipX, flipY);
                    }
                }
                if (ShowCollisionPathB)
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
                    g.DrawImage(EditorTilesheet.GetBitmap(new Rectangle(0, 3 * TILE_SIZE, TILE_SIZE, TILE_SIZE), false, false),
                                new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE));
                }
                if (Methods.Solution.SolutionState.Main.ShowTileID == true)
                {
                    g.DrawImage(IdentificationTilesheet.GetBitmap(new Rectangle(0, TileIndex * TILE_SIZE, TILE_SIZE, TILE_SIZE), false, false),
                                new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE));
                }
            }



            if (isSelected)
            {
                g.DrawRectangle(Pens.Red, new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE - 1, TILE_SIZE - 1));
            }

            void DrawCollision(bool drawA, System.Drawing.Color colour, bool flipX, bool flipY)
            {
                //create some image attributes
                ImageAttributes attributes = new ImageAttributes();

                //TODO : Collision Opacity
                int opacity = CollisionOpacity;

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
                int _width = TILE_SIZE;
                int _height = TILE_SIZE;

                Rectangle dest = new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);

                Bitmap collisionMap;

                if (drawA) collisionMap = CollisionMaskA.GetBitmap(new Rectangle(0, (tile & 0x3ff) * TILE_SIZE, TILE_SIZE, TILE_SIZE), flipX, flipY);
                else collisionMap = CollisionMaskB.GetBitmap(new Rectangle(0, (tile & 0x3ff) * TILE_SIZE, TILE_SIZE, TILE_SIZE), flipX, flipY);


                g.DrawImage(collisionMap, dest, _x, _y, _width, _height, GraphicsUnit.Pixel, attributes);

                attributes.Dispose();
                attributes = null;

                colourMatrixElements = null;
            }


        }
        public TextureExt DrawChunk(DevicePanel d, int x, int y)
        {
            bool isSelected = isChunkSelected(x, y);
            if (ChunkMap[y][x] != null && ChunkMap[y][x].IsReady && !ChunkMap[y][x].HasBeenSelectedPrior && !isSelected)
            {
                ChunkMap[y][x].HasBeenRendered = true;
                return (ChunkMap[y][x].Empty ? null : ChunkMap[y][x].Texture);
            }
            else
            {
                Rectangle rect = GetTilesChunkArea(x, y);

                if (ChunkMap[y][x] != null && ChunkMap[y][x].HasBeenSelectedPrior) InvalidateChunk(x, y);
                else if (ChunkMap[y][x] != null) InvalidateChunk(x, y);

                if (rect.Width <= 0 || rect.Height <= 0) return null;

                Bitmap bmp2 = new Bitmap(rect.Width * TILE_SIZE, rect.Height * TILE_SIZE, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                var squareSize = (bmp2.Width > bmp2.Height ? bmp2.Width : bmp2.Height);
                int factor = 32;
                int newSize = (int)Math.Round((squareSize / (double)factor), MidpointRounding.AwayFromZero) * factor;
                if (newSize == 0) newSize = factor;
                while (newSize < squareSize) newSize += factor;

                Bitmap bmp = new Bitmap(newSize, newSize, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                bool hasBeenSelected = false;
                bool HasAnyContent = false;

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
                                var tile = GetTileToDraw(point);
                                if (tile != 0xffff || tileSelected) HasAnyContent = true;
                                DrawTile(g, tile, tx - rect.X, ty - rect.Y, tileSelected, CollisionAllSolidColor, CollisionLRDSolidColor, CollisionTopOnlySolidColor);
                                if (tileSelected) hasBeenSelected = true;

                            }
                        }
                    }
                    ChunkMap[y][x] = new ChunkVBO();
                    ChunkMap[y][x].Texture = Methods.Drawing.TextureCreator.FromBitmap(d._device, bmp);
                    ChunkMap[y][x].IsReady = true;
                    ChunkMap[y][x].HasBeenRendered = true;
                    ChunkMap[y][x].HasBeenSelectedPrior = hasBeenSelected;
                    ChunkMap[y][x].Empty = !HasAnyContent;
                }

                bmp.Dispose();
                bmp2.Dispose();
                bmp = null;
                bmp2 = null;

                return (ChunkMap[y][x].Empty ? null : ChunkMap[y][x].Texture);
            }

            bool isChunkSelected(int _x, int _y)
            {
                Rectangle rect = GetTilesChunkArea(_x, _y);

                int rx = rect.X * TILE_SIZE;
                int ry = rect.Y * TILE_SIZE;
                int rx2 = rect.Right * TILE_SIZE;
                int ry2 = rect.Bottom * TILE_SIZE;

                int mouse_x = (int)Methods.Solution.SolutionState.Main.LastX;
                int mouse_y = (int)Methods.Solution.SolutionState.Main.LastY;

                if (mouse_x >= rx && mouse_x <= rx2 && mouse_y >= ry && mouse_y <= ry2)
                {
                    return true;
                }
                else return false;
            }

            ushort GetTileToDraw(Point source)
            {
                if (SelectedTiles.Values.ContainsKey(source)) return SelectedTiles.Values[source];
                else return Layer.Tiles[source.Y][source.X];
            }
        }


        #endregion

        #region Disposal Methods

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
    }
}
