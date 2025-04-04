using UnityEngine;
using UnityEngine.Rendering;

namespace CodeBase
{
    public class GridService : IGridService
    {
        private Vector2Int _gridSize;
        private float _cellSize;
        private float _lineWidth;
        private Vector2 _gridPosition;

        private Mesh _gridMesh;
        private readonly MeshFilter _meshFilter;
        private readonly MeshRenderer _meshRenderer;
        private readonly GameObject _gridObject;


        public GridService()
        {
            _gridObject = new GameObject("Grid");

            _meshFilter = _gridObject.AddComponent<MeshFilter>();
            _meshRenderer = _gridObject.AddComponent<MeshRenderer>();
            _meshRenderer.shadowCastingMode = ShadowCastingMode.Off;

            Material material = new(Shader.Find("Unlit/Color"))
            {
                color = Color.white
            };

            _meshRenderer.material = material;
        }


        public void ToggleGrid(bool toggle)
        {
            _gridObject.gameObject.SetActive(toggle);
        }


        public void CreateGrid(Vector2Int gridSize, float cellSize, float lineWidth, Vector2 gridPosition)
        {
            _gridSize = gridSize;
            _cellSize = cellSize;
            _lineWidth = lineWidth;
            _gridPosition = gridPosition;

            BuildGrid();
            _meshFilter.mesh = _gridMesh;
        }


        private void BuildGrid()
        {
            _gridMesh = new Mesh
            {
                name = "Grid Mesh"
            };

            int horizontalLines = _gridSize.y + 1;
            int verticalLines = _gridSize.x + 1;
            int totalQuads = horizontalLines + verticalLines;
            int totalVertices = totalQuads * 4;
            int totalTriangles = totalQuads * 6;

            Vector3[] vertices = new Vector3[totalVertices];
            int[] triangles = new int[totalTriangles];

            int vertIndex = 0;
            int triIndex = 0;
            float halfWidth = _lineWidth / 2f;

            for (int i = 0; i < horizontalLines; i++)
            {
                float y = i * _cellSize;
                vertices[vertIndex] = new Vector3(0, y - halfWidth, 0);
                vertices[vertIndex + 1] = new Vector3(_gridSize.x * _cellSize, y - halfWidth, 0);
                vertices[vertIndex + 2] = new Vector3(0, y + halfWidth, 0);
                vertices[vertIndex + 3] = new Vector3(_gridSize.x * _cellSize, y + halfWidth, 0);

                CreateTriangle(ref triangles, ref triIndex, ref vertIndex);
            }

            for (int i = 0; i < verticalLines; i++)
            {
                float x = i * _cellSize;
                vertices[vertIndex] = new Vector3(x - halfWidth, 0, 0);
                vertices[vertIndex + 1] = new Vector3(x + halfWidth, 0, 0);
                vertices[vertIndex + 2] = new Vector3(x - halfWidth, _gridSize.y * _cellSize, 0);
                vertices[vertIndex + 3] = new Vector3(x + halfWidth, _gridSize.y * _cellSize, 0);


                CreateTriangle(ref triangles, ref triIndex, ref vertIndex);
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] += (Vector3)_gridPosition;
            }

            _gridMesh.vertices = vertices;
            _gridMesh.triangles = triangles;
            _gridMesh.RecalculateBounds();
        }


        private void CreateTriangle(ref int[] triangles, ref int triIndex, ref int vertIndex)
        {
            triangles[triIndex] = vertIndex;
            triangles[triIndex + 1] = vertIndex + 2;
            triangles[triIndex + 2] = vertIndex + 1;
            // Второй треугольник
            triangles[triIndex + 3] = vertIndex + 2;
            triangles[triIndex + 4] = vertIndex + 3;
            triangles[triIndex + 5] = vertIndex + 1;

            vertIndex += 4;
            triIndex += 6;
        }
    }
}