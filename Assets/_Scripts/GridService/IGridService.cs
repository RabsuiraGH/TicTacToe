using UnityEngine;

namespace CodeBase
{
    public interface IGridService
    {
        public void ToggleGrid(bool toggle);
        public void CreateGrid(int gridSize, float cellSize, float lineWidth, Vector2 gridPosition);
    }
}