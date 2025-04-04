using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CodeBase
{
    public class BoardService
    {
        /// <summary>
        /// Width of the board
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Height of the board
        /// </summary>
        public int Height { get; private set; }

        private Marker[,] _board;

        private int _leftPivot;
        private int _rightPivot;
        private int _bottomPivot;
        private int _topPivot;

        private readonly int[][] _directions =
        {
            new[] { 1, 0 },
            new[] { 0, 1 },
            new[] { 1, 1 },
            new[] { 1, -1 }
        };


        /// <summary>
        /// Create new board with given size
        /// </summary>
        public void CreateBoard(Vector2Int size)
        {
            _leftPivot = int.MaxValue;
            _rightPivot = int.MinValue;
            _bottomPivot = int.MaxValue;
            _topPivot = int.MinValue;

            this._board = new Marker[size.x, size.y];
            Width = size.x;
            Height = size.y;
        }


        /// <summary>
        /// Set mark at given position
        /// </summary>
        public bool TrySetMarker(int x, int y, Marker marker)
        {
            if (!IsInsideBoard(x, y)) return false;
            _board[x, y] = marker;
            if (x < _leftPivot) _leftPivot = x;
            if (x > _rightPivot) _rightPivot = x;
            if (y < _bottomPivot) _bottomPivot = y;
            if (y > _topPivot) _topPivot = y;
            return true;
        }


        /// <summary>
        /// Get mark at given position
        /// </summary>
        public Marker GetMarker(int x, int y)
        {
            if (!IsInsideBoard(x, y)) return Marker.None;
            return _board[x, y];
        }


        /// <summary>
        /// Looking for win all over the map
        /// </summary>
        /// <param name="marker">Win marker</param>
        /// <param name="targetCount">Same mark win count</param>
        /// <returns>Status of victory</returns>
        public bool CheckFullMapForWin(Marker marker, int targetCount)
        {
            for (int i = _leftPivot; i <= _rightPivot; i++)
            {
                for (int j = _bottomPivot; j <= _topPivot; j++)
                {
                    if (_board[i, j] != marker)
                        continue;

                    foreach (var dir in _directions)
                    {
                        int count = 1;

                        count += CountInDirection(i, j, marker, dir[0], dir[1]);
                        count += CountInDirection(i, j, marker, -dir[0], -dir[1]);

                        if (count == targetCount) return true;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// Checks all direction from given point and calculate all combinations in given range
        /// </summary>
        /// <param name="x">X position of point</param>
        /// <param name="y">Y position of point</param>
        /// <param name="marker">Marker for check</param>
        /// <param name="minCount">Minimum combination length</param>
        /// <param name="maxCount">Maximum combination length</param>
        /// <returns>
        /// Collection with number of combinations,
        /// where 0 index is number of <b>minCount</b> and the last index is number of <b>maxCount</b>
        /// </returns>
        public ReadOnlyCollection<int> GetSequencesFromPosition(int x, int y, Marker marker, int minCount,
                                                                int maxCount)
        {
            int size = maxCount - minCount + 1;
            List<int> result = new(size);

            for (int i = 0; i < size; i++)
            {
                result.Add(0);
            }

            foreach (int[] dir in _directions)
            {
                int count = 1;

                count += CountInDirection(x, y, marker, dir[0], dir[1]);
                count += CountInDirection(x, y, marker, -dir[0], -dir[1]);

                if (count < minCount || count > maxCount)
                    continue;

                result[count - minCount]++;
            }

            return result.AsReadOnly();
        }


        /// <summary>
        /// Wipe all marks from border to type  <see cref="Marker.None"/>
        /// </summary>
        public void ClearAllMarks()
        {
            for (int i = 0; i < _board.GetLength(0); i++)
            {
                for (int j = 0; j < _board.GetLength(1); j++)
                {
                    _board[i, j] = Marker.None;
                }
            }
        }


        /// <summary>
        /// Used to count all equal marks in a given direction
        /// </summary>
        /// <param name="x">Start position x</param>
        /// <param name="y">Start position y</param>
        /// <param name="marker">Mark type looking for</param>
        /// <param name="dx">Direction x</param>
        /// <param name="dy">Direction y</param>
        /// <returns>Number of equal marks</returns>
        private int CountInDirection(int x, int y, Marker marker, int dx, int dy)
        {
            int count = -1;
            do
            {
                count++;
                x += dx;
                y += dy;
            } while (IsInsideBoard(x, y) && _board[x, y] == marker);

            return count;
        }


        /// <summary>
        /// Checks for out of bounds
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsInsideBoard(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }
    }
}