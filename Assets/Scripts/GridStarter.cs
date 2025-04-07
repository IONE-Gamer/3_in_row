using UnityEngine;

public class GridStarter : MonoBehaviour
{
    public GridView gridView;
    public int width = 5;
    public int height = 5;

    private void Start()
    {
        var controller = new GridController(width, height);
        gridView.CreateGrid(controller.Model, controller);
    }
}