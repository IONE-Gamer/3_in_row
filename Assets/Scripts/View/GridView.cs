using UnityEngine;

public class GridView : MonoBehaviour
{
    public GameObject cellPrefab;
    public float cellSize = 1.0f;

    public void CreateGrid(GridModel model)
    {
        for (int x = 0; x < model.Width; x++)
        {
            for (int y = 0; y < model.Height; y++)
            {
                var cellObj = Instantiate(cellPrefab, new Vector3(x * cellSize, y * cellSize), Quaternion.identity);
                cellObj.name = $"Cell_{x}_{y}";
            }
        }
    }
}