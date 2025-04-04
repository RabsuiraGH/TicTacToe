using System;
using CodeBase.Config.MarkConfig;
using UnityEngine;
using Zenject;

namespace CodeBase
{
    public class Test : MonoBehaviour
    {
        [SerializeField] private GameConfiguration _config;
        [SerializeField] private Game _game;


        [Inject]
        public void Construct(Game game)
        {
            _game = game;
        }
        [EasyButtons.Button]
        public void CreateRoom()
        {
            _game.CreateGame(_config);
        }
        [EasyButtons.Button]
        public void StartGame()
        {
            _game.StartGame();
        }
        [EasyButtons.Button]
        public void PlaceMark(Vector2 position)
        {
            _game.TryPlaceMark(Vector2Int.FloorToInt(position));
        }

        [EasyButtons.Button]
        public void NextTurn()
        {
            _game.NextTurn();
        }
    }
}