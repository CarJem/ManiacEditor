using System;
using System.Collections.Generic;
using System.Drawing;

namespace ManiacEditor.Classes.Scene
{
    public class PointsMap
    {
        const int TILES_CHUNK_SIZE = 16;

        static int DivideRoundUp(int number, int by)
        {
            return (number + by - 1) / by;
        }

        public Dictionary<Point, ushort> Values { get; set; } = new Dictionary<Point, ushort>();
        HashSet<Point>[][] PointsChunks;
        public HashSet<Point> OutOfBoundsPoints { get; private set; } = new HashSet<Point>();
        public int Count = 0;

        public PointsMap(int width, int height)
        {
            PointsChunks = new HashSet<Point>[DivideRoundUp(height, TILES_CHUNK_SIZE)][];
            for (int i = 0; i < PointsChunks.Length; ++i)
            {
                PointsChunks[i] = new HashSet<Point>[DivideRoundUp(width, TILES_CHUNK_SIZE)];
                for (int j = 0; j < PointsChunks[i].Length; ++j)
                    PointsChunks[i][j] = new HashSet<Point>();
            }
        }

        public void Add(Point point)
        {

            HashSet<Point> h;
            if (point.Y < 0 || point.X < 0 || point.Y / TILES_CHUNK_SIZE >= PointsChunks.Length || point.X / TILES_CHUNK_SIZE >= PointsChunks[0].Length)
                h = OutOfBoundsPoints;
            else
                h = PointsChunks[point.Y / TILES_CHUNK_SIZE][point.X / TILES_CHUNK_SIZE];
            Count -= h.Count;
            h.Add(point);
            Count += h.Count;
        }

        public void Remove(Point point)
        {
            HashSet<Point> h;
            if (point.Y < 0 || point.X < 0 || point.Y / TILES_CHUNK_SIZE >= PointsChunks.Length || point.X / TILES_CHUNK_SIZE >= PointsChunks[0].Length)
                h = OutOfBoundsPoints;
            else
                h = PointsChunks[point.Y / TILES_CHUNK_SIZE][point.X / TILES_CHUNK_SIZE];
            Count -= h.Count;
            h.Remove(point);
            Count += h.Count;
        }

        public bool Contains(Point point)
        {
            if (point.Y < 0 || point.X < 0 || point.Y / TILES_CHUNK_SIZE >= PointsChunks.Length || point.X / TILES_CHUNK_SIZE >= PointsChunks[0].Length)
                return OutOfBoundsPoints.Contains(point);
            else
                return PointsChunks[point.Y / TILES_CHUNK_SIZE][point.X / TILES_CHUNK_SIZE].Contains(point);
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
}
