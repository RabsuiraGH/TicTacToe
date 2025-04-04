using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace CodeBase
{
    public class GameFieldService
    {
        private IGridService _gridService;
        private BoardService _boardService;
        private IFactory<Vector2, Marker, Transform, MarkCell> _cellFactory;
        private Transform _marksRoot;
        private Dictionary<Vector2Int, MarkCell> _markCells = new();


        [Inject]
        public void Construct(IGridService gridService, BoardService boardService, MarkCellFactory cellFactory)
        {
            _cellFactory = cellFactory;
            _gridService = gridService;
            _boardService = boardService;
        }


        [EasyButtons.Button]
        public void CreateField(Vector2Int fieldSize)
        {
            _gridService.CreateGrid(fieldSize, 1f, 0.1f, new Vector2(-0.5f, -0.5f));
            _boardService.CreateBoard(fieldSize);
        }


        public void PlaceMark(Vector2Int position, Marker mark)
        {
            if (!_boardService.TrySetMarker(position.x, position.y, mark))
                throw new ArgumentOutOfRangeException(nameof(position), "Mark position out of bounds");


            if (!_markCells.ContainsKey(position))
            {
                MarkCell markCell = _cellFactory.Create(position, mark, _marksRoot);
                _markCells.Add(position, markCell);
            }

            _markCells[position].ChangeMarker(mark);
        }

        public Marker GetMark(Vector2Int position)
        {
            return _boardService.GetMarker(position.x, position.y);
        }



        public bool CheckForWin(Marker marker, int winCount)
        {
            return _boardService.CheckFullMapForWin(marker, winCount);
        }


        public int GetPoints(Vector2Int position, Marker marker, int minCount, int maxCount, List<int> pointPerSequence)
        {
            if (pointPerSequence == null)
                throw new ArgumentNullException(nameof(pointPerSequence));

            if (!pointPerSequence.Any() || pointPerSequence.Count > maxCount - minCount + 1)
                throw new Exception($"{nameof(pointPerSequence)} Illegal sequence count");


            return _boardService.GetSequencesFromPosition(position.x, position.y, marker, minCount, maxCount)
                                .Select((t, i) => t * pointPerSequence[i]).Sum();
        }
    }
}