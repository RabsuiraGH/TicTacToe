using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace CodeBase
{
    [Serializable]
    public class Game
    {
        private GameConfiguration _gameConfig;
        [SerializeField] private bool _gameActive = false;

        [SerializeField] private List<GamePlayer> _players = new();
        [SerializeField] private int _currentPlayer = 0;

        private GameFieldService _gameFieldService;


        [Inject]
        public void Construct(GameFieldService gameFieldService)
        {
            _gameFieldService = gameFieldService;
        }


        public void CreateGame(GameConfiguration gameConfiguration)
        {
            _gameConfig = gameConfiguration;
            _players.Clear();
            for (int i = 1; i <= gameConfiguration.PlayerCount; i++)
            {
                _players.Add(new GamePlayer() { PlayerMarker = (Marker)i });
            }
        }


        public void StartGame()
        {
            _gameFieldService.CreateField(_gameConfig.FieldSize);
            _gameActive = true;
        }


        public bool TryPlaceMark(Vector2Int position, bool allowOverrides = false)
        {
            GamePlayer gamePlayer = GetPlayer(_currentPlayer);
            Marker playerMarker = gamePlayer.PlayerMarker;

            if (_gameFieldService.GetMark(position) != Marker.None &&
                _gameFieldService.GetMark(position) != playerMarker &&
                !allowOverrides)
                return false;

            _gameFieldService.PlaceMark(position, playerMarker);
            if (_gameFieldService.CheckForWin(playerMarker, _gameConfig.WinCount))
            {
                Win(_currentPlayer);
                return true;
            }


            gamePlayer.GamePoints += _gameFieldService.GetPoints(position, playerMarker,
                                                             _gameConfig.MinSequenceCount,
                                                             _gameConfig.MaxSequenceCount,
                                                             _gameConfig.PointPerSequence);
            return true;
        }


        public void NextTurn()
        {
            _currentPlayer = ((++_currentPlayer) % _gameConfig.PlayerCount);
        }


        public GamePlayer GetPlayer(int player)
        {
            if (!_players.Any()) throw new Exception("No players in the session");
            if (player >= _players.Count) throw new IndexOutOfRangeException($"No player with index {player}");
            return _players[player];
        }


        public void Win(int player)
        {
            _gameActive = false;
            Debug.Log(($"{GetPlayer(player)} Won!"));
        }
    }

    [Serializable]
    public class GameConfiguration
    {
        public int PlayerCount;
        public Vector2Int FieldSize;
        public int WinCount;
        public int MinSequenceCount;
        public int MaxSequenceCount;
        public List<int> PointPerSequence = new();
    }

    [Serializable]
    public class GamePlayer
    {
        public Marker PlayerMarker;
        public int GamePoints;
    }
}