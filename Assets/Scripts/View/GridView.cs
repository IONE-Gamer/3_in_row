using JetBrains.Annotations;
using UnityEngine;

public class GridView : MonoBehaviour
{
    public GameObject cellPrefab;
    public float cellSize = 1.0f;

    public void CreateGrid([CanBeNull] GridModel model, GridController controller)
    {
        for (int x = 0; x < model.Width; x++)
        {
            for (int y = 0; y < model.Height; y++)
            {
                var cellObj = Instantiate(cellPrefab, new Vector3(x * cellSize, y * cellSize), Quaternion.identity);
                cellObj.name = $"Cell_{x}_{y}";
                
                var cellView = cellObj.GetComponent<CellView>();
                cellView.Init(x, y, controller);            }
        }
    }
}